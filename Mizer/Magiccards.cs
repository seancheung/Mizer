using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mizer
{
    public class Magiccards : ISourceProvider
    {
        private const string SiteUrl = "http://magiccards.info/sitemap.html";

        #region Implementation of ISourceProvider

        public bool IsBusy { get; private set; }

        public async Task<Set[]> ReadSetList()
        {
            IsBusy = true;
            var page = await Utilities.MakeWebRequest(SiteUrl);
            IsBusy = false;
            return ParsePage(page).ToArray();
        }

        public async Task<Card[]> ReadCardList(Set set)
        {
            throw new NotImplementedException();
        }

        #endregion

        private IEnumerable<Set> ParsePage(string page)
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
                    yield return new Set(currentBlock, name.Value, code.Value, url.Value);
                }
            }
        }
    }
}