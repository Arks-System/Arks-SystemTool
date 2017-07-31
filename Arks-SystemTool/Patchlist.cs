using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arks_SystemTool
{
    class Patchlist
    {
        private List<String> _line;
        public String filename { get; set; }
        public String path { get; set; }
        public String url { get; set; }
        public String hash { get; set; }
        public UInt64 size { get; set; }
        public String flags { get; set; }
        public String management { get; set; }

        public new String ToString
        {
            get
            {
                return (String.Join("\t", this._line));
            }
        }

        public Patchlist(String line, String uri)
        {
            this._line = new List<String>(line.Split('\t'));
            this.filename = this._line[0];
            this.path = this._line[0];
            this.hash = this._line[1];
            this.size = UInt64.Parse(this._line[2]);
            this.flags = String.Join("\t", this._line.GetRange(3, this._line.Count() - 3));
            this.management = this._line[3];
            this.url = uri + this.filename;
        }

    }
}
