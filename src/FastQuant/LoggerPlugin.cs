using System.Xml.Serialization;

namespace FastQuant
{
    public class LoggerPlugin
    {
        public LoggerPlugin(string typeName)
        {
            TypeName = typeName;
        }

        public override string ToString() => TypeName;

        [XmlElement("TypeName")]
        public string TypeName { get; set; }
    }
}