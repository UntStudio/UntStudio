using System.Xml.Serialization;

namespace UntStudio.Bootstrapper.Models
{
    internal class Plugin
    {
        public Plugin(string name, bool enabled)
        {
            Name = name;
            Enabled = enabled;
        }

        public Plugin()
        {
        }



        [XmlAttribute]
        public string Name;

        [XmlAttribute]
        public bool Enabled;
    }
}
