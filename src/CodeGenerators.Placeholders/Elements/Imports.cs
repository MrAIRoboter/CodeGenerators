using System.Text;
using CodeGenerators.Placeholders.Elements.Interfaces;

namespace CodeGenerators.Placeholders.Elements;
public sealed class Imports : IRenderableCode
{
    private readonly HashSet<string> _namespaces;

    public Imports(HashSet<string> namespaces)
    {
        _namespaces = namespaces;
    }

    public Imports(params string[] namespaces)
    {
        _namespaces = namespaces.ToHashSet();
    }

    public string Render()
    {
        StringBuilder sb = new StringBuilder();

        foreach (string ns in _namespaces)
            sb.AppendLine($"using {ns};");

        return sb.ToString();
    }
}
