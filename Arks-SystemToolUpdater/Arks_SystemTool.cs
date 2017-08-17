using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;
using System.IO;
using System.Security.Cryptography;
using System.Net;

namespace Arks_SystemToolUpdater
{
    public class Arks_SystemTool
    {
        public static readonly String BaseURL = "https://tool.arks-system.eu/repository/";
        public static readonly String ToolList = "toollist.txt";
        
        private String _path;
        public String path { get { return this._path; } }

        public Arks_SystemTool(String path)
        {
            this._path = path;
        }

        public bool IsRunning()
        {
            Process[] processes = Process.GetProcessesByName("Arks-SystemTool");

            return (processes.Length > 0);
        }

        public Version GetASTVersion()
        {
            String fullpath = Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path);
            String folderpath = Directory.GetParent(fullpath).FullName;

            if (File.Exists(folderpath + "Arks-SystemTool.exe"))
                return (AssemblyName.GetAssemblyName(folderpath + "Arks-SystemTool.exe").Version);
            return (new Version("0.0.0.0"));
        }

        public bool UpdateRequired()
        {
            String fullpath = Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path);
            String path = Directory.GetParent(fullpath).FullName;

            try
            {
                String[] r = Requests.Get("http://tool.arks-system.eu/repository/versions.txt").Split('\n');

                foreach (String e in r)
                {
                    String[] toollist = e.Split('=');
                    if (toollist[0] == "ProductVersion")
                    {
                        Version remote = new Version(toollist[1]);
                        //Version local = Assembly.GetExecutingAssembly().GetName().Version;
                        Version local = new Version(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion);

                        return (remote > local);
                    }
                }
            }
            catch (WebException we)
            {
                Console.WriteLine(we.Message);
                Console.WriteLine(we.StackTrace);
            }
            return (false);
        }

        public void Update()
        {
            String fullpath = Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path);
            String folderpath = Directory.GetParent(fullpath).FullName;

            foreach (String r in Requests.Get(BaseURL + ToolList).Replace("\r", "").Split('\n'))
            {
                if (String.IsNullOrEmpty(r))
                    return;
                Toollist file = new Toollist(r.Split('\t'), folderpath);
                String url = String.Format("{0}/{1}", Arks_SystemTool.BaseURL, file.url);
                String path = String.Format(@"{0}\{1}", folderpath, file.path);
                
                if (file.filename != "Arks-SystemToolUpdate.exe")
                {
                    try
                    {
                        if (!Directory.Exists(Directory.GetParent(file.fullpath).FullName))
                            Directory.CreateDirectory(Directory.GetParent(file.fullpath).FullName);
                        Console.WriteLine("GET {0}", file.path.Replace(@"\", "/"));
                        Requests.Download(file.url, folderpath + @"\" + file.filename);
                    }
                    catch (WebException we)
                    {
                        Console.WriteLine(we.Message);
                    }
                }
            }
        }

        public String GetMD5()
        {
            using (MD5 md5 = MD5.Create())
            {
                using (Stream stream = File.OpenRead(this._path))
                {
                    byte[] hash = md5.ComputeHash(stream);
                    String str = BitConverter.ToString(hash).Replace("-", String.Empty);

                    return (str.ToUpper());
                }
            }
        }
    }
}
