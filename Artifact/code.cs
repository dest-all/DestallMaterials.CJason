
using JsonPiece = System.ReadOnlySpan<char>;
using CJason.Provision;
using System;
using System.Collections.Generic;
using System.Linq;


namespace SampleNamespace
{
    public static class ArtificialStringifier
    {

        public static int CalculateJsonLength(this Span<System.Boolean> consts) => (consts.Length + 1) * 5 + 2;
        public static int CalculateJsonLength(this IEnumerable<KeyValuePair<string, Boolean>> items)
        {
            var result = 2;
            foreach (var (key, value) in items)
            {
                result += key.Length * 2 + 2 + 1;
                result += 5;
            }
            return result == 3 ? 2 : result;
        }

        public static Span<char> FillWith(this Span<char> span, Span<Boolean> items)
        {
            span[0] = '[';
            span = span[1..];
            var length = items.Length;
            for (int i = 0; i < length; i++)
            {
                if (i > 0)
                {
                    span[0] = ',';
                    span = span[1..];
                }
                var itemString = items[i].ToString().AsSpan();
                span = span[itemString.CopiedTo(span)..];
            }
            span[0] = ']';
            return span[1..];
        }

        public static Span<char> FillWith(this Span<char> json, IEnumerable<KeyValuePair<string, Boolean>> items)
        {
            json[0] = '{';
            json = json[1..];
            bool isFirst = true;
            foreach (var (key, value) in items)
            {
                if (!isFirst)
                {
                    json[0] = ',';
                    json = json[1..];
                }
                else
                {
                    isFirst = false;
                }
                json[0] = '"';
                json = json[1..];
                ReadOnlySpan<char> keySpan = key;
                keySpan.CopyTo(json);
                json = json[keySpan.Length..];
                json[0] = '"';
                json[1] = ':';
                json = json[2..];

                ReadOnlySpan<char> valueSpan = value.ToString();
                valueSpan.CopyTo(json);
                json = json[valueSpan.Length..];
            }

            json[0] = '}';
            json = json[1..];
            return json;
        }


        public static int CalculateJsonLength(this Span<System.Char> consts) => (consts.Length + 1) * 3 + 2;
        public static int CalculateJsonLength(this IEnumerable<KeyValuePair<string, Char>> items)
        {
            var result = 2;
            foreach (var (key, value) in items)
            {
                result += key.Length * 2 + 2 + 1;
                result += 3;
            }
            return result == 3 ? 2 : result;
        }

        public static Span<char> FillWith(this Span<char> span, Span<Char> items)
        {
            span[0] = '[';
            span = span[1..];
            var length = items.Length;
            for (int i = 0; i < length; i++)
            {
                if (i > 0)
                {
                    span[0] = ',';
                    span = span[1..];
                }
                var itemString = items[i].ToString().AsSpan();
                span = span[itemString.CopiedTo(span)..];
            }
            span[0] = ']';
            return span[1..];
        }

        public static Span<char> FillWith(this Span<char> json, IEnumerable<KeyValuePair<string, Char>> items)
        {
            json[0] = '{';
            json = json[1..];
            bool isFirst = true;
            foreach (var (key, value) in items)
            {
                if (!isFirst)
                {
                    json[0] = ',';
                    json = json[1..];
                }
                else
                {
                    isFirst = false;
                }
                json[0] = '"';
                json = json[1..];
                ReadOnlySpan<char> keySpan = key;
                keySpan.CopyTo(json);
                json = json[keySpan.Length..];
                json[0] = '"';
                json[1] = ':';
                json = json[2..];

                ReadOnlySpan<char> valueSpan = value.ToString();
                valueSpan.CopyTo(json);
                json = json[valueSpan.Length..];
            }

            json[0] = '}';
            json = json[1..];
            return json;
        }


        public static int CalculateJsonLength(this Span<System.SByte> consts) => (consts.Length + 1) * 4 + 2;
        public static int CalculateJsonLength(this IEnumerable<KeyValuePair<string, SByte>> items)
        {
            var result = 2;
            foreach (var (key, value) in items)
            {
                result += key.Length * 2 + 2 + 1;
                result += 4;
            }
            return result == 3 ? 2 : result;
        }

        public static Span<char> FillWith(this Span<char> span, Span<SByte> items)
        {
            span[0] = '[';
            span = span[1..];
            var length = items.Length;
            for (int i = 0; i < length; i++)
            {
                if (i > 0)
                {
                    span[0] = ',';
                    span = span[1..];
                }
                var itemString = items[i].ToString().AsSpan();
                span = span[itemString.CopiedTo(span)..];
            }
            span[0] = ']';
            return span[1..];
        }

        public static Span<char> FillWith(this Span<char> json, IEnumerable<KeyValuePair<string, SByte>> items)
        {
            json[0] = '{';
            json = json[1..];
            bool isFirst = true;
            foreach (var (key, value) in items)
            {
                if (!isFirst)
                {
                    json[0] = ',';
                    json = json[1..];
                }
                else
                {
                    isFirst = false;
                }
                json[0] = '"';
                json = json[1..];
                ReadOnlySpan<char> keySpan = key;
                keySpan.CopyTo(json);
                json = json[keySpan.Length..];
                json[0] = '"';
                json[1] = ':';
                json = json[2..];

                ReadOnlySpan<char> valueSpan = value.ToString();
                valueSpan.CopyTo(json);
                json = json[valueSpan.Length..];
            }

            json[0] = '}';
            json = json[1..];
            return json;
        }


        public static int CalculateJsonLength(this Span<System.Byte> consts) => (consts.Length + 1) * 3 + 2;
        public static int CalculateJsonLength(this IEnumerable<KeyValuePair<string, Byte>> items)
        {
            var result = 2;
            foreach (var (key, value) in items)
            {
                result += key.Length * 2 + 2 + 1;
                result += 3;
            }
            return result == 3 ? 2 : result;
        }

        public static Span<char> FillWith(this Span<char> span, Span<Byte> items)
        {
            span[0] = '[';
            span = span[1..];
            var length = items.Length;
            for (int i = 0; i < length; i++)
            {
                if (i > 0)
                {
                    span[0] = ',';
                    span = span[1..];
                }
                var itemString = items[i].ToString().AsSpan();
                span = span[itemString.CopiedTo(span)..];
            }
            span[0] = ']';
            return span[1..];
        }

