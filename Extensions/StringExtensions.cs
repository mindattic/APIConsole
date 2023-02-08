using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIConsole.Extensions
{
    /// <summary>
    /// Helper methods for the lists.
    /// </summary>
    public static class StringExtensions
    {
        public static string Repeat(this string source, int amount)
        {
            return $"{string.Concat(Enumerable.Repeat(source, amount))}";
        }
    }
}
