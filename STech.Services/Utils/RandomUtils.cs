using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STech.Services.Utils
{
    public static class RandomUtils
    {
        public static int RandomNumbers(int min, int max)
        {
            Random random = new Random();

            return random.Next(min, max);
        }
    }
}
