using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using INIProcess;
using static FileProc;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;
using Microsoft.Win32;
using System.Diagnostics;

/** 工程日志
*  1.有一个疑难杂症，就是debug模式下，有些文件File.Exist存在，但是找不到图标，并且打开说找不到文件路径 【GoodSync】
* 
* 
* 
* 
* 
* 
* 
*/
namespace massbox
{

    public partial class Form1 : Form
    {

        [DllImport("kernel32.dll")]
        static extern uint GetTickCount();
        static void Delay(uint ms)
        {
            uint start = GetTickCount();
            while (GetTickCount() - start < ms)
            {
                System.Windows.Forms.Application.DoEvents();
            }
        } //延迟函数
        public string configFile = ""; //配置文件路径
        public string modulePath = ""; //模块文件路径
        private Point mouseOff; //鼠标坐标点,用于移动
        private bool leftFlag = false; //标题栏按下标识
        private OperIni config; //配置文件
        private OperIni workspace; //工作区文件
        private OperIni inspire; //工作区文件
        private OperIni password; //工作区文件
        private String dPath = ""; //dock目录
        private String noteDir = ""; //note目录
        private String dataDir = ""; //data目录
        private String now_note = ""; //当前打开的note文档
        private int now_password = -1; //当前打开的password的id
        private Dictionary<String, int> f2i = new Dictionary<String, int>(); //图标索引Map
        private string Status_dir = ""; //dock 当前目录
        private Point mouse; //鼠标指针位置,用于吸附
        private Point temp; //临时的窗口坐标
        private Boolean inside = false; //鼠标是否再窗口内
        private Boolean status = false; //是否已经吸附
        private int S_width = -1; //屏幕宽度
        private int S_height = -1; //屏幕高度
        public String appPath;  //程序路径
        public String appName; //程序名称
        private Boolean recordXY = false; //记录窗口位置
        private static RegistryKey _rlocal = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run"); //开机自启动路径
        private Boolean keepTop = false; //保持在前端显示
        private Boolean edgeHide = false;
        private Boolean MainStatus;  //程序是否加载完成，这个关联到后面的配置项
        private sqliteHelper money;
        String pkey = "[*AlC%.]";
        private Dictionary<int, String> p2n = new Dictionary<int, string>();//密码索引Map
        public Form1()
        {

            InitializeComponent();
            InitMyself();
        }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        } //应对窗口闪烁
        public class FullTabControl : TabControl
        {

