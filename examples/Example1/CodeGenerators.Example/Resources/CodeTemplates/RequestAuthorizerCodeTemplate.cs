using System;
using System.Collections.Generic;
%Imports%

namespace %Namespace%;

public partial class %ClassName% : IRequestAuthorizer<%RequestType%, %ResponseType%>
{
    %MachineFlags%

    private readonly List<Func<%RequestType%, IUserContext, bool>> _customChecks = new List<Func<%RequestType%, IUserContext, bool>>();
    
    public bool IsAuthorized(%RequestType% request, IUserContext userContext, out List<Func<%RequestType%, IUserContext, bool>> failedCustomChecks, out List<string> failedPermissions)
    {
        failedCustomChecks = new List<Func<%RequestType%, IUserContext, bool>>();
        failedPermissions = new List<string>();
        
        %BuiltInChecks%
        foreach (Func<%RequestType%, IUserContext, bool> check in _customChecks)
            if (check.Invoke(request, userContext) == false)
                failedCustomChecks.Add(check);

        if(failedPermissions.Count > 0 || failedCustomChecks.Count > 0)
            return false;

        return true;
    }
}