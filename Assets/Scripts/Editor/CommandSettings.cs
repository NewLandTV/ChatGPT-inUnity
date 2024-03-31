using UnityEditor;

namespace ChatGPTInUnity
{
    [FilePath("UserSettings/CommandSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public sealed class CommandSettings : ScriptableSingleton<CommandSettings>
    {
        public string apiKey = string.Empty;
        public int timeout = 0;

        private void OnDisable()
        {
            Save();
        }

        public void Save()
        {
            Save(true);
        }
    }

    public sealed class CommandSettingsProvider : SettingsProvider
    {
        public CommandSettingsProvider() : base("Project/Command", SettingsScope.Project) { }

        public override void OnGUI(string searchContext)
        {
            CommandSettings settings = CommandSettings.instance;

            string key = settings.apiKey;
            int timeout = settings.timeout;

            EditorGUI.BeginChangeCheck();

            key = EditorGUILayout.TextField("API Key", key);
            timeout = EditorGUILayout.IntField("Timeout", timeout);

            if (EditorGUI.EndChangeCheck())
            {
                settings.apiKey = key;
                settings.timeout = timeout;

                settings.Save();
            }
        }

        [SettingsProvider]
        public static SettingsProvider CreatecustomSettingsProvider()
        {
            return new CommandSettingsProvider();
        }
    }
}