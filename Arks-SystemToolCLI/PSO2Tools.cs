using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Arks_SystemToolCLI
{
    class PSO2Tools
    {
        public static String SegaSource { get { return ("http://download.pso2.jp/"); } }
        public static String ArksSystemSource { get { return ("https://patch.arks-system.eu/"); } }

        public static String GetVersion(String source = "http://download.pso2.jp/")
        {
            String version = "";
            String url = String.Format("{0}/patch_prod/patches/version.ver", source);
            using (WebClient web = new WebClient())
            {
                web.Headers.Add("User-Agent: AQUA_HTTP");
                version = web.DownloadString(url);
            }
            return (version.Trim());
        }
        public static String GetGameVersion(String source = "http://download.pso2.jp/")
        {
            String version = "";
            String url = String.Format("{0}/patch_prod/patches/gameversion.ver.pat", source);
            using (WebClient web = new WebClient())
            {
                web.Headers.Add("User-Agent: AQUA_HTTP");
                version = web.DownloadString(url);
            }
            return (version.Trim());
        }
        public static String GetManagement(String source = "http://patch01.pso2gs.net/")
        {
            String management = "";
            String url = String.Format("{0}/patch_prod/patches/management_beta.txt", source);

            using (WebClient web = new WebClient())
            {
                web.Headers.Add("User-Agent: AQUA_HTTP");
                management = web.DownloadString(url);
            }
            return (management.Trim());
        }


    }
}
