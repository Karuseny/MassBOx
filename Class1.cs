using System;

public class FileProc
{
	String path
	public FileProc(String path)
	{
		this.path = path;
	}
	public String getType()
    {
		String tmp = "";
		for(int i = path.Length - 1; path[i]!='.'; i--)
        {
			tmp = path[i]+tmp;
        }
		return tmp;
    }
}
