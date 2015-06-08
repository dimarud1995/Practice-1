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
 


namespace Lab1_ec
{
    public partial class MultikolForm : Form
    {
        double[,] rez;
        int sumRow = 0;
    
        public MultikolForm()
        {
            InitializeComponent();
        }

        private void OpenFile_Click(object sender, EventArgs e)// Відкриття файлу
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

        private void SaveFile_Click(object sender, EventArgs e)//Збереження файлу
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

            foreach (DataGridViewRow row in dataGridView2.Rows)
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

        private void Corel_Click(object sender, EventArgs e)//Кореляція
        {
            int i = 0;
            int j = 0;
            double[,] matrs;
            matrs = new double[dataGridView1.RowCount, dataGridView1.ColumnCount-1];

            for ( i = 0; i < matrs.GetLength(0); i++)
            {
                for ( j = 0; j < matrs.GetLength(1); j++)
                {
                    matrs[i, j] = Convert.ToDouble(dataGridView1.Rows[i].Cells[j+1].Value) ;
                }
            }
           
            double[] avr;
            double[] sumkv;
            double[] disp;
            double[,] matrs2;
            matrs2 = new double[dataGridView1.RowCount, dataGridView1.ColumnCount];
            sumRow = matrs.GetLength(0) - 1;
            avr = new double[matrs.GetLength(1)];
            sumkv = new double[matrs.GetLength(1)];
            disp = new double[matrs.GetLength(1)];
            double summ = 0;

            // Середнє
            for ( j = 0; j < matrs.GetLength(1); j++) 
            {
                summ = 0;
                for ( i = 0; i < matrs.GetLength(0); i++) 
                {
                    summ += matrs[i, j];
                }
                avr[j] = summ / sumRow;                
            }

            // Дисперсія
            for (j = 0; j < matrs.GetLength(1); j++)
            {
                summ = 0;
                for (i = 0; i < matrs.GetLength(0)-1; i++)
                {
                    summ += (matrs[i, j] - avr[j]) * (matrs[i, j] - avr[j]);
                }
                disp[j] = summ / sumRow;
            }
            
            // Матриця кореляції
            label9.Text = "Матриця стандартизованих незалежних змінних"; ;
            for ( i= 0; i < matrs.GetLength(0)-1; i++)
            { 
                for (j = 0; j < matrs.GetLength(1) ; j++)
                {
                    matrs2[i, j] = (matrs[i, j] - avr[j]) / Math.Sqrt(disp[j] * sumRow);
                    dataGridView1.Rows[i].Cells[j].Value = Math.Round(matrs2[i, j]*1000)/1000.0;
                }
                
            }

            label8.Text = "Кореляційна матриця"; 

            int k = 0;
            int l = 0;
            double sumxy = 0;
            double avrxy = 0;
            double koefkorel = 0;
            dataGridView2.RowCount = matrs.GetLength(1);
            dataGridView2.ColumnCount = matrs.GetLength(1); 

            for (l = 0; l < matrs.GetLength(1); l++)
            {
                for (k = 0; k < matrs.GetLength(1); k++)
                {
                    sumxy = 0;
                    for (i = 0; i < sumRow; i++)
                    { 
                        sumxy = sumxy + matrs[i, l] * matrs[i, k];
                    }  
                        avrxy = sumxy / sumRow;
                        koefkorel = (avrxy - avr[k] * avr[l]) / (Math.Sqrt(disp[l]) * Math.Sqrt(disp[k]));
                        dataGridView2.Rows[l].Cells[k].Value = koefkorel;
                } 
                dataGridView2.Columns[l].HeaderText = dataGridView1.Columns[l+1].HeaderText;
                dataGridView2.Rows[l].HeaderCell.Value = dataGridView1.Columns[l+1].HeaderText; 
            }

            rez = new double[dataGridView2.RowCount, dataGridView2.ColumnCount];
            for (i = 0; i < dataGridView2.RowCount; i++)
            {
                for (j = 0; j < dataGridView2.ColumnCount; j++)
                {
                    rez[i, j] = Convert.ToDouble(dataGridView2.Rows[i].Cells[j].Value);
                }
            }

            // Визначник матриці, ХІ - квадрат
            const double xitable = 24.996;
            double det = Determ(rez);
            textBox3.Text = Convert.ToString(det);
            double xi = -(sumRow - 1 - 1 / 6 * (2 * matrs.GetLength(1)+ 5)) * Math.Log(det, Math.E);
            textBox4.Text = Convert.ToString(xi);

            if (xi > xitable)
            {
                textBox1.Text = "В масиві незалежних змінних існує мультиколінеарність.";
                button5.Enabled = true;
            }
            else
            {
                MessageBox.Show("Аналіз завершено. Мультиколінеарності неіснує!");
            }
            button3.Enabled = false;
        }

        public static double[,] GetMinor(double[,] matrix, int row, int column)//Розрахунок мінора матриці
        {
            double[,] buf = new double[matrix.GetLength(0) - 1, matrix.GetLength(0) - 1];
            for (int i = 0; i < matrix.GetLength(0); i++)
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if ((i != row) || (j != column))
                    {
                        if (i > row && j < column) buf[i - 1, j] = matrix[i, j];
                        if (i < row && j > column) buf[i, j - 1] = matrix[i, j];
                        if (i > row && j > column) buf[i - 1, j - 1] = matrix[i, j];
                        if (i < row && j < column) buf[i, j] = matrix[i, j];
                    }
                }
            return buf;
        }