        public static Span<char> FillWith(this Span<char> json, IEnumerable<KeyValuePair<string, Byte>> items)
        {
            json[0] = '{';
            json = json[1..];
            bool isFirst = true;
            foreach (var (key, value) in items)
            {
                if (!isFirst)
                {
                    json[0] = ',';
                    json = json[1..];
                }
                else
                {
                    isFirst = false;
                }
                json[0] = '"';
                json = json[1..];
                ReadOnlySpan<char> keySpan = key;
                keySpan.CopyTo(json);
                json = json[keySpan.Length..];
                json[0] = '"';
                json[1] = ':';
                json = json[2..];

                ReadOnlySpan<char> valueSpan = value.ToString();
                valueSpan.CopyTo(json);
                json = json[valueSpan.Length..];
            }

            json[0] = '}';
            json = json[1..];
            return json;
        }


        public static int CalculateJsonLength(this Span<System.Int16> consts) => (consts.Length + 1) * 6 + 2;
        public static int CalculateJsonLength(this IEnumerable<KeyValuePair<string, Int16>> items)
        {
            var result = 2;
            foreach (var (key, value) in items)
            {
                result += key.Length * 2 + 2 + 1;
                result += 6;
            }
            return result == 3 ? 2 : result;
        }

        public static Span<char> FillWith(this Span<char> span, Span<Int16> items)
        {
            span[0] = '[';
            span = span[1..];
            var length = items.Length;
            for (int i = 0; i < length; i++)
            {
                if (i > 0)
                {
                    span[0] = ',';
                    span = span[1..];
                }
                var itemString = items[i].ToString().AsSpan();
                span = span[itemString.CopiedTo(span)..];
            }
            span[0] = ']';
            return span[1..];
        }

        public static Span<char> FillWith(this Span<char> json, IEnumerable<KeyValuePair<string, Int16>> items)
        {
            json[0] = '{';
            json = json[1..];
            bool isFirst = true;
            foreach (var (key, value) in items)
            {
                if (!isFirst)
                {
                    json[0] = ',';
                    json = json[1..];
                }
                else
                {
                    isFirst = false;
                }
                json[0] = '"';
                json = json[1..];
                ReadOnlySpan<char> keySpan = key;
                keySpan.CopyTo(json);
                json = json[keySpan.Length..];
                json[0] = '"';
                json[1] = ':';
                json = json[2..];

                ReadOnlySpan<char> valueSpan = value.ToString();
                valueSpan.CopyTo(json);
                json = json[valueSpan.Length..];
            }

            json[0] = '}';
            json = json[1..];
            return json;
        }


        public static int CalculateJsonLength(this Span<System.UInt16> consts) => (consts.Length + 1) * 5 + 2;
        public static int CalculateJsonLength(this IEnumerable<KeyValuePair<string, UInt16>> items)
        {
            var result = 2;
            foreach (var (key, value) in items)
            {
                result += key.Length * 2 + 2 + 1;
                result += 5;
            }
            return result == 3 ? 2 : result;
        }

        public static Span<char> FillWith(this Span<char> span, Span<UInt16> items)
        {
            span[0] = '[';
            span = span[1..];
            var length = items.Length;
            for (int i = 0; i < length; i++)
            {
                if (i > 0)
                {
                    span[0] = ',';
                    span = span[1..];
                }
                var itemString = items[i].ToString().AsSpan();
                span = span[itemString.CopiedTo(span)..];
            }
            span[0] = ']';
            return span[1..];
        }

        public static Span<char> FillWith(this Span<char> json, IEnumerable<KeyValuePair<string, UInt16>> items)
        {
            json[0] = '{';
            json = json[1..];
            bool isFirst = true;
            foreach (var (key, value) in items)
            {
                if (!isFirst)
                {
                    json[0] = ',';
                    json = json[1..];
                }
                else
                {
                    isFirst = false;
                }
                json[0] = '"';
                json = json[1..];
                ReadOnlySpan<char> keySpan = key;
                keySpan.CopyTo(json);
                json = json[keySpan.Length..];
                json[0] = '"';
                json[1] = ':';
                json = json[2..];

                ReadOnlySpan<char> valueSpan = value.ToString();
                valueSpan.CopyTo(json);
                json = json[valueSpan.Length..];
            }

            json[0] = '}';
            json = json[1..];
            return json;
        }


        public static int CalculateJsonLength(this Span<System.Int32> consts) => (consts.Length + 1) * 11 + 2;
        public static int CalculateJsonLength(this IEnumerable<KeyValuePair<string, Int32>> items)
        {
            var result = 2;
            foreach (var (key, value) in items)
            {
                result += key.Length * 2 + 2 + 1;
                result += 11;
            }
            return result == 3 ? 2 : result;
        }

        public static Span<char> FillWith(this Span<char> span, Span<Int32> items)
        {
            span[0] = '[';
            span = span[1..];
            var length = items.Length;
            for (int i = 0; i < length; i++)
            {
                if (i > 0)
                {
                    span[0] = ',';
                    span = span[1..];
                }
                var itemString = items[i].ToString().AsSpan();
                span = span[itemString.CopiedTo(span)..];
            }
            span[0] = ']';
            return span[1..];
        }

        public static Span<char> FillWith(this Span<char> json, IEnumerable<KeyValuePair<string, Int32>> items)
        {
            json[0] = '{';
            json = json[1..];
            bool isFirst = true;
            foreach (var (key, value) in items)
            {
                if (!isFirst)
                {
                    json[0] = ',';
                    json = json[1..];
                }
                else
                {
                    isFirst = false;
                }
                json[0] = '"';
                json = json[1..];
                ReadOnlySpan<char> keySpan = key;
                keySpan.CopyTo(json);
                json = json[keySpan.Length..];
                json[0] = '"';
                json[1] = ':';
                json = json[2..];

                ReadOnlySpan<char> valueSpan = value.ToString();
                valueSpan.CopyTo(json);
                json = json[valueSpan.Length..];
            }

            json[0] = '}';
            json = json[1..];
            return json;
        }


        public static int CalculateJsonLength(this Span<System.UInt32> consts) => (consts.Length + 1) * 10 + 2;
        public static int CalculateJsonLength(this IEnumerable<KeyValuePair<string, UInt32>> items)
        {
            var result = 2;
            foreach (var (key, value) in items)
            {
                result += key.Length * 2 + 2 + 1;
                result += 10;
            }
            return result == 3 ? 2 : result;
        }

        public static Span<char> FillWith(this Span<char> span, Span<UInt32> items)
        {
            span[0] = '[';
            span = span[1..];
            var length = items.Length;
            for (int i = 0; i < length; i++)
            {
                if (i > 0)
                {
                    span[0] = ',';
                    span = span[1..];
                }
                var itemString = items[i].ToString().AsSpan();
                span = span[itemString.CopiedTo(span)..];
            }
            span[0] = ']';
            return span[1..];
        }

