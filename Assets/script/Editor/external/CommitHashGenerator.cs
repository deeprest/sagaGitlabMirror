using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace NekomimiDaimao
{
    /// <summary>
    /// generate commit hash.
    /// add generated to .gitignore
    /// 
    /// 1. Create -> CommitHash
    /// 2. Build
    /// 
    /// https://gist.github.com/nekomimi-daimao/0e987de15a6cd84995319c19ba077fe9
    /// </summary>
    public sealed class CommitHashGenerator : IPreprocessBuildWithReport
    {
        #region GenerateCode

        // replace if necessary
        private const string FilePath = "Assets/script/generated";

        private const string FileName = "Commit.cs";

        private const string CodeTemplate = @"
public static class Commit
{
    public const string Hash = ""#COMMIT_HASH#"";
}
";

        private const string ReplaceHash = "#COMMIT_HASH#";

        private const string KeyCommitHash = "KeyCommitHash";

        public static void GenerateScript(string hash)
        {
            var fullPath = Path.Combine(FilePath, FileName);

            var current = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(fullPath);
            if (current == null)
            {
                var dirs = FilePath.Split('/');
                for (var count = 0; count < dirs.Length; count++)
                {
                    var dir = Path.Combine(dirs.Take(count + 1).ToArray());
                    if (AssetDatabase.IsValidFolder(dir))
                    {
                        continue;
                    }

                    AssetDatabase.CreateFolder(Path.Combine(dirs.Take(count).ToArray()), dirs[count]);
                }
            }

            File.WriteAllText(fullPath, CodeTemplate.Replace(ReplaceHash, hash));
            AssetDatabase.Refresh();
        }

        private static async Task CheckAndGenerateAsync()
        {
            var hash = await CheckCommitHashAsync();
            if (string.IsNullOrEmpty(hash)
                || string.Equals(hash, EditorUserSettings.GetConfigValue(KeyCommitHash)))
            {
                return;
            }

            GenerateScript(hash);

            if (AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(Path.Combine(FilePath, FileName)) != null)
            {
                EditorUserSettings.SetConfigValue(KeyCommitHash, hash);
            }
        }

        [MenuItem("Assets/Create/CommitHash")]
        private static void ForceGenerate()
        {
            EditorUserSettings.SetConfigValue(KeyCommitHash, null);
#pragma warning disable 4014
            CheckAndGenerateAsync();
#pragma warning restore 4014 
        }

        #endregion

        #region CommitHash

        public static Task<string> CheckCommitHashAsync()
        {
            var psi = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = "rev-parse --short HEAD",

                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                WorkingDirectory = Application.dataPath
            };

            try
            {
                var p = Process.Start(psi);
                var tcs = new TaskCompletionSource<string>();
                p.EnableRaisingEvents = true;
                p.Exited += (object sender, System.EventArgs e) =>
                {
                    var data = p.StandardOutput.ReadToEnd().Trim();
                    p.Dispose();
                    p = null;

                    tcs.TrySetResult(data);
                };

                return tcs.Task;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
                return Task.FromResult<string>(null);
            }
        }

        #endregion

        #region IPreprocessBuildWithReport

        public int callbackOrder => 0;

        public async void OnPreprocessBuild(BuildReport report)
        {
            await CheckAndGenerateAsync();
        }

        #endregion
    }
}
