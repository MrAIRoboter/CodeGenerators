using CodeGenerators.Example.Application.Interfaces.ScopedContexts;
using CodeGenerators.Example.Contracts.Common.Abstracts;

namespace CodeGenerators.Example.Application.Interfaces.Authorization.Authorizers;
public interface IRequestAuthorizer<TRequest, TResponse> where TRequest : AbstractAuthorizableRequest<TResponse>
                                                         where TResponse : AbstractAuthorizableResponse
{

    public bool IsAuthorized(TRequest request, IUserContext userContext, out List<Func<TRequest, IUserContext, bool>> failedCustomChecks, out List<string> failedPermissions);
}
