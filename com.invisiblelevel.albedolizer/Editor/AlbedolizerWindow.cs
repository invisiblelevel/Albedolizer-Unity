using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AlbedolizerWindow : EditorWindow
{
    // ═══ Поля UI ═══
    private string albedoPath = "";

    private int presetIndex = 0;
    private int correctModeIndex = 1;
    private int aiModelIndex = 0;
    private int engineIndex = 0;

    private bool seamless = false;
    private bool maps_height = true;
    private bool maps_normal = true;
    private bool maps_ao = true;
    private bool maps_roughness = true;
    private bool maps_metallic = true;
    private bool maps_edge = true;
    private bool maps_orm = true;

    // ═══ Прогресс ═══
    private bool isGenerating = false;
    private int progressPct = 0;
    private string progressStage = "";
    private string statusMessage = "";
    private MessageType statusType = MessageType.None;

    // ═══ Данные ═══
    private static readonly string[] PRESETS = new[]
    {
        "metal", "rust", "oxidized_metal", "patina", "brass", "aluminum", "copper",
        "wood", "leaves", "moss", "organic", "grass", "bark",
        "stone", "concrete", "brick", "ground", "asphalt", "marble", "sand",
        "clay", "granite", "stucco", "gemstone", "tile", "gravel", "coal",
        "roof_tiles", "plastic", "rubber", "glass", "ceramic", "painted_metal",
        "carbon", "cardboard", "cotton", "wool", "silk", "denim", "carpet",
        "velvet", "water", "mud", "snow", "ice", "leather", "fur", "skin",
        "scales", "bone"
    };

    private static readonly string[] CORRECT_MODES = new[] { "None", "AI", "Math" };
    private static readonly string[] CORRECT_VALUES = new[] { "none", "ai", "math" };
    private static readonly string[] AI_MODELS = new[] { "Autolevels", "LUTwithBGrid" };
    private static readonly string[] AI_VALUES = new[] { "autolevels", "lutwithbgrid" };
    private static readonly string[] ENGINES = new[] { "None", "Unity URP", "Unity HDRP", "Unreal", "Godot" };
    private static readonly string[] ENGINE_VALUES = new[] { "", "unity_urp", "unity_hdrp", "unreal", "godot" };

    private Vector2 scroll;

    [MenuItem("Window/Albedolizer")]
    public static void ShowWindow()
    {
        GetWindow<AlbedolizerWindow>("Albedolizer");
    }

    private void OnGUI()
    {
        scroll = EditorGUILayout.BeginScrollView(scroll);

        EditorGUILayout.LabelField("Albedolizer PBR Generator", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Generate PBR maps from Albedo textures.", EditorStyles.miniLabel);
        EditorGUILayout.Space(10);

        DrawCliSection();
        EditorGUILayout.Space(10);

        DrawAlbedoSection();
        EditorGUILayout.Space(10);

        DrawPresetSection();
        EditorGUILayout.Space(10);

        DrawMapsSection();
        EditorGUILayout.Space(10);

        DrawEngineSection();
        EditorGUILayout.Space(10);

        if (isGenerating)
        {
            EditorGUILayout.LabelField($"Progress: {progressPct}% — {progressStage}", EditorStyles.boldLabel);
            Rect r = EditorGUILayout.GetControlRect(false, 20);
            EditorGUI.ProgressBar(r, progressPct / 100f, $"{progressPct}%");
            EditorGUILayout.Space(10);
        }

        EditorGUI.BeginDisabledGroup(isGenerating);
        if (GUILayout.Button("Generate PBR", GUILayout.Height(40)))
        {
            OnGenerateClicked();
        }
        EditorGUI.EndDisabledGroup();

        if (!string.IsNullOrEmpty(statusMessage))
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox(statusMessage, statusType);
        }

        EditorGUILayout.Space(10);

        // ═══ Поддержка ═══
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("💛 Support", GUILayout.Height(26)))
        {
            DonateWindow.ShowWindow();
        }
        if (GUILayout.Button("GitHub", GUILayout.Height(26)))
        {
            Application.OpenURL("https://github.com/invisiblelevel/Albedolizer");
        }
        if (GUILayout.Button("itch.io", GUILayout.Height(26)))
        {
            Application.OpenURL("https://invlvl.itch.io/albedolizer");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndScrollView();
    }

    private void DrawCliSection()
    {
        EditorGUILayout.LabelField("CLI", EditorStyles.boldLabel);
        var cliPath = AlbedolizerBridge.GetCliPath();
        if (string.IsNullOrEmpty(cliPath))
        {
            EditorGUILayout.HelpBox("CLI not found. Reinstall the package.", MessageType.Error);
        }
        else
        {
            EditorGUILayout.HelpBox("✓ CLI found", MessageType.Info);
            EditorGUILayout.SelectableLabel(cliPath, EditorStyles.miniLabel, GUILayout.Height(16));
        }
    }

    private void DrawAlbedoSection()
    {
        EditorGUILayout.LabelField("Albedo Texture", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        albedoPath = EditorGUILayout.TextField(albedoPath);
        if (GUILayout.Button("...", GUILayout.Width(30)))
        {
            var picked = EditorUtility.OpenFilePanel("Pick Albedo", "", "png,jpg,jpeg,tif,tiff,bmp");
            if (!string.IsNullOrEmpty(picked))
            {
                albedoPath = picked;
                AutoDetectPreset(picked);
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private void AutoDetectPreset(string path)
    {
        var name = Path.GetFileNameWithoutExtension(path).ToLowerInvariant();
        for (int i = 0; i < PRESETS.Length; i++)
        {
            if (name.Contains(PRESETS[i]))
            {
                presetIndex = i;
                return;
            }
        }
    }

    private void DrawPresetSection()
    {
        EditorGUILayout.LabelField("Material Preset", EditorStyles.boldLabel);
        presetIndex = EditorGUILayout.Popup(presetIndex, PRESETS);

        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("Correction", EditorStyles.boldLabel);
        correctModeIndex = EditorGUILayout.Popup(correctModeIndex, CORRECT_MODES);

        if (correctModeIndex == 1)
        {
            EditorGUI.indentLevel++;
            aiModelIndex = EditorGUILayout.Popup("AI Model", aiModelIndex, AI_MODELS);
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space(6);
        seamless = EditorGUILayout.Toggle("Seamless", seamless);
    }

    private void DrawMapsSection()
    {
        EditorGUILayout.LabelField("Maps", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        maps_height = EditorGUILayout.ToggleLeft("Height", maps_height, GUILayout.Width(80));
        maps_normal = EditorGUILayout.ToggleLeft("Normal", maps_normal, GUILayout.Width(80));
        maps_ao = EditorGUILayout.ToggleLeft("AO", maps_ao, GUILayout.Width(60));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        maps_roughness = EditorGUILayout.ToggleLeft("Rough", maps_roughness, GUILayout.Width(80));
        maps_metallic = EditorGUILayout.ToggleLeft("Metal", maps_metallic, GUILayout.Width(80));
        maps_edge = EditorGUILayout.ToggleLeft("Edge", maps_edge, GUILayout.Width(60));
        EditorGUILayout.EndHorizontal();
        maps_orm = EditorGUILayout.ToggleLeft("ORM", maps_orm);
    }

    private void DrawEngineSection()
    {
        EditorGUILayout.LabelField("Engine Packing", EditorStyles.boldLabel);
        engineIndex = EditorGUILayout.Popup(engineIndex, ENGINES);

        if (engineIndex == 0)
        {
            EditorGUILayout.HelpBox("No engine packing. Individual PNGs only.", MessageType.None);
        }
        else if (engineIndex == 1)
        {
            EditorGUILayout.HelpBox("Creates URP MetallicSmoothness map (R=Metallic, G=AO, A=Smoothness).", MessageType.Info);
        }
        else if (engineIndex == 2)
        {
            EditorGUILayout.HelpBox("Creates HDRP MaskMap (R=Metallic, G=AO, B=Detail, A=Smoothness).", MessageType.Info);
        }
    }

    private string GetMapsString()
    {
        var maps = new List<string>();
        if (maps_height) maps.Add("height");
        if (maps_normal) maps.Add("normal");
        if (maps_ao) maps.Add("ao");
        if (maps_roughness) maps.Add("roughness");
        if (maps_metallic) maps.Add("metallic");
        if (maps_edge) maps.Add("edge");
        if (maps_orm) maps.Add("orm");
        return string.Join(",", maps);
    }

    private void OnGenerateClicked()
    {
        if (string.IsNullOrEmpty(albedoPath) || !File.Exists(albedoPath))
        {
            statusMessage = "Pick a valid Albedo texture.";
            statusType = MessageType.Error;
            return;
        }

        if (string.IsNullOrEmpty(GetMapsString()))
        {
            statusMessage = "Select at least one map.";
            statusType = MessageType.Error;
            return;
        }

        var baseName = Path.GetFileNameWithoutExtension(albedoPath);

        isGenerating = true;
        progressPct = 0;
        progressStage = "starting";
        statusMessage = "Generating...";
        statusType = MessageType.Info;
        Repaint();

        var engine = ENGINE_VALUES[engineIndex];
        var preset = PRESETS[presetIndex];
        var correctMode = CORRECT_VALUES[correctModeIndex];
        var aiModel = AI_VALUES[aiModelIndex];
        var mapsStr = GetMapsString();

        // 1. Генерация во временную папку
        var result = AlbedolizerBridge.RunGenerateToTemp(
            albedoPath: albedoPath,
            preset: preset,
            correctMode: correctMode,
            aiModel: aiModel,
            mapsString: mapsStr,
            engine: engine,
            seamless: seamless,
            onProgress: (pct, stage) =>
            {
                progressPct = pct;
                progressStage = stage;
                Repaint();
            }
        );

        if (!result.ok)
        {
            isGenerating = false;
            statusMessage = $"Generation failed: {result.error}";
            statusType = MessageType.Error;
            Debug.LogError($"[Albedolizer] {result.error}");
            Repaint();
            return;
        }

        // 2. Копируем albedo в Assets
        var albedoAssetPath = AlbedolizerBridge.EnsureAlbedoInAssets(albedoPath, baseName);

        // 3. Копируем карты в Assets
        var mapsAssetPaths = AlbedolizerBridge.CopyMapsToAssets(result.maps, baseName);

        isGenerating = false;

        // 4. Создаём материал
        if (engine == "unity_urp" &&
            mapsAssetPaths.ContainsKey("packed") &&
            mapsAssetPaths.ContainsKey("normal"))
        {
            var packedAsset = mapsAssetPaths["packed"];
            var normalAsset = mapsAssetPaths["normal"];
            var matName = $"{baseName}_PBR";
            var matFolder = $"Assets/Albedolizer_Output/{baseName}";

            var mat = AlbedolizerMaterialBuilder.BuildUrpMaterial(
                materialName: matName,
                albedoPath: albedoAssetPath,
                normalPath: normalAsset,
                packedPath: packedAsset,
                outputFolder: matFolder
            );

            if (mat != null)
            {
                statusMessage = $"✓ PBR generated + material created: {matName}";
                statusType = MessageType.Info;
                Selection.activeObject = mat;
            }
            else
            {
                statusMessage = "✓ PBR generated, but material creation failed. Check Console.";
                statusType = MessageType.Warning;
            }
        }
        else
        {
            statusMessage = $"✓ PBR generated. Output: Assets/Albedolizer_Output/{baseName}/";
            statusType = MessageType.Info;
        }

        Debug.Log($"[Albedolizer] Done. Output: Assets/Albedolizer_Output/{baseName}/");
        Repaint();
    }
}