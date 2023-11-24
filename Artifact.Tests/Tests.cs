using CJason.Provision;
using Newtonsoft.Json;
using SampleNamespace;
using System.Diagnostics;
using System.Text.Json;

namespace Artifact.Tests
{
    public class Tests
    {
        static T[] ToArray<T>(T item) => new T[1] { item };

        static Father BigFather = CreateInstance();

        static string GetString() => Guid.NewGuid().ToString();
        static int GetInt() => new object().GetHashCode();
        static Father CreateInstance() => new Father
        {
            Name = GetString(),
            Age = 51,
            Children = new Child[]
                            {
                new() { Name = GetString(), Age = GetInt() },
                new() { Name = GetString(), Age = GetInt() }
                            },
            Complaints = ToArray(ToArray(ToArray(ToArray(new Dictionary<string, Dictionary<string, Child[]>>
            {
                { "Complaint", new()
                    {
                        {
                            "Subcomplaint", new Child[]
                            {
                                new Child
                                {
                                    Age = GetInt(),
                                    Name = GetString()
                                }
                            }
                        }
                    }
                }
            })))),

            Priorities = new()
            {
                { GetString(),
                    new()
                    {
                        { GetString(), new()
                            {
                                {GetString(), new Child[]
                                    {
                                        new()
                                        {
                                            Age = GetInt(),
                                            Name = GetString()
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            },
            Delays = TimeSpan.FromSeconds(GetInt()),
            Symbol = '\t',
            Times = new Dictionary<DateTime, TimeSpan>()
            {
                {
                    new DateTime(324,2,3), TimeSpan.FromHours(GetInt())
                }
            }
        };

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestJsonSerialization_ChildObject()
        {
            var child = BigFather.Children[0];

            var childJson = child.Serialize().AsSpan();

            var deserialized = childJson.RemoveObject(out Child child1);

            Assert.AreEqual(child.Name, child1.Name);
            Assert.AreEqual(child.Age, child1.Age);
        }

        [Test]
        public void TestJsonSerialization_NumberDictionary()
        {
            var dict = Enumerable.Range(0, 100).ToDictionary(i => i.ToString(), i => i * 4);

            Span<char> dictJson = stackalloc char[dict.CalculateJsonLength()];

            dictJson.FillWith(dict);

            var deserialized = ((ReadOnlySpan<char>)dictJson).RemoveStringKeyDictionary<int>(JsonDeserializationUtilities.RemoveNumber<int>, out var numbers);

            Assert.That(numbers.All(kv => dict[kv.Key] == kv.Value));
        }

        [Test]
        public void TestJsonDeserialization_Full()
        {
            var father = BigFather;

            var fatherJson = father.Serialize();

            var json = fatherJson.AsSpan();

            ArtificialStringifier.RemoveObject(json, out father);

            Assert.AreEqual(BigFather.Age, father.Age);
            Assert.AreEqual(BigFather.Name, father.Name);

            Assert.AreEqual(BigFather.Priorities.Count, father.Priorities.Count);
            Assert.AreEqual(BigFather.Complaints.Length, father.Complaints.Length);
            Assert.AreEqual(BigFather.Children.Length, father.Children.Length);

            Assert.AreEqual(BigFather.Delays, father.Delays);
            Assert.AreEqual(BigFather.Symbol, father.Symbol);
            Assert.AreEqual(BigFather.Times.Count, father.Times.Count);
        }

        [Test]
        public void TestEmptyAndNullableFields_Serialization()
        {
            var father = new Father
            {
                Children = Array.Empty<Child>(),
                AlsoNullable = DateTime.Today,
                Name = "\' \" \t dsfds \\"
            };

            var json = father.Serialize();

            var jsonRemainder = json.AsSpan().RemoveObject(out Father fatherDeserialized);

            Assert.AreEqual(father.Children.Length, fatherDeserialized.Children.Length);
            Assert.AreEqual(father.AlsoNullable, fatherDeserialized.AlsoNullable);
            Assert.AreEqual(father.CanBeNull, fatherDeserialized.CanBeNull);
            Assert.AreEqual(father.Name, fatherDeserialized.Name);
            Assert.AreEqual(0, jsonRemainder.Length);
        }

        [Test]
        public void EscapedString_FillQuoted()
        {
            const string escaped = "\' \" \t dsfds \\";

            Span<Char> buffer = stackalloc char[100];

            buffer.FillWithQuoted(escaped);

            ((ReadOnlySpan<char>)buffer).RemoveString(out var deserialized);

            Assert.AreEqual(escaped, deserialized);
        }

        private static string SerializeFathers(IEnumerable<Father> fathers)
        {
            var length = fathers.CalculateJsonLength(f => f.CalculateJsonLength());
            Span<char> span = stackalloc char[length];
            length = length - span.FillWith(fathers, (j, obj) => j.FillWith(obj)).Length;
            var result = new string(span[..length]);
            return result;
        }

        [Test]
        public void StandardDeserialize_AutogeneratedJson()
        {
            var fathers = Enumerable.Range(0, 2).Select(i => CreateInstance()).ToArray();
            var json = SerializeFathers(fathers);

            var standard = JsonConvert.DeserializeObject<Father[]>(json);

            DeserializeFathers(json, out var des);
        }

        private static ReadOnlySpan<char> DeserializeFathers(string json, out Father[] fathers)
            => json.AsSpan().RemoveArray((ReadOnlySpan<char> json, out Father item) => json.RemoveObject(out item), out fathers);

        [Test]
        public void Match_Newtonsoft_Standard()
        {
            const string escaped = "\' \" \t dsfds \\";

            var ser = JsonConvert.SerializeObject(escaped);

            var des = System.Text.Json.JsonSerializer.Deserialize<string>(ser);

            Assert.AreEqual(escaped, des);
        }

        static TimeSpan MeasureRunTime(Action action)
        {
            var init = Stopwatch.GetTimestamp();
            action();
            return TimeSpan.FromTicks(Stopwatch.GetTimestamp() - init);
        }
    }
}