        public static Span<char> FillWith(this Span<char> json, IEnumerable<KeyValuePair<string, UInt32>> items)
        {
            json[0] = '{';
            json = json[1..];
            bool isFirst = true;
            foreach (var (key, value) in items)
            {
                if (!isFirst)
                {
                    json[0] = ',';
                    json = json[1..];
                }
                else
                {
                    isFirst = false;
                }
                json[0] = '"';
                json = json[1..];
                ReadOnlySpan<char> keySpan = key;
                keySpan.CopyTo(json);
                json = json[keySpan.Length..];
                json[0] = '"';
                json[1] = ':';
                json = json[2..];

                ReadOnlySpan<char> valueSpan = value.ToString();
                valueSpan.CopyTo(json);
                json = json[valueSpan.Length..];
            }

            json[0] = '}';
            json = json[1..];
            return json;
        }


        public static int CalculateJsonLength(this Span<System.Int64> consts) => (consts.Length + 1) * 20 + 2;
        public static int CalculateJsonLength(this IEnumerable<KeyValuePair<string, Int64>> items)
        {
            var result = 2;
            foreach (var (key, value) in items)
            {
                result += key.Length * 2 + 2 + 1;
                result += 20;
            }
            return result == 3 ? 2 : result;
        }

        public static Span<char> FillWith(this Span<char> span, Span<Int64> items)
        {
            span[0] = '[';
            span = span[1..];
            var length = items.Length;
            for (int i = 0; i < length; i++)
            {
                if (i > 0)
                {
                    span[0] = ',';
                    span = span[1..];
                }
                var itemString = items[i].ToString().AsSpan();
                span = span[itemString.CopiedTo(span)..];
            }
            span[0] = ']';
            return span[1..];
        }

        public static Span<char> FillWith(this Span<char> json, IEnumerable<KeyValuePair<string, Int64>> items)
        {
            json[0] = '{';
            json = json[1..];
            bool isFirst = true;
            foreach (var (key, value) in items)
            {
                if (!isFirst)
                {
                    json[0] = ',';
                    json = json[1..];
                }
                else
                {
                    isFirst = false;
                }
                json[0] = '"';
                json = json[1..];
                ReadOnlySpan<char> keySpan = key;
                keySpan.CopyTo(json);
                json = json[keySpan.Length..];
                json[0] = '"';
                json[1] = ':';
                json = json[2..];

                ReadOnlySpan<char> valueSpan = value.ToString();
                valueSpan.CopyTo(json);
                json = json[valueSpan.Length..];
            }

            json[0] = '}';
            json = json[1..];
            return json;
        }


        public static int CalculateJsonLength(this Span<System.UInt64> consts) => (consts.Length + 1) * 20 + 2;
        public static int CalculateJsonLength(this IEnumerable<KeyValuePair<string, UInt64>> items)
        {
            var result = 2;
            foreach (var (key, value) in items)
            {
                result += key.Length * 2 + 2 + 1;
                result += 20;
            }
            return result == 3 ? 2 : result;
        }

        public static Span<char> FillWith(this Span<char> span, Span<UInt64> items)
        {
            span[0] = '[';
            span = span[1..];
            var length = items.Length;
            for (int i = 0; i < length; i++)
            {
                if (i > 0)
                {
                    span[0] = ',';
                    span = span[1..];
                }
                var itemString = items[i].ToString().AsSpan();
                span = span[itemString.CopiedTo(span)..];
            }
            span[0] = ']';
            return span[1..];
        }

        public static Span<char> FillWith(this Span<char> json, IEnumerable<KeyValuePair<string, UInt64>> items)
        {
            json[0] = '{';
            json = json[1..];
            bool isFirst = true;
            foreach (var (key, value) in items)
            {
                if (!isFirst)
                {
                    json[0] = ',';
                    json = json[1..];
                }
                else
                {
                    isFirst = false;
                }
                json[0] = '"';
                json = json[1..];
                ReadOnlySpan<char> keySpan = key;
                keySpan.CopyTo(json);
                json = json[keySpan.Length..];
                json[0] = '"';
                json[1] = ':';
                json = json[2..];

                ReadOnlySpan<char> valueSpan = value.ToString();
                valueSpan.CopyTo(json);
                json = json[valueSpan.Length..];
            }

            json[0] = '}';
            json = json[1..];
            return json;
        }


        public static int CalculateJsonLength(this Span<System.Single> consts) => (consts.Length + 1) * 9 + 2;
        public static int CalculateJsonLength(this IEnumerable<KeyValuePair<string, Single>> items)
        {
            var result = 2;
            foreach (var (key, value) in items)
            {
                result += key.Length * 2 + 2 + 1;
                result += 9;
            }
            return result == 3 ? 2 : result;
        }

        public static Span<char> FillWith(this Span<char> span, Span<Single> items)
        {
            span[0] = '[';
            span = span[1..];
            var length = items.Length;
            for (int i = 0; i < length; i++)
            {
                if (i > 0)
                {
                    span[0] = ',';
                    span = span[1..];
                }
                var itemString = items[i].ToString().AsSpan();
                span = span[itemString.CopiedTo(span)..];
            }
            span[0] = ']';
            return span[1..];
        }

        public static Span<char> FillWith(this Span<char> json, IEnumerable<KeyValuePair<string, Single>> items)
        {
            json[0] = '{';
            json = json[1..];
            bool isFirst = true;
            foreach (var (key, value) in items)
            {
                if (!isFirst)
                {
                    json[0] = ',';
                    json = json[1..];
                }
                else
                {
                    isFirst = false;
                }
                json[0] = '"';
                json = json[1..];
                ReadOnlySpan<char> keySpan = key;
                keySpan.CopyTo(json);
                json = json[keySpan.Length..];
                json[0] = '"';
                json[1] = ':';
                json = json[2..];

                ReadOnlySpan<char> valueSpan = value.ToString();
                valueSpan.CopyTo(json);
                json = json[valueSpan.Length..];
            }

            json[0] = '}';
            json = json[1..];
            return json;
        }


        public static int CalculateJsonLength(this Span<System.Double> consts) => (consts.Length + 1) * 17 + 2;
        public static int CalculateJsonLength(this IEnumerable<KeyValuePair<string, Double>> items)
        {
            var result = 2;
            foreach (var (key, value) in items)
            {
                result += key.Length * 2 + 2 + 1;
                result += 17;
            }
            return result == 3 ? 2 : result;
        }

        public static Span<char> FillWith(this Span<char> span, Span<Double> items)
        {
            span[0] = '[';
            span = span[1..];
            var length = items.Length;
            for (int i = 0; i < length; i++)
            {
                if (i > 0)
                {
                    span[0] = ',';
                    span = span[1..];
                }
                var itemString = items[i].ToString().AsSpan();
                span = span[itemString.CopiedTo(span)..];
            }
            span[0] = ']';
            return span[1..];
        }

