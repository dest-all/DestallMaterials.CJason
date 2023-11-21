using CJason.Provision;
using SampleNamespace;
using System.Text.Json;

namespace Artifact.Tests
{
    public class Tests
    {
        static T[] ToArray<T>(T item) => new T[1] { item };

        static Father SampleFather = new Father
        {
            Name = "Alex",
            Age = 51,
            Children = new Child[]
                {
                new() { Name = "Igor", Age = 26 },
                new() { Name = "Someone", Age = 555 }
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
                                    Age = 100,
                                    Name = "Zerg"
                                }
                            }
                        }
                    }
                }
            })))),

            Priorities = new()
            {
                { "naruto",
                    new()
                    {
                        { "Sasuke", new()
                            {
                                {"Cangaroo", new Child[]
                                    {
                                        new()
                                        {
                                            Age = 180,
                                            Name = "Irrity"
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            },
            Delays = TimeSpan.FromSeconds(324324),
            Symbol = '\t',
            Times = new Dictionary<DateTime, TimeSpan>()
            {
                {
                    new DateTime(1,2,3), TimeSpan.FromHours(234)
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
            var child = SampleFather.Children[0];

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
            var father = SampleFather;

            var fatherJson = father.Serialize();

            var json = fatherJson.AsSpan();

            ArtificialStringifier.RemoveObject(json, out father);

            Assert.AreEqual(SampleFather.Age, father.Age);
            Assert.AreEqual(SampleFather.Name, father.Name);

            Assert.AreEqual(SampleFather.Priorities.Count, father.Priorities.Count);
            Assert.AreEqual(SampleFather.Complaints.Length, father.Complaints.Length);
            Assert.AreEqual(SampleFather.Children.Length, father.Children.Length);

            Assert.AreEqual(SampleFather.Delays, father.Delays);
            Assert.AreEqual(SampleFather.Symbol, father.Symbol);
            Assert.AreEqual(SampleFather.Times.Count, father.Times.Count);
        }

        static void AssertRightSerialization()
        {
        }
    }
}