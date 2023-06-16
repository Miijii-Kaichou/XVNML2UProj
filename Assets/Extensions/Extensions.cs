using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XVNML2U.Assets.Extensions
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
    }
}
