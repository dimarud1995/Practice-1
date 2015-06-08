using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Windows.Forms.DataVisualization.Charting;
 


namespace Lab1_ec
{
    public partial class RegresForm : Form
    {
        double[,] rez;
        double[,] matrs;
        double[,] mainmatrs;
        double [] koef;
        double[,] prog;

        public RegresForm()
        {
            InitializeComponent();
         }

        private void OpenFile_Click(object sender, EventArgs e) //Відкриття файлу
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "Файл Excel|*.XLSX;*.XLS"; ;

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                OleDbConnection connection = new OleDbConnection(string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES\";", dialog.FileName));
                OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT * FROM [Лист1$]", connection);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView1.DataSource = table;
            }
        }

        private void SaveFile_Click(object sender, EventArgs e) //Збереження файлу
        {
            var save = new SaveFileDialog
           {
               AddExtension = true,
               Filter = @"Файл Excel|*.XLS",
               FilterIndex = 2,
               RestoreDirectory = true
           };

            if (save.ShowDialog() != DialogResult.OK) return;
            var sw = new StreamWriter(save.FileName, true, Encoding.UTF8);

            foreach (DataGridViewRow row in dataGridView1.Rows)
                if (!row.IsNewRow)
                {
                    var first = true;
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (!first) sw.Write(";");
                        sw.Write(cell.Value.ToString());
                        first = false;
                    }
                    sw.WriteLine();
                }
            sw.Close();
        }        

        private void Koef_Click(object sender, EventArgs e) //Розрахунок коефіцієнтів
        {
            matrs = new double[dataGridView1.RowCount-1, dataGridView1.ColumnCount - 2];
            mainmatrs = new double[dataGridView1.RowCount-1, 1];

            for (int i = 0; i < matrs.GetLength(0); i++)
            {
                for (int j = 0; j < matrs.GetLength(1); j++)
                {
                    matrs[i, j] = Convert.ToDouble(dataGridView1.Rows[i].Cells[j + 1].Value);
                    mainmatrs[i, 0] = Convert.ToDouble(dataGridView1.Rows[i].Cells[dataGridView1.ColumnCount - 1].Value);
                }
            }

            rez = new double[matrs.GetLength(0), matrs.GetLength(1) + 1];

            for (int i = 0; i < rez.GetLength(0); i++)
            {
                for (int j = 1; j < rez.GetLength(1); j++)
                {   
                    rez[i, 0] = 1;
                    rez[i, j] = matrs[i, j - 1];              
                }
            }

           double[,] transpmatres = Transp(rez);

           double[,] multimatrs = Multiplication(transpmatres, rez);

           double[,] ober = Onemftrix(multimatrs, multimatrs.GetLength(0));

           double[,] multimatrs2 = Multiplication(transpmatres, mainmatrs);

           double[,] x = Multiplication(ober, multimatrs2);

           koef = new double[x.GetLength(0)];

           for (int i = 0; i < x.GetLength(0); i++)
             {
                 textBox2.Text += "    a" + i + " = " + Math.Round(x[i, 0] * 1000) / 1000.0;
                 if (i == 2) textBox2.Text += " \r\n";
                 koef[i] = x[i, 0];
             }

        }
       
        static double[,] Multiplication(double[,] a, double[,] b) //Множення 2х матриць
        {
            if (a.GetLength(1) != b.GetLength(0)) throw new Exception("Матриці неможливо перемножити!");
            double[,] rez = new double[a.GetLength(0), b.GetLength(1)];
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < b.GetLength(1); j++)
                {
                    for (int k = 0; k < b.GetLength(0); k++)
                    {
                        rez[i, j] += a[i, k] * b[k, j];
                    }
                }
            }
            return rez;
        }

        static double[,] Transp(double[,] a) //Розрахунок транспонованої матриці
        {
            double[,] temp = new double[a.GetLength(1), a.GetLength(0)];

            for (int i = 0; i < temp.GetLength(0); i++)
            {
                for (int j = 0; j < temp.GetLength(1); j++)
                {
                    temp[i, j] = a[j, i];
                }
            }
            return temp;
        }

        static double[,] Onemftrix(double[,] a, int len)// Обернена матриця
        {
            double[,] ob = new double[len, len];

            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < len; j++)
                {
                    if (i == j)
                    { ob[i, j] = 1; }
                    else
                    { ob[i, j] = 0; }
                }

            }

            double arg;
            int i1;

            for (int j = 0; j < len; )
            {
                for (int i = 0; i < len; )
                {
                    if (i == j)
                    { goto k; }
                    arg = a[i, j] / a[j, j];
                    for (i1 = 0; i1 < len; )
                    {
                        a[i, i1] = a[i, i1] - a[j, i1] * arg;
                        ob[i, i1] = ob[i, i1] - ob[j, i1] * arg;
                        i1++;
                    }
                k:
                    i++;
                }

                j++;
            }

            for (int j = 0; j < len; j++)
            {
                for (int i = 0; i < len; i++)
                {
                    double arg_2;
                    if (i == j)
                    {
                        arg_2 = a[i, j];
                        for (i1 = 0; i1 < len; )
                        {
                            a[i, i1] = a[i, i1] / arg_2;
                            ob[i, i1] = ob[i, i1] / arg_2;
                            i1++;
                        }
                    }

                }

            }
            return ob;
        }

        private void Future_Click(object sender, EventArgs e)// Розрахунок прогнозованого значення 
        {
            string k = " + ";
            textBox1.Text += Math.Round(koef[0] * 1000) / 1000.0 + " + ";

            for (int i = 1; i < koef.GetLength(0); i++)
            {
                if (i == koef.GetLength(0)-1) k = " ";
                textBox1.Text += Math.Round(koef[i] * 1000) / 1000.0 + "*f" + i + k;      
            }

            dataGridView1.Columns.Add("Y*", "Y*");
            double u = 0;
            prog = new double[rez.GetLength(0),  1];

            for (int i = 0; i < rez.GetLength(0); i++)
            {
                for (int j = 0; j < koef.GetLength(0); j++)
                {
                    u = u + koef[j]*rez[i,j];                   
                }

                dataGridView1.Rows[i].Cells["Y*"].Value += Convert.ToString(Math.Round(u * 1000) / 1000.0);
                dataGridView1.Rows[i].Cells["Y*"].Style.ForeColor = Color.Red;
                prog[i, 0] = u; 
                u = 0;
            }
        }

        private void Graf_Click(object sender, EventArgs e) //Побудова графіків
        {
            chart1.Series.Clear();            
            chart1.Series.Add("Реальні значення");
            chart1.Series.Add("Прогнозовані значення");
            chart1.Series["Реальні значення"].Color = System.Drawing.Color.Blue;
            chart1.Series["Прогнозовані значення"].Color = System.Drawing.Color.Red;

            if (radioButton1.Checked)
            {
                chart1.Series["Реальні значення"].MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Cross;
                chart1.Series["Прогнозовані значення"].MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Cross;
            }

            if (radioButton2.Checked)
            {
                chart1.Series["Реальні значення"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
                chart1.Series["Прогнозовані значення"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            }

            chart1.Legends[0].BackColor = System.Drawing.Color.Beige;
       
            for (int i = 0; i < rez.GetLength(0); i++)
            {
                chart1.Series["Реальні значення"].Points.Add(mainmatrs[i, 0]);
                chart1.Series["Прогнозовані значення"].Points.Add(prog[i, 0]);
                
            }            
        }
    }
}
