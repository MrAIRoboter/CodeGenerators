namespace CodeGenerators.Example.Application.Interfaces.ScopedContexts;
public interface IUserContext
{
    public bool HasPermission(string permission);
}
