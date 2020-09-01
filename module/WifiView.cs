using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using module;
namespace module
{
    public partial class WifiView : Form
    {
        private const String tmpPath = "c:\\users\\public\\tmp_wifipath";
        public WifiView()
        {
            InitializeComponent();
        }
        public void addList(String arg1, String arg2, String arg3, String arg4, String arg5, String arg6, String arg7, String arg8, String arg9, String arg10, String arg11)
        {
            listView7.BeginUpdate();
            ListViewItem newListViewItem = new ListViewItem(arg1);
            newListViewItem.SubItems.Add(arg2);
            newListViewItem.SubItems.Add(arg3);
            newListViewItem.SubItems.Add(arg4);
            newListViewItem.SubItems.Add(arg5);
            newListViewItem.SubItems.Add(arg6);
            newListViewItem.SubItems.Add(arg7);
            newListViewItem.SubItems.Add(arg8);
            newListViewItem.SubItems.Add(arg9);
            newListViewItem.SubItems.Add(arg10);
            newListViewItem.SubItems.Add(arg11);
            listView7.Items.Add(newListViewItem);
            listView7.Show();
            listView7.EndUpdate();
            this.Refresh();
           
        }

        private void WifiView_Load(object sender, EventArgs e)
        {
            
            if (Directory.Exists(tmpPath))
            {
                try
                {
                    Directory.Delete(tmpPath);
                }
                catch (Exception)
                {

                }
            }
            Directory.CreateDirectory(tmpPath);
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口
            p.Start();//启动程序
            p.StandardInput.WriteLine("netsh wlan export profile key=clear folder=\"c:\\users\\public\\tmp_wifipath\"" + "&&exit");
            p.StandardInput.AutoFlush = true;
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();//等待程序执行完退出进程
            p.Close();
            DirectoryInfo dir = new DirectoryInfo(tmpPath);
            FileInfo[] files = dir.GetFiles();
            
            foreach (FileInfo f in files)
            {
                
                String wifiConfig = f.FullName;
                String content = File.ReadAllText(wifiConfig);
                addList(
                    new module.Main().GetMiddleStr(content, "<hex>", "</hex>"),
                    new module.Main().GetMiddleStr(content, "<name>", "</name>"),
                    new module.Main().GetMiddleStr(content, "<connectionType>", "</connectionType>"),
                    new module.Main().GetMiddleStr(content, "<connectionMode>", "</connectionMode>"),
                    new module.Main().GetMiddleStr(content, "<authentication>", "</authentication>"),
                    new module.Main().GetMiddleStr(content, "<encryption>", "</encryption>"),
                    new module.Main().GetMiddleStr(content, "<useOneX>", "</useOneX>"),
                    new module.Main().GetMiddleStr(content, "<keyType>", "</keyType>"),
                    new module.Main().GetMiddleStr(content, "<protected>", "</protected>"),
                    new module.Main().GetMiddleStr(content, "<keyMaterial>", "</keyMaterial>"),
                    new module.Main().GetMiddleStr(content, "<enableRandomization>", "</enableRandomization>"));

            }
            listView7.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void WifiView_FormClosing(object sender, FormClosingEventArgs e)
        {
            try { Directory.Delete(tmpPath); } catch (Exception) { }
            
        }
    }

}
