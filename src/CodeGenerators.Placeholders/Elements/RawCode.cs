using CodeGenerators.Placeholders.Elements.Interfaces;

namespace CodeGenerators.Placeholders.Elements;
public sealed class RawCode : IRenderableCode
{
    private readonly string _code;

    public RawCode(string code)
    {
        _code = code;
    }

    public string Render() => _code;
}
