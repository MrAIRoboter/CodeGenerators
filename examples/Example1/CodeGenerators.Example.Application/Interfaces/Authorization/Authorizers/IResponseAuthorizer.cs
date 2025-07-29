using CodeGenerators.Example.Application.Interfaces.ScopedContexts;
using CodeGenerators.Example.Contracts.Common.Abstracts;

namespace CodeGenerators.Example.Application.Interfaces.Authorization.Authorizers;
public interface IResponseAuthorizer<TResponse> where TResponse : AbstractAuthorizableResponse
{
    public bool IsAuthorized(TResponse response, IUserContext userContext, out List<string> failedPermissions);
}