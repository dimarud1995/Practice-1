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
    public partial class DESForm : Form 
    {   
        
        public string codeline = "";
        public string key = "";
        bool checkKey = true;    
    
        public DESForm()
        {
            InitializeComponent();            
        }
       
        public void Checking()// Перевірка коректності
        {
            checkKey = true;

            if (textBox1.Text == String.Empty)
            {
                MessageBox.Show("Введіть вираз для шифрування чи загрузіть файл!");
            }

            if (maskedTextBox1.Text == String.Empty)
            {
                checkKey = false;
                MessageBox.Show("Введіть ключ!"); 
            }
           
            if (maskedTextBox1.Text.Length < 8)
            {
                checkKey = false;
                MessageBox.Show("Неправильна довжина ключа!");
            }

            codeline = textBox1.Text;
            key = maskedTextBox1.Text;
        }

     
        private void open(object sender, EventArgs e)// Вікриття файлу
        {
            OpenFileDialog o = new OpenFileDialog();

            o.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";

            if (o.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = File.ReadAllText(o.FileName, Encoding.Default);
            }

        }

        private void save(object sender, EventArgs e)//Збереження файлу
        {
            SaveFileDialog s = new SaveFileDialog();

            s.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";

            if (s.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(s.FileName, textBox2.Text, Encoding.Default);
            }

        }

        private void onquery(object sender, EventArgs e)// Заміна місцями текта
        {
            textBox1.Text = textBox2.Text;
            textBox2.Text = string.Empty;
        }

        private void Empty1(object sender, EventArgs e)//Видалення тексту
        {
            textBox1.Text = string.Empty;
        }

        private void Empty2(object sender, EventArgs e)//Видалення тексту
        {
            textBox2.Text = string.Empty;
        }

        private void Encrypt(object sender, EventArgs e)//Шифрування
        {
            Checking();            

            if (checkKey == true)
            {
                DES l = new DES();
                this.textBox2.Text = l.Encrypt(codeline, key);
             }
           
        }

        private void Decrypt(object sender, EventArgs e)//Розшифрування
        {
            Checking();

            if (checkKey == true)
            {
                DES l = new DES();
                this.textBox2.Text = l.Decript(codeline, key);
            }
            
        }
             
    }
}


     