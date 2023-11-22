using System;
using System.Collections.Generic;
using System.Linq;

namespace SampleNamespace
{
    public class Father : IEquatable<Father?>
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public Child[] Children { get; set; }

        public Dictionary<string, Dictionary<string, Child[]>>[][][][] Complaints { get; set; } 
            = Array.Empty<Dictionary<string, Dictionary<string, Child[]>>[][][]>();

        public Dictionary<string, Dictionary<string, Dictionary<string, Child[]>>> Priorities { get; set; } = new();

        public Dictionary<DateTime, TimeSpan> Times { get; set; }

        public TimeSpan Delays { get; set; }

        public Char Symbol { get; set; }

        public DateTime? CanBeNull { get; set; }

        public DateTime? AlsoNullable { get; set; }

        public override bool Equals(object? obj) 
            => Equals(obj as Father);

        public bool Equals(Father? other) => other is not null &&
                   Name == other.Name &&
                   Age == other.Age &&
                   Children?.Length == other.Children?.Length &&
                   Children?.All(c => other.Children.Contains(c)) != false;

        public override int GetHashCode() 
            => HashCode.Combine(Name, Age, Children);

        public static bool operator ==(Father? left, Father? right) 
            => EqualityComparer<Father>.Default.Equals(left, right);

        public static bool operator !=(Father? left, Father? right) 
            => !(left == right);
    }

    public class Child : IEquatable<Child?>
    {
        public int Age { get; set; }
        public string Name { get; set; }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Child);
        }

        public bool Equals(Child? other)
        {
            return other is not null &&
                   Age == other.Age &&
                   Name == other.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Age, Name);
        }

        public static bool operator ==(Child? left, Child? right)
        {
            return EqualityComparer<Child>.Default.Equals(left, right);
        }

        public static bool operator !=(Child? left, Child? right)
        {
            return !(left == right);
        }
    }
}
