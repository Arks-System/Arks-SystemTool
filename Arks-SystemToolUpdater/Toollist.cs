using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace Arks_SystemToolUpdater
{
    public class Toollist
    {
        public String filename { get; private set; }
        public String hash { get; private set; }
        public UInt64 size { get; private set; }
        public String path { get; private set; }
        public String fullpath { get; private set; }
        public String url { get; private set; }

        public Toollist(String[] line, string path)
        {
            this.filename = line[0];
            this.hash = line[1];
            this.size = (line.Length > 2) ? UInt64.Parse(line[2]) : 0;
            //this.path = path + this.filename.Replace(@"/", @"\");
            this.path = line[0];
            this.fullpath = path + @"\" + this.filename.Replace(@"/", @"\");
            this.url = Arks_SystemTool.BaseURL + this.path;
        }

        public bool IsSame(String path)
        {
            if (File.Exists(path))
            {
                using (MD5 md5 = MD5.Create())
                {
                    using (Stream stream = File.OpenRead(this.path))
                    {
                        byte[] hash = md5.ComputeHash(stream);
                        String str = BitConverter.ToString(hash).Replace("-", String.Empty);

                        return (str.ToUpper() == this.hash);
                    }
                }
            }
            return (false);
        }
    }
}