            public override Rectangle DisplayRectangle
            {
                get
                {
                    Rectangle rect = base.DisplayRectangle;
                    return new Rectangle(rect.Left - 4, rect.Top - 4, rect.Width + 8, rect.Height + 8);
                }
            }
        } //解决tabcontro 边框问题
        private void title_MouseDown(object sender, MouseEventArgs e)
        {
            this.Opacity = 0.9;
            if (e.Button == MouseButtons.Left)
            {
                leftFlag = true;
                mouseOff = new Point(-e.X, -e.Y);
            }


        } //窗口移动控件 down
        private void title_MouseMove(object sender, MouseEventArgs e)
        {
            if (leftFlag)
            {
                Point mouseSet = Control.MousePosition;
                mouseSet.Offset(mouseOff.X, mouseOff.Y);//设置移动后的位置
                Location = mouseSet;
            }

        } //窗口移动控件 move
        private void Title_MouseUp(object sender, MouseEventArgs e)
        {
            if (leftFlag)
                leftFlag = false;
            this.Opacity = 1;
        } //窗口移动控件 up
        private void InitMyself()
        {

            MainStatus = false;
            imageList1.Images.Clear();
            imageList2.Images.Clear();
            listView1.Items.Clear();
            listView2.Items.Clear();
            listView3.Items.Clear();
            listView4.Items.Clear();
            listView5.Items.Clear();
            listView6.Items.Clear();
            listView7.Items.Clear();
            listView8.Items.Clear();
            f2i.Clear();
            p2n.Clear();


            //初始化程序基本认知
            appPath = Assembly.GetExecutingAssembly().Location;
            appName = appPath.Substring(appPath.LastIndexOf('\\') + 1);
            //指定配置文件位置
            configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sys.ini");
            //关联配置文件
            config = new OperIni(configFile);
            //关联模块文件
            modulePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "module.dll");
            //判断data目录是否存在
            dataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
            if (!Directory.Exists(dataDir))
            {
                //创建data目录
                Directory.CreateDirectory(dataDir);
            }
            //判断note目录是否存在
            noteDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "note");
            if (!Directory.Exists(noteDir))
            {
                //创建note目录
                Directory.CreateDirectory(noteDir);
            }
            //关联workspace文件
            workspace = new OperIni(AppDomain.CurrentDomain.BaseDirectory + "\\data\\workspace.ini");
            //关联inspire文件
            inspire = new OperIni(AppDomain.CurrentDomain.BaseDirectory + "\\data\\inspire.ini");
            //关联password文件
            password = new OperIni(AppDomain.CurrentDomain.BaseDirectory + "\\data\\password.ini");
            //设置页面中的初始化

            //读取dock目录
            dPath = config.GetPrivateProfileString("Main", "dockPath");
            DockDir.Text = dPath; //置Dock目录
                                  //开机自启动复选框
            try
            {
                if (_rlocal.GetValue(appName) != null)
                    checkBox1.Checked = true;
            }
            catch (Exception)
            {
                checkBox1.Checked = false;
            }
            //记录窗口位置
            if (config.GetPrivateProfileString("Main", "recordXY") == "1" && !string.IsNullOrEmpty(config.GetPrivateProfileString("Main", "X")) && !string.IsNullOrEmpty(config.GetPrivateProfileString("Main", "Y")))
            {
                checkBox2.Checked = true;
                recordXY = true;
                temp.X = int.Parse(config.GetPrivateProfileString("Main", "X"));
                temp.Y = int.Parse(config.GetPrivateProfileString("Main", "Y"));
                Location = temp;
            }
            //边缘吸附隐藏
            if (config.GetPrivateProfileString("Main", "edgeHide") == "1")
            {
                checkBox3.Checked = true;
                edgeHide = true;
            }
            //保持top
            if (config.GetPrivateProfileString("Main", "keepTop") == "1")
            {
                keepTop = true;
                checkBox4.Checked = true;
            }
            //扫描根文件夹
            if (!string.IsNullOrEmpty(dPath))
                 scanDir();
            //获取显示器信息
            Rectangle rect = System.Windows.Forms.SystemInformation.VirtualScreen;
            S_width = rect.Width;
            S_height = rect.Height;
            //初始化Workspace数据
            List<String> workspaceSession = workspace.ReadSections();
            foreach (String item in workspaceSession)
            {
                String arg1 = workspace.GetPrivateProfileString(item, "title");
                String arg2 = workspace.GetPrivateProfileString(item, "uri");
                String arg3 = workspace.GetPrivateProfileString(item, "datetime");
                insertWorkspaceList(arg1, arg2, arg3);
            }
            //初始化灵感区
            List<String> inspireSession = inspire.ReadSections();
            foreach (String item in inspireSession)
            {
                String arg1 = inspire.GetPrivateProfileString(item, "title");
                String arg2 = inspire.GetPrivateProfileString(item, "datetime");
                insertInspireList(arg1, arg2);
            }
            //初始化便签
            reload_Note();
            //初始化密码
            reload_Psw();
            //获取模块信息
            ArrayList moduleList = new module.Main().getModuleList();
            foreach (Array a in moduleList)
            {
                listView7.BeginUpdate();
                ListViewItem newListViewItem = new ListViewItem(a.GetValue(0).ToString());
                newListViewItem.SubItems.Add(a.GetValue(1).ToString());
                newListViewItem.SubItems.Add(a.GetValue(2).ToString());
                newListViewItem.SubItems.Add(a.GetValue(3).ToString());
                listView7.Items.Add(newListViewItem);
                listView7.Show();
                listView7.EndUpdate();
            }
            //初始化钱包
            reload_Money();
            MainStatus = true; //程序初始化完成标识
        } //程序初始化 ※※※
        private string GetFileSize(long filesize)
        {
            if (filesize < 0)
            {
                return "0";
            }
            else if (filesize >= 1024 * 1024 * 1024) //文件大小大于或等于1024MB
            {
                return string.Format("{0:0.00} GB", (double)filesize / (1024 * 1024 * 1024));
            }
            else if (filesize >= 1024 * 1024) //文件大小大于或等于1024KB
            {
                return string.Format("{0:0.00} MB", (double)filesize / (1024 * 1024));
            }
            else if (filesize >= 1024) //文件大小大于等于1024bytes
            {
                return string.Format("{0:0.00} KB", (double)filesize / 1024);
            }
            else
            {
                return string.Format("{0:0.00} bytes", filesize);
            }
        } //文件尺寸格式化
        private void reload_Money()
        {
            listView8.Items.Clear();
            money = new sqliteHelper();
            ArrayList mlist = money.getAllData();
            foreach (Array a in mlist)
            {
                insertWallet(a.GetValue(0).ToString(),
                    a.GetValue(1).ToString(),
                    a.GetValue(2).ToString(),
                    a.GetValue(3).ToString(),
                    a.GetValue(4).ToString(),
                    a.GetValue(5).ToString());
            }
            getBalanceInfo();
        } //初始化钱包
        private void reload_Note()
        {
            listView5.Items.Clear();
            DirectoryInfo di = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "\\note"); //指定文件所在目录
            FileInfo[] files = di.GetFiles(); //扫描目录内文件
            listView5.BeginUpdate();
            foreach (FileInfo f in files)
            {
                ListViewItem newListViewItem = new ListViewItem(f.Name);
                newListViewItem.SubItems.Add(GetFileSize(f.Length));
                listView5.Items.Add(newListViewItem);
            }
            listView5.Show();
            listView5.EndUpdate();
        }//初始化便签
        private void reload_Psw()
        {
            listView6.Items.Clear();
            p2n.Clear();
            List<String> passwordSession = password.ReadSections();
            foreach (String item in passwordSession)
            {
                p2n.Add(listView6.Items.Count, item);
                String arg1 = Security.EncryptHelper.DESDecrypt(password.GetPrivateProfileString(item, "desp"), pkey);
                String arg2 = Security.EncryptHelper.DESDecrypt(password.GetPrivateProfileString(item, "account"), pkey);
                String arg3 = Security.EncryptHelper.DESDecrypt(password.GetPrivateProfileString(item, "password"), pkey);
                String arg4 = Security.EncryptHelper.DESDecrypt(password.GetPrivateProfileString(item, "remarks"), pkey);
                insert_password(arg1, arg2, arg3, arg4);
            }
        }//初始化密码

        private void selectDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            DialogResult status = dialog.ShowDialog(this);
            if (status == DialogResult.OK)
            {
                MainStatus = false;
                DockDir.Text = dialog.SelectedPath;// 同步到GUI
                config.WritePrivateProfileString("Main", "dockPath", dialog.SelectedPath);//写入配置项
                MainStatus = true;

            }
        }//选择dock目录按钮

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (listView1.SelectedItems.Count != 0)
            {
                Status_dir = listView1.SelectedItems[0].Text;
                scanFile("", listView1.SelectedItems[0].Text);
            }

        } //选择目录列表后
        private void scanDir(String path = "")
        {
            listView1.Clear();
            f2i.Clear();
            if (path == "")
                path = dPath;
            DirectoryInfo di = new DirectoryInfo(path);
            DirectoryInfo[] dirs = di.GetDirectories();     //扫描文件夹
            for (int i = 0; i < dirs.Length; i++)
            {
                listView1.Items.Add(dirs[i].Name); //添加表项
                OperIni icon = new OperIni(dirs[i].FullName + "/Desktop.ini"); //打开desktop文件 
                String icoPath = icon.GetPrivateProfileString(".ShellClassInfo", "IconResource"); // 读取desktop文件中的图标路径
                string[] sArray = icoPath.Split(',');//分割文件
                imageList1.Images.Add(Image.FromFile(sArray[0])); //图片加入到图片列表
                listView1.Items[i].ImageIndex = i; //设置图标索引
                getIcon(dirs[i].FullName);
            }
            listView1.SmallImageList = imageList1;
        } //扫描目录
        private void getIcon(String path)
        {
            DirectoryInfo di = new DirectoryInfo(path); //指定文件所在目录
            FileInfo[] files = di.GetFiles(); //扫描目录内文件
            FileProc fileproc = new FileProc(""); //初始化文件处理模块
            for (int i = 0; i < files.Length; i++) //逐一添加至列表
            {
                fileproc.setPath(files[i].FullName); //文件处理锁定指定文件名
                imageList2.Images.Add(fileproc.getIcon()); //将图标置入集合
                f2i.Add(files[i].FullName, imageList2.Images.Count);
            }
        }//扫描图标并且加入至索引
        private void scanFile(String WorkDir = "", String path = "")
        {
            listView2.Clear();
            if (WorkDir == "")
                WorkDir = dPath;
            DirectoryInfo di = new DirectoryInfo(WorkDir + "/" + path); //指定文件所在目录
            FileInfo[] files = di.GetFiles(); //扫描目录内文件
            FileProc fileproc = new FileProc(""); //初始化文件处理模块
            int flag = 0; //去除ini文件后索引变更,因此采用新索引
            int v; //图标索引值
            for (int i = 0; i < files.Length; i++) //逐一添加至列表
            {
                fileproc.setPath(files[i].FullName); //文件处理锁定指定文件名
                if (fileproc.getType() == "ini") //排除ini文件
                {
                    continue;
                }
                listView2.Items.Add(files[i].Name); //添加至列表
                if (!f2i.TryGetValue(files[i].FullName, out v)) //获取图标索引
                {
                    scanDir();
                }
                listView2.Items[flag].ImageIndex = v - 1; //设置图标索引
                flag++;
            }
            listView2.LargeImageList = imageList2; //锁定图标集
        }//扫描文件
        private void listView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Clicks == 2)
            {
                if (listView1.SelectedItems.Count == 0)
                {
                    return;
                }
                else
                {
                    System.Diagnostics.Process.Start("explorer.exe", dPath + "\\" + listView1.SelectedItems[0].Text);
                }
            }
        }//双击目录后
        private void listView2_MouseDown(object sender, MouseEventArgs e)
        {
            if (listView2.SelectedItems.Count == 0)
            {
                return;
            }
            if (e.Clicks == 2)
            {
                Process p = new Process();
                if (e.Button == MouseButtons.Left)
                { //左键双击打开文件
                    String fileURI = dPath + "\\" + Status_dir + "\\" + listView2.SelectedItems[0].Text;
                    FileProc f = new FileProc(fileURI);
                    if (f.getType().ToLower() == "lnk")
                    {
                        p.StartInfo.CreateNoWindow = false;
                        p.StartInfo.FileName = f.ReadShortcut().TargetPath;
                        p.StartInfo.Arguments = f.ReadShortcut().Arguments;
                        if (string.IsNullOrEmpty(f.ReadShortcut().WorkDir))
                        {
                            f.setPath(f.ReadShortcut().TargetPath);
                            p.StartInfo.WorkingDirectory = f.getPath();
                        }
                        else
                        {
                            p.StartInfo.WorkingDirectory = f.ReadShortcut().WorkDir;
                        }
                        p.StartInfo.UseShellExecute = true;
                    }
                    else
                    {
                        p.StartInfo.CreateNoWindow = false;
                        p.StartInfo.FileName = fileURI;
                        p.StartInfo.UseShellExecute = true;
                    }
                    try { p.Start(); } catch { MessageBox.Show("文件不存在", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error); }

                }
                else if (e.Button == MouseButtons.Right) //右键双击打开文件目录
                {
                    String dirURI = dPath + "\\" + Status_dir + "\\" + listView2.SelectedItems[0].Text;
                    FileProc f = new FileProc(dirURI);
                    ShortcutDescription sd = f.ReadShortcut();
                    f.setPath(sd.TargetPath); ;
                    p.StartInfo.CreateNoWindow = false;
                    p.StartInfo.FileName = f.getPath();
                    p.StartInfo.UseShellExecute = true;
                    try { p.Start(); } catch { MessageBox.Show("目录不存在", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error); }

                }
            }
        }//双击文件后
        private void listView2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                listView2_MouseDown(new object(), new MouseEventArgs(MouseButtons.Left, 2, 0, 0, 0));
            }
        }//启动器中的回车事件监听

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!edgeHide)
            {
                timer1.Enabled = false;
                return;
            }
            mouse = Cursor.Position;
            if (mouse.X >= Location.X && mouse.Y >= Location.Y && mouse.X <= Location.X + this.Width && mouse.Y <= Location.Y + this.Height)
            {
                inside = true;
            }
            else
            {
                inside = false;
            }
            if (Location.X <= 0 && !inside && !status)  //左缩
            {
                int finalValue = (this.Width + Location.X) - 3;
                timer1.Enabled = false;
                status = true;
                temp.Y = Location.Y;
                temp.X = Location.X;
                this.Opacity = 0.8;
                for (int i = 0; i < finalValue; i += 3)
                {
                    temp.X -= 3;
                    Location = temp;
                }
                temp.X = -this.Width + 3;
                Location = temp;
            }
            if (Location.X <= 0 && inside && status)  //左回
            {
                int finalValue = this.Width - 3;
                timer1.Enabled = false;
                temp.Y = Location.Y;
                temp.X = Location.X;
                this.Opacity = 1;
                for (int i = 0; i < finalValue; i += 3)
                {
                    temp.X += 3;
                    Location = temp;
                }
                temp.X = 0;
                Location = temp;
                status = false;
            }

            if (this.Width + Location.X >= S_width && !inside && !status)  //右缩
            {
                int finalValue = (S_width - Location.X) - 3;
                timer1.Enabled = false;
                status = true;
                temp.Y = Location.Y;
                temp.X = Location.X;
                this.Opacity = 0.8;
                for (int i = 0; i < finalValue; i += 3)
                {
                    temp.X += 3;
                    Location = temp;
                }
                temp.X = S_width - 3;
                Location = temp;
            }
            if (this.Width + Location.X >= S_width && inside && status)  //右回
            {
                int finalValue = this.Width;
                timer1.Enabled = false;
                temp.Y = Location.Y;
                temp.X = Location.X;
                this.Opacity = 1;
                for (int i = 0; i < finalValue; i += 3)
                {
                    temp.X -= 3;
                    Location = temp;

                }
                temp.X = S_width - this.Width;
                Location = temp;
                status = false;
            }

            if (Location.Y <= 0 && !inside && !status) //上缩
            {
                int finalValue = (this.Height + Location.Y) - 3;
                timer1.Enabled = false;
                status = true;
                temp.Y = Location.Y;
                temp.X = Location.X;
                this.Opacity = 0.8;
                for (int i = 0; i < finalValue; i += 3)
                {
                    temp.Y -= 3;
                    Location = temp;
                }
                temp.Y = -this.Height + 3;
                Location = temp;
            }

            if (Location.Y <= 0 && inside && status)  //上回
            {
                int finalValue = this.Height;
                timer1.Enabled = false;
                temp.Y = Location.Y;
                temp.X = Location.X;
                this.Opacity = 1;
                for (int i = 0; i < finalValue; i += 3)
                {
                    temp.Y += 3;
                    Location = temp;

                }
                temp.Y = 0;
                Location = temp;
                status = false;
            }


            if (this.Height + Location.Y >= S_height && !inside && !status) //下缩
            {
                int finalValue = (S_height - Location.Y) + 3;
                timer1.Enabled = false;
                status = true;
                temp.Y = Location.Y;
                temp.X = Location.X;
                this.Opacity = 0.8;
                for (int i = 0; i < finalValue; i += 3)
                {
                    temp.Y += 3;
                    Location = temp;
                }
                temp.Y = S_height - 3;
                Location = temp;
            }

            if (this.Height + Location.Y >= S_height && inside && status)  //下回
            {
                int finalValue = S_height - this.Height;
                timer1.Enabled = false;
                temp.Y = Location.Y;
                temp.X = Location.X;
                this.Opacity = 1;
                for (int i = 0; i < finalValue; i += 3)
                {
                    temp.Y -= 3;
                    Location = temp;
                    if (temp.Y < S_height - this.Height)
                    {
                        temp.Y = S_height - this.Height;
                    }
                }

                Location = temp;
                status = false;
            }

            timer1.Enabled = true;
        }//吸附

        private void listView3_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;

            }
        } //工作区拖入入口
        private void listView3_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop);
            insertWorkspaceList(new FileProc(s[0]).getName(), s[0], DateTime.Now.ToString());
            writeWorkspace(s[0]);
        }//工作区被拖入文件
        private void insertWorkspaceList(String arg1, String arg2, String arg3)
        {
            listView3.BeginUpdate();
            ListViewItem newListViewItem = new ListViewItem(arg1);
            newListViewItem.SubItems.Add(arg2);
            newListViewItem.SubItems.Add(arg3);
            listView3.Items.Add(newListViewItem);
            listView3.Show();
            listView3.EndUpdate();
        } //插入到工作区列表
        private void writeWorkspace(String s) //写入工作区配置项
        {
            workspace.WritePrivateProfileString(s, "title", new FileProc(s).getName());
            workspace.WritePrivateProfileString(s, "uri", s);
            workspace.WritePrivateProfileString(s, "datetime", DateTime.Now.ToString());
        }
        private void listView3_MouseDown(object sender, MouseEventArgs e)
        {
            if (listView3.SelectedItems.Count == 0)
            {
                return;
            }
            if (e.Clicks == 2)
            {
                if (e.Button == MouseButtons.Left)
                {
                    int type = new FileProc(listView3.SelectedItems[0].SubItems[1].Text).whatsType();
                    switch (type)
                    {
                        case 0:
                            MessageBox.Show("文件不存在", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        case 1:
                            System.Diagnostics.Process.Start("explorer.exe", listView3.SelectedItems[0].SubItems[1].Text);
                            break;
                        case 2:
                            System.Diagnostics.Process.Start(listView3.SelectedItems[0].SubItems[1].Text);
                            break;
                    }
                }
                else if (e.Button == MouseButtons.Right)
                {
                    DialogResult v = MessageBox.Show("是否要删除项目[" + listView3.SelectedItems[0].SubItems[0].Text + "]?", "确认？", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (v == DialogResult.Yes)
                    {
                        String sessionName = listView3.SelectedItems[0].SubItems[1].Text;
                        workspace.EraseSection(sessionName);
                        listView3.Items.Remove(listView3.SelectedItems[0]);
                    }
                }
            }

        } //工作区中的鼠标操作
        private void listView3_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                listView3_MouseDown(new object(), new MouseEventArgs(MouseButtons.Left, 2, 0, 0, 0));
            }
            else if (e.KeyCode == Keys.Delete)
            {
                listView3_MouseDown(new object(), new MouseEventArgs(MouseButtons.Right, 2, 0, 0, 0));
            }
        }//工作区回车键事件监听

        private void insertInspire_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "") //空文本不予添加
                return;
            insertInspireList(textBox1.Text, DateTime.Now.ToString());
            writeInspire(textBox1.Text);
            textBox1.Text = "";
        }//插入到灵感区按钮事件
        private void writeInspire(String s)
        {
            inspire.WritePrivateProfileString(s, "title", s);
            inspire.WritePrivateProfileString(s, "datetime", DateTime.Now.ToString());

        }//写入灵感区配置项
        private void insertInspireList(String arg1, String arg2)
        {
            listView4.BeginUpdate();
            ListViewItem newListViewItem = new ListViewItem(arg1);
            newListViewItem.SubItems.Add(arg2);
            listView4.Items.Add(newListViewItem);
            listView4.Show();
            listView4.EndUpdate();
        }//插入到灵感区列表
        private void listView4_MouseDown(object sender, MouseEventArgs e)
        {
            if (listView4.SelectedItems.Count == 0)
                return;
            if (e.Button == MouseButtons.Right && e.Clicks == 2)
            {
                DialogResult v = MessageBox.Show("是否要删除项目[" + listView4.SelectedItems[0].SubItems[0].Text + "]?", "确认？", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (v == DialogResult.Yes)
                {
                    String sessionName = listView4.SelectedItems[0].SubItems[0].Text;
                    inspire.EraseSection(sessionName);
                    listView4.Items.Remove(listView4.SelectedItems[0]);
                }
            }
        } //灵感区鼠标事件
        private void listView4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                listView4_MouseDown(new object(), new MouseEventArgs(MouseButtons.Right, 2, 0, 0, 0));
            }

        } //灵感区列表键盘事件监听
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                insertInspire_Click(new object(), new EventArgs());
            }
        } //灵感区输入框键盘事件监听

        private void listView5_MouseDown(object sender, MouseEventArgs e)
        {
            if (listView5.SelectedItems.Count == 0) //没有选中的情况下
            {
                if (e.Button == MouseButtons.Left && e.Clicks == 2) //双击空白处
                {
                    if (keepTop) { this.TopMost = false; }
                    string str = Interaction.InputBox("请输入便签名称", "新建", "");
                    if (keepTop) { this.TopMost = true; }
                    if (str == "") return;
                    if (File.Exists(Path.Combine(noteDir, str)))
                    { //文件已经存在

                        MessageBox.Show("创建文件失败,请保证文件名的合法性。" + "\n1.不包含非法字符\n*2.文件已经存在\n3.当前目录是否拥有权限", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    FileProc f = new FileProc(Path.Combine(noteDir, str));
                    if (f.getType() == "")
                        f.setPath(f.getName() + ".rtf");
                    else if (f.getType().ToLower() != "rtf")
                    {
                        f.setPath(f.getName() + ".rtf");
                    }

                    if (File.Exists(Path.Combine(noteDir, f.getName())))
                    {
                        MessageBox.Show("创建文件失败,请保证文件名的合法性。" + "\n1.不包含非法字符\n*2.文件已经存在\n3.当前目录是否拥有权限", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    richTextBox2.Text = "";
                    richTextBox2.SaveFile(Path.Combine(noteDir, f.getName()));
                    now_note = Path.Combine(noteDir, f.getName());
                    if (!File.Exists(Path.Combine(noteDir, f.getName()))) //创建后仍不存在
                    {
                        MessageBox.Show("创建文件失败,请保证文件名的合法性。" + "\n*1.不包含非法字符\n2.文件已经存在\n*3.当前目录是否拥有权限", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                reload_Note();
            }
            else //选中的情况
            {
                if (e.Button == MouseButtons.Left && e.Clicks == 2) //双击文件
                {
                    now_note = listView5.SelectedItems[0].Text;
                    String fileName = listView5.SelectedItems[0].Text;
                    try
                    {
                        richTextBox1.LoadFile(Path.Combine(noteDir, fileName));
                    }
                    catch (System.ArgumentException)
                    {
                        MessageBox.Show("文件格式错误,文件的类型是rtf,需要严格遵守rtf格式要求。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    catch (Exception oe)
                    {
                        MessageBox.Show(oe.Message, "错误[尚未定义的错误类型]", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        now_note = "";
                    }


                }
                if (e.Button == MouseButtons.Right && e.Clicks == 2)
                {
                    DialogResult v = MessageBox.Show("是否要删除项目[" + listView5.SelectedItems[0].SubItems[0].Text + "]?", "确认？", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (v == DialogResult.Yes)
                    {
                        if (listView5.SelectedItems[0].SubItems[0].Text == now_note)
                        {
                            now_note = "";
                            richTextBox1.Text = "";
                        }
                        String noteName = listView5.SelectedItems[0].SubItems[0].Text;
                        listView5.Items.Remove(listView5.SelectedItems[0]);
                        File.Delete(Path.Combine(noteDir, noteName));
                        if (File.Exists(Path.Combine(noteDir, noteName)))
                        {
                            MessageBox.Show("删除失败", "错误[尚未定义的错误类型]", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }//note文件部分鼠标事件
        private void timer2_Tick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(now_note))
            {
                timer2.Enabled = false;
                return;
            }
            richTextBox1.SaveFile(Path.Combine(noteDir, now_note));
            timer2.Enabled = false;
            reload_Note(); //刷新note文件框的信息
        }// 保存note到文件
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (now_note == "")
            {
                if (MessageBox.Show("尚未创建便签,是否要创建？", "错误", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    listView5_MouseDown(new object(), new MouseEventArgs(MouseButtons.Left, 2, 0, 0, 0));
                    return;
                }
                else
                {
                    richTextBox1.Text = "";
                    return;
                }
            }
            timer2.Enabled = false;
            timer2.Enabled = true;

        }//note数据发生改变
        private void listView5_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                listView5_MouseDown(new object(), new MouseEventArgs(MouseButtons.Right, 2, 0, 0, 0));
            }
            if (e.KeyCode == Keys.Enter)
            {
                listView5_MouseDown(new object(), new MouseEventArgs(MouseButtons.Left, 2, 0, 0, 0));
            }
            if (e.KeyCode == Keys.Insert)
            {
                if (listView5.SelectedItems.Count != 0)
                    listView5.SelectedItems[0].Selected = false;
                listView5_MouseDown(new object(), new MouseEventArgs(MouseButtons.Left, 2, 0, 0, 0));
            }
        }//note 键盘事件监听

        private void addpsw_button_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = true;
            groupBox1.Text = "添加密码";
        }//密码管理中的添加密码按钮事件
        private void delpsw_button_Click(object sender, EventArgs e)
        {
            if (listView6.SelectedItems.Count == 0)
            {
                return;
            }
            else
            {
                DialogResult v = MessageBox.Show("是否要删除密码[" + listView6.SelectedItems[0].SubItems[0].Text + "]?", "确认？", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (v == DialogResult.Yes)
                {
                    String session;
                    p2n.TryGetValue(listView6.SelectedItems[0].Index, out session);
                    password.EraseSection(session);
                    reload_Psw();
                }
            }
        }//密码管理中的删除密码按钮事件
        private void modpsw_button_Click(object sender, EventArgs e)
        {
            if (listView6.SelectedItems.Count == 0)
            {
                return;
            }
            else
            {
                now_password = listView6.SelectedItems[0].Index;
                groupBox1.Visible = true;
                groupBox1.Text = "修改密码";
                String session;
                p2n.TryGetValue(now_password, out session);
                textBox2.Text = Security.EncryptHelper.DESDecrypt(password.GetPrivateProfileString(session, "desp"), pkey);
                textBox3.Text = Security.EncryptHelper.DESDecrypt(password.GetPrivateProfileString(session, "account"), pkey);
                textBox4.Text = Security.EncryptHelper.DESDecrypt(password.GetPrivateProfileString(session, "password"), pkey);
                textBox5.Text = Security.EncryptHelper.DESDecrypt(password.GetPrivateProfileString(session, "remarks"), pkey);
            }

        }//密码管理中的修改密码按钮事件
        private void pswok_button_Click(object sender, EventArgs e)
        {
            String desp = textBox2.Text;
            String account = textBox3.Text;
            String password = textBox4.Text;
            String remarks = textBox5.Text;
            String session;
            if (groupBox1.Text == "添加密码" && groupBox1.Visible == true)
            {
                insert_password(desp, account, password, remarks);
                desp = Security.EncryptHelper.DESEncrypt(desp, pkey);
                account = Security.EncryptHelper.DESEncrypt(account, pkey);
                password = Security.EncryptHelper.DESEncrypt(password, pkey);
                remarks = Security.EncryptHelper.DESEncrypt(remarks, pkey);
                password_write(desp, account, password, remarks);
            }
            if (groupBox1.Text == "修改密码" && groupBox1.Visible == true)
            {
                p2n.TryGetValue(now_password, out session);
                modify_password(desp, account, password, remarks, now_password);
                desp = Security.EncryptHelper.DESEncrypt(desp, pkey);
                account = Security.EncryptHelper.DESEncrypt(account, pkey);
                password = Security.EncryptHelper.DESEncrypt(password, pkey);
                remarks = Security.EncryptHelper.DESEncrypt(remarks, pkey);
                password_write(desp, account, password, remarks, session);
            }
            groupBox1.Visible = false;
            listView6.Refresh();
            reload_Psw();
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";

        }//密码管理中的确定按钮事件
        private void password_write(String arg1, String arg2, String arg3, String arg4, String _session = "")
        {
            string session;
            if (_session == "") //没有指定session就是创建新密码
                session = (Convert.ToDouble(DateTime.UtcNow.Ticks - 621355968000000000) / (10 * 1000 * 1000)).ToString();
            else  //否则就是修改
                session = _session;
            password.WritePrivateProfileString(session, "desp", arg1);
            password.WritePrivateProfileString(session, "account", arg2);
            password.WritePrivateProfileString(session, "password", arg3);
            password.WritePrivateProfileString(session, "remarks", arg4);
        }//密码管理器的本地储存中添加密码
        private void insert_password(String arg1, String arg2, String arg3, String arg4)
        {
            arg2 = Security.EncryptHelper.cover(arg2);
            arg3 = Security.EncryptHelper.cover(arg3);

            listView6.BeginUpdate();
            ListViewItem newListViewItem = new ListViewItem(arg1);
            newListViewItem.SubItems.Add(arg2);
            newListViewItem.SubItems.Add(arg3);
            newListViewItem.SubItems.Add(arg4);
            listView6.Items.Add(newListViewItem);
            listView6.Show();
            listView6.EndUpdate();

        }//密码管理器的列表中添加密码
        private void modify_password(String arg1, String arg2, String arg3, String arg4, int index)
        {

            listView6.Items[index].SubItems[0].Text = Security.EncryptHelper.cover(arg1);
            listView6.Items[index].SubItems[1].Text = Security.EncryptHelper.cover(arg2);
            listView6.Items[index].SubItems[2].Text = Security.EncryptHelper.cover(arg3);
            listView6.Items[index].SubItems[3].Text = Security.EncryptHelper.cover(arg4);
        } //列表中修改密码
        private void pswx_button_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = false;
            groupBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
        }//密码管理中的取消按钮事件
        private void viewpsw_button_Click(object sender, EventArgs e)
        {
            if (listView6.SelectedItems.Count == 0)
            {
                return;
            }
            else
            {
                DialogResult v = MessageBox.Show("请确定周边安全,确定即显示全部信息", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (v == DialogResult.Yes)
                {
                    now_password = listView6.SelectedItems[0].Index;
                    String session;
                    p2n.TryGetValue(now_password, out session);
                    String info = "描述:" + Security.EncryptHelper.DESDecrypt(password.GetPrivateProfileString(session, "desp"), pkey) + "\n账号:" +
                    Security.EncryptHelper.DESDecrypt(password.GetPrivateProfileString(session, "account"), pkey) + "\n密码:" +
                    Security.EncryptHelper.DESDecrypt(password.GetPrivateProfileString(session, "password"), pkey) + "\n备注:" +
                    Security.EncryptHelper.DESDecrypt(password.GetPrivateProfileString(session, "remarks"), pkey);
                    MessageBox.Show(info, "查看密码", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }//查看密码按钮事件
        private void querypsw_button_Click(object sender, EventArgs e)
        {
            this.TopMost = false;
            string str = Interaction.InputBox("请输入相关信息", "查询", "");
            this.TopMost = true;
            if (str != "")
            {
                String rst = "***************************\n";
                List<String> passwordSession = password.ReadSections();
                foreach (String item in passwordSession)
                {
                    String arg1 = Security.EncryptHelper.DESDecrypt(password.GetPrivateProfileString(item, "desp"), pkey);
                    String arg2 = Security.EncryptHelper.DESDecrypt(password.GetPrivateProfileString(item, "account"), pkey);
                    String arg3 = Security.EncryptHelper.DESDecrypt(password.GetPrivateProfileString(item, "password"), pkey);
                    String arg4 = Security.EncryptHelper.DESDecrypt(password.GetPrivateProfileString(item, "remarks"), pkey);
                    if (arg1.IndexOf(str) > -1 || arg2.IndexOf(str) > -1 || arg3.IndexOf(str) > -1 || arg4.IndexOf(str) > -1)
                    {
                        String info = "描述:" + arg1 + "\n账号:" + arg2 + "\n密码:" + arg3 + "\n备注" + arg4 + "\n";
                        rst += info + "***************************\n";
                    }
                }
                if (rst == "***************************\n")
                {
                    MessageBox.Show("尚未发现相关密码信息", "查询密码", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(rst, "查询密码", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
        }

        private void listView7_DoubleClick(object sender, EventArgs e)
        {
            if (listView7.SelectedItems.Count == 0) return;
            else
            {
                if (keepTop) { this.TopMost = false; }
                new module.Main().RunModule(listView7.SelectedItems[0].Text);
                if (keepTop) { this.TopMost = true; }

            }
        }//双击模块表项后执行
        private void listView7_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                listView7_DoubleClick(new object(), new EventArgs());
            }
        }//在模块表项中键盘事件的监听
        private void checkBox1_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                try { _rlocal.SetValue(appName, string.Format(@"""{0}""", appPath)); }
                catch (Exception)
                {
                    MessageBox.Show("设置开机自动启动失败，请检查是否授权", "自动启动", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    checkBox1.Checked = false;
                }
            }
            else
            {
                try { _rlocal.DeleteValue(appName); }
                catch (Exception)
                {
                    MessageBox.Show("取消开机自动启动失败，请检查是否授权", "自动启动", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    checkBox1.Checked = true;
                }
            }
        } //开机自动启动checkbox
        private void checkBox2_Click(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                config.WritePrivateProfileString("Main", "recordXY", "1");
                recordXY = true;
            }
            else
            {
                config.WritePrivateProfileString("Main", "recordXY", "0");
                recordXY = false;
            }
        } //记录位置checkbox
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (recordXY)
            {
                config.WritePrivateProfileString("Main", "X", Location.X.ToString());
                config.WritePrivateProfileString("Main", "Y", Location.Y.ToString());
            }
        } //窗口关闭时执行的事件
        private void checkBox3_Click(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                config.WritePrivateProfileString("Main", "edgeHide", "1");
                edgeHide = true;
                timer1.Enabled = true;
            }
            else
            {
                config.WritePrivateProfileString("Main", "edgeHide", "0");
                edgeHide = false;
                timer1.Enabled = false;
            }
        }//边缘吸附隐藏checkBox事件
        private void checkBox4_Click(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
            {
                config.WritePrivateProfileString("Main", "keepTop", "1");
                keepTop = true;
            }
            else
            {
                config.WritePrivateProfileString("Main", "keepTop", "1");
                keepTop = false;
            }
        }//保持置顶checkbox

        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 8)
            {
                e.Handled = false;
                return;
            }//退格键
            if (e.KeyChar == '.')
            {
                if (textBox6.Text.IndexOf('.') != -1)
                {
                    e.Handled = true;
                    return;
                }
                else
                {
                    e.Handled = false;
                    return;
                }
            }//如果输入的时小数点，检查之前是否输入过小数点
            if ((e.KeyChar < '0' || (e.KeyChar) > '9'))
            {
                e.Handled = true;
            }
            if (e.KeyChar == '0')
            {
                if (textBox6.Text == "")
                {
                    e.Handled = true;
                }
            }//不允许前导0
        }//钱包金额框仅接受数字或者小数点
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox6.Text == "" || textBox7.Text == "" || textBox6.Text=="0")
            {
                MessageBox.Show("请输入完整数据", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }// 没有输入金额或用途/来源
            if (radioButton1.Checked == false && radioButton2.Checked == false)
            {
                MessageBox.Show("请选择收入或支出", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }// 没有选择收入支出
            if (radioButton3.Checked == false && radioButton4.Checked == false && radioButton5.Checked == false && radioButton6.Checked == false)
            {
                MessageBox.Show("请选择途经[现金？银行卡？微信？支付宝？]", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }//没有选择途经
            String money_IO = "";
            String money_Type = "";
            String value = "";
            String useTo = "";
            String dates = "";
            if (radioButton1.Checked)
            {
                money_IO = "in";
            }else
            if (radioButton2.Checked)
            {
                money_IO = "out";
            }
            if (radioButton3.Checked)
            {
                money_Type = "wechat";
            }
            else
            if (radioButton4.Checked)
            {
                money_Type = "alipay";
            }
            else
            if (radioButton5.Checked)
            {
                money_Type = "cash";
            }
            else
            if (radioButton6.Checked)
            {
                money_Type = "card";
            }
            value = textBox6.Text;
            useTo = textBox7.Text;
            DateTime t = DateTime.Now;
            dates = t.Year.ToString() + "-";
            if (t.Month < 10)
            {
                dates += "0" + t.Month.ToString()+"-";
            }
            else
            {
                dates += t.Month.ToString() + "-";
            }
            if (t.Day < 10)
            {
                dates += "0" + t.Day.ToString();
            }
            else
            {
                dates += t.Day.ToString();
            }
            insertWallet("", money_IO, money_Type, value, useTo, dates);
            money.insertData(money_IO, money_Type, value, useTo, dates);
            getBalanceInfo();
            textBox6.Text = "";
            textBox7.Text = "";
        }//钱包录入按钮事件
        private void insertWallet(String arg0, String arg1, String arg2, String arg3, String arg4, String arg5)
        {
            if (arg3 == "0" || arg5=="0000-00-00")
            {
                return;
            }//清洗脏数据（计算余额时如果不存在就显示null，获取失败，因此必须把每种情况都加进去）
            if (arg0 == "")
            {
                arg0 = (int.Parse(money.getnNum()) + 1).ToString();
            }
            switch (arg1)
            {
                case "in": arg1 = "收入"; break;
                case "out": arg1 = "支出"; break;
            }
            switch (arg2)
            {
                case "wechat": arg2 = "微信"; break;
                case "alipay": arg2 = "支付宝"; break;
                case "cash": arg2 = "现金"; break;
                case "card": arg2 = "银行卡"; break;
            }

            listView8.BeginUpdate();
            ListViewItem newListViewItem = new ListViewItem(arg0);
            newListViewItem.SubItems.Add(arg1);
            newListViewItem.SubItems.Add(arg2);
            newListViewItem.SubItems.Add(arg3);
            newListViewItem.SubItems.Add(arg4);
            newListViewItem.SubItems.Add(arg5);
            listView8.Items.Add(newListViewItem);
            listView8.Show();
            listView8.EndUpdate();
            
        }//录入到钱包列表
        private void getBalanceInfo()
        {
            textBox8.Text = money.getBalance("cash");
            textBox9.Text = money.getBalance("wechat");
            textBox10.Text = money.getBalance("alipay");
            textBox11.Text = money.getBalance("card");
        }//获取钱包余额
        private void button2_Click(object sender, EventArgs e)
        {
            reload_Money();
        }//钱包中的刷新显示,一般情况下不需要点，是自动的。

        private void button3_Click(object sender, EventArgs e)
        {
            new wallet().Show();
        }//账单汇总窗口

        private void button4_Click(object sender, EventArgs e)
        {
            InitMyself();
        }
    }
}