using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.IO;

namespace Arks_SystemTool
{
    public class Requests
    {
        static readonly public String UserAgent = "AQUA_HTTP";

        static public String Get(String url)
        {
            String data = String.Empty;
            using (WebClient web = new WebClient())
            {
                web.Headers.Add("User-Agent: " + UserAgent);
                try
                {
                    data = web.DownloadString(url);
                }
                catch (WebException we)
                {
                    Console.WriteLine(we.Message);
                }
            }
            return (data);
        }

        static public void Download(String url, String path)
        {
            if (!Directory.Exists(Directory.GetParent(path).FullName))
                Directory.CreateDirectory(Directory.GetParent(path).FullName);
            using (WebClient web = new WebClient())
            {
                web.Headers.Add("User-Agent: " + UserAgent);
                web.DownloadFile(url, path);
            }
        }
    }
}
