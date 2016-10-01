using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Base64Hepler
    {
        public static string EncodeTo64UTF8(string value)
        {
            try
            {
                byte[] toEncodeAsBytes =
                    Encoding.UTF8.GetBytes(value);
                var returnValue =
                    Convert.ToBase64String(toEncodeAsBytes);
                return returnValue;
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }

        public static string DecodeFrom64(string value)
        {
            try
            {
                byte[] encodedDataAsBytes =
                    Convert.FromBase64String(value);
                var returnValue =
                    Encoding.UTF8.GetString(encodedDataAsBytes);
                return returnValue;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
