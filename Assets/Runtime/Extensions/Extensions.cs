using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace XVNML2U 
{
    internal static class Extensions
    {
        internal static int ToInt(this object target, int valueDefault = 0)
        {
            int value = valueDefault;
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

        internal static float ToFloat(this object target, float valueDefault = 0f)
        {
            float value = valueDefault;
            try
            {
                value = Convert.ToSingle(target);
            }
            catch
            {
                return value;
            }

            return value;
        }

        internal static bool ToBool(this object target, bool valueDefault = false)
        {
            bool value = valueDefault;
            try
            {
                value = Convert.ToBoolean(target);
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

        internal static void DoForEvery<T,K>(this IEnumerable<T> objects, Func<T, K> method)
        {
            foreach(var obj in objects)
            {
                method(obj);
            }
        }

        internal static void DoForEvery<T>(this T[] objects, Action<T> method)
        {
            var array = objects.AsEnumerable();
            array.DoForEvery(o =>
            {
                method(o);
                return (T)default;
            });
        }
    }
}
