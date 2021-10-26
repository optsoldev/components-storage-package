using Optsol.Components.Storage.Exception;

namespace Optsol.Components.Storage.Settings
{
    public class StorageSettings
    {
        public string ConnectionString { get; set; }

        public void Validate()
        {
            if (string.IsNullOrEmpty(ConnectionString))
            {
                ShowingException(nameof(ConnectionString));
            }
        }

        public static void ShowingException(string objectName)
        {
            throw new SettingsNullException(objectName);
        }
    }
}