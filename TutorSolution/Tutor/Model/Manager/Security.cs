using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Tutor.Model.Manager
{
    public class Security
    {
        public static string MD5Hash(string plaintext)
        {
            var md5 = MD5.Create();
            var result = md5.ComputeHash(Encoding.UTF8.GetBytes(plaintext));
            string hash = BitConverter.ToString(result).Replace("-", String.Empty);
            return hash.ToLower();
        }
    }
}
