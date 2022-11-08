using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace tft
{
    public partial class Form2 : Form
    {
        public Form1 form1 = null;
        public Form2(Form1 f)
        {
            form1 = f;
            InitializeComponent();

            if ( form1.listBox4.SelectedIndices.Count == 0)
            {
                MessageBox.Show("not selected [use]");
                return;
            }
            listBox4.Items.Clear();
            for (int i = 0; i < form1.listBox1.Items.Count; i++)
            {
                listBox4.Items.Add(form1.listBox1.Items[i]);
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listBox4.Items.Count; i++)
            {
                listBox4.SetSelected(i, true);
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listBox4.Items.Count; i++)
            {
                listBox4.SetSelected(i, !listBox4.GetSelected(i));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string cmd = "";

            cmd += form1.tft_header_ru();
            cmd += "df <- read.csv(\"" + form1.base_name + ".csv\", header=T, stringsAsFactors = F, na.strings = c(\"\", \"NA\"))\r\n";
            cmd += "ids_cols = c(";

            int ids_cols = 0;
            for (int j = 0; j < form1.listBox4.SelectedIndices.Count; j++)
            {
                bool dup = false;
                for (int i = 0; i < listBox4.SelectedIndices.Count; i++)
                {
                    if (form1.listBox4.Items[form1.listBox4.SelectedIndices[j]].ToString() == listBox4.Items[listBox4.SelectedIndices[i]].ToString())
                    {
                        dup = true;
                        break;
                    }
                }
                if ( !dup )
                {
                    if (ids_cols > 0)
                    {
                        cmd += ",";
                    }
                    cmd += "'" + form1.listBox4.Items[form1.listBox4.SelectedIndices[j]] + "'";
                    ids_cols++;
                }
            }
            cmd += ")\r\n";

            cmd += "key_cols = c(";
            cmd += "'" + listBox4.Items[listBox4.SelectedIndices[0]] + "'";
            for (int i = 1; i < listBox4.SelectedIndices.Count; i++)
            {
                cmd += ",'" + listBox4.Items[listBox4.SelectedIndices[i]] + "'";
            }
            cmd += ")\r\n";

            cmd += "df2 <- horizontally_to_vertically(df, ids_cols, key_cols)\r\n";
            cmd += "write.csv(df2,'" + form1.csv_dir.Replace("\\", "/") + "/" + form1.base_name + "_V.csv', row.names = FALSE)\r\n";

            string file = "htov_" + form1.base_name + ".R";

            try
            {
                using (System.IO.StreamWriter sw = new StreamWriter(file, false, System.Text.Encoding.GetEncoding("shift_jis")))
                {
                    sw.Write(cmd);
                }
            }
            catch
            {
                if (MessageBox.Show("Cannot write in " + file, "", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    return;
            }

            form1.execute(file);

            form1.csv_file = form1.csv_dir.Replace("\\", "/") + "/" + form1.base_name + "_V.csv";
            form1.base_name = form1.base_name + "_V";
            File.Copy(form1.csv_file, form1.base_name + ".csv", true);
            
            Directory.CreateDirectory(form1.base_name);
            form1.work_dir = form1.base_dir + "\\work\\" + form1.base_name;
            System.IO.Directory.SetCurrentDirectory(form1.work_dir);
            File.Copy(form1.csv_file, form1.base_name + ".csv", true);
            
            form1.listBox_remake();

            form1.comboBox3.Text = "date";
            form1.comboBox4.Text = "target";
            form1.comboBox5.Text = "key";

            for (int i = 0; i < form1.listBox4.Items.Count; i++)
            {
                if (form1.listBox4.Items[i].ToString() == form1.comboBox3.Text)
                {
                    form1.listBox4.SetSelected(i, true);
                    continue;
                }
                if (form1.listBox4.Items[i].ToString() == form1.comboBox4.Text)
                {
                    form1.listBox4.SetSelected(i, true);
                    continue;
                }
                if (form1.listBox4.Items[i].ToString() == form1.comboBox5.Text)
                {
                    form1.listBox4.SetSelected(i, true);
                    continue;
                }
            }
            Close();
        }
    }
}
