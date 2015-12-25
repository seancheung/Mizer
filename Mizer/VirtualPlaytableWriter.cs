using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Mizer
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
            throw new NotImplementedException();
        }

        #endregion

        private Color GetColor(Card card)
        {
            if (Regex.IsMatch(card.Text, "This card has no color"))
                return Color.Colorless;
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
            return color;
        }

        private string GetFormatedManaCost(Card card)
        {
            var matches = Regex.Matches(card.ManaCost, @"(?<=\{)[RGBUW]/[RGBUW](?=\})|[\dRGBUWX]");
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
            return string.Format("{0}/{1}", card.CollectionNumber, set.Cards.Length);
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