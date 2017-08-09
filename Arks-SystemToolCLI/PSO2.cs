using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Arks_SystemToolCLI
{
    class PSO2
    {
        public static String GetVersion()
        {
            String version = "";
            String url = @"http://patch.arks-system.eu/patch_prod/patches/version.ver";
            using (WebClient web = new WebClient())
            {
                web.Headers.Add("User-Agent: AQUA_HTTP");
                version =  web.DownloadString(url);
            }
            return (version.Trim());
        }

        public static String GetManagement()
        {
            String management = "";
            String url = @"http://patch.arks-system.eu/patch_prod/patches/management_beta.txt";

            using (WebClient web = new WebClient())
            {
                web.Headers.Add("User-Agent: AQUA_HTTP");
                management = web.DownloadString(url);
            }
            return (management.Trim());
        }


    }
}
