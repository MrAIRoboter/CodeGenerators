using System.Text;
using CodeGenerators.Placeholders.Elements.Interfaces;

namespace CodeGenerators.Example.Generator.Placeholders.Elements;
public sealed class MachineFlags : IRenderableCode
{
    private const string _template = """
public bool %FlagName% = true;
""";

    private List<string> _flags;

    public MachineFlags(List<string> flags)
    {
        _flags = flags;
    }

    public string Render()
    {
        StringBuilder sb = new StringBuilder();

        foreach (string flag in _flags)
            sb.AppendLine(_template.Replace("%FlagName%", flag));

        return sb.ToString();
    }
}
