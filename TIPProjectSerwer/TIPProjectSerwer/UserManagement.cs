using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TIPProjectSerwer
{
    class UserManagement
    {
        Aes aes;

        public UserManagement()
        {
            aes = Aes.Create();
            aes.Key = new byte[]{ 96, 167, 199, 81, 63, 253, 14, 246, 83, 54, 28, 1, 37, 7, 254, 238, 148, 54, 180, 52, 66, 234, 65, 215, 116, 199, 163, 25, 239, 135, 2, 155};
            aes.IV = new byte[] { 5, 185, 145, 14, 181, 76, 50, 198, 236, 182, 110, 164, 66, 68, 226, 33};
        }

        public bool Login(string login, string password)
        {
            string data = $"{login};{password}";

            try
            {
                FileStream fileStream = new FileStream("TIPDB.bin", FileMode.OpenOrCreate);
                CryptoStream cryptoStream = new CryptoStream(fileStream, aes.CreateDecryptor(aes.Key, aes.IV), CryptoStreamMode.Read);
                StreamReader streamReader = new StreamReader(cryptoStream);
                string dbData;

                try
                {
                    dbData = streamReader.ReadToEnd();

                    foreach (var line in dbData.Split('|'))
                    {
                        if (line == data)
                        {
                            return true;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    streamReader.Close();
                    cryptoStream.Close();
                    fileStream.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return false;
        }

        public bool Register(string login, string password)
        {
            FileStream fileStream = null;
            CryptoStream cryptoStream = null;
            StreamReader streamReader = null;
            string dbData;
            string data = $"{login};{password}|";
            try
            {
                fileStream = new FileStream("TIPDB.bin", FileMode.OpenOrCreate);
                cryptoStream = new CryptoStream(fileStream, aes.CreateDecryptor(aes.Key, aes.IV), CryptoStreamMode.Read);
                streamReader = new StreamReader(cryptoStream);
                dbData = streamReader.ReadToEnd();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return false;
            }
            finally
            {
                streamReader.Close();
                cryptoStream.Close();
                fileStream.Close();
            }
            dbData = dbData + data;

            try
            {
                fileStream = new FileStream("TIPDB.bin", FileMode.OpenOrCreate);
                cryptoStream = new CryptoStream(fileStream, aes.CreateEncryptor(aes.Key, aes.IV), CryptoStreamMode.Write);

                byte[] binaryData = new byte[dbData.Length];

                for (int i = 0; i < dbData.Length; i++)
                {
                    binaryData[i] = (byte)dbData[i];
                }

                cryptoStream.Write(binaryData, 0, binaryData.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return false;
            }
            finally
            {
                cryptoStream.Close();
                fileStream.Close();
            }
            return true;
        }
    }
}
