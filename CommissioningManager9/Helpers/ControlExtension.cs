using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
//using System.Windows.Forms;

namespace Helpers
{
    public static class ControlExtension
    {
        public static IEnumerable<string> GetRandomString(this List<string> numberOfRandoms, int length)
        {
            var allowedChars = "ABCDEFGHJKLMNOPQRSTUVWXYZ";

            var unique = new HashSet<string>();
            var rd = new Random();

            var chars = new char[length];

            for (int i = 0; i < numberOfRandoms.Count + 10; i++)
            {
                for (var j = 0; j < length; j++)
                {
                    chars[j] = allowedChars[rd.Next(0, allowedChars.Length)];                    
                }
                if (!unique.Contains(new String(chars)))
                    unique.Add(new String(chars));
            }
            
            return unique.ToList();
        }

        
    }
}
