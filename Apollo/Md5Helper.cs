using System;
using System.Security.Cryptography;
using System.Text;

namespace Apollo{
    public class Md5Helper{
        public static string StrToMD5(string str)
        {
            byte[] data = Encoding.Default.GetBytes(str);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] OutBytes = md5.ComputeHash(data);

            string OutString = "";
            for (int i = 0; i < OutBytes.Length; i++)
            {
                OutString += OutBytes[i].ToString("x2");
            }
           // return OutString.ToUpper();
            return OutString;
        }
    }
}