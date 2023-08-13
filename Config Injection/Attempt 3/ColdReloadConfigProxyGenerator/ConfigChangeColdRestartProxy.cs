using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace ConfigInjection;

/// <summary>
/// So the idea here is to use a source generator to build the
/// Cold reload proxy... but I haven't figured out how to use it
/// but it's cool. One day I suppose it'll make sense and I can
/// use a debugger to watch it.
/// </summary>
[Generator]
public class MySourceGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {


        var sourceBuilder = new StringBuilder(
@"
using System;
namespace ConfigInjection
{
    public static class HelloWorld
    {
        public static void SayHello() 
        {
            Console.WriteLine(""Hello from generated code!"");
            Console.WriteLine(""The following syntax trees existed in the compilation that created this program:"");
");

        // using the context, get a list of syntax trees in the users compilation
        var syntaxTrees = context.Compilation.SyntaxTrees;

        // add the filepath of each tree to the class we're building
        foreach (SyntaxTree tree in syntaxTrees)
        {
            sourceBuilder.AppendLine($@"Console.WriteLine(@"" - {tree.FilePath}"");");
        }

        // finish creating the source to inject
        sourceBuilder.Append(
@"
        }
    }
}");

        // inject the created source into the users compilation
        context.AddSource("helloWorldGenerator.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));


        //throw new NotImplementedException();
    }

    public void Initialize(GeneratorInitializationContext context)
    {

    }
}

