using System.IO;
using UnityEditor;
using UnityEngine;

public static class AlbedolizerMaterialBuilder
{
    /// <summary>
    /// Создаёт URP/Lit материал из PBR-карт, сгенерированных CLI.
    /// </summary>
    public static Material BuildUrpMaterial(
        string materialName,
        string albedoPath,
        string normalPath,
        string packedPath,
        string outputFolder)
    {
        // ═══ 1. Импорт-настройки для текстур ═══
        ConfigureTextureImport(albedoPath, isSRGB: true, isNormal: false);
        ConfigureTextureImport(normalPath, isSRGB: false, isNormal: true);
        ConfigureTextureImport(packedPath, isSRGB: false, isNormal: false);

        // ═══ 2. Загрузка текстур ═══
        var albedo = AssetDatabase.LoadAssetAtPath<Texture2D>(ToAssetPath(albedoPath));
        var normal = AssetDatabase.LoadAssetAtPath<Texture2D>(ToAssetPath(normalPath));
        var packed = AssetDatabase.LoadAssetAtPath<Texture2D>(ToAssetPath(packedPath));

        if (albedo == null || normal == null || packed == null)
        {
            Debug.LogError($"[Albedolizer] Failed to load textures. " +
                           $"Albedo: {albedo != null}, Normal: {normal != null}, Packed: {packed != null}");
            return null;
        }

        // ═══ 3. Создаём материал ═══
        var shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
        {
            Debug.LogError("[Albedolizer] URP/Lit shader not found. Is URP installed?");
            return null;
        }

        var mat = new Material(shader);
        mat.name = materialName;

        // ═══ 4. Назначаем карты ═══
        mat.SetTexture("_BaseMap", albedo);
        mat.SetTexture("_BumpMap", normal);
        mat.SetTexture("_MetallicGlossMap", packed);
        mat.SetTexture("_OcclusionMap", packed);  // URP берёт G-канал

        // ═══ 5. Включаем фичи шейдера ═══
        mat.EnableKeyword("_NORMALMAP");
        mat.EnableKeyword("_METALLICSPECGLOSSMAP");
        mat.EnableKeyword("_OCCLUSIONMAP");

        // ═══ 6. Настройка Smoothness / Metallic / AO ═══
        mat.SetFloat("_SmoothnessTextureChannel", 0f);  // 0 = Metallic Alpha
        mat.SetFloat("_Smoothness", 1.0f);
        mat.SetFloat("_Metallic", 1.0f);
        mat.SetFloat("_OcclusionStrength", 1.0f);

        // ═══ 7. Сохраняем материал ═══
        var matPath = Path.Combine(outputFolder, $"{materialName}.mat").Replace("\\", "/");
        var assetMatPath = ToAssetPath(matPath);

        // Удаляем существующий материал если есть
        if (AssetDatabase.LoadAssetAtPath<Material>(assetMatPath) != null)
        {
            AssetDatabase.DeleteAsset(assetMatPath);
        }

        AssetDatabase.CreateAsset(mat, assetMatPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[Albedolizer] Material created: {assetMatPath}");
        return mat;
    }

    // ═══ Настройка импорта текстуры ═══
    private static void ConfigureTextureImport(string path, bool isSRGB, bool isNormal)
    {
        var assetPath = ToAssetPath(path);
        var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer == null)
        {
            Debug.LogWarning($"[Albedolizer] TextureImporter not found for: {assetPath}");
            return;
        }

        importer.textureType = isNormal ? TextureImporterType.NormalMap : TextureImporterType.Default;
        importer.sRGBTexture = isSRGB;
        importer.alphaSource = TextureImporterAlphaSource.FromInput;
        importer.alphaIsTransparency = false;
        importer.mipmapEnabled = true;
        importer.maxTextureSize = 4096;
        importer.textureCompression = TextureImporterCompression.CompressedHQ;
        importer.wrapMode = TextureWrapMode.Repeat;

        EditorUtility.SetDirty(importer);
        importer.SaveAndReimport();
    }

    // ═══ Преобразование абсолютного пути в Assets/... ═══
    private static string ToAssetPath(string fullPath)
    {
        var dataPath = Application.dataPath.Replace("\\", "/");
        var normalized = fullPath.Replace("\\", "/");

        if (normalized.StartsWith(dataPath))
        {
            return "Assets" + normalized.Substring(dataPath.Length);
        }
        return normalized;
    }
}