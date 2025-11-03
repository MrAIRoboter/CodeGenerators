using CodeGenerators.Example.Generator.Placeholders.Elements;
using CodeGenerators.Placeholders;
using CodeGenerators.Placeholders.Elements;

namespace CodeGenerators.Example.Generator.Elements.RequestAuthorizers;
public sealed class RequestAuthorizerCodeBuilder
{
    private readonly string _saveFilePath;
    private readonly string _codeTemplate;

    #region %Imports%
    private readonly string _requestAuthorizerInterfaceNamespace;
    private readonly string _userContextInterfaceNamespace;
    private readonly string _requestTypeNamespace;
    private readonly string _responseTypeNamespace;
    #endregion

    private readonly string _targetNamespace;
    private readonly string _className;
    private readonly string _requestTypeName;
    private readonly string _responseTypeName;

    #region %MachineFlags%
    private readonly List<string> _requiredPermissions;
    #endregion

    public RequestAuthorizerCodeBuilder(string saveFilePath, string codeTemplate, string requestAuthorizerInterfaceNamespace, string userContextInterfaceNamespace, string requestTypeNamespace, string responseTypeNamespace, string targetNamespace, string className, string requestTypeName, string responseTypeName, List<string> requiredPermissions)
    {
        _saveFilePath = saveFilePath;
        _codeTemplate = codeTemplate;
        _requestAuthorizerInterfaceNamespace = requestAuthorizerInterfaceNamespace;
        _userContextInterfaceNamespace = userContextInterfaceNamespace;
        _requestTypeNamespace = requestTypeNamespace;
        _responseTypeNamespace = responseTypeNamespace;
        _targetNamespace = targetNamespace;
        _className = className;
        _requestTypeName = requestTypeName;
        _responseTypeName = responseTypeName;
        _requiredPermissions = requiredPermissions;
    }

    public RequestAuthorizerCode Build()
    {
        Imports imports = new Imports(_requestAuthorizerInterfaceNamespace,
                                      _userContextInterfaceNamespace,
                                      _requestTypeNamespace,
                                      _responseTypeNamespace);

        Dictionary<string, string> flagPermissionPairs = GenerateFlagPermissionPairs();
        MachineFlags machineFlags = new MachineFlags(flagPermissionPairs.Keys.ToList());
        BuiltInChecks builtInChecks = new BuiltInChecks(flagPermissionPairs);
        CodePlaceholders placeholders = new CodePlaceholders(_codeTemplate);

        placeholders.Replace("%Imports%", imports)
                    .Replace("%Namespace%", _targetNamespace)
                    .Replace("%ClassName%", _className)
                    .Replace("%RequestType%", _requestTypeName)
                    .Replace("%ResponseType%", _responseTypeName)
                    .Replace("%MachineFlags%", machineFlags)
                    .Replace("%BuiltInChecks%", builtInChecks);

        return new RequestAuthorizerCode(saveFilePath: _saveFilePath,
                                         code: placeholders.Build());
    }

    private Dictionary<string, string> GenerateFlagPermissionPairs()
    {
        Dictionary<string, string> result = new Dictionary<string, string>();

        foreach (string permission in _requiredPermissions)
            result.Add($"DoCheck{permission.Replace(".", "")}", permission);

        return result;
    }
}
