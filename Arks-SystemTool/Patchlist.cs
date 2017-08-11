using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace Arks_SystemTool
{
    public class Patchlist
    {
        private List<String> _line;
        //private String[] _line;
        public String filename { get; set; }
        public String path { get; set; }
        public String url { get; set; }
        public String hash { get; set; }
        public UInt64 size { get; set; }
        public String flags { get; set; }
        public String management { get; set; }
        public String patchBase { get; set; }

        public new String ToString
        {
            get
            {
                return (String.Join("\t", this._line));
            }
        }

        public Patchlist(String line, Management management)
        {
            this._line = new List<String>(line.Split('\t'));
            //this._line = line.Split('\t');
            this.filename = this._line[0].Replace(".pat", "");
            this.path = this._line[0].Replace(".pat", "");
            this.hash = this._line[1];
            this.size = UInt64.Parse(this._line[2]);
            this.flags = String.Join("\t", this._line.GetRange(3, this._line.Count() - 3));
            this.management = this._line[3];
            this.url = management.Get(this._line[3][0]) + this._line[0];
            this.patchBase = management.Get(this._line[3].ToCharArray()[0]);
        }

        public bool IsSame(String path)
        {
            if (!File.Exists(path))
                return (false);
            using (MD5 md5 = MD5.Create())
            {
                using (Stream stream = File.OpenRead(path))
                {
                    byte[] hash = md5.ComputeHash(stream);
                    String str = BitConverter.ToString(hash).Replace("-", String.Empty);

                    return (str.ToUpper() == this.hash.ToUpper());
                }
            }
        }
    }
}
