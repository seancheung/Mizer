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
                http.Encoding = Encoding.UTF8;
                return await http.DownloadStringTaskAsync(new Uri(url));
            }
        }

        public static async Task<byte[]> DownloadBytes(string url)
        {
            using (var http = new WebClient())
            {
                http.Encoding = Encoding.UTF8;
                return await http.DownloadDataTaskAsync(new Uri(url));
            }
        }

        public static async Task DownloadFile(string url, string path)
        {
            using (var http = new WebClient())
            {
                http.Encoding = Encoding.UTF8;
                await http.DownloadFileTaskAsync(new Uri(url), path);
            }
        }
    }
}