using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Text;
using System.Windows.Forms;
//原始生成路径 bin\Debug\
namespace module
{
    public class Main
    {
        public ArrayList moduleList = new ArrayList(); //模块列表
        public static string Base64Decode(string value)
        {
            string result = Encoding.Default.GetString(Convert.FromBase64String(value));
            return result;
        } //base64解码
        public string GetMiddleStr(string oldStr, string preStr, string nextStr)
        {
            string tempStr = oldStr.Substring(oldStr.IndexOf(preStr) + preStr.Length);
            if (tempStr.IndexOf(nextStr) == -1)
                return "";
            tempStr = tempStr.Substring(0, tempStr.IndexOf(nextStr));
            return tempStr;

        }//取文本中间内容
        public Main() //主入口
        {
            Initialization();
        }
        private void Initialization() //初始化
        {
            moduleList.Add(new string[] { "查看wifi密码", "1.0α", "获取电脑上已经连接过的wifi信息", "2020-08-30" });
            moduleList.Add(new string[] { "迅雷直连转换器", "1.0α", "将迅雷连接转换", "2020-08-30" });
            //moduleList.Add(new string[] { "编码与加解密", "1.0α", "常见的编码以及加解密工具", "2020-08-30" });
        }
        public ArrayList getModuleList() //获取模块列表
        {
            return moduleList;
        }
        public void RunModule(String moduleName) //接收到运行指令
        {
            switch (moduleName)
            {
                case "查看wifi密码":
                    viewWiFiPassword();
                    break;
                case "迅雷直连转换器":
                    thunder2link();
                    break;
                case "编码与加解密":
                    thunder2link();
                    break;
            }
        }
        private void viewWiFiPassword() 
        {
            Form wifiview = new WifiView();
            wifiview.Show();
        }//查看wifi信息
        private void thunder2link()
        {
            
            string str = Interaction.InputBox("请输入迅雷连接", "迅雷直连转换器", "");
            if (!string.IsNullOrEmpty(str))
            {
                // thunder://QUFodHRwOi8veGlhb2JhaS5ydWFuamlhbmRvd24uY29tOjc0NTcvd2luZG93cyBTZXJ2ZXIgMjAwOC5pc29aWg==
                if (str.ToLower().StartsWith(@"thunder://"))
                {
                    String[] tmp;
                    tmp = str.Split(":////".ToCharArray());
                    if (tmp.Length == 4)
                    {
                        String t2 = Base64Decode(tmp[3]);

                        Interaction.InputBox("转换完成的迅雷直连", "迅雷直连转换器", t2.Substring(2, t2.Length - 4) );
                    }
                    else
                    {
                        MessageBox.Show("迅雷链接貌似不合法", "迅雷直连转换器", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("尚未发现迅雷链接的关键字[thunder:////]", "迅雷直连转换器", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        } // 迅雷直连转换
        private void eodncryption()
        {

        }//加解密工具
    }
}
