using System;
using System.Xml.Serialization;

namespace Mizer
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

        [XmlIgnore]
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

    [Serializable]
    public class Card
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string Rarity { get; set; }

        [XmlAttribute]
        public string CollectionNumber { get; set; }

        [XmlAttribute]
        public string Type { get; set; }

        [XmlAttribute]
        public string Power { get; set; }

        [XmlAttribute]
        public string Toughness { get; set; }

        [XmlAttribute]
        public string Loyalty { get; set; }

        [XmlAttribute]
        public string ManaCost { get; set; }

        [XmlAttribute]
        public string ConvertedManaCost { get; set; }

        [XmlAttribute]
        public string Text { get; set; }

        [XmlAttribute]
        public string Flavor { get; set; }

        [XmlAttribute]
        public string Artist { get; set; }

        [XmlAttribute]
        public string Url { get; set; }
    }
}