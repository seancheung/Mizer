using System;
using System.Xml.Serialization;

namespace Mizer.Core
{
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