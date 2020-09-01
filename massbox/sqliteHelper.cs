using System;
using System.Collections;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;

namespace massbox
{
    class sqliteHelper
    {
        private string appPath;
        private string dataPath;
        private string dbpath;
        private const String tableCreateCmd = @"CREATE TABLE money  (
	 _id  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE, 
	 _direct  VARCHAR NOT NULL,
	 _type  VARCHAR NOT NULL,
	 _value  NUMERIC NOT NULL,
	 _useto  TEXT NOT NULL,
	 _dates  DATE NOT NULL);
insert into main.money(_id,_direct,_type,_value,_useto,_dates) values(-8,'in','alipay','0','0','0000-00-00');
insert into main.money(_id,_direct,_type,_value,_useto,_dates) values(-7,'in','wechat','0','0','0000-00-00');
insert into main.money(_id,_direct,_type,_value,_useto,_dates) values(-6,'in','cash','0','0','0000-00-00');
insert into main.money(_id,_direct,_type,_value,_useto,_dates) values(-5,'in','card','0','0','0000-00-00');
insert into main.money(_id,_direct,_type,_value,_useto,_dates) values(-4,'out','alipay','0','0','0000-00-00');
insert into main.money(_id,_direct,_type,_value,_useto,_dates) values(-3,'out','wechat','0','0','0000-00-00');
insert into main.money(_id,_direct,_type,_value,_useto,_dates) values(-2,'out','cash','0','0','0000-00-00');
insert into main.money(_id,_direct,_type,_value,_useto,_dates) values(-1,'out','card','0','0','0000-00-00');
"; //建表sql
        public sqliteHelper()
        {
            appPath = AppDomain.CurrentDomain.BaseDirectory;
            dataPath = Path.Combine(appPath, "data");
            dbpath = Path.Combine(dataPath, "money.db");
            if (!File.Exists(dbpath))
            {
                createDB();
                CreateTable();
            }
            else
            {
                try
                {
                    getnNum();
                }
                catch (Exception)
                {
                    DialogResult v = MessageBox.Show("数据库文件打开失败，可能数据库文件已经损坏，是否重新创建？", "错误", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    if (v == DialogResult.Yes)
                    {
                        try
                        {
                            DeleteDB();
                            createDB();
                            CreateTable();
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("创建失败，请联系开发商", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }
        public void createDB()
        {
            try
            {
                SQLiteConnection.CreateFile(dbpath);
            }
            catch (Exception ex)
            {
                throw new Exception("新建数据库文件" + dbpath + "失败：" + ex.Message);
            }

        } //创建数据库
        public void DeleteDB()
        {
            if (File.Exists(dbpath))
            {
                File.Delete(dbpath);
            }
        }//删除数据库
        public void CreateTable()
        {
           
            SQLiteConnection sc = new SQLiteConnection("data source=" + dbpath); 
            if (sc.State != System.Data.ConnectionState.Open)
            {
                sc.Open();
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.Connection = sc;
                cmd.CommandText = tableCreateCmd;
                cmd.ExecuteNonQuery();
            }
        } //创建数据表
        public void DeleteTable(String tableName)
        {
            SQLiteConnection sc = new SQLiteConnection("data source=" + dbpath);
            SQLiteCommand cmd = new SQLiteCommand();
            sc.Open();
            cmd.Connection = sc;
            cmd.CommandText = "DROP TABLE IF EXISTS " + tableName;
            cmd.ExecuteNonQuery();
            sc.Close();
            cmd.Dispose();
        }//删除表
        public void insertData(String arg1, String arg2, String arg3, String arg4, String arg5)
        {
            using (SQLiteConnection sc = new SQLiteConnection("data source=" + dbpath))
            {
                SQLiteCommand cmd = new SQLiteCommand();
                sc.Open();
                cmd.Connection = sc;
                cmd.CommandText = string.Format(@"insert into main.money(_direct,_type,_value,_useto,_dates) values('{0}','{1}','{2}','{3}','{4}')", arg1, arg2, arg3, arg4, arg5);
                cmd.ExecuteNonQuery();
            }
               
        } //插入money数据
        public String getnNum()
        {
            using (SQLiteConnection sc = new SQLiteConnection("data source=" + dbpath))
            {
                sc.Open();
                string sql = "select seq from main.sqlite_sequence";
                SQLiteCommand command = new SQLiteCommand(sql, sc);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read()) {
                        return reader.GetInt32(0).ToString();
                    }
                }
            }
            return "0";
        } //获取编号
        public ArrayList getAllData()
        {
            ArrayList rst = new ArrayList();
            
            using (SQLiteConnection sc = new SQLiteConnection("data source=" + dbpath))
            {
                sc.Open();
                string sql = "select * from main.money";
                SQLiteCommand command = new SQLiteCommand(sql, sc);
                
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read()) {

                        String[] record = { "", "", "", "", "", "" };
                        record[0] = reader.GetInt32(0).ToString();
                        record[1] = reader.GetString(1);
                        record[2] = reader.GetString(2);
                        record[3] = reader.GetFloat(3).ToString();
                        record[4] = reader.GetString(4);
                        record[5] = reader.GetString(5);

                        rst.Add(record);
                        
                    }
                }
            }
            return rst;
        } //获取全部数据
        public String getBalance(String t)
        {
            String rst="";
            using (SQLiteConnection sc = new SQLiteConnection("data source=" + dbpath))
            {
                sc.Open();
                String sql = string.Format(@"SELECT (sum(_value)-(SELECT sum(_value) FROM money where _direct = 'out' and _type = '{0}'))FROM money where _direct = 'in' and _type = '{0}'", t);
                SQLiteCommand command = new SQLiteCommand(sql, sc);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        try { 
                        return reader.GetFloat(0).ToString();
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
            }
            return rst;
        }//获取总余额
        public ArrayList getDataByWhere(String where)
        {
            ArrayList rst = new ArrayList();
            using (SQLiteConnection sc = new SQLiteConnection("data source=" + dbpath))
            {
                sc.Open();
                string sql = "select * from main.money "+where;
                SQLiteCommand command = new SQLiteCommand(sql, sc);
                Console.WriteLine(sql);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        String[] record = { "", "", "", "", "", "" };
                        record[0] = reader.GetInt32(0).ToString();
                        record[1] = reader.GetString(1);
                        record[2] = reader.GetString(2);
                        record[3] = reader.GetFloat(3).ToString();
                        record[4] = reader.GetString(4);
                        record[5] = reader.GetString(5);

                        rst.Add(record);

                    }
                }
            }
            return rst;
        }
    }
}
