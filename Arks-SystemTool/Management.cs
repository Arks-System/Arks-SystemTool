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

        public Management()
        {
            String management = Requests.Get(management_beta);
            this._base = new Dictionary<String, String>();
            this._patchlist = new List<Patchlist>();

            foreach (var e in management.Split('\n'))
            {
                List<String> lst = new List<String>(e.Replace("\r", "").Split('='));
                if (lst[0] == "MasterURL" || lst[0] == "PatchURL")
                {
                    this._base.Add(lst[0], lst[1]);
                    //_BuildPatchlist(lst[1]);
                }
            }
            foreach (KeyValuePair<String, String> entry in this._base)
            {
                this._BuildPatchlist(entry.Value);
            }
        }

        public List<Patchlist> GetPatchlist()
        {
            return (this._patchlist);
        }

        private void _BuildPatchlist(String uri)
        {
            String r = Requests.Get(String.Format("{0}patchlist.txt", uri));

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
            else
                return ("");
        }
    }
}
