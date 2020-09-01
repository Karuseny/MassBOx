using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

public class FileProc
{
	String path;
	public struct ShortcutDescription
	{
		public String TargetPath;
		public String Arguments;
		public String WorkDir;
	};

	public FileProc(String path)
	{
		this.path = path;
	}
	public void setPath(String path)
	{
		this.path = path;
	}
	public String getType()
	{
		String tmp = "";
		for (int i = path.Length - 1; path[i] != '.'; i--)
		{
			tmp = path[i] + tmp;
            if (path[i] == '\\')
            {
				return " ";
            }
		}
		return tmp;
	}

	public String getName()
	{
		String tmp = "";
		for (int i = path.Length - 1; path[i] != '\\'; i--)
		{
			tmp = path[i] + tmp;
			if (i == 0)
            {
				break;
            }
		}
		return tmp;
	}
	public String getNameNonType()
	{
		return getName().Replace("." + getType(), "");
	}
	public String getPath()
	{
		String[] tmp;
		String rst = "";
		tmp = path.Split('\\');
		for (int i = 0; i < tmp.Length-1; i++)
		{
			rst = rst + "\\" + tmp[i];
		}
		if (rst == "")
			return "";
		else
			return rst.Remove(0, 1); ;
	}
	public Image getIcon()
	{
		if (!File.Exists(path))
		{
			MessageBox.Show(path);
		}
		Image img = Icon.ExtractAssociatedIcon(path).ToBitmap();
		return img;
	}


	public  ShortcutDescription ReadShortcut()
	{
		ShortcutDescription desc = new ShortcutDescription();
		if (this.getType() != "lnk")
        {
			desc.TargetPath = this.getPath();
        }
        else { 
		var shellType = Type.GetTypeFromProgID("WScript.Shell");
		dynamic shell = Activator.CreateInstance(shellType);
		var shortcut = shell.CreateShortcut(path);
		desc.TargetPath = shortcut.TargetPath;
		desc.Arguments = shortcut.Arguments;
		desc.WorkDir = shortcut.WorkingDirectory;
		}
		return desc;
	}

	public int whatsType()
    {
		 if (Directory.Exists(path))
        {
			return 1;
        }
        else if(File.Exists(path))
        {
			return 2;
        }
        else
        {
			return 0;
        }

    }

}