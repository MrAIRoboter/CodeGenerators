using System.Text;
using CodeGenerators.Placeholders.Elements.Interfaces;

namespace CodeGenerators.Example.Generator.Placeholders.Elements;
public sealed class BuiltInChecks : IRenderableCode
{
    private const string _template = """
if (%FlagName% == true && userContext.HasPermission("%Permission%") == false)
            failedPermissions.Add("%Permission%");
""";

    private Dictionary<string, string> _flagPermissionPairs;

    public BuiltInChecks(Dictionary<string, string> flagPermissionPairs)
    {
        _flagPermissionPairs = flagPermissionPairs;
    }

    public string Render()
    {
        StringBuilder stringBuilder = new StringBuilder();

        foreach (KeyValuePair<string, string> flagPermissionPair in _flagPermissionPairs)
            stringBuilder.AppendLine(_template.Replace("%FlagName%", flagPermissionPair.Key).Replace("%Permission%", flagPermissionPair.Value));

        return stringBuilder.ToString();
    }
}
