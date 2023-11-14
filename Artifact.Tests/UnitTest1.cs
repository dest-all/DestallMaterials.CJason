using CJason.Provision;
using SampleNamespace;
using System.Text.Json;

namespace Artifact.Tests
{
    public class Tests
    {
        static Father SampleFather = new Father
        {
            Name = "Alex",
            Age = 51,
            Children = new Child[]
                {
                new() { Name = "Igor", Age = 26 },
                new() { Name = "Someone", Age = 555 }
                }
        };


        [SetUp]
        public void Setup()
        {
        }


        [Test]
        public void TestJsonDeserialization()
        {
            var father = SampleFather;

            var fathers = new Father[]
            {
                father
            };

            var json = fathers.Serialize().AsSpan();

            json = json.RemoveArray(ArtificialStringifier.RemoveObject, out fathers);
        }
    }
}