using System.Runtime.Serialization;

namespace Optsol.Components.Storage.Exception
{
    public class SettingsNullException : System.Exception
    {
        public SettingsNullException(string objectName)
            : base($"O Atributo {objectName} está nulo")
        {
        }

        protected SettingsNullException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}