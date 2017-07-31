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
        private HttpWebRequest _requests;
        private String _uri;

        public Requests(String uri)
        {
            this._requests = (HttpWebRequest)WebRequest.Create(uri);
            this._requests.UserAgent = "AQUA_HTTP";
            this._uri = uri;
        }

        public String Get()
        {
            this._requests.Method = "GET";
            try
            {
                Stream s = this._requests.GetResponse().GetResponseStream();
                StringBuilder builder = new StringBuilder();
                using (StreamReader reader = new StreamReader(s))
                {
                    builder.Append(reader.ReadToEnd());
                }

                return (builder.ToString());
            }
            catch (WebException e)
            {
                return ("");
            }
            
        }

        public void Download(String path)
        {
            this._requests.Method = "GET";
            Stream s = this._requests.GetResponse().GetResponseStream();
        }
    }
}
