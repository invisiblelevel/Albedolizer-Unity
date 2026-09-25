using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static class AlbedolizerBridge
{
    public class GenerateResult
    {
        public bool ok;
        public string error;
        public string outputDir;
        public string preset;
        public Dictionary<string, string> maps = new Dictionary<string, string>();
        public string rawJson;
    }

    public static string GetCliPath()
    {
        var guids = AssetDatabase.FindAssets("albedolizer_cli t:DefaultAsset");
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.EndsWith("albedolizer_cli.exe"))
            {
                return Path.GetFullPath(path);
            }
        }
        return null;
    }

    /// <summary>
    /// Генерирует PBR-карты во временную папку. НЕ копирует в Assets.
    /// </summary>
    public static GenerateResult RunGenerateToTemp(
        string albedoPath,
        string preset,
        string correctMode,
        string aiModel,
        string mapsString,
        string engine,
        bool seamless,
        Action<int, string> onProgress)
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"albedolizer_{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);

        var result = RunGenerateCore(albedoPath, tempDir, preset, correctMode, aiModel, mapsString, engine, seamless, onProgress);
        return result;
    }

    /// <summary>
    /// Копирует сгенерированные карты в Assets\Albedolizer_Output\<baseName>\
    /// и импортирует их. Возвращает словарь новых (Asset) путей.
    /// </summary>
    public static Dictionary<string, string> CopyMapsToAssets(
        Dictionary<string, string> sourceMaps,
        string baseName)
    {
        var result = new Dictionary<string, string>();

        // Целевая папка в Assets
        var assetsOutputDir = $"Assets/Albedolizer_Output/{baseName}";
        var absoluteOutputDir = Path.Combine(Application.dataPath, "Albedolizer_Output", baseName);
        Directory.CreateDirectory(absoluteOutputDir);

        foreach (var kv in sourceMaps)
        {
            var key = kv.Key;
            var src = kv.Value;
            if (!File.Exists(src)) continue;

            var fileName = Path.GetFileName(src);
            var dst = Path.Combine(absoluteOutputDir, fileName);
            File.Copy(src, dst, overwrite: true);

            // Asset path для импорта
            var assetPath = $"{assetsOutputDir}/{fileName}";
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);

            result[key] = assetPath;
        }

        AssetDatabase.Refresh();
        return result;
    }

    /// <summary>
    /// Копирует исходный albedo в Assets, если он вне проекта.
    /// Возвращает Asset-путь (Assets/...).
    /// </summary>
    public static string EnsureAlbedoInAssets(string albedoPath, string baseName)
    {
        var normalized = albedoPath.Replace("\\", "/");
        var dataPath = Application.dataPath.Replace("\\", "/");

        // Уже в Assets?
        if (normalized.StartsWith(dataPath))
        {
            return "Assets" + normalized.Substring(dataPath.Length);
        }

        // Копируем в Assets/Albedolizer_Output/<baseName>/
        var assetsDir = $"Assets/Albedolizer_Output/{baseName}";
        var absoluteDir = Path.Combine(Application.dataPath, "Albedolizer_Output", baseName);
        Directory.CreateDirectory(absoluteDir);

        var ext = Path.GetExtension(albedoPath);
        var dstFileName = $"{baseName}_albedo{ext}";
        var dst = Path.Combine(absoluteDir, dstFileName);
        File.Copy(albedoPath, dst, overwrite: true);

        var assetPath = $"{assetsDir}/{dstFileName}";
        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        AssetDatabase.Refresh();

        return assetPath;
    }

    // ═══════════════════════════════════════════════════════
    //  ЯДРО — запуск CLI
    // ═══════════════════════════════════════════════════════
    private static GenerateResult RunGenerateCore(
        string albedoPath,
        string outputDir,
        string preset,
        string correctMode,
        string aiModel,
        string mapsString,
        string engine,
        bool seamless,
        Action<int, string> onProgress)
    {
        var result = new GenerateResult();

        var cliPath = GetCliPath();
        if (string.IsNullOrEmpty(cliPath) || !File.Exists(cliPath))
        {
            result.error = "CLI not found. Reinstall the package.";
            return result;
        }

        if (!File.Exists(albedoPath))
        {
            result.error = $"Albedo not found: {albedoPath}";
            return result;
        }

        try { Directory.CreateDirectory(outputDir); }
        catch (Exception e) { result.error = $"Cannot create output dir: {e.Message}"; return result; }

        var reportPath = Path.Combine(outputDir, "report.json");

        var args = new StringBuilder();
        args.Append($"-i \"{albedoPath}\" ");
        args.Append($"-o \"{outputDir}\" ");
        args.Append($"--preset {preset} ");
        args.Append($"--maps \"{mapsString}\" ");
        args.Append($"--json-report \"{reportPath}\" ");

        switch (correctMode)
        {
            case "none": args.Append("--correct none "); break;
            case "ai":   args.Append($"--correct ai --ai-model {aiModel} "); break;
            case "math": args.Append("--correct math "); break;
        }

        if (seamless) args.Append("--seamless ");
        args.Append("--pbr ");
        if (!string.IsNullOrEmpty(engine)) args.Append($"--engine {engine} ");

        var psi = new ProcessStartInfo
        {
            FileName = cliPath,
            Arguments = args.ToString(),
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8,
        };

        Process process = null;
        try
        {
            process = Process.Start(psi);

            string line;
            while ((line = process.StandardOutput.ReadLine()) != null)
            {
                if (line.StartsWith("PROGRESS:"))
                {
                    var parts = line.Split(new[] { ':' }, 3);
                    if (parts.Length >= 2 && int.TryParse(parts[1], out int pct))
                    {
                        var stage = parts.Length >= 3 ? parts[2] : "";
                        onProgress?.Invoke(pct, stage);
                    }
                }
            }

            process.WaitForExit(600000);
        }
        catch (Exception e)
        {
            result.error = $"{e.GetType().Name}: {e.Message}";
            return result;
        }
        finally
        {
            try { process?.Dispose(); } catch { }
        }

        if (!File.Exists(reportPath))
        {
            result.error = $"CLI did not produce a report. Exit code: {process?.ExitCode}";
            return result;
        }

        try
        {
            var json = File.ReadAllText(reportPath);
            result.rawJson = json;
            result.ok = json.Contains("\"ok\": true") || json.Contains("\"ok\":true");
            result.outputDir = outputDir;

            int mapsStart = json.IndexOf("\"maps\"");
            if (mapsStart >= 0)
            {
                int openBrace = json.IndexOf('{', mapsStart);
                int closeBrace = json.IndexOf('}', openBrace);
                if (openBrace >= 0 && closeBrace > openBrace)
                {
                    var mapsBlock = json.Substring(openBrace + 1, closeBrace - openBrace - 1);
                    foreach (var pair in mapsBlock.Split(','))
                    {
                        var kv = pair.Split(new[] { ':' }, 2);
                        if (kv.Length == 2)
                        {
                            var key = kv[0].Trim().Trim('"');
                            var val = kv[1].Trim().Trim('"').Replace("\\\\", "\\").Replace("\\/", "/");
                            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(val))
                                result.maps[key] = val;
                        }
                    }
                }
            }

            if (!result.ok)
            {
                int msgIdx = json.IndexOf("\"message\"");
                if (msgIdx >= 0)
                {
                    int q1 = json.IndexOf('"', msgIdx + 10);
                    int q2 = json.IndexOf('"', q1 + 1);
                    if (q1 >= 0 && q2 > q1)
                        result.error = json.Substring(q1 + 1, q2 - q1 - 1);
                }
                if (string.IsNullOrEmpty(result.error))
                    result.error = "CLI reported failure";
            }

            return result;
        }
        catch (Exception e)
        {
            result.error = $"Report parse error: {e.Message}";
            return result;
        }
    }
}