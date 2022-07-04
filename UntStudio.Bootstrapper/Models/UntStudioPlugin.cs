using System.Xml.Serialization;

namespace UntStudio.Bootstrapper.Models
{
    public class UntStudioPlugin
    {
        public UntStudioPlugin(string name, bool enabled)
        {
            Name = name;
            Enabled = enabled;
        }

        public UntStudioPlugin()
        {
        }



        [XmlAttribute]
        public string Name;

        [XmlAttribute]
        public bool Enabled;
    }
}
