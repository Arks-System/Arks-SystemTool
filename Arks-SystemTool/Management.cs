using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arks_SystemTool
{
    public class Management
    {
        static public String management_beta = "http://patch01.pso2gs.net/patch_prod/patches/management_beta.txt";
        private Dictionary<String, String> _base;
        private List<Patchlist> _patchlist;
        private List<String> _sources;
        public List<String> Sources { get { return (this._sources); } }
        public Dictionary<String, String> Bases { get { return (this._base); } }

        public Management(String source = null)
        {
            String management = String.Empty;
            management = Requests.Get(management_beta);
            this._base = new Dictionary<String, String>();
            this._patchlist = null;

            this._sources = new List<String>();
            this._sources.Add("http://patch01.pso2gs.net/");
            this._sources.Add("https://patch.arks-system.eu/");
            List <String> repositories = new List<String>();

            repositories.Add("MasterURL");
            repositories.Add("PatchURL");
            repositories.Add("TranslationURL");

            if (!String.IsNullOrEmpty(source))
                management = Requests.Get(management_beta.Replace("http://patch01.pso2gs.net/", source));
            else
                management = Requests.Get(management_beta);

            foreach (var e in management.Split('\n'))
            {
                List<String> lst = new List<String>(e.Replace("\r", "").Split('='));
                //if (lst[0] == "MasterURL" || lst[0] == "PatchURL")
                if (repositories.Contains(lst[0]))
                {
                    if (!String.IsNullOrEmpty(source))
                        lst[1] = lst[1].Replace(PSO2.SegaSource, source);
                    this._base.Add(lst[0], lst[1]);
                }
            }
        }

        public List<Patchlist> GetPatchlist(String sourceBase = null)
        {
            List<String> lists = new List<string>();
            List<Patchlist> patchlist = new List<Patchlist>();
            /*
            lists.Add(this._base["PatchURL"]);

            foreach (var entry in lists)
            {
                String url = String.Format("{0}patchlist.txt", entry);
                foreach (String e in Requests.Get(url).Trim('\r').Split('\n'))
                {
                    if (e.Length > 0)
                        patchlist.Add(new Patchlist(e, this));
                }
            }
            */
            if (String.IsNullOrEmpty(sourceBase))
                sourceBase = this._base["PatchURL"];

            String url = String.Format("{0}patchlist.txt", sourceBase);
            foreach (String e in Requests.Get(url).Trim('\r').Split('\n'))
            {
                if (e.Length > 0)
                    patchlist.Add(new Patchlist(e, this));
            }
            return (patchlist);
        }

        private void _BuildPatchlist(String uri)
        {
            String r = Requests.Get(String.Format("{0}patchlist.txt", uri));
            this._patchlist = new List<Patchlist>();

            foreach (var e in r.Split('\n'))
            {
                if (e.Length > 0)
                    this._patchlist.Add(new Patchlist(e.Replace("\r", ""), this));
            }
        }

        public String GetMasterBaseURL()
        {
            return (this._base["MasterURL"]);
        }

        public String GetPatchBaseURL()
        {
            return (this._base["PatchURL"]);
        }

        public String Get(char c)
        {
            if (c == 'm')
                return (this._base["MasterURL"]);
            else if (c == 'p')
                return (this._base["PatchURL"]);
            else if (c == 't')
                return (this._base["TranslationURL"]);
            else
                return (String.Empty);
        }

        public String GetRemoteVersion()
        {
            String url = String.Format("{0}gameversion.ver.pat", this.GetPatchBaseURL());
            String version = Requests.Get(url);
            return (version.Replace("\r\n", ""));
        }
    }
}
