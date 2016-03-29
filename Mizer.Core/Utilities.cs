using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Mizer
{
    public static class Utilities
    {
        public static async Task<string> DownloadString(string url)
        {
            using (var http = new WebClient())
            {
                http.Headers["User-Agent"] = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E)";
                http.Encoding = Encoding.UTF8;
                return await http.DownloadStringTaskAsync(new Uri(url));
            }
        }

        public static async Task<byte[]> DownloadBytes(string url)
        {
            using (var http = new WebClient())
            {
                http.Headers["User-Agent"] = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E)";
                http.Encoding = Encoding.UTF8;
                return await http.DownloadDataTaskAsync(new Uri(url));
            }
        }

        public static async Task DownloadFile(string url, string path)
        {
            using (var http = new WebClient())
            {
                http.Headers["User-Agent"] = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E)";
                http.Encoding = Encoding.UTF8;
                await http.DownloadFileTaskAsync(new Uri(url), path);
            }
        }
    }
}