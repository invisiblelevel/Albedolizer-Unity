using UnityEditor;
using UnityEngine;

public class DonateWindow : EditorWindow
{
    private static readonly (string label, string address)[] WALLETS = new[]
    {
        ("BTC", "bc1q2ka70s4vtmrskandqj8l4d6n3kdxyxa7kf3wf7"),
        ("USDT (TRC-20)", "TUjY9p6oxKmeCQwNZwMfHqHdXuabaHpgT7"),
        ("GRAM", "UQDWumGNNlnITx48WBzyI7Clb5wrpFRlJ3Se7xhfVKY5E2ad"),
    };

    private const string GITHUB_URL = "https://github.com/invisiblelevel/Albedolizer";
    private const string ITCH_URL = "https://invlvl.itch.io/albedolizer";

    private Vector2 scroll;
    private string copiedLabel = "";

    public static void ShowWindow()
    {
        var w = GetWindow<DonateWindow>("Support", true);
        w.minSize = new Vector2(500, 380);
        w.maxSize = new Vector2(700, 500);
    }

    private void OnGUI()
    {
        scroll = EditorGUILayout.BeginScrollView(scroll);

        var titleStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 16,
            alignment = TextAnchor.MiddleCenter
        };
        EditorGUILayout.LabelField("💛 Support Albedolizer", titleStyle);
        EditorGUILayout.Space(8);

        var centered = new GUIStyle(EditorStyles.label)
        {
            alignment = TextAnchor.MiddleCenter,
            wordWrap = true
        };
        EditorGUILayout.LabelField(
            "This add-on is free. If it saves your time —\ndrop some smokes for uncle. Thank you!",
            centered, GUILayout.Height(40));

        EditorGUILayout.Space(12);

        // ═══ Кошельки ═══
        foreach (var w in WALLETS)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal();
            var labelStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                normal = { textColor = new Color(0.36f, 0.55f, 0.85f) }
            };
            EditorGUILayout.LabelField(w.label, labelStyle, GUILayout.Width(130));

            EditorGUILayout.SelectableLabel(w.address, EditorStyles.textField, GUILayout.Height(18));

            if (GUILayout.Button("Copy", GUILayout.Width(60)))
            {
                EditorGUIUtility.systemCopyBuffer = w.address;
                copiedLabel = w.label;
                Repaint();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        if (!string.IsNullOrEmpty(copiedLabel))
        {
            EditorGUILayout.Space(6);
            var okStyle = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(0.3f, 0.7f, 0.3f) }
            };
            EditorGUILayout.LabelField($"✅ {copiedLabel} address copied to clipboard", okStyle);
        }

        EditorGUILayout.Space(14);

        // ═══ Ссылки ═══
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("🌐 GitHub — Full desktop app", GUILayout.Height(28)))
        {
            Application.OpenURL(GITHUB_URL);
        }
        if (GUILayout.Button("🎮 itch.io — Download", GUILayout.Height(28)))
        {
            Application.OpenURL(ITCH_URL);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(8);

        if (GUILayout.Button("Close", GUILayout.Height(24)))
        {
            Close();
        }

        EditorGUILayout.EndScrollView();
    }
}