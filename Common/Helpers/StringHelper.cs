using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
namespace Common
{
    public static class StringHelper
    {//Hàm chuyển chuỗi ký tự thông thường sang chuỗi ký tự được mã hóa dạng MD5
        public static string StringToMd5(string value)
        {
            var md5 = string.Empty;
            var md5Hasher = new MD5CryptoServiceProvider();
            var encoder = new System.Text.UTF8Encoding();
            byte[] hashedBytes = md5Hasher.ComputeHash(encoder.GetBytes(value));
            return hashedBytes.Aggregate(md5, (current, b) => current + b.ToString("X2"));
        }
        //Hàm xóa các ký tự đặc biệt nhằm chống tấn công SQL injection
        public static string KillChars(string strInput)
        {
            string result = "";
            if (!String.IsNullOrEmpty(strInput))
            {
                string[] arrBadChars = new string[] { "select", "drop", "--", "insert", "delete", "xp_", "sysobjects", "syscolumns", "'", "1=1", "truncate", };//Loại bỏ "or, table" để tránh lỗi không nhập được những từ có chứa "or, table"
                result = strInput.Trim().Replace("'", "''");
                result = strInput.Replace("%20", " ");
                //result = result.ToLower();
                for (int i = 0; i < arrBadChars.Length; i++)
                {
                    string strBadChar = arrBadChars[i].ToString();
                    result = result.Replace(strBadChar, "");
                }
            }
            return result;
        }
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng)
        {
            T[] elements = source.ToArray();
            for (int i = elements.Length - 1; i > 0; i--)
            {
                int swapIndex = rng.Next(i + 1);
                T tmp = elements[i];
                elements[i] = elements[swapIndex];
                elements[swapIndex] = tmp;
            }
            foreach (T element in elements)
            {
                yield return element;
            }
        }
        //Tổng hợp chi tiết các ký tự có dấu và không dấu tiếng Việt
        private static readonly string[] VietNamChar = new string[] 
        { 
            "aAeEoOuUiIdDyY", 
            "áàạảãâấầậẩẫăắằặẳẵ", 
            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ", 
            "éèẹẻẽêếềệểễ", 
            "ÉÈẸẺẼÊẾỀỆỂỄ", 
            "óòọỏõôốồộổỗơớờợởỡ", 
            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ", 
            "úùụủũưứừựửữ", 
            "ÚÙỤỦŨƯỨỪỰỬỮ", 
            "íìịỉĩ", 
            "ÍÌỊỈĨ", 
            "đ", 
            "Đ", 
            "ýỳỵỷỹ", 
            "ÝỲỴỶỸ" 
        };
        //Lọc các ký tự có dấu tiếng Việt về dạng không dấu
        public static string StringFilter(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                str = str.Trim().Replace("\"", "");
                str = str.Trim().Replace(".", "");
                str = str.Trim().Replace(",", "");
                str = str.Trim().Replace(";", "");
                str = str.Trim().Replace(" - ", " ");
                str = str.Trim().Replace("/", "-");
                str = Regex.Replace(str, " ", "-");
                str = Regex.Replace(str, "--", "-");
                for (int i = 1; i < VietNamChar.Length; i++)
                {
                    for (int j = 0; j < VietNamChar[i].Length; j++)
                        str = str.Replace(VietNamChar[i][j], VietNamChar[0][i - 1]);
                }
                return str.ToLower();
            }
            else
            {
                return string.Empty;
            }
        }
        public static string CreateRandomString(int len)
        {
            string _allowedChars = "abcdefghijk0123456789mnopqrstuvwxyz";
            Random randNum = new Random();
            char[] chars = new char[len];
            int allowedCharCount = _allowedChars.Length;
            for (int i = 0; i < len; i++)
            {
                chars[i] = _allowedChars[(int)((_allowedChars.Length) * randNum.NextDouble())];
            }
            return new string(chars);
        }
    }
}
