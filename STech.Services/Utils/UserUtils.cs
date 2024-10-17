using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace STech.Services.Utils
{
    public static class UserUtils
    {
        public static string GenerateRandomId(int length)
        {
            string pattern = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                sb.Append(pattern[new Random().Next(0, pattern.Length)]);
            }

            return sb.ToString();
        }

        public static string GenerateRandomString(int length)
        {
            string pattern = "bQ5sWak49EGPJTdw8LqBgcAFMK6fYjZznVemDhrpUHR27vS3";
            StringBuilder sb = new StringBuilder();

            for(int i = 0; i < length; i++)
            {
                sb.Append(pattern[new Random().Next(0, pattern.Length)]);
            }

            return sb.ToString();
        }

        public static string HashPasswordMD5(this string password, string key)
        {
            using(MD5 md5 = MD5.Create())
            {
                byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(string.Concat(password, key)));
                StringBuilder sb = new StringBuilder();
                
                for(int i = 0;i < data.Length; i++)
                {
                    sb.Append(data[i].ToString("x2"));
                }
                
                return sb.ToString();
            }
        }
    }
}
