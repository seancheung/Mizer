using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Mizer
{
    public static class Utilities
    {
        public static async Task<string> MakeWebRequest(string url)
        {
            using (var http = new WebClient())
            {
                http.Encoding = Encoding.UTF8;
                return await http.DownloadStringTaskAsync(new Uri(url));
            }
        }
    }
}