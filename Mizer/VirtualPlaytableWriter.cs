using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Mizer.VirtualPlaytable
{
    public class VirtualPlaytableWriter : IDataWriter
    {
        #region Implementation of IDataWriter

        public void WriteSets(IEnumerable<Set> sets)
        {
            throw new NotImplementedException();
        }

        public void WriteCards(Set set)
        {
            using (var stream = File.CreateText(string.Format("{0}.{1}.vpt.xml", set.Name, set.Lang)))
            {
                var ser = new XmlSerializer(typeof (Items));
                ser.Serialize(stream, new Items(set));
            }
        }

        #endregion
    }

    [XmlRoot("items"), Serializable]
    public class Items : Set, IXmlSerializable
    {
        public Item[] items;

        public Items(Set set)
        {
            Code = set.Code;
            Name = set.Name;
            Lang = set.Lang;
            items = set.Cards.Select(c => new Item(c, set)).ToArray();
        }

        protected Items()
        {
        }

        #region Implementation of IXmlSerializable

        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("game", "mtg");
            writer.WriteAttributeString("set", Code.ToUpper());
            writer.WriteAttributeString("lang", Lang.ToUpper());
            writer.WriteAttributeString("setname", Name);
            writer.WriteAttributeString("imagepath", Name);
            writer.WriteAttributeString("date", "");
            writer.WriteAttributeString("border", "Black");
            writer.WriteAttributeString("copyright", "™ &amp; © 2015 Wizards of the Coast");
            foreach (var item in items)
            {
                var ser = new XmlSerializer(typeof (Item));
                ser.Serialize(writer, item);
            }
        }

        #endregion
    }

    [Serializable, XmlRoot("item")]
    public class Item : Card, IXmlSerializable
    {
        protected Set set;

        public bool IsLand
        {
            get { return Regex.IsMatch(Type, @"\bLand\b", RegexOptions.IgnoreCase); }
        }

        public bool IsCreature
        {
            get { return Regex.IsMatch(Type, @"\bCreature\b", RegexOptions.IgnoreCase); }
        }

        public bool IsPlanesWalker
        {
            get { return Regex.IsMatch(Type, @"\bPlaneswalker\b", RegexOptions.IgnoreCase); }
        }

        public Item(Card card, Set set)
        {
            this.set = set;
            Name = card.Name;
            Rarity = card.Rarity;
            CollectionNumber = card.CollectionNumber;
            Type = card.Type;
            Power = card.Power;
            Toughness = card.Toughness;
            Loyalty = card.Loyalty;
            ManaCost = card.ManaCost;
            ConvertedManaCost = card.ConvertedManaCost;
            Text = card.Text;
            Flavor = card.Flavor;
            Artist = card.Artist;
        }

        protected Item()
        {
        }

        #region Implementation of IXmlSerializable

        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("id", Name);
            if (IsLand)
                writer.WriteAttributeString("ver", GetVer(this));
            writer.WriteAttributeString("name", Name);
            writer.WriteAttributeString("color", GetColor(this));
            writer.WriteAttributeString("cost", GetFormatedManaCost(this));
            writer.WriteAttributeString("cmc", ConvertedManaCost);
            writer.WriteAttributeString("type", Type);
            writer.WriteAttributeString("types", GetFormatedType(this));
            if (IsCreature)
            {
                writer.WriteAttributeString("power", Power);
                writer.WriteAttributeString("toughness", Toughness);
            }
            else if (IsPlanesWalker)
                writer.WriteAttributeString("toughness", Loyalty);
            var mana = GetMana(this);
            if (!string.IsNullOrEmpty(mana))
                writer.WriteAttributeString("mana", mana);
            writer.WriteAttributeString("text", GetFormatedText(this));
            writer.WriteAttributeString("flavor", Flavor);
            writer.WriteAttributeString("artist", Artist);
            writer.WriteAttributeString("number", GetFormatedNumber(this, set));
            writer.WriteAttributeString("booster", "True");
            writer.WriteAttributeString("foil", "false|true");
        }

        private string GetMana(Card card)
        {
            var matches = Regex.Matches(card.Text, @"(?<=Add.+?)\{[\dRUBGW]}(?=.+?)");
            if (matches.Count == 0)
                return null;
            var mana = matches.Cast<Match>().Aggregate(string.Empty, (current, match) => current + match.Value);
            var color = Color.Colorless;
            if (Regex.IsMatch(mana, "B"))
                color = Color.Black | color;
            if (Regex.IsMatch(mana, "U"))
                color = Color.Blue | color;
            if (Regex.IsMatch(mana, "G"))
                color = Color.Green | color;
            if (Regex.IsMatch(mana, "R"))
                color = Color.Red | color;
            if (Regex.IsMatch(mana, "W"))
                color = Color.White | color;
            return Regex.Replace(color.ToString(), @"\|,", " ");
        }

        private string GetVer(Card card)
        {
            return "1";
        }

        #endregion

        private string GetColor(Card card)
        {
            if (Regex.IsMatch(card.Text, "This card has no color"))
                return Color.Colorless.ToString();
            var color = Color.Colorless;
            if (Regex.IsMatch(card.ManaCost, "B"))
                color = Color.Black | color;
            if (Regex.IsMatch(card.ManaCost, "U"))
                color = Color.Blue | color;
            if (Regex.IsMatch(card.ManaCost, "G"))
                color = Color.Green | color;
            if (Regex.IsMatch(card.ManaCost, "R"))
                color = Color.Red | color;
            if (Regex.IsMatch(card.ManaCost, "W"))
                color = Color.White | color;
            return Regex.Replace(color.ToString(), @"\|,", " ");
        }

        private string GetFormatedManaCost(Card card)
        {
            var matches = Regex.Matches(card.ManaCost, @"(?<=\{)[RGBUW]/[RGBUW](?=\})|[RGBUWX]|[\d]+");
            if (matches.Count == 0)
                return string.Empty;
            var sb = new StringBuilder();
            foreach (Match match in matches)
                sb.AppendFormat("{{{0}}}", Regex.Replace(match.Value, "/", string.Empty));
            return sb.ToString();
        }

        private string GetFormatedType(Card card)
        {
            return Regex.Replace(card.Type, @"\s*—\s*", " ");
        }

        private string GetFormatedText(Card card)
        {
            return Regex.Replace(card.Text, @"(?<=\{[RGBUW])/(?=[RGBUW]\})", string.Empty);
        }

        private string GetFormatedNumber(Card card, Set set)
        {
            return string.Format("{0}/{1}", Regex.Match(card.CollectionNumber, @"[\d]+").Value, set.Cards.Length);
        }

        [Flags]
        public enum Color
        {
            Colorless = 0,
            Black = 1,
            Blue = 2,
            Green = 4,
            Red = 8,
            White = 16
        }
    }
}