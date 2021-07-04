using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace WindowsServiceBase.Sistema
{
    public class IniFile
    {
        string Path;
        string EXE = Assembly.GetExecutingAssembly().GetName().Name;

        [DllImport("kernel32")]
        static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        [DllImport("kernel32")]
        static extern int GetPrivateProfileSection(string lpAppName, byte[] lpszReturnBuffer, int nSize, string FilePath);

        public IniFile(string IniPath = null, string exe = null)
        {
            EXE = exe;
            Path = new FileInfo(IniPath ?? EXE + ".ini").FullName.ToString();
        }

        public List<string> ReadSection(string Key, string Section)
        {
            byte[] buffer = new byte[2048];
            GetPrivateProfileSection(Section, buffer, 2048, Path);
            String[] tmp = Encoding.ASCII.GetString(buffer).Trim('\0').Split('\0');
            List<string> result = new List<string>();

            foreach (String entry in tmp)
            {
                result.Add(entry.Substring(0, entry.IndexOf("=")));
            }
            return result;
        }

        public string Read(string Key, string Section = null)
        {
            var RetVal = new StringBuilder(255);
            GetPrivateProfileString(Section ?? EXE, Key, "", RetVal, 255, Path);
            return RetVal.ToString();
        }

        public void Write(string Key, string Value, string Section = null)
        {
            WritePrivateProfileString(Section ?? EXE, Key, Value, Path);
        }

        public void DeleteKey(string Key, string Section = null)
        {
            Write(Key, null, Section ?? EXE);
        }

        public void DeleteSection(string Section = null)
        {
            Write(null, null, Section ?? EXE);
        }

        public bool KeyExists(string Key, string Section = null)
        {
            return Read(Key, Section).Length > 0;
        }
    }
}
