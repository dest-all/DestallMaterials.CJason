using System;
using System.Collections.Generic;
using System.Linq;

namespace CJason
{
    public static class StringExtensions
    {
        public static string MustNotEndWith(this string inputString, string unwantedEnding)
        {
            if (!inputString.EndsWith(unwantedEnding))
            {
                return inputString;
            }

            return MustNotEndWith(
                new string(inputString.Take(inputString.Length - unwantedEnding.Length).ToArray()),
                unwantedEnding
            );
        }

        public static string MustNotStartWith(this string inputString, string unwantedBeginning)
        {
            if (!inputString.StartsWith(unwantedBeginning))
            {
                return inputString;
            }

            return MustNotStartWith(
                new string(inputString.Skip(unwantedBeginning.Length).ToArray()),
                unwantedBeginning
            );
        }

        public static string MustEndWith(this string input, string desiredEnding)
        {
            if (input.EndsWith(desiredEnding))
            {
                return input;
            }

            return input + desiredEnding;
        }

        public static string MustEndWith(this string input, char desiredEnding)
        {
            if (input.LastOrDefault() == desiredEnding)
            {
                return input;
            }

            return input + desiredEnding;
        }

        public static string MustStartWith(this string input, string desiredBeginning)
        {
            if (input.StartsWith(desiredBeginning))
            {
                return input;
            }

            return desiredBeginning + input;
        }

        public static string Capitalize(this string str)
        {
            var firstLetter = str.First();
            return firstLetter.ToString().ToUpper() + String.Concat(str.Skip(1));
        }

        public static string LowerFirstLetter(this string str)
        {
            var firstLetter = str.First();
            return firstLetter.ToString().ToLower() + String.Concat(str.Skip(1));
        }

        public static bool IsEmpty(this string str) => string.IsNullOrEmpty(str);

        public static bool HasContent(this string str) => !IsEmpty(str);

        public static string Join(this IEnumerable<string> strings, string separator = ", ")
        {
            return string.Join(separator, strings);
        }

        public static bool ToBool(this string str)
        {
            return !string.IsNullOrEmpty(str);
        }

        public static string ToPluralForm(this string initialString)
        {
            return initialString.EndsWith("s") || initialString.EndsWith("c")
                ? initialString + "es"
                : initialString.EndsWith("y")
                    ? initialString.Substring(0, initialString.Length - 1) + "ies"
                    : initialString + "s";
        }

        public static uint CalculateSymbolsInFront(this string str, char symbol)
        {
            uint result = 0;
            var enumerator = str.GetEnumerator();
            while (enumerator.MoveNext() && enumerator.Current == symbol)
            {
                result++;
            }
            return result;
        }

        public static string ReplaceSeveralOccurencies(
            this string inputString,
            string replacedString,
            string replaceWith,
            uint number
        )
        {
            var splitted = inputString.Split(replacedString.ToCharArray());
            return splitted.Take((int)number).Join(replaceWith)
                + splitted.Skip((int)number).Join(replacedString);
        }

        public static string ExtractFromBetween(
            this string str,
            char firstSymbol,
            char secondSymbol
        )
        {
            var strSpan = str;
            int beginning = 0;
            int length = str.Length - 1;
            int end = length;
            for (int i = 0; i < strSpan.Length; i++)
            {
                var symbol = strSpan[i];

                if (beginning == 0)
                {
                    if (symbol == firstSymbol && beginning < length - 1)
                    {
                        beginning = i + 1;
                    }
                }
                else if (symbol == secondSymbol)
                {
                    end = i;
                    break;
                }
            }
            return strSpan.Substring(beginning, end - beginning + 1);
        }
    }
}
