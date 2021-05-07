using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Ktyl.Util
{
    public static class BuildCommand
    {
        private static string BuildsPath => Path
            .Combine(Application.dataPath, "..", "Build")
            .MustExist();

        private static string BuildName => Application.productName;
        
        public static void Run()
        {
            // TODO: check if can build
            
            // TODO: pre-process
            BuildsPath.ClearDirectory();
            
            // TODO: build
            var path = Path.Combine(BuildsPath, $"{BuildName}.exe");
            Debug.Log(path);
            
            var scenes = EditorBuildSettings.scenes.Select(s => s.path).ToArray();
            Debug.Log(string.Join(", ", scenes));

            BuildOptions buildOptions = default;
            if (EditorUserBuildSettings.development)
            {
                buildOptions |= BuildOptions.Development;
            }
            
            var options = new BuildPlayerOptions
            {
                scenes = scenes,
                // TODO: handle platforms that arent windows
                locationPathName = path,
                options = buildOptions,
                target = BuildTarget.StandaloneWindows64
            };

            BuildPipeline.BuildPlayer(options);

            // TODO: post-process
        }
    }
}