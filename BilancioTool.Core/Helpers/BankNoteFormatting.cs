using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace BilancioTool.Core.Helpers
{
    public static class BankNoteFormatting
    {
        public static string FormatBankDescription(string input)
        {
            string result = input;
            if (!String.IsNullOrEmpty(result))
            {
                // Rimuovi i simboli
                result = result.Replace("*", " ").Replace("?", " ").Replace(":", " ")
                    .Replace(";", " ").Replace("-", " ").Replace("/", " ");
                // Toglie gli spazi dall'inizio alla fine
                result = result.Trim();
                // Sostituisci due o più spazi con ';'
                result = Regex.Replace(result, @"\s{2,}", " ");
            }
            return result;
        }
    }
}
