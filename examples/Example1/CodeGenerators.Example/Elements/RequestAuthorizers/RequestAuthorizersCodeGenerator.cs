using CodeGenerators.CodeAnalysis.Extensions;
using CodeGenerators.Elements;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeGenerators.Example.Generator.Elements.RequestAuthorizers;
public sealed class RequestAuthorizersCodeGenerator
{
    public readonly string _applicationProjectFilePath;
    public readonly string _contractsProjectFilePath;
    public readonly string _authorizersSaveDirectoryPath;

    private readonly ProjectScanner _applicationScanner;
    private readonly ProjectScanner _contractsScanner;

    private string _codeTemplate;

    private string _requestAuthorizerInterfaceNamespace;
    private string _userContextInterfaceNamespace;

    public RequestAuthorizersCodeGenerator(string applicationProjectFilePath, string contractsProjectFilePath, string authorizersSaveDirectoryPath)
    {
        _applicationProjectFilePath = applicationProjectFilePath;
        _contractsProjectFilePath = contractsProjectFilePath;
        _authorizersSaveDirectoryPath = authorizersSaveDirectoryPath;

        _applicationScanner = new ProjectScanner();
        _contractsScanner = new ProjectScanner();
    }

    public async Task Run()
    {
        await LoadProjects();
        await LoadCodeTemplate();

        InitializeBaseNamespaces();

        if (Directory.Exists(_authorizersSaveDirectoryPath) == false)
            Directory.CreateDirectory(_authorizersSaveDirectoryPath);

        foreach (ClassDeclarationSyntax requestClass in GetRequestClasses())
        {
            RequestAuthorizerCode authorizerCode = BuildAuthorizer(requestClass);
            string saveFilePath = authorizerCode.SaveFilePath;

            if (File.Exists(saveFilePath) == true)
                File.Delete(saveFilePath);

            await File.WriteAllTextAsync(path: saveFilePath,
                                         contents: authorizerCode.Code);
        }
    }

    private async Task LoadProjects()
    {
        await _applicationScanner.LoadProject(_applicationProjectFilePath);
        await _contractsScanner.LoadProject(_contractsProjectFilePath);
    }

    private async Task LoadCodeTemplate()
    {
        _codeTemplate = await File.ReadAllTextAsync(@"..\..\..\Resources\CodeTemplates\RequestAuthorizerCodeTemplate.cs");
    }

    private void InitializeBaseNamespaces()
    {
        _requestAuthorizerInterfaceNamespace = _applicationScanner.GetGeneralNodeNamespaceByName("IRequestAuthorizer")?.Name.ToString() ?? throw new NullReferenceException();
        _userContextInterfaceNamespace = _applicationScanner.GetGeneralNodeNamespaceByName("IUserContext")?.Name.ToString() ?? throw new NullReferenceException();
    }

    private List<ClassDeclarationSyntax> GetRequestClasses()
    {
        List<ClassDeclarationSyntax>? result;

        if (_contractsScanner.TryGetImplementsByName("AbstractAuthorizableRequest", out result) == false || result == null)
            throw new NullReferenceException(nameof(result));

        return result;
    }

    private RequestAuthorizerCode BuildAuthorizer(ClassDeclarationSyntax requestClass)
    {
        TypeSyntax? baseType = requestClass.GetBaseTypeByName("AbstractAuthorizableRequest");

        if (baseType == null)
            throw new NullReferenceException(nameof(baseType));

        TypeSyntax responseType = ((GenericNameSyntax)baseType).TypeArgumentList.Arguments[0];
        ClassDeclarationSyntax? responseClass = _contractsScanner.GetMemberDeclarationSyntaxByType<ClassDeclarationSyntax>(responseType); // Крч надо из requestClass достать generic аргумент с типом респонса

        if (responseClass == null)
            throw new NullReferenceException(nameof(responseClass));

        string rootNamespace = _applicationScanner.RootNamespace;
        string authorizerName = requestClass.Identifier.Text + "Authorizer";
        string requestTypeNamespace = requestClass.GetNamespace()?.Name.ToString() ?? throw new NullReferenceException();
        string responseTypeNamespace = responseClass.GetNamespace()?.Name.ToString() ?? throw new NullReferenceException();
        string targetNamespace = $"{rootNamespace}.Generated.Authorizers";

        RequestAuthorizerCodeBuilder builder = new RequestAuthorizerCodeBuilder(saveFilePath: GenerateAuthorizerSaveFilePath(authorizerName),
                                                                                codeTemplate: _codeTemplate,
                                                                                requestAuthorizerInterfaceNamespace: _requestAuthorizerInterfaceNamespace,
                                                                                userContextInterfaceNamespace: _userContextInterfaceNamespace,
                                                                                requestTypeNamespace: requestTypeNamespace,
                                                                                responseTypeNamespace: responseTypeNamespace,
                                                                                targetNamespace: targetNamespace,
                                                                                className: authorizerName,
                                                                                requestTypeName: requestClass.Identifier.Text,
                                                                                responseTypeName: responseClass.Identifier.Text,
                                                                                requiredPermissions: GenerateRequiredPermissions(requestClass));

        return builder.Build();
    }

    private string GenerateAuthorizerSaveFilePath(string authorizerName) => Path.Combine(_authorizersSaveDirectoryPath, $"{authorizerName}.g.cs");

    private List<string> GenerateRequiredPermissions(ClassDeclarationSyntax requestClass)
    {
        List<string> result = new List<string>();

        foreach (PropertyDeclarationSyntax property in requestClass.DescendantNodes()
                                                                  .OfType<PropertyDeclarationSyntax>())
        {
            foreach (AttributeSyntax attribute in property.AttributeLists
                                                         .SelectMany(list => list.Attributes)
                                                         .Where(a => a.Name.GetName() == "RequiresPermission"))
            {
                if (attribute.ArgumentList == null)
                    throw new NullReferenceException(nameof(attribute.ArgumentList));

                AttributeArgumentSyntax argument = attribute.ArgumentList.Arguments[0];
                SyntaxToken argumentToken = ((LiteralExpressionSyntax)argument.Expression).Token;

                if (argumentToken.Kind() != SyntaxKind.StringLiteralToken)
                    throw new NullReferenceException();

                string requiredPermission = argumentToken.ValueText;

                result.Add(requiredPermission);
            }
        }

        return result;
    }
}