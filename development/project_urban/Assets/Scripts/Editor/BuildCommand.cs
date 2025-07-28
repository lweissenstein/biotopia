#nullable enable
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;

namespace Editor
{
    // ReSharper disable once UnusedType.Global
    public static class BuildCommand
    {
        // those are environment variable names.
        // the variables are exported in .gitlab-ci.yml and in the .sh scripts in development/ci/
        // ReSharper disable InconsistentNaming
        private const string ANDROID_KEYSTORE_PASS = "ANDROID_KEYSTORE_PASS";
        private const string ANDROID_KEY_PASS = "ANDROID_KEY_PASS";
        private const string ANDROID_KEY_ALIAS = "ANDROID_KEY_ALIAS";
        private const string ANDROID_KEYSTORE = "ANDROID_KEYSTORE";
        private const string BUILD_OPTIONS = "BuildOptions";
        private const string ANDROID_VERSION_CODE = "ANDROID_VERSION_CODE";
        private const string ANDROID_VERSION_NAME = "ANDROID_VERSION_NAME";
        private const string ANDROID_BUILD_APP_BUNDLE = "ANDROID_BUILD_APP_BUNDLE";
        // ReSharper restore InconsistentNaming

        // ReSharper disable once UnusedMember.Global
        public static void PerformBuild()
        {
#if !UNITY_2018_3_OR_NEWER
            throw Exception("no");
#endif
#if !UNITY_2019_1_OR_NEWER
            throw Exception("not doing it");
#endif
            Console.WriteLine(":: Performing build");

            // settings
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP); // TODO check
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel29; // not sure which one to target
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel35;
            PlayerSettings.applicationIdentifier = "com.abductedrhino.biotopia";
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;

            // those are app bundle settings (.aab)
            if (bool.Parse(GetEnvOrThrow(ANDROID_BUILD_APP_BUNDLE)))
            {
                PlayerSettings.bundleVersion = GetEnvOrThrow(ANDROID_VERSION_NAME);
                EditorUserBuildSettings.buildAppBundle = bool.Parse(GetEnvOrThrow(ANDROID_BUILD_APP_BUNDLE));
                PlayerSettings.Android.bundleVersionCode = int.Parse(GetEnvOrThrow(ANDROID_VERSION_CODE));
                PlayerSettings.Android.useCustomKeystore = true;
                if (!File.Exists(GetEnvOrThrow(ANDROID_KEYSTORE)))
                    throw new Exception($"android keystore is missing: \"{GetEnvOrThrow(ANDROID_KEYSTORE)}\"");
                PlayerSettings.Android.keystoreName = GetEnvOrThrow(ANDROID_KEYSTORE);
                PlayerSettings.Android.keyaliasName = GetEnvOrThrow(ANDROID_KEY_ALIAS);
                PlayerSettings.Android.keystorePass = GetEnvOrThrow(ANDROID_KEYSTORE_PASS);
                PlayerSettings.Android.keyaliasPass = GetEnvOrThrow(ANDROID_KEY_PASS);
            }

            var buildReport = BuildPipeline.BuildPlayer(
                levels: EnabledScenes(),
                locationPathName: GetArgumentOrThrow("customBuildPath") +
                                  GetArgumentOrThrow("customBuildName") +
                                  (EditorUserBuildSettings.buildAppBundle ? ".aab" : ".apk"),
                target: BuildTarget.Android,
                options: BuildOptions()
            );

            if (buildReport.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
                throw new Exception($"Build ended with {buildReport.summary.result} status");

            Console.WriteLine(":: Done with build");
        }

        private static string GetArgumentOrThrow(string name)
            => GetArgument(name) ?? throw new Exception($"missing command line argument: \"{name}\"");

        private static string? GetArgument(string name)
        {
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Contains(name))
                {
                    return args[i + 1];
                }
            }

            return null;
        }

        private static string[] EnabledScenes()
            => (
                from scene in EditorBuildSettings.scenes
                where scene.enabled
                where !string.IsNullOrEmpty(scene.path)
                select scene.path
            ).ToArray();

        private static BuildOptions BuildOptions()
        {
            if (TryGetEnv(BUILD_OPTIONS, out string envVar))
            {
                string[] allOptionVars = envVar.Split(',');
                BuildOptions allOptions = UnityEditor.BuildOptions.None;
                BuildOptions option;
                string optionVar;
                int length = allOptionVars.Length;

                Console.WriteLine($":: Detecting {BUILD_OPTIONS} env var with {length} elements ({envVar})");

                for (int i = 0; i < length; i++)
                {
                    optionVar = allOptionVars[i];

                    if (optionVar.TryConvertToEnum(out option))
                    {
                        allOptions |= option;
                    }
                    else
                    {
                        Console.WriteLine(
                            $":: Cannot convert {optionVar} to {nameof(UnityEditor.BuildOptions)} enum, skipping it.");
                    }
                }

                return allOptions;
            }

            return UnityEditor.BuildOptions.None;
        }

        // https://stackoverflow.com/questions/1082532/how-to-tryparse-for-enum-value
        private static bool TryConvertToEnum<TEnum>(this string strEnumValue, out TEnum? value)
        {
            if (!Enum.IsDefined(typeof(TEnum), strEnumValue))
            {
                value = default;
                return false;
            }

            value = (TEnum) Enum.Parse(typeof(TEnum), strEnumValue);
            return true;
        }

        private static string GetEnvOrThrow(string key)
            => Environment.GetEnvironmentVariable(key) ??
               throw new Exception($"missing environment variable: \"{key}\"");

        private static bool TryGetEnv(string key, out string? value)
        {
            value = Environment.GetEnvironmentVariable(key);
            return !string.IsNullOrEmpty(value);
        }
    }
}
