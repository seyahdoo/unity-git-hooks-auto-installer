using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GitHooksInstaller {
    public static class GitHooksInstaller {
        private const string REMEMBER_KEY = "git_hooks_installer_remembered_choice";
        private const string DESTINATION_SEARCH_PATH = ".git/";
        private const string DESTINATION_PARENT_PATH = ".git/hooks/";
        private const string MODULE_FOLDER_NAME = "GitHooks";
        private const string HOOKS_FOLDER_PATH_IN_MODULE = "Hooks";
        [InitializeOnLoadMethod]
        private static void OnLoad() {
            if (RememberedChoice == RememberChoice.None && InstallNeeded) {
                GitHooksInstallWindow.OpenWindow();
            }
            if (RememberedChoice == RememberChoice.Install && InstallNeeded) {
                Install();
            }
        }
        internal enum RememberChoice {
            None = 0,
            DoNotInstall = 1,
            Install = 2,
        }
        internal static RememberChoice RememberedChoice {
            get => (RememberChoice) EditorPrefs.GetInt(REMEMBER_KEY);
            set => EditorPrefs.SetInt(REMEMBER_KEY, (int) value);
        }
        private static bool InstallNeeded {
            get {
                int sourceVersion;
                try {
                    sourceVersion = int.Parse(File.ReadAllText(Path.Combine(GetSourceHooksPath(), "version.txt")));
                }
                catch (Exception) {
                    return false;
                }

                int destinationVersion = 0;
                try {
                    destinationVersion = int.Parse(File.ReadAllText(Path.Combine(DESTINATION_PARENT_PATH, "version.txt")));
                }
                catch (Exception) {
                    /* ignored */
                }

                return sourceVersion > destinationVersion;
            }
        }
        internal static void Install() {
            var hooksDirectoriesList = FindHooksDirectories();
            foreach (var directory in hooksDirectoriesList) {
                // Clear Hooks Path Directory
                DeleteDirectory(directory);
                // Copy Source Hooks Path to Hooks Path
                CopyDirectoryWithoutMeta(new DirectoryInfo(GetSourceHooksPath()), directory);
                Debug.Log($"installed to {directory}");
            }
            Debug.Log("hooks installed");
        }
        public static void Remove() {
            var hooksDirectoriesList = FindHooksDirectories();
            foreach (var directory in hooksDirectoriesList) {
                // Clear Hooks Path Directory
                DeleteDirectory(directory);
                directory.Create();
                Debug.Log($"hooks removed from {directory}");
            }
            Debug.Log("hooks removed");
        }
        private static List<DirectoryInfo> FindHooksDirectories() {
            var hooksDirectoriesList = new List<DirectoryInfo>();
            RecursivelyAddHookFolder(hooksDirectoriesList, new DirectoryInfo(DESTINATION_SEARCH_PATH));
            return hooksDirectoriesList;
        }
        private static void RecursivelyAddHookFolder(List<DirectoryInfo> list, DirectoryInfo path) {
            if (path.Name == "hooks") {
                list.Add(path);
                return;
            }
            foreach (var directory in path.GetDirectories()) {
                RecursivelyAddHookFolder(list, directory);
            }
        }
        private static void DeleteDirectory(DirectoryInfo directory) {
            try { directory.Delete(true); } catch (Exception) { /* ignored */ }
        }
        private static void CopyDirectoryWithoutMeta(DirectoryInfo source, DirectoryInfo target) {
            Directory.CreateDirectory(target.FullName);
            // Copy each file into the new directory.
            foreach (var fileInfo in source.GetFiles()) {
                if (!fileInfo.Name.EndsWith(".meta")) {
                    fileInfo.CopyTo(Path.Combine(target.FullName, fileInfo.Name), true);
                }
            }
            // Copy each subdirectory using recursion.
            foreach (var diSourceSubDir in source.GetDirectories()) {
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                CopyDirectoryWithoutMeta(diSourceSubDir, nextTargetSubDir);
            }
        }
        private static string GetSourceHooksPath() {
            var guids = AssetDatabase.FindAssets($"t:folder {MODULE_FOLDER_NAME}");
            foreach (var guid in guids) {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.EndsWith(MODULE_FOLDER_NAME)) {
                    return Path.Combine(path, HOOKS_FOLDER_PATH_IN_MODULE);
                }
            }
            return null;
        }
    }
}


