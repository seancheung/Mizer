using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Mizer.Core;

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
            if (set.Lang != "en" && File.Exists(string.Format("{0}.en.xml", set.Code.ToLower())))
            {
                var eng = File.ReadAllText(string.Format("{0}.en.xml", set.Code.ToLower()));
                var engSet = (Set) new XmlSerializer(typeof (Set)).Deserialize(new StringReader(eng));
                using (
                    var stream =
                        File.CreateText(string.Format("{0}.{1}.vpt.xml", engSet.Name,
                            set.Lang.Replace("cn", "cs").Replace("tw", "ct").Replace("es", "sp").Replace("ko", "kr"))))
                {
                    var ser = new XmlSerializer(typeof (Items));
                    ser.Serialize(stream, new Items(set, engSet));
                }
            }
            else
            {
                using (var stream = File.CreateText(string.Format("{0}.{1}.vpt.xml", set.Name, set.Lang)))
                {
                    var ser = new XmlSerializer(typeof (Items));
                    ser.Serialize(stream, new Items(set, set));
                }
            }
        }

        #endregion
    }

    [XmlRoot("items"), Serializable]
    public class Items : Set, IXmlSerializable
    {
        public Set engSet;
        public Item[] items;

        public Items(Set set, Set engSet)
        {
            this.engSet = engSet;
            Code = set.Code;
            Name = set.Name;
            Lang = set.Lang;
            items = new Item[set.Cards.Length];
            for (var i = 0; i < items.Length; i++)
                items[i] = new Item(set.Cards[i], engSet.Cards[i], set);
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
            writer.WriteAttributeString("set", engSet.Code.ToUpper());
            writer.WriteAttributeString("lang",
                Lang.Replace("cn", "cs").Replace("tw", "ct").Replace("es", "sp").Replace("ko", "kr").ToUpper());
            writer.WriteAttributeString("setname", engSet.Name);
            writer.WriteAttributeString("imagepath", engSet.Name);
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
        public Card engCard;
        protected Set set;

        public bool IsLand
        {
            get { return Regex.IsMatch(engCard.Type, @"\bLand\b", RegexOptions.IgnoreCase); }
        }

        public bool IsCreature
        {
            get { return Regex.IsMatch(engCard.Type, @"\bCreature\b", RegexOptions.IgnoreCase); }
        }

        public bool IsPlanesWalker
        {
            get { return Regex.IsMatch(engCard.Type, @"\bPlaneswalker\b", RegexOptions.IgnoreCase); }
        }

        public Item(Card card, Card engCard, Set set)
        {
            this.engCard = engCard;
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
            writer.WriteAttributeString("id", engCard.Name);
            if (IsLand)
                writer.WriteAttributeString("ver", GetVer(engCard));
            writer.WriteAttributeString("name", Name);
            writer.WriteAttributeString("color", GetColor(engCard));
            writer.WriteAttributeString("cost", GetFormatedManaCost(engCard));
            writer.WriteAttributeString("cmc", engCard.ConvertedManaCost);
            writer.WriteAttributeString("type", Type);
            writer.WriteAttributeString("types", GetFormatedType(engCard));
            if (IsCreature)
            {
                writer.WriteAttributeString("power", engCard.Power);
                writer.WriteAttributeString("toughness", engCard.Toughness);
            }
            else if (IsPlanesWalker)
                writer.WriteAttributeString("toughness", engCard.Loyalty);
            var mana = GetMana(engCard);
            if (!string.IsNullOrEmpty(mana))
                writer.WriteAttributeString("mana", mana);
            writer.WriteAttributeString("text", GetFormatedText(this));
            writer.WriteAttributeString("flavor", Flavor);
            writer.WriteAttributeString("artist", engCard.Artist);
            writer.WriteAttributeString("number", GetFormatedNumber(engCard, set));
            writer.WriteAttributeString("booster", "True");
            writer.WriteAttributeString("rarity", engCard.Rarity);
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