        public static Span<char> FillWith(this Span<char> json, IEnumerable<KeyValuePair<string, Double>> items)
        {
            json[0] = '{';
            json = json[1..];
            bool isFirst = true;
            foreach (var (key, value) in items)
            {
                if (!isFirst)
                {
                    json[0] = ',';
                    json = json[1..];
                }
                else
                {
                    isFirst = false;
                }
                json[0] = '"';
                json = json[1..];
                ReadOnlySpan<char> keySpan = key;
                keySpan.CopyTo(json);
                json = json[keySpan.Length..];
                json[0] = '"';
                json[1] = ':';
                json = json[2..];

                ReadOnlySpan<char> valueSpan = value.ToString();
                valueSpan.CopyTo(json);
                json = json[valueSpan.Length..];
            }

            json[0] = '}';
            json = json[1..];
            return json;
        }


        public static int CalculateJsonLength(this Span<System.Decimal> consts) => (consts.Length + 1) * 29 + 2;
        public static int CalculateJsonLength(this IEnumerable<KeyValuePair<string, Decimal>> items)
        {
            var result = 2;
            foreach (var (key, value) in items)
            {
                result += key.Length * 2 + 2 + 1;
                result += 29;
            }
            return result == 3 ? 2 : result;
        }

        public static Span<char> FillWith(this Span<char> span, Span<Decimal> items)
        {
            span[0] = '[';
            span = span[1..];
            var length = items.Length;
            for (int i = 0; i < length; i++)
            {
                if (i > 0)
                {
                    span[0] = ',';
                    span = span[1..];
                }
                var itemString = items[i].ToString().AsSpan();
                span = span[itemString.CopiedTo(span)..];
            }
            span[0] = ']';
            return span[1..];
        }

        public static Span<char> FillWith(this Span<char> json, IEnumerable<KeyValuePair<string, Decimal>> items)
        {
            json[0] = '{';
            json = json[1..];
            bool isFirst = true;
            foreach (var (key, value) in items)
            {
                if (!isFirst)
                {
                    json[0] = ',';
                    json = json[1..];
                }
                else
                {
                    isFirst = false;
                }
                json[0] = '"';
                json = json[1..];
                ReadOnlySpan<char> keySpan = key;
                keySpan.CopyTo(json);
                json = json[keySpan.Length..];
                json[0] = '"';
                json[1] = ':';
                json = json[2..];

                ReadOnlySpan<char> valueSpan = value.ToString();
                valueSpan.CopyTo(json);
                json = json[valueSpan.Length..];
            }

            json[0] = '}';
            json = json[1..];
            return json;
        }


        public static int CalculateJsonLength(this Span<System.DateTime> consts) => (consts.Length + 1) * 30 + 2;
        public static int CalculateJsonLength(this IEnumerable<KeyValuePair<string, DateTime>> items)
        {
            var result = 2;
            foreach (var (key, value) in items)
            {
                result += key.Length * 2 + 2 + 1;
                result += 30;
            }
            return result == 3 ? 2 : result;
        }

        public static Span<char> FillWith(this Span<char> span, Span<DateTime> items)
        {
            span[0] = '[';
            span = span[1..];
            var length = items.Length;
            for (int i = 0; i < length; i++)
            {
                if (i > 0)
                {
                    span[0] = ',';
                    span = span[1..];
                }
                var itemString = items[i].ToString().AsSpan();
                span = span[itemString.CopiedTo(span)..];
            }
            span[0] = ']';
            return span[1..];
        }

        public static Span<char> FillWith(this Span<char> json, IEnumerable<KeyValuePair<string, DateTime>> items)
        {
            json[0] = '{';
            json = json[1..];
            bool isFirst = true;
            foreach (var (key, value) in items)
            {
                if (!isFirst)
                {
                    json[0] = ',';
                    json = json[1..];
                }
                else
                {
                    isFirst = false;
                }
                json[0] = '"';
                json = json[1..];
                ReadOnlySpan<char> keySpan = key;
                keySpan.CopyTo(json);
                json = json[keySpan.Length..];
                json[0] = '"';
                json[1] = ':';
                json = json[2..];

                ReadOnlySpan<char> valueSpan = value.ToString();
                valueSpan.CopyTo(json);
                json = json[valueSpan.Length..];
            }

            json[0] = '}';
            json = json[1..];
            return json;
        }


        public static int CalculateJsonLength(this Span<System.TimeSpan> consts) => (consts.Length + 1) * 15 + 2;
        public static int CalculateJsonLength(this IEnumerable<KeyValuePair<string, TimeSpan>> items)
        {
            var result = 2;
            foreach (var (key, value) in items)
            {
                result += key.Length * 2 + 2 + 1;
                result += 15;
            }
            return result == 3 ? 2 : result;
        }

        public static Span<char> FillWith(this Span<char> span, Span<TimeSpan> items)
        {
            span[0] = '[';
            span = span[1..];
            var length = items.Length;
            for (int i = 0; i < length; i++)
            {
                if (i > 0)
                {
                    span[0] = ',';
                    span = span[1..];
                }
                var itemString = items[i].ToString().AsSpan();
                span = span[itemString.CopiedTo(span)..];
            }
            span[0] = ']';
            return span[1..];
        }

        public static Span<char> FillWith(this Span<char> json, IEnumerable<KeyValuePair<string, TimeSpan>> items)
        {
            json[0] = '{';
            json = json[1..];
            bool isFirst = true;
            foreach (var (key, value) in items)
            {
                if (!isFirst)
                {
                    json[0] = ',';
                    json = json[1..];
                }
                else
                {
                    isFirst = false;
                }
                json[0] = '"';
                json = json[1..];
                ReadOnlySpan<char> keySpan = key;
                keySpan.CopyTo(json);
                json = json[keySpan.Length..];
                json[0] = '"';
                json[1] = ':';
                json = json[2..];

                ReadOnlySpan<char> valueSpan = value.ToString();
                valueSpan.CopyTo(json);
                json = json[valueSpan.Length..];
            }

            json[0] = '}';
            json = json[1..];
            return json;
        }


        public static int CalculateJsonLength(this Span<System.DateTimeOffset> consts) => (consts.Length + 1) * 36 + 2;
        public static int CalculateJsonLength(this IEnumerable<KeyValuePair<string, DateTimeOffset>> items)
        {
            var result = 2;
            foreach (var (key, value) in items)
            {
                result += key.Length * 2 + 2 + 1;
                result += 36;
            }
            return result == 3 ? 2 : result;
        }

        public static Span<char> FillWith(this Span<char> span, Span<DateTimeOffset> items)
        {
            span[0] = '[';
            span = span[1..];
            var length = items.Length;
            for (int i = 0; i < length; i++)
            {
                if (i > 0)
                {
                    span[0] = ',';
                    span = span[1..];
                }
                var itemString = items[i].ToString().AsSpan();
                span = span[itemString.CopiedTo(span)..];
            }
            span[0] = ']';
            return span[1..];
        }

