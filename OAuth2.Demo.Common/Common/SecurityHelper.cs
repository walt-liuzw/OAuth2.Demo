using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OAuth2.Demo.Common.Common
{
    public static class SecurityHelper
    {
        public static string GetMd5(string str)
        {
            //创建MD5哈稀算法的默认实现的实例
            MD5 md5 = MD5.Create();
            //将指定字符串的所有字符编码为一个字节序列
            byte[] buffer = Encoding.Default.GetBytes(str);
            //计算指定字节数组的哈稀值
            byte[] bufferMd5 = md5.ComputeHash(buffer);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bufferMd5.Length; i++)
            {
                //x:表示将十进制转换成十六进制
                sb.Append(bufferMd5[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
