using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
    class Stirl
    {
        int [] randi;
        int [] randj;
        int [,] randomkey;    

        public void RandomKey(string str, string [,] f)// Створюється псевдо випадковий ключ
        {
            int n = 0;
            randomkey = new int[str.Length,2];

            Random rand = new Random( );

            for (int k = 0; k < str.Length; k++)
            {
                n = 0;
                randi = new int[f.GetLength(0) * f.GetLength(1)];
                randj = new int[f.GetLength(0) * f.GetLength(1)];

                for (int i = 0; i < f.GetLength(0); i++)
                {
                    for (int j = 0; j < f.GetLength(1); j++)
                    {
                        if (str[k].ToString() == f[i, j])
                        {
                            randi[n] = i;
                            randj[n] = j;
                            n++;
                        }
                        
                    }
                }

                int b = rand.Next(0, n);

                randomkey[k,0] = randi[b];

                randomkey[k, 1] = randj[b];
            }

        }

        public string Encrypt(string str, string[,] matrtext) //Шифрування
        {
            StringBuilder codingtext = new StringBuilder(str.Length);

            RandomKey(str, matrtext);

            string zero = "0";
            string symbol;

                for (int i = 0; i < randomkey.GetLength(0); i++)
                {
                    for (int j = 0; j < randomkey.GetLength(1); j++)
                    {
                                                   
                        if (randomkey[i, j] < 10)
                        {
                            symbol = zero + Convert.ToString(randomkey[i, j]);
                        }
                        else
                        {
                            symbol = Convert.ToString(randomkey[i, j]);
                        }

                        codingtext.Append(symbol);
                    }

                    codingtext.Append(" ");
                }
            
           return codingtext.ToString();
        }

        public string Decript(string str,  string[,] matrtext)//Розшифрування
        {
            StringBuilder codingtext = new StringBuilder(str.Length);

            string symbol;

            str = str.Replace(" ", string.Empty);
            randomkey = new int[str.Length/4, 2];
            int k = 0;

            for (int i = 0; i < str.Length/4; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    randomkey[i, j] = Convert.ToInt32(str.Substring(k, 2));
                    k = k + 2;
                }

            }
                 
           for (int i = 0; i < randomkey.GetLength(0); i++)
            {
                 symbol = Convert.ToString(matrtext[randomkey[i, 0], randomkey[i, 1]]);
                 codingtext.Append(symbol);
            }
                       
            return codingtext.ToString();
        }
    }

}














