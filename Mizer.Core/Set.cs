using System;
using System.Xml.Serialization;

namespace Mizer.Core
{
    [Serializable]
    public class Set
    {
        [XmlAttribute]
        public string Block { get; set; }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string Code { get; set; }

        [XmlAttribute]
        public string Lang { get; set; }

        [XmlAttribute]
        public string URL { get; set; }

        public Card[] Cards { get; set; }

        public Set(string block, string name, string code, string lang, string url)
        {
            Block = block;
            Name = name;
            Code = code;
            Lang = lang;
            URL = url;
        }

        protected Set()
        {
        }
    }
}