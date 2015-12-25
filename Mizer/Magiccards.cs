using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mizer
{
    public class Magiccards : ISourceProvider
    {
        private const string SiteUrl = "http://magiccards.info";
        private const string SiteMap = "/sitemap.html";

        #region Implementation of ISourceProvider

        public bool IsBusy { get; private set; }

        public async Task<Set[]> ReadSetList()
        {
            IsBusy = true;
            var page = await Utilities.MakeWebRequest(SiteUrl + SiteMap);
            var sets = MatchSets(page).ToArray();
            IsBusy = false;
            return sets;
        }

        public async Task<Card[]> ReadCardList(Set set)
        {
            IsBusy = true;
            var url = GetSpoilerUrl(set);
            var page = await Utilities.MakeWebRequest(url);
            set.Cards = MatchCards(page).ToArray();
            IsBusy = false;
            return set.Cards;
        }

        #endregion

        private IEnumerable<Set> MatchSets(string page)
        {
            var matches = Regex.Matches(page, "<table.+</table>");

            foreach (Match match in matches)
            {
                var list = Regex.Matches(match.Value, "<li>(.*?)</li>");
                string currentBlock = null;
                foreach (Match set in list)
                {
                    var block = Regex.Match(set.Value, "(?<=<li>)[^<>]+(?=<ul>)");
                    if (block.Success)
                        currentBlock = block.Value;
                    var name = Regex.Match(set.Value, @"(?<=html"">)[^<>/\\]+(?=<)");
                    var url = Regex.Match(set.Value, "(?<=href=\")[^<>\"]+");
                    var code = Regex.Match(set.Value, "[a-z0-9]+(?=</small>)");
                    var lang = Regex.Match(url.Value, @"[^\./]+(?=\.html$)");
                    yield return new Set(currentBlock, name.Value, code.Value, lang.Value, url.Value);
                }
            }
        }

        private IEnumerable<Card> MatchCards(string page)
        {
            var matches = Regex.Matches(page, @"(?<=<td valign=""top"" width=""25%"">\s+).+?(?=\s+</td>)",
                RegexOptions.Singleline);

            foreach (Match match in matches)
            {
                var url = Regex.Match(match.Value, "(?<=<a href=\").+?(?=\">)");
                var name = Regex.Match(match.Value, "(?<=<a.+?>).+?(?=</a>)");
                var text = Regex.Match(match.Value, "(?<=<p class=\"ctext\"><b>).+?(?=</b></p>)",
                    RegexOptions.Singleline);
                var flavor = Regex.Match(match.Value, "(?<=<p><i>).+?(?=</i></p>)", RegexOptions.Singleline);
                var artist = Regex.Match(match.Value, @"(?<=<p>Illus\.\s+).+?(?=</p>)");
                var setRarity = Regex.Match(match.Value, @"(?<=<p><img.+?>\s+).+?(?=</p>)");
                var rarity = Regex.Match(setRarity.Value, "(?<=<i>).+?(?=</i>)");
                var head = Regex.Match(match.Value, @"(?<=<p>)(?!<i>|<img|Illus\.).+?(?=</p>)(?<!</i>)",
                    RegexOptions.Singleline);
                var type = Regex.Match(head.Value, @"[^\d\*]+?(?=\s[/\d*]+,|,|\s\(Loyalty)");
                var pt = Regex.Match(head.Value, @"[\d\*]+/[\d\*]");
                var power = Regex.Match(pt.Value, ".+?(?=/)");
                var toghness = Regex.Match(pt.Value, "(?<=/).+?");
                var loyalty = Regex.Match(head.Value, @"(?<=Loyalty:\s*)\d+(?=\))");
                var cmc = Regex.Match(head.Value, @"(?<=\()\d+(?=\))");
                var cost = Regex.Match(head.Value, @"(?<=,\s+)[\w\(\)]+(?=(\s+\()?)", RegexOptions.Singleline);
                var number = Regex.Match(url.Value, @"\w+(?=\.html)");

                var card = new Card();
                card.Url = url.Value;
                card.Name = name.Value;
                card.Rarity = rarity.Value;
                card.CollectionNumber = number.Value;
                card.Type = type.Value;
                card.Power = power.Value;
                card.Toughness = toghness.Value;
                card.Loyalty = loyalty.Value;
                card.ManaCost = cost.Value;
                card.ConvertedManaCost = cmc.Value;
                card.Text = Regex.Replace(Regex.Replace(text.Value, "(<br>|</br>)+", Environment.NewLine),
                    "<.+?>|(?<=>).+?(?=</)", string.Empty);
                card.Flavor = Regex.Replace(flavor.Value, "<.+?>|(?<=>).+?(?=</)", string.Empty);
                card.Artist = artist.Value;

                yield return card;
            }
        }

        private string GetSpoilerUrl(Set set)
        {
            return
                Uri.EscapeUriString(string.Format("{0}/query?q=++e:{1}/{2}&v=spoiler&s=issue", SiteUrl, set.Code,
                    set.Lang));
        }
    }
}