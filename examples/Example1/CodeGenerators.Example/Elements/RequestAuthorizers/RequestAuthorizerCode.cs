namespace CodeGenerators.Example.Generator.Elements.RequestAuthorizers;
public sealed record RequestAuthorizerCode
{
    public readonly string SaveFilePath;
    public readonly string Code;

    public RequestAuthorizerCode(string saveFilePath, string code)
    {
        SaveFilePath = saveFilePath;
        Code = code;
    }
}
