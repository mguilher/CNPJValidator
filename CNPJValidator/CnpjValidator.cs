using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CNPJValidator
{
    public static class CnpjValidator
    {
        public static bool IsValidCNPJ(string document)
        {
            if (string.IsNullOrWhiteSpace(document))
            {
                return false;
            }

            var cleanerRegex = new Regex(@"[^A-Za-z0-9]");

            var cnpjDocument = cleanerRegex.Replace(document, "").ToUpper();

            if (cnpjDocument.Length != 14)
            {
                return false;
            }

            // Reject known invalid CNPJs (all characters the same)
            var validationRepeated = new Regex(@"^(.)\1{13}$");
            bool hasRepeatedCharacters = validationRepeated.IsMatch(cnpjDocument);
            if (hasRepeatedCharacters)
            {
                return false;
            }

            Span<int> numbers = stackalloc int[14];
            for (int i = 0; i < 14; i++)
            {
                numbers[i] = cnpjDocument[i] - 48;
            }

            // Validate first check digit
            int[] multiplierDigit1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int sum = 0;
            for (int i = 0; i < 12; i++)
            {
                sum += numbers[i] * multiplierDigit1[i];
            }

            int remainder = sum % 11;
            int digit1 = remainder < 2 ? 0 : 11 - remainder;

            if (int.Parse(cnpjDocument[12].ToString()) != digit1)
            {
                return false;
            }

            // Validate second check digit
            int[] multiplierDigit2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            sum = 0;
            for (int i = 0; i < 13; i++)
            {
                sum += numbers[i] * multiplierDigit2[i];
            }

            remainder = sum % 11;
            int digit2 = remainder < 2 ? 0 : 11 - remainder;

            return int.Parse(cnpjDocument[13].ToString()) == digit2;
        }
    }
}
