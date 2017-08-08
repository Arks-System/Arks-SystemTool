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

            HttpWebRequest requ = (HttpWebRequest)WebRequest.Create(url);
            requ.UserAgent = "AQUA_HTTP";

            using (HttpWebResponse resp = (HttpWebResponse)requ.GetResponse())
            using (Stream stream = resp.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                version = reader.ReadToEnd();
            }
            //Console.WriteLine("Version {0}", version);
            return (version);
        }

        public static String GetManagement()
        {
            String management = "";
            String url = @"http://patch.arks-system.eu/patch_prod/patches/management_beta.txt";

            HttpWebRequest requ = (HttpWebRequest)WebRequest.Create(url);
            requ.UserAgent = "AQUA_HTTP";

            using (HttpWebResponse resp = (HttpWebResponse)requ.GetResponse())
            using (Stream stream = resp.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                management = reader.ReadToEnd();
            }
            //Console.WriteLine("Version {0}", version);
            return (management);
        }


    }
}
