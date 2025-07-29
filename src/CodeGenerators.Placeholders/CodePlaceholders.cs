using System.Text;
using CodeGenerators.Placeholders.Elements;
using CodeGenerators.Placeholders.Elements.Interfaces;

namespace CodeGenerators.Placeholders;
public sealed class CodePlaceholders
{
    private string _codeTemplate;
    private Dictionary<string, IRenderableCode> _replacements;

    public CodePlaceholders(string codeTemplate)
    {
        _codeTemplate = codeTemplate;
        _replacements = new Dictionary<string, IRenderableCode>();
    }

    public CodePlaceholders Replace(string placeholder, IRenderableCode code)
    {
        _replacements[placeholder] = code;

        return this;
    }

    public CodePlaceholders Replace(string placeholder, string code)
    {
        _replacements[placeholder] = new RawCode(code);

        return this;
    }

    public string Build()
    {
        StringBuilder stringBuilder = new StringBuilder(_codeTemplate);

        foreach (KeyValuePair<string, IRenderableCode> replacement in _replacements)
            stringBuilder.Replace(replacement.Key, replacement.Value.Render());

        return stringBuilder.ToString();
    }
}
