using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ChatGPTInUnity
{
    public sealed class CommandWindow : EditorWindow
    {
        // Constants
        private const string TEMP_FILE_PATH = "Assets/CommandTemp.cs";
        private const string API_KEY_ERROR_TEXT = "API Key hasn't been set. Please chcek the project settings (Edit > Project Settings > Command > API Key).";

        // Flags
        private bool tempFileExists = File.Exists(TEMP_FILE_PATH);

        private bool IsAPIKeyOk => !string.IsNullOrEmpty(CommandSettings.instance.apiKey);

        // Variables
        private string prompt = "Create 100 cubes at random points.";

        private void OnGUI()
        {
            if (!IsAPIKeyOk)
            {
                EditorGUILayout.HelpBox(API_KEY_ERROR_TEXT, MessageType.Error);

                return;
            }

            prompt = EditorGUILayout.TextArea(prompt, GUILayout.ExpandHeight(true));

            if (GUILayout.Button("Run"))
            {
                RunGenerator();
            }
        }

        private void OnEnable()
        {
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        }

        private void OnDisable()
        {
            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
        }

        private void OnAfterAssemblyReload()
        {
            if (!tempFileExists)
            {
                return;
            }

            EditorApplication.ExecuteMenuItem("Edit/Do Task");
            AssetDatabase.DeleteAsset(TEMP_FILE_PATH);
        }

        [MenuItem("Window/Command")]
        private static void Init()
        {
            GetWindow<CommandWindow>(true, "Command");
        }

        // Temporary script file operations
        private void CreateScriptAsset(string code)
        {
            BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic;

            MethodInfo method = typeof(ProjectWindowUtil).GetMethod("CreateScriptAssetWithContent", flags);

            method.Invoke(null, new object[] { TEMP_FILE_PATH, code });
        }

        // Script generator
        private static string WrapPrompt(string input)
            => "Write a Unity Editor script.\n" +
               " - It provides its functionality as a menu item placed \"Edit\" > \"Do Task\".\n" +
               " - It doesn¡¯t provide any editor window. It immediately does the task when the menu item is invoked.\n" +
               " - Don¡¯t use GameObject.FindGameObjectsWithTag.\n" +
               " - There is no selected object. Find game objects manually.\n" +
               " - I only need the script body. Don¡¯t add any explanation.\n" +
               "The task is described as follows:\n" + input;

        private void RunGenerator()
        {
            string code = OpenAI.OpenAIUtil.InvokeChat(WrapPrompt(prompt));

            Debug.Log($"Command script : {code}");

            CreateScriptAsset(code);
        }
    }
}