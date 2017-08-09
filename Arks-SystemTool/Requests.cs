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
        static public String Get(String url)
        {
            String data = String.Empty;
            using (WebClient web = new WebClient())
            {
                web.Headers.Add("User-Agent: AQUA_HTTP");
                data = web.DownloadString(url);
            }
            return (data);
        }

        static public void Download(String url, String path)
        {
            using (WebClient web = new WebClient())
            {
                web.Headers.Add("User-Agent: AQUA_HTTP");
                web.DownloadFile(url, path);
            }
        }
    }
}
