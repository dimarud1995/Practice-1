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
using System.Text.RegularExpressions;

namespace Lab1
{
    public partial class Form1 : Form 
    {   
        
        public string codeline = "";
        public string[,] matrtext;
        bool checkalf = false;

        public Form1()
        {
            InitializeComponent();
        }
       
        public void Checking() //Перевірка коректності
        {
           if (textBox3.Text == String.Empty)
            {
                MessageBox.Show("Введіть фрагмент тексту!");
                codeline = "";
                textBox3.Text = " ";
            }

            if (textBox1.Text == String.Empty)
            {
                MessageBox.Show("Введіть вираз для шифрування чи загрузіть файл!");
            }

            codeline = textBox1.Text;

            if (checkalf == true)
            {
                if (!radioButton1.Checked && !radioButton2.Checked)
                {
                    MessageBox.Show("Виберіть мову!");
                    codeline = "";
                }

                string check = LanguagetText(codeline);

                if (check == "ua")
                {
                    MessageBox.Show("Увімкнений український алфавіт. Змініть розкладку клавіатури!");
                    codeline = "";
                }

                if (check == "uni")
                {
                    MessageBox.Show("Використано недоступні символи!");
                    codeline = "";
                }

                if ((check == "rus") && (radioButton1.Checked))
                {
                    MessageBox.Show("Переключіть мову!");
                    codeline = "";
                }

                if ((check == "eng") && (radioButton2.Checked))
                {
                    MessageBox.Show("Переключіть мову!");
                    codeline = "";
                }

                int n = 0;

                for (int k = 0; k < codeline.Length; k++)
                {
                    for (int i = 0; i < matrtext.GetLength(0); i++)
                    {
                        for (int j = 0; j < matrtext.GetLength(1); j++)
                        {
                            if (Convert.ToString(codeline[k])==matrtext[i,j])
                                n += 1;
                        }
                    }

                    if (n == 0)
                    {
                        MessageBox.Show("Немає символів!");
                        codeline = "";
                        textBox1.Text = "";
                    }

                    n = 0;
                }
                
            }        
        }

        public string LanguagetText(string text)//Визначення введеної мови
        {
            bool rus = false, uni = false, eng = false, ua = false;  

            text = text.ToLower();

            byte[] Ch = System.Text.Encoding.Default.GetBytes(text);

            foreach (byte ch in Ch)
            {
                if ((ch >= 224) && (ch <= 255)  || (ch == 184)) rus = true;
                if ((ch >= 97) && (ch <= 122)) eng = true;
                if ((ch == 179) || (ch == 191)) ua = true;
                if (ch > 255) uni = true;
            }

            if (uni) return "uni";

            if (eng) return "eng";

            if (rus) return "rus";

            if (ua) return "ua";

            return "";
        }

        private void Open_text(object sender, EventArgs e)//Відкриття та зчитування фрагменту текста
        {
            OpenFileDialog o = new OpenFileDialog();

            o.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";

            if (o.ShowDialog() == DialogResult.OK)
            {
                textBox3.Text = File.ReadAllText(o.FileName, Encoding.Default);
            }

            string[] lines = File.ReadAllLines(o.FileName, Encoding.Default);

            char[] temp = lines[1].ToCharArray(); 

            matrtext = new string[lines.Length, temp.Length];

            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].Replace(" ", string.Empty);
                temp = lines[i].ToCharArray();

                for (int j = 0; j < temp.Length; j++)
                  {
                     matrtext[i, j] = Convert.ToString(temp[j]);
                  }
                 
            }

            matrtext[matrtext.GetLength(0) - 1, matrtext.GetLength(1) - 1] = " ";          
        }

        private void Encrypt_Click(object sender, EventArgs e)// Шифрування
        {
            checkalf = true;
            Checking();
            Stirl l = new Stirl();
            this.textBox2.Text = l.Encrypt(codeline, matrtext);
        }

        private void Decrypt_Click(object sender, EventArgs e)//Розшифрування
        {
            checkalf = false;
            Checking();
            Stirl l = new Stirl();
            this.textBox2.Text = l.Decript(codeline, matrtext);
        }

        private void OpenFile_Click(object sender, EventArgs e)//Відкриття файлу
        {
            OpenFileDialog o = new OpenFileDialog();
            o.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (o.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = File.ReadAllText(o.FileName, Encoding.Default);
            }
        }

        private void SaveFile_Click(object sender, EventArgs e)//Збереження файлу
        {
            SaveFileDialog s = new SaveFileDialog();
            s.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (s.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(s.FileName, textBox2.Text, Encoding.Default);
            }
        }

        private void Change_Click(object sender, EventArgs e)//Міняється текст в TextBox-ах
        {
            textBox1.Text = textBox2.Text;
            textBox2.Text = string.Empty;
        }

        private void CleanText1_Click(object sender, EventArgs e)//Видалення тексту
        {
            textBox1.Text = string.Empty;
        }

        private void CleanText2_Click(object sender, EventArgs e)//Видалення тексту
        {
            textBox2.Text = string.Empty;
        }

    }
}


     