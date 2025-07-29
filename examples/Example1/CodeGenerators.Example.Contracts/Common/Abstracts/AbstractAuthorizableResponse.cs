namespace CodeGenerators.Example.Contracts.Common.Abstracts;
public abstract class AbstractAuthorizableResponse
{
    public Dictionary<string, string> AccessRestrictions { get; set; } = new Dictionary<string, string>();
}
