using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace 学生信息查询系统
{
    public partial class frm : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        SqlDataAdapter da = new SqlDataAdapter();
        DataSet ds;
        string sTableName, sColumnList;
        public frm()
        {
            InitializeComponent();
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void frm_Load(object sender, EventArgs e)
        {
            this.Text="数据表曝光";
            this.StartPosition = FormStartPosition.CenterScreen;
            string con = @"Data Source=.;Initial Catalog=StudentRoll;Integrated Security=true";
            cn.ConnectionString = con;
            cn.Open();
            cm.Connection = cn;
            cm.CommandText = "select name from sysobjects where type='U' order by name";
            SqlDataReader dr;
            dr = cm.ExecuteReader();
            int i, iCount = dr.FieldCount;
            string s;
            while (dr.Read())
            {
                comboBox1.Items.Add(dr["name"]);
            }
            dr.Close();
            cn.Close();
            view1.AllowUserToAddRows = false;
            view1.AllowUserToDeleteRows = false;
            view1.AllowUserToOrderColumns = false;
            view1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            view1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            view1.ReadOnly = true;
            view1.MultiSelect = true;
            view1.RowHeadersVisible = false;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.SelectedIndex = 0;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex < 0) return;
            if (cn.State != ConnectionState.Open)
                cn.Open();
            view1.Columns.Clear();
            string sTable = comboBox1.SelectedItem.ToString();
            cm.Connection = cn;
            cm.CommandText = "select c.name 列名,t.name 类型,c.length 长度,c.prec 精度,c.scale 小数"
                + " from sysobjects o,syscolumns c,systypes t"
                + " where o.id=c.id and c.xtype=t.xtype and o.name='" + sTable + "' order by colorder";
            ds = new DataSet();
            da.SelectCommand = cm;
            da.Fill(ds, "结构");
            view1.DataSource = ds;
            view1.DataMember = "结构";
            view1.Columns["列名"].Width = 66;
            view1.Columns["类型"].Width = 66;
            view1.Columns["长度"].Width = 40;
            view1.Columns["精度"].Width = 40;
            view1.Columns["小数"].Width = 40;
            view1.Columns["长度"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            view1.Columns["精度"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            view1.Columns["小数"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            view1.Columns[0].Frozen = true;
            cn.Close();
        }

        private void view1_SelectionChanged(object sender, EventArgs e)
        {
            int i, iCount;
            string s, sType;
            sColumnList = "";
            comboBox2.Items.Clear();
            comboBox2.Items.Add("");
            iCount = view1.Rows.Count;
            for (i = 0; i < iCount; i++)
            {
                if (view1.Rows[i].Selected)
                {
                    s = view1.Rows[i].Cells["列名"].Value.ToString();
                    sColumnList += "," + s;
                    sType = view1.Rows[i].Cells["类型"].Value.ToString();
                    if (sType == "varbinary" | sType == "image" | sType == "text")
                        continue;
                    comboBox2.Items.Add(s);
                }
            }
            if (sColumnList == "") return;
            sColumnList = sColumnList.Substring(1);
            sTableName = comboBox1.SelectedItem.ToString();
            textBox1.Text = "select " + sColumnList + "\r\nfrom " + sTableName;
            
            comboBox2.SelectedIndex = 0;
            checkBox1.Checked = false;
        }
        private void SetOrder()
        {
            if (comboBox2.SelectedIndex != 0)
            {
                textBox1.Text = "select " + sColumnList + "\r\nfrom " + sTableName+ "\r\norder by  " + comboBox2.SelectedItem.ToString() + " ASC";
            }
            if (checkBox1.Checked)
            {
                    textBox1.Text += "select " + sColumnList + "\r\nfrom " + sTableName+ "\r\norder by "+comboBox2.SelectedItem.ToString()+" desc";
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetOrder();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            SetOrder();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            cm.Connection = cn;
            cm.CommandText = textBox1.Text;
            da.SelectCommand = cm;
            if (ds.Tables.Count > 1)
            {
                ds.Tables["数据"].Clear();
                ds.Tables["数据"].Columns.Clear();
            }
            da.Fill(ds,"数据");
            view2.DataSource = ds;
            view2.DataMember = "数据";
            view2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            cn.Close();
        }
    }
}