        public static double Determ(double[,] matrix)// Розрахунок детермінанта матриці
        {
            if (matrix.GetLength(0) != matrix.GetLength(1)) throw new Exception(" Число строк в матрице не совпадает с числом столбцов");
           
            double det = 0;
            int Rank = matrix.GetLength(0);

            if (Rank == 1) det = matrix[0, 0];

            if (Rank == 2) det = matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];

            if (Rank > 2)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    det += Math.Pow(-1, 0 + j) * matrix[0, j] * Determ(GetMinor(matrix, 0, j));
                }
            }
            return det;
         }

        public static double[,] Onematrix(double[,] matrix, int len)//Обернена матриця
        { 
            double[,] ob = new double[len, len];

            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < len;j++ )
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
                    arg = matrix[i, j] / matrix[j, j];
                    for (i1 = 0; i1 < len; )
                    {
                        matrix[i, i1] = matrix[i, i1] - matrix[j, i1] * arg;
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
                        arg_2 = matrix[i, j];
                        for (i1 = 0; i1 < len; )
                        {
                            matrix[i, i1] = matrix[i, i1] / arg_2;
                            ob[i, i1] = ob[i, i1] / arg_2;
                            i1++;
                        }
                    }
                    
                }
                
            }
            return ob;
        }   

        private void F_Click(object sender, EventArgs e)//F-критерії
        {
            button5.Enabled = false;
            const double fish = 2.740;
            label8.Text = "Обернена матриця";
            textBox1.Text = " ";
            //Обернена матриця
            double[,] ober = Onematrix(rez, rez.GetLength(0));

            for (int i = 0; i < dataGridView2.RowCount; i++)
            {
                for (int j = 0; j < dataGridView2.ColumnCount; j++)
                {
                     dataGridView2.Rows[i].Cells[j].Value = ober[i,j];
                     rez[i, j] = ober[i, j];
                }
            }

            //Коефіцієнти фішера
            double[] fisher = new double[rez.GetLength(0)];

            for (int i = 0; i < dataGridView2.RowCount; i++)
            {
                fisher[i] = (ober[i, i] - 1) * (sumRow - rez.GetLength(0)) / (rez.GetLength(0)-1);
            }

            for (int i = 0; i < dataGridView2.RowCount; i++)
            {
                textBox5.Text += Convert.ToString(Math.Round(fisher[i], 2)) + "\r\n";
                label7.Text += dataGridView2.Columns[i].HeaderText + "\r\n";
            }

            bool check = false;

            for (int i = 0; i < dataGridView2.RowCount; i++)
             
                if (fisher[i] > fish)
                {
                    textBox1.Text += dataGridView2.Columns[i].HeaderText + "  - незалежна змінна, що мультиколінеарна з іншими. \r\n";
                    button4.Enabled = true;
                    check = true;
                }

            if (check == false)
            {
                MessageBox.Show("Аналіз завершено. Мультиколінеарності неіснує!");
            }
        }

        private void Student_Click(object sender, EventArgs e)
        {
            textBox1.Text = " ";

            double[,] matrparkoef = new double[rez.GetLength(0), rez.GetLength(1)];

            label8.Text = "Матриця частинних коефіцієнтів кореляції";

            for (int i = 0; i < dataGridView2.RowCount; i++)
            {
                for (int j = 0; j < dataGridView2.ColumnCount; j++)
                {
                    matrparkoef[i, j] = -rez[i, j] / (Math.Sqrt(rez[i, i] * rez[j, j]));
                    dataGridView2.Rows[i].Cells[j].Value = matrparkoef[i, j];
                }
            }

            //System.Threading.Thread.Sleep(3000);

            double[,]  krst = new double[rez.GetLength(0), rez.GetLength(1)];
            label8.Text = "Матриця критеріїв Стьюдента";
            const double st = 2.093;

            for (int i = 0; i < dataGridView2.RowCount; i++)
            {
                for (int j = 0; j < dataGridView2.ColumnCount; j++)
                {
                    dataGridView2.Rows[i].Cells[j].Value = " ";
                }
            }

            for (int i = 0; i < dataGridView2.RowCount; i++)
            {
                for (int j = 0; j < dataGridView2.ColumnCount; j++)
                {                    
                    if (j > i)
                    {
                        krst[i, j] = matrparkoef[i, j] * Math.Sqrt(sumRow - rez.GetLength(0) - 1) / Math.Sqrt(1 - matrparkoef[i, j] * matrparkoef[i, j]);
                        dataGridView2.Rows[i].Cells[j].Value = krst[i, j];
                        if (krst[i, j] > st) 
                        {
                            dataGridView2.Rows[i].Cells[j].Style.BackColor = Color.DarkSeaGreen ;
                        textBox1.Text += "Між незалежна змінними " + dataGridView2.Columns[i].HeaderText + " і " + dataGridView2.Columns[j].HeaderText + " існує мультиколінеарність. \r\n";
                        }
                    }
                   
                }
            }

            button4.Enabled = false;
        }

    }

}
