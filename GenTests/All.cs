using System.Runtime.InteropServices;
using System.Text.Json;
using CJason;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SampleNamespace;
using SpanBuilder;

namespace GenTests;

public class All
{
    const string _sampleClassCode =
        @"namespace SampleNamespace {
        public class Father
        {
            public string Name { get; set; }
            public int Age { get; set; }

            public Child[] Children { get; set; }
        }

        public class Child 
        {
            public int Age { get; set; }
            public string Name { get; set; }
        }
    }";

    [SetUp]
    public void Setup() { }

    [Test]
    public void TestJsonSerialization()
    {
        var father = new Father
        {
            Name = "Alex",
            Age = 51,
            Children = new Child[]
            {
                new() { Name = "Igor", Age = 26 },
                new() { Name = "Someone", Age = 555 }
            }
        };

        var fathers = new Father[] 
        {
            father
        };

        var json = fathers.Serialize();

        var deserialized = JsonSerializer.Deserialize<Father[]>(json);
    }

    [Test]
    public void TestCodeGeneration()
    {
        var tree = CSharpSyntaxTree.ParseText(_sampleClassCode);

        var coreLib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
        var compilation = CSharpCompilation.Create(
            "SampleCompilation",
            new[] { tree },
            references: new[] { coreLib }
        );

        var model = compilation.GetSemanticModel(tree);

        var classSyntaxes = tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>();

        string generated = "";

        foreach (var cds in classSyntaxes)
        {
            var classSymbol = model.GetDeclaredSymbol(cds);
            generated += (
                "\n" + SerializationGenerator.GenerateSerializationMethodsEtc(classSymbol)
            );
        }

        generated =
            $@"{_sampleClassCode}
        namespace SampleNamespace 
        {{
            public static class ArtificialStringifier
            {{
                public static Span<T> EnsureSpan<T>(this IEnumerable<T> source)
                {{
                    if (source is T[] span)
                    {{
                        return span;
                    }}
                    if (source is List<T> list)
                    {{
                        return System.Runtime.InteropServices.CollectionsMarshal.AsSpan(list);
                    }}
                    var array = source.ToArray();
                    return array;
                }}

                public static int CopiedTo<T>(this Span<T> source, Span<T> target)
                {{
                    source.CopyTo(target);
                    return source.Length;
                }}

                public static int CopiedTo<T>(this ReadOnlySpan<T> source, Span<T> target)
                {{
                    source.CopyTo(target);
                    return source.Length;
                }}

                {generated}
            }}
        }}";

        File.WriteAllText("..\\..\\..\\..\\Artifact\\Code.cs", generated);

        return;
    }
}
