using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreExtend
{
    internal static class Checker
    {
        public static void CheckNull(this object obj, string paramName)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        public static void CheckNull(this object obj, string paramName, string msg)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(paramName, msg);
            }
        }

        public static void CheckStringIsNullOrEmpty(this string val, string paramName)
        {
            if (string.IsNullOrEmpty(val))
            {
                throw new ArgumentNullException(paramName);
            }
        }

        public static void CheckStringIsNullOrWhiteSpace(this string val, string paramName)
        {
            if (string.IsNullOrWhiteSpace(val))
            {
                throw new ArgumentNullException(paramName);
            }
        }

        public static void CheckStringIsNullOrEmpty(this string val, string msg, string paramName)
        {
            if (string.IsNullOrEmpty(val))
            {
                throw new ArgumentException(msg, paramName);
            }
        }

        public static void CheckStringIsNullOrWhiteSpace(this string val, string msg, string paramName)
        {
            if (string.IsNullOrWhiteSpace(val))
            {
                throw new ArgumentException(msg, paramName);
            }
        }

        public static void CheckPairValueIsNull<TK, TV>(this IEnumerable<KeyValuePair<TK, TV>> pairs, string paramName)
        {
            pairs.CheckNull(paramName);

            var pair = pairs.Where(l => l.Value == null).FirstOrDefault();
            if (pair.Key != null)
            {
                throw new ArgumentException($"The value of the key [{pair.Key}] can not be null.", paramName);
            }
        }

    }
}
