using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.IO;

namespace INIProcess
{
    public class OperIni
    {
        public String FileName;
        public OperIni(String name)
        {
            this.FileName = name;
            if (!File.Exists(name))
            {
                File.Create(name);
            }
        }
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string value, string filePath);

        [DllImport("kernel32", EntryPoint = "GetPrivateProfileString")]
        private static extern uint GetPrivateProfileStringA(string section, string key,
            string def, Byte[] retVal, int size, string filePath);

        public string GetPrivateProfileString(string section, string key)
        {
            int nCapacity = 255;
            StringBuilder temp = new StringBuilder(nCapacity);
            int i = GetPrivateProfileString(section, key, "", temp, nCapacity, FileName);

            if (i < 0)
                return "";

            return temp.ToString();
        }
        public  List<string> ReadSections()
        {
            List<string> result = new List<string>();
            Byte[] buf = new Byte[65536];
            uint len = GetPrivateProfileStringA(null, null, null, buf, buf.Length, FileName);
            int j = 0;
            for (int i = 0; i < len; i++)
                if (buf[i] == 0)
                {
                    result.Add(Encoding.Default.GetString(buf, j, i - j));
                    j = i + 1;
                }
            return result;
        }
        public long WritePrivateProfileString(string section, string key, string value)
        {
            if (section.Trim().Length <= 0 || key.Trim().Length <= 0 || value.Trim().Length <= 0)
                return 0;

            return WritePrivateProfileString(section, key, value, FileName);
        }
        public void EraseSection(string sectionName)

        {

            WritePrivateProfileString(sectionName, null, null, FileName);

        }
    }
}