        public static Span<char> FillWith(this Span<char> json, IEnumerable<KeyValuePair<string, DateTimeOffset>> items)
        {
            json[0] = '{';
            json = json[1..];
            bool isFirst = true;
            foreach (var (key, value) in items)
            {
                if (!isFirst)
                {
                    json[0] = ',';
                    json = json[1..];
                }
                else
                {
                    isFirst = false;
                }
                json[0] = '"';
                json = json[1..];
                ReadOnlySpan<char> keySpan = key;
                keySpan.CopyTo(json);
                json = json[keySpan.Length..];
                json[0] = '"';
                json[1] = ':';
                json = json[2..];

                ReadOnlySpan<char> valueSpan = value.ToString();
                valueSpan.CopyTo(json);
                json = json[valueSpan.Length..];
            }

            json[0] = '}';
            json = json[1..];
            return json;
        }




        public static Span<T> EnsureSpan<T>(this IEnumerable<T> source)
        {
            if (source is T[] span)
            {
                return span;
            }
            if (source is List<T> list)
            {
                return System.Runtime.InteropServices.CollectionsMarshal.AsSpan(list);
            }
            var array = source.ToArray();
            return array;
        }

        public static int CopiedTo<T>(this Span<T> source, Span<T> target)
        {
            source.CopyTo(target);
            return source.Length;
        }

        public static int CopiedTo<T>(this ReadOnlySpan<T> source, Span<T> target)
        {
            source.CopyTo(target);
            return source.Length;
        }

        public static System.ReadOnlySpan<char> RemoveObject(this System.ReadOnlySpan<char> json, out SampleNamespace.Father item)
        {
            string name = default;
            int age = default;
            SampleNamespace.Child[] children = default;
            System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, SampleNamespace.Child[]>>[][][][] complaints = default;
            System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, SampleNamespace.Child[]>>> priorities = default;
            System.Collections.Generic.Dictionary<System.DateTime, System.TimeSpan> times = default;
            System.TimeSpan delays = default;
            char symbol = default;
            System.DateTime? canBeNull = default;
            System.DateTime? alsoNullable = default;
            const int propsCount = 10;
            int i = 0;
            json = json.EnterObject();
            while (i < propsCount)
            {
                json = json.SkipInsignificantSymbolsLeft();
                if (json[0] == '}')
                {
                    break;
                }

                json = json
                    .SkipToPropertyName()
                    .RemovePropertyName(out var propertyName)
                    .SkipToPropertyValue();

                short propertyIndex = propertyName switch
                {
                    "name" => 0,
                    "age" => 1,
                    "children" => 2,
                    "complaints" => 3,
                    "priorities" => 4,
                    "times" => 5,
                    "delays" => 6,
                    "symbol" => 7,
                    "canBeNull" => 8,
                    "alsoNullable" => 9,
                    _ => -1
                };

                json = propertyIndex switch
                {
                    0 => json.TryReadNull(out name) ? json[4..] : json.RemoveQuotedValue(j => new string(j), out name),
                    1 => json.RemovePrimitiveValue<int>(j => int.Parse(j), out age),
                    2 => json.TryReadNull(out children) ? json[4..] : json.RemoveArray((JsonPiece json_j, out SampleNamespace.Child children_i) => { var r = json_j.TryReadNull(out children_i) ? json_j[4..] : json_j.RemoveObject(out children_i); return r; }, out children),
                    3 => json.TryReadNull(out complaints) ? json[4..] : json.RemoveArray((JsonPiece json_j, out System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, SampleNamespace.Child[]>>[][][] complaints_i) =>
                    {
                        var r = json_j.TryReadNull(out complaints_i) ? json_j[4..] : json_j.RemoveArray((JsonPiece json_j_j, out System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, SampleNamespace.Child[]>>[][] complaints_i_i) =>
                        {
                            var r = json_j_j.TryReadNull(out complaints_i_i) ? json_j_j[4..] : json_j_j.RemoveArray((JsonPiece json_j_j_j, out System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, SampleNamespace.Child[]>>[] complaints_i_i_i) =>
                            {
                                var r = json_j_j_j.TryReadNull(out complaints_i_i_i) ? json_j_j_j[4..] : json_j_j_j.RemoveArray((JsonPiece json_j_j_j_j, out System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, SampleNamespace.Child[]>> complaints_i_i_i_i) =>
                                {
                                    var r = json_j_j_j_j.TryReadNull(out complaints_i_i_i_i) ? json_j_j_j_j[4..] : json_j_j_j_j.RemoveDictionary(
                    (JsonPiece json_j_j_j_j_j, out string complaints_i_i_i_i_v) => { var r = json_j_j_j_j_j.TryReadNull(out complaints_i_i_i_i_v) ? json_j_j_j_j_j[4..] : json_j_j_j_j_j.RemoveQuotedValue(j => new string(j), out complaints_i_i_i_i_v); return r; },
                    (JsonPiece json_j_j_j_j_j, out System.Collections.Generic.Dictionary<string, SampleNamespace.Child[]> complaints_i_i_i_i_v) =>
                    {
                        var r = json_j_j_j_j_j.TryReadNull(out complaints_i_i_i_i_v) ? json_j_j_j_j_j[4..] : json_j_j_j_j_j.RemoveDictionary(
                    (JsonPiece json_j_j_j_j_j_j, out string complaints_i_i_i_i_v_v) => { var r = json_j_j_j_j_j_j.TryReadNull(out complaints_i_i_i_i_v_v) ? json_j_j_j_j_j_j[4..] : json_j_j_j_j_j_j.RemoveQuotedValue(j => new string(j), out complaints_i_i_i_i_v_v); return r; },
                    (JsonPiece json_j_j_j_j_j_j, out SampleNamespace.Child[] complaints_i_i_i_i_v_v) => { var r = json_j_j_j_j_j_j.TryReadNull(out complaints_i_i_i_i_v_v) ? json_j_j_j_j_j_j[4..] : json_j_j_j_j_j_j.RemoveArray((JsonPiece json_j_j_j_j_j_j_j, out SampleNamespace.Child complaints_i_i_i_i_v_v_i) => { var r = json_j_j_j_j_j_j_j.TryReadNull(out complaints_i_i_i_i_v_v_i) ? json_j_j_j_j_j_j_j[4..] : json_j_j_j_j_j_j_j.RemoveObject(out complaints_i_i_i_i_v_v_i); return r; }, out complaints_i_i_i_i_v_v); return r; },
                    out complaints_i_i_i_i_v); return r;
                    },
                    out complaints_i_i_i_i); return r;
                                }, out complaints_i_i_i); return r;
                            }, out complaints_i_i); return r;
                        }, out complaints_i); return r;
                    }, out complaints),
                    4 => json.TryReadNull(out priorities) ? json[4..] : json.RemoveDictionary(
                    (JsonPiece json_j, out string priorities_v) => { var r = json_j.TryReadNull(out priorities_v) ? json_j[4..] : json_j.RemoveQuotedValue(j => new string(j), out priorities_v); return r; },
                    (JsonPiece json_j, out System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, SampleNamespace.Child[]>> priorities_v) =>
                    {
                        var r = json_j.TryReadNull(out priorities_v) ? json_j[4..] : json_j.RemoveDictionary(
                    (JsonPiece json_j_j, out string priorities_v_v) => { var r = json_j_j.TryReadNull(out priorities_v_v) ? json_j_j[4..] : json_j_j.RemoveQuotedValue(j => new string(j), out priorities_v_v); return r; },
                    (JsonPiece json_j_j, out System.Collections.Generic.Dictionary<string, SampleNamespace.Child[]> priorities_v_v) =>
                    {
                        var r = json_j_j.TryReadNull(out priorities_v_v) ? json_j_j[4..] : json_j_j.RemoveDictionary(
                    (JsonPiece json_j_j_j, out string priorities_v_v_v) => { var r = json_j_j_j.TryReadNull(out priorities_v_v_v) ? json_j_j_j[4..] : json_j_j_j.RemoveQuotedValue(j => new string(j), out priorities_v_v_v); return r; },
                    (JsonPiece json_j_j_j, out SampleNamespace.Child[] priorities_v_v_v) => { var r = json_j_j_j.TryReadNull(out priorities_v_v_v) ? json_j_j_j[4..] : json_j_j_j.RemoveArray((JsonPiece json_j_j_j_j, out SampleNamespace.Child priorities_v_v_v_i) => { var r = json_j_j_j_j.TryReadNull(out priorities_v_v_v_i) ? json_j_j_j_j[4..] : json_j_j_j_j.RemoveObject(out priorities_v_v_v_i); return r; }, out priorities_v_v_v); return r; },
                    out priorities_v_v); return r;
                    },
                    out priorities_v); return r;
                    },
                    out priorities),
                    5 => json.TryReadNull(out times) ? json[4..] : json.RemoveDictionary(
                    (JsonPiece json_j, out System.DateTime times_v) => { var r = json_j.RemoveQuotedValue(j => System.DateTime.Parse(j), out times_v); return r; },
                    (JsonPiece json_j, out System.TimeSpan times_v) => { var r = json_j.RemoveQuotedValue(j => System.TimeSpan.Parse(j), out times_v); return r; },
                    out times),
                    6 => json.RemoveQuotedValue(j => System.TimeSpan.Parse(j), out delays),
                    7 => json.RemoveQuotedValue(j => j[0], out symbol),
                    8 => json.TryReadNull(out canBeNull) ? json[4..] : json.RemoveQuotedValue(j => System.DateTime.Parse(j), out canBeNull),
                    9 => json.TryReadNull(out alsoNullable) ? json[4..] : json.RemoveQuotedValue(j => System.DateTime.Parse(j), out alsoNullable),
                    _ => json.SkipValue()
                };

                if (propertyIndex != -1)
                {
                    i++;
                }
            }

            item = new()
            {
                Name = name,
                Age = age,
                Children = children,
                Complaints = complaints,
                Priorities = priorities,
                Times = times,
                Delays = delays,
                Symbol = symbol,
                CanBeNull = canBeNull,
                AlsoNullable = alsoNullable
            };

            json = json.SkipOverClosedBracket('}');

            return json;

        }



