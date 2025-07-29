using System.Text;
using CodeGenerators.Placeholders.Elements.Interfaces;

namespace CodeGenerators.Placeholders.Elements;
public sealed class HeaderImports : IRenderableCode
{
    private HashSet<string> _namespaces;

    public HeaderImports(HashSet<string> namespaces)
    {
        _namespaces = namespaces;
    }

    public HeaderImports(params string[] namespaces)
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
