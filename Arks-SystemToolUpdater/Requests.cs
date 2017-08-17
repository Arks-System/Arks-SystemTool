using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace Arks_SystemToolUpdater
{
    public class Requests
    {
        public static readonly String UserAgent = String.Format("User-Agent: Arks-System Updater ({0})", Assembly.GetExecutingAssembly().GetName().Version);

        static public String Get(String url)
        {
            String data = String.Empty;
            using (WebClient web = new WebClient())
            {
                web.Headers.Add(UserAgent);
                data = web.DownloadString(url);
            }
            return (data);
        }

        static public void Download(String url, String path)
        {
            if (!Directory.Exists(Directory.GetParent(path).FullName))
                Directory.CreateDirectory(Directory.GetParent(path).FullName);
            using (WebClient web = new WebClient())
            {
                web.Headers.Add(UserAgent);
                web.DownloadFile(url, path);
            }
        }
    }
}