        public static int CalculateJsonLength(this SampleNamespace.Father item)
        {
            var item_Name = item.Name;
            var item_Name_Length = item_Name is null ? 0 : (7 + item_Name.Length * 2 + 2);
            int item_Age_Length = item.Age == default ? 0 : (6 + 11);
            var item_Children = item.Children;
            var item_Children_Length = item_Children is null ? 0 : (11 + item_Children.CalculateJsonLength(item_Children_item => item_Children_item.CalculateJsonLength()));
            var item_Complaints = item.Complaints;
            var item_Complaints_Length = item_Complaints is null ? 0 : (13 + item_Complaints.CalculateJsonLength(item_Complaints_item => item_Complaints_item.CalculateJsonLength(item_Complaints_item_item => item_Complaints_item_item.CalculateJsonLength(item_Complaints_item_item_item => item_Complaints_item_item_item.CalculateJsonLength(item_Complaints_item_item_item_item => item_Complaints_item_item_item_item.CalculateJsonLength(item_Complaints_item_item_item_item_key => item_Complaints_item_item_item_item_key.Length * 2 + 2, item_Complaints_item_item_item_item_value => item_Complaints_item_item_item_item_value.CalculateJsonLength(item_Complaints_item_item_item_item_value_key => item_Complaints_item_item_item_item_value_key.Length * 2 + 2, item_Complaints_item_item_item_item_value_value => item_Complaints_item_item_item_item_value_value.CalculateJsonLength(item_Complaints_item_item_item_item_value_value_item => item_Complaints_item_item_item_item_value_value_item.CalculateJsonLength()))))))));
            var item_Priorities = item.Priorities;
            var item_Priorities_Length = item_Priorities is null ? 0 : (13 + item_Priorities.CalculateJsonLength(item_Priorities_key => item_Priorities_key.Length * 2 + 2, item_Priorities_value => item_Priorities_value.CalculateJsonLength(item_Priorities_value_key => item_Priorities_value_key.Length * 2 + 2, item_Priorities_value_value => item_Priorities_value_value.CalculateJsonLength(item_Priorities_value_value_key => item_Priorities_value_value_key.Length * 2 + 2, item_Priorities_value_value_value => item_Priorities_value_value_value.CalculateJsonLength(item_Priorities_value_value_value_item => item_Priorities_value_value_value_item.CalculateJsonLength())))));
            var item_Times = item.Times;
            var item_Times_Length = item_Times is null ? 0 : (8 + item_Times.CalculateJsonLength(item_Times_key => 30, item_Times_value => 15));
            int item_Delays_Length = item.Delays == default ? 0 : (9 + 15);
            int item_Symbol_Length = item.Symbol == default ? 0 : (9 + 3);
            int item_CanBeNull_Length = item.CanBeNull == default ? 0 : (12 + 30);
            int item_AlsoNullable_Length = item.AlsoNullable == default ? 0 : (15 + 30);
            var result = 10 - 1 + item_Name_Length + item_Age_Length + item_Children_Length + item_Complaints_Length + item_Priorities_Length + item_Times_Length + item_Delays_Length + item_Symbol_Length + item_CanBeNull_Length + item_AlsoNullable_Length;
            return result;
        }

