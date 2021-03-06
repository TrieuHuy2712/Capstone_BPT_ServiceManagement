using System;
using System.Linq;

namespace BPT_Service.Common.Support
{
    public class RandomSupport
    {
        private readonly Random random = new Random();

        public  string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}