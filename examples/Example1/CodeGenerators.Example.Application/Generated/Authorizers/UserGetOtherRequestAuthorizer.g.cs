using System;
using System.Collections.Generic;
using CodeGenerators.Example.Application.Interfaces.Authorization.Authorizers;
using CodeGenerators.Example.Application.Interfaces.ScopedContexts;
using CodeGenerators.Example.Contracts.Users;


namespace CodeGenerators.Example.Application.Generated.Authorizers;

public partial class UserGetOtherRequestAuthorizer : IRequestAuthorizer<UserGetOtherRequest, UserGetOtherResponse>
{
    public bool DoCheckUserGetOtherRequestUserId = true;


    private readonly List<Func<UserGetOtherRequest, IUserContext, bool>> _customChecks = new List<Func<UserGetOtherRequest, IUserContext, bool>>();
    
    public bool IsAuthorized(UserGetOtherRequest request, IUserContext userContext, out List<Func<UserGetOtherRequest, IUserContext, bool>> failedCustomChecks, out List<string> failedPermissions)
    {
        failedCustomChecks = new List<Func<UserGetOtherRequest, IUserContext, bool>>();
        failedPermissions = new List<string>();
        
        if (DoCheckUserGetOtherRequestUserId == true && userContext.HasPermission("User.Get.Other.Request.UserId") == false)
            failedPermissions.Add("User.Get.Other.Request.UserId");

        foreach (Func<UserGetOtherRequest, IUserContext, bool> check in _customChecks)
            if (check.Invoke(request, userContext) == false)
                failedCustomChecks.Add(check);

        if(failedPermissions.Count > 0 || failedCustomChecks.Count > 0)
            return false;

        return true;
    }
}