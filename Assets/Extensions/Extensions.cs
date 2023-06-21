using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XVNML2U 
{
    internal static class Extensions
    {
        internal static int ToInt(this object target)
        {
            int value = -1;
            try
            {
                value = Convert.ToInt32(target);
            }
            catch
            {
                return value;
            }

            return value;
        }

        internal static E Parse<E>(this string target)
        {
            return (E)Enum.Parse(typeof(E), target);
        }

        internal static string TakeOff(this string target, string matching)
        {
            var word = target;

            int startPoint, endPoint = 0;

            for(int i = 0; i < word.Length; i++)
            {
                var character = target[i];
                if (character == matching[0])
                {
                    startPoint = i;
                    endPoint = i;
                    while(endPoint < matching.Length)
                    {
                        endPoint++;
                    }

                    var result = word.Substring(startPoint, endPoint);

                    if (matching.Equals(result))
                    {
                        return word.Remove(startPoint, endPoint);
                    }
                }
            }

            return string.Empty;
        }
    }
}
