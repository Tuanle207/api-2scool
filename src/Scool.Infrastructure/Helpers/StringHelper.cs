using System;

namespace Scool.Infrastructure.Helpers
{
    public static class StringHelper
    {
        public static string GetRandomPasswordString(int length = 8)
        {
            //message: 'Mật khẩu phải gồm 6-20 kí tự, chỉ có thể gồm 0-9, a-z, A-Z, _'

            string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_";
            Random random = new();

            // Select one random character at a time from the string  
            // and create an array of chars  
            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = validChars[random.Next(0, validChars.Length)];
            }
            return $"P{new string(chars)}";
        }
    }
}
