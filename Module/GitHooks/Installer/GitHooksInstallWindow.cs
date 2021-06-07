using UnityEditor;
using UnityEngine;

namespace GitHooksInstaller {
    public class GitHooksInstallWindow : EditorWindow {
        private bool _rememberMyChoiceToggle = true;
        private void OnGUI() {
            GUILayout.Label("Do you want to install git hooks?", EditorStyles.boldLabel);
            var oldColor = GUI.color;
            GUI.color = Color.red;
                if (GUILayout.Button("Do Not Install", GUILayout.Width(200))){
                    if (_rememberMyChoiceToggle) {
                        GitHooksInstaller.RememberedChoice = GitHooksInstaller.RememberChoice.DoNotInstall;
                    }
                    Close();
                }
            GUI.color = Color.yellow;
                if (GUILayout.Button("Remove", GUILayout.Width(200))) {
                    GitHooksInstaller.RememberedChoice = GitHooksInstaller.RememberChoice.None;
                    GitHooksInstaller.Remove();
                    Close();
                }
            GUI.color = Color.green;
                if (GUILayout.Button("Install", GUILayout.Width(200))) {
                    if (_rememberMyChoiceToggle) {
                        GitHooksInstaller.RememberedChoice = GitHooksInstaller.RememberChoice.Install;
                    }
                    GitHooksInstaller.Install();
                    Close();
                }
            GUI.color = oldColor;
            _rememberMyChoiceToggle = GUILayout.Toggle(_rememberMyChoiceToggle, "Remember My Choice");
        }
        [MenuItem("Window/Git Hooks Install Window", false, 100000)]
        internal static void OpenWindow() {
            EditorWindow window = EditorWindow.FindObjectOfType<GitHooksInstallWindow>();
            if (window == null) {
                window = EditorWindow.GetWindow<GitHooksInstallWindow>(true, nameof(GitHooksInstallWindow));
            }
            window.Show();
        }
    }
}