        public static Span<char> FillWith(
                this Span<char> span,
                SampleNamespace.Father item
            )
        {
            span[0] = '{';
            bool isFirst = true;
            span = span[1..];
            if (item.Name is not null)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    span[0] = ',';
                    span = span[1..];
                }
                "\"name\":".AsSpan().CopyTo(span);
                span = span[7..];

                var item_Name = item.Name;
                span = span.FillWithQuoted(item_Name);

            }
            if (item.Age != default)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    span[0] = ',';
                    span = span[1..];
                }
                "\"age\":".AsSpan().CopyTo(span);
                span = span[6..];

                var item_Age = item.Age;
                span = span.FillWith(item_Age);

            }
            if (item.Children is not null)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    span[0] = ',';
                    span = span[1..];
                }
                "\"children\":".AsSpan().CopyTo(span);
                span = span[11..];

                var item_Children = item.Children;
                span = span.FillWith(item_Children.EnsureSpan(),
                (Span<char> span_arraySpan, SampleNamespace.Child item_Children_item) => span_arraySpan.FillWith(item_Children_item));

            }
            if (item.Complaints is not null)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    span[0] = ',';
                    span = span[1..];
                }
                "\"complaints\":".AsSpan().CopyTo(span);
                span = span[13..];

                var item_Complaints = item.Complaints;
                span = span.FillWith(item_Complaints.EnsureSpan(),
                (Span<char> span_arraySpan, System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, SampleNamespace.Child[]>>[][][] item_Complaints_item) => span_arraySpan.FillWith(item_Complaints_item.EnsureSpan(),
                (Span<char> span_arraySpan_arraySpan, System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, SampleNamespace.Child[]>>[][] item_Complaints_item_item) => span_arraySpan_arraySpan.FillWith(item_Complaints_item_item.EnsureSpan(),
                (Span<char> span_arraySpan_arraySpan_arraySpan, System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, SampleNamespace.Child[]>>[] item_Complaints_item_item_item) => span_arraySpan_arraySpan_arraySpan.FillWith(item_Complaints_item_item_item.EnsureSpan(),
                (Span<char> span_arraySpan_arraySpan_arraySpan_arraySpan, System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, SampleNamespace.Child[]>> item_Complaints_item_item_item_item) => span_arraySpan_arraySpan_arraySpan_arraySpan.FillWith(item_Complaints_item_item_item_item,
                    (Span<char> span_arraySpan_arraySpan_arraySpan_arraySpan_dictionarySpan, string item_Complaints_item_item_item_item_key) => span_arraySpan_arraySpan_arraySpan_arraySpan_dictionarySpan.FillWithQuoted(item_Complaints_item_item_item_item_key),
                    (Span<char> span_arraySpan_arraySpan_arraySpan_arraySpan_dictionarySpan, System.Collections.Generic.Dictionary<string, SampleNamespace.Child[]> item_Complaints_item_item_item_item_value) => span_arraySpan_arraySpan_arraySpan_arraySpan_dictionarySpan.FillWith(item_Complaints_item_item_item_item_value,
                    (Span<char> span_arraySpan_arraySpan_arraySpan_arraySpan_dictionarySpan_dictionarySpan, string item_Complaints_item_item_item_item_value_key) => span_arraySpan_arraySpan_arraySpan_arraySpan_dictionarySpan_dictionarySpan.FillWithQuoted(item_Complaints_item_item_item_item_value_key),
                    (Span<char> span_arraySpan_arraySpan_arraySpan_arraySpan_dictionarySpan_dictionarySpan, SampleNamespace.Child[] item_Complaints_item_item_item_item_value_value) => span_arraySpan_arraySpan_arraySpan_arraySpan_dictionarySpan_dictionarySpan.FillWith(item_Complaints_item_item_item_item_value_value.EnsureSpan(),
                (Span<char> span_arraySpan_arraySpan_arraySpan_arraySpan_dictionarySpan_dictionarySpan_arraySpan, SampleNamespace.Child item_Complaints_item_item_item_item_value_value_item) => span_arraySpan_arraySpan_arraySpan_arraySpan_dictionarySpan_dictionarySpan_arraySpan.FillWith(item_Complaints_item_item_item_item_value_value_item))))))));

            }
            if (item.Priorities is not null)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    span[0] = ',';
                    span = span[1..];
                }
                "\"priorities\":".AsSpan().CopyTo(span);
                span = span[13..];

                var item_Priorities = item.Priorities;
                span = span.FillWith(item_Priorities,
                    (Span<char> span_dictionarySpan, string item_Priorities_key) => span_dictionarySpan.FillWithQuoted(item_Priorities_key),
                    (Span<char> span_dictionarySpan, System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, SampleNamespace.Child[]>> item_Priorities_value) => span_dictionarySpan.FillWith(item_Priorities_value,
                    (Span<char> span_dictionarySpan_dictionarySpan, string item_Priorities_value_key) => span_dictionarySpan_dictionarySpan.FillWithQuoted(item_Priorities_value_key),
                    (Span<char> span_dictionarySpan_dictionarySpan, System.Collections.Generic.Dictionary<string, SampleNamespace.Child[]> item_Priorities_value_value) => span_dictionarySpan_dictionarySpan.FillWith(item_Priorities_value_value,
                    (Span<char> span_dictionarySpan_dictionarySpan_dictionarySpan, string item_Priorities_value_value_key) => span_dictionarySpan_dictionarySpan_dictionarySpan.FillWithQuoted(item_Priorities_value_value_key),
                    (Span<char> span_dictionarySpan_dictionarySpan_dictionarySpan, SampleNamespace.Child[] item_Priorities_value_value_value) => span_dictionarySpan_dictionarySpan_dictionarySpan.FillWith(item_Priorities_value_value_value.EnsureSpan(),
                (Span<char> span_dictionarySpan_dictionarySpan_dictionarySpan_arraySpan, SampleNamespace.Child item_Priorities_value_value_value_item) => span_dictionarySpan_dictionarySpan_dictionarySpan_arraySpan.FillWith(item_Priorities_value_value_value_item)))));

            }
            if (item.Times is not null)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    span[0] = ',';
                    span = span[1..];
                }
                "\"times\":".AsSpan().CopyTo(span);
                span = span[8..];

                var item_Times = item.Times;
                span = span.FillWith(item_Times,
                    (Span<char> span_dictionarySpan, System.DateTime item_Times_key) => span_dictionarySpan.FillWithQuoted(item_Times_key),
                    (Span<char> span_dictionarySpan, System.TimeSpan item_Times_value) => span_dictionarySpan.FillWithQuoted(item_Times_value));

            }
            if (item.Delays != default)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    span[0] = ',';
                    span = span[1..];
                }
                "\"delays\":".AsSpan().CopyTo(span);
                span = span[9..];

                var item_Delays = item.Delays;
                span = span.FillWithQuoted(item_Delays);

            }
            if (item.Symbol != default)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    span[0] = ',';
                    span = span[1..];
                }
                "\"symbol\":".AsSpan().CopyTo(span);
                span = span[9..];

                var item_Symbol = item.Symbol;
                span = span.FillWithQuoted(item_Symbol);

            }
            if (item.CanBeNull is not null)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    span[0] = ',';
                    span = span[1..];
                }
                "\"canBeNull\":".AsSpan().CopyTo(span);
                span = span[12..];

                var item_CanBeNull = item.CanBeNull;
                span = span.FillWithQuoted(item_CanBeNull.Value);

            }
            if (item.AlsoNullable is not null)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    span[0] = ',';
                    span = span[1..];
                }
                "\"alsoNullable\":".AsSpan().CopyTo(span);
                span = span[15..];

                var item_AlsoNullable = item.AlsoNullable;
                span = span.FillWithQuoted(item_AlsoNullable.Value);

            }
            span[0] = '}';
            span = span[1..];
            ;
            return span;
        }


        public static string Serialize(this SampleNamespace.Father item)
        {
            var length = CalculateJsonLength(item);
            Span<char> resultSpan = stackalloc char[length];
            var jsonLength = length - resultSpan.FillWith(item).Length;
            var result = new string(resultSpan[..jsonLength]);
            return result;
        }


        /*
        public static Span<char> FillWith(this Span<char> span,
            IEnumerable<KeyValuePair<string, SampleNamespace.Father>> keyValues)
        {
            span[0] = '{';
            span = span[1..];
            bool isFirst = true;
            foreach (var (key, value) in keyValues)
            {
                if (!isFirst)
                {
                    span[0] = ',';
                    span = span[1..];
                }
                else 
                {
                    isFirst = false;
                }
                var spanKey = key.AsSpan();
                span[0] = '"';
                span = span[1..];
                spanKey.CopyTo(span);
                span = span[spanKey.Length..];
                span[0] = '"';
                span[1] = ':';
                span = span[2..];
                span = span.FillWith(value);
            }
            span[0] = '}';
            return span[1..];
        }

        public static Span<char> FillWith(this Span<char> targetSpan, IEnumerable<SampleNamespace.Father> source)
            => targetSpan.FillWith(source.EnsureSpan());

        public static string Serialize(this Span<SampleNamespace.Father> items)
        {
            var jsonLength = CalculateJsonLength(items);
            Span<char> spanResult = stackalloc char[jsonLength];

            spanResult.FillWith(items);

            var result = new string(spanResult);

            return result;
        }

        public static string Serialize(this IEnumerable<SampleNamespace.Father> source)
            => source.EnsureSpan().Serialize();

        */

        public static System.ReadOnlySpan<char> RemoveObject(this System.ReadOnlySpan<char> json, out SampleNamespace.Child item)
        {
            int age = default;
            string name = default;
            const int propsCount = 2;
            int i = 0;
            json = json.EnterObject();
            while (i < propsCount)
            {
                json = json.SkipInsignificantSymbolsLeft();
                if (json[0] == '}')
                {
                    break;
                }

                json = json
                    .SkipToPropertyName()
                    .RemovePropertyName(out var propertyName)
                    .SkipToPropertyValue();

                short propertyIndex = propertyName switch
                {
                    "age" => 0,
                    "name" => 1,
                    _ => -1
                };

                json = propertyIndex switch
                {
                    0 => json.RemovePrimitiveValue<int>(j => int.Parse(j), out age),
                    1 => json.TryReadNull(out name) ? json[4..] : json.RemoveQuotedValue(j => new string(j), out name),
                    _ => json.SkipValue()
                };

                if (propertyIndex != -1)
                {
                    i++;
                }
            }

            item = new()
            {
                Age = age,
                Name = name
            };

            json = json.SkipOverClosedBracket('}');

            return json;

        }



        public static int CalculateJsonLength(this SampleNamespace.Child item)
        {
            int item_Age_Length = item.Age == default ? 0 : (6 + 11);
            var item_Name = item.Name;
            var item_Name_Length = item_Name is null ? 0 : (7 + item_Name.Length * 2 + 2);
            var result = 2 - 1 + item_Age_Length + item_Name_Length;
            return result;
        }

        public static Span<char> FillWith(
                this Span<char> span,
                SampleNamespace.Child item
            )
        {
            span[0] = '{';
            bool isFirst = true;
            span = span[1..];
            if (item.Age != default)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    span[0] = ',';
                    span = span[1..];
                }
                "\"age\":".AsSpan().CopyTo(span);
                span = span[6..];

                var item_Age = item.Age;
                span = span.FillWith(item_Age);

            }
            if (item.Name is not null)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    span[0] = ',';
                    span = span[1..];
                }
                "\"name\":".AsSpan().CopyTo(span);
                span = span[7..];

                var item_Name = item.Name;
                span = span.FillWithQuoted(item_Name);

            }
            span[0] = '}';
            span = span[1..];
            ;
            return span;
        }


        public static string Serialize(this SampleNamespace.Child item)
        {
            var length = CalculateJsonLength(item);
            Span<char> resultSpan = stackalloc char[length];
            var jsonLength = length - resultSpan.FillWith(item).Length;
            var result = new string(resultSpan[..jsonLength]);
            return result;
        }


        /*
        public static Span<char> FillWith(this Span<char> span,
            IEnumerable<KeyValuePair<string, SampleNamespace.Child>> keyValues)
        {
            span[0] = '{';
            span = span[1..];
            bool isFirst = true;
            foreach (var (key, value) in keyValues)
            {
                if (!isFirst)
                {
                    span[0] = ',';
                    span = span[1..];
                }
                else 
                {
                    isFirst = false;
                }
                var spanKey = key.AsSpan();
                span[0] = '"';
                span = span[1..];
                spanKey.CopyTo(span);
                span = span[spanKey.Length..];
                span[0] = '"';
                span[1] = ':';
                span = span[2..];
                span = span.FillWith(value);
            }
            span[0] = '}';
            return span[1..];
        }

        public static Span<char> FillWith(this Span<char> targetSpan, IEnumerable<SampleNamespace.Child> source)
            => targetSpan.FillWith(source.EnsureSpan());

        public static string Serialize(this Span<SampleNamespace.Child> items)
        {
            var jsonLength = CalculateJsonLength(items);
            Span<char> spanResult = stackalloc char[jsonLength];

            spanResult.FillWith(items);

            var result = new string(spanResult);

            return result;
        }

        public static string Serialize(this IEnumerable<SampleNamespace.Child> source)
            => source.EnsureSpan().Serialize();

        */

    }
}