using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.DirectoryServices;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace massbox
{
    public partial class wallet : Form
    {
        private sqliteHelper money;
        public wallet()
        {
            InitializeComponent();
            money = new sqliteHelper();
            dateTimePicker1.Value= dateTimePicker1.Value.AddDays(-dateTimePicker1.Value.Day + 1); //规定为本月初
            dateTimePicker2.Value = dateTimePicker1.Value.AddMonths(1);//如果今天是1号，dtp1_vc不能生效，故手动生效。
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker2.Value = dateTimePicker1.Value.AddMonths(1);
        } //起始日期改变后默认终止日期为一个月后
        private String getSQLDate(DateTime t)
        {
            String dates;
            dates = t.Year.ToString() + "-";
            if (t.Month < 10)
            {
                dates += "0" + t.Month.ToString() + "-";
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
            return dates;
        }//获取标准sqlite时间类型
        private void button1_Click(object sender, EventArgs e)
        {
            listView8.Items.Clear();
            String direct = ""; 
            String type = "";
            String sd = "";
            String ed = "";
            if (radioButton1.Checked == false && radioButton2.Checked == false && radioButton3.Checked == false)
            {
                MessageBox.Show("请选择收入或支出", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }// 没有选择收入支出
            if ( radioButton4.Checked == false && radioButton5.Checked == false && radioButton6.Checked == false && radioButton7.Checked == false && radioButton8.Checked == false)
            {
                MessageBox.Show("请选择途经[现金？银行卡？微信？支付宝？]", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }//没有选择途经
            if (radioButton1.Checked)
            {
                direct = "in";
            }else if (radioButton2.Checked)
            {
                direct = "out";
            }else if (radioButton3.Checked)
            {
                direct = "";
            }

            if (radioButton7.Checked)
            {
                type = "wechat";
            }
            else if (radioButton4.Checked)
            {
                type = "alipay";
            }
            else if (radioButton5.Checked)
            {
                type = "cash";
            }
            else if (radioButton6.Checked)
            {
                type = "card";
            }
            else if (radioButton8.Checked)
            {
                type = "";
            }

            sd = getSQLDate(dateTimePicker1.Value);
            ed = getSQLDate(dateTimePicker2.Value);
            String where= string.Format(@"where _dates between date('{0}') and date('{1}')",sd,ed);
            if (direct != "") {
            where += string.Format(@"and _direct = '{0}'", direct);
            }
            if (type != "")
            {
                where += string.Format(@"and _type = '{0}'", type);
            }
            queryInfo(where);
        }//汇总按钮事件
        private void queryInfo(String where)
        {
            ArrayList tmp = new ArrayList();
            tmp = money.getDataByWhere(where);
            foreach (Array a in tmp)
            {
                insertList(a.GetValue(0).ToString(),
                    a.GetValue(1).ToString(),
                    a.GetValue(2).ToString(),
                    a.GetValue(3).ToString(),
                    a.GetValue(4).ToString(),
                    a.GetValue(5).ToString());
            }
            getAbsoluteValueToTexBox();
        }//根据选择的执行查询操作
        private void insertList(String arg0, String arg1, String arg2, String arg3, String arg4, String arg5)
        {
            if (arg3 == "0" || arg5 == "0000-00-00")
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
        private void getAbsoluteValueToTexBox()
        {
            Boolean aom = false;
            double cash = 0;
            double wechat = 0;
            double alipay = 0;
            double card = 0;
            double value = 0;
            for (int i = 0; i < listView8.Items.Count; i++)
            {
                value = double.Parse(listView8.Items[i].SubItems[3].Text);
                if (listView8.Items[i].SubItems[1].Text == "收入")
                {
                    aom = true;
                }
                if (listView8.Items[i].SubItems[1].Text == "支出")
                {
                    aom = false;
                }
                if (listView8.Items[i].SubItems[2].Text == "现金")
                {
                    cash += aom ? value : -value;
                }
                if (listView8.Items[i].SubItems[2].Text == "微信")
                {
                    wechat += aom ? value : -value;
                }
                if (listView8.Items[i].SubItems[2].Text == "支付宝")
                {
                    alipay += aom ? value : -value;
                }
                if (listView8.Items[i].SubItems[2].Text == "银行卡")
                {
                    card += aom ? value : -value;
                }
                textBox8.Text = cash.ToString();
                textBox9.Text = wechat.ToString();
                textBox10.Text = alipay.ToString();
                textBox11.Text = card.ToString();
            }
        }
    }
}
