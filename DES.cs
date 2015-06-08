using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace Lab1
{
    class DES
    {
        public string Encrypt(string str, string key)//Шифрування DES
        {
            string text;

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();

            cryptoProvider.Key = ASCIIEncoding.ASCII.GetBytes(key);

            cryptoProvider.IV = ASCIIEncoding.ASCII.GetBytes(key);

            MemoryStream memoryStream = new MemoryStream();

            CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateEncryptor(), CryptoStreamMode.Write);

            StreamWriter writerStream = new StreamWriter(cryptoStream);

            writerStream.Write(str);

            writerStream.Flush();

            cryptoStream.FlushFinalBlock();

            text  = Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);   
           
            return text ;
        }

        public string Decript(string str, string key) //Розшифрування DES
        {
            string text;

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();

            cryptoProvider.Key = ASCIIEncoding.ASCII.GetBytes(key);

            cryptoProvider.IV = ASCIIEncoding.ASCII.GetBytes(key);

            MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(str));

            CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateDecryptor(), CryptoStreamMode.Read);
            
            StreamReader readerStream = new StreamReader(cryptoStream);
        
            text = readerStream.ReadToEnd();

            return text;
        }
        
    }

}














