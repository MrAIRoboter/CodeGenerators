using CodeGenerators.CodeAnalysis.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;

namespace CodeGenerators.Elements;
public sealed class ProjectScanner
{
    public string RootNamespace { get; private set; }
    public List<CompilationUnitSyntax>? CompilationUnits { get; private set; }

    private MSBuildWorkspace _workspace;

    public ProjectScanner()
    {
        CompilationUnits = null;

        _workspace = MSBuildWorkspace.Create();
    }

    public async Task LoadProject(string projectFilePath)
    {
        CompilationUnits = new List<CompilationUnitSyntax>();

        Project project = await _workspace.OpenProjectAsync(projectFilePath);
        RootNamespace = project.Name;

        foreach (Document document in project.Documents)
        {
            SyntaxTree? syntaxTree = await document.GetSyntaxTreeAsync();

            if (syntaxTree == null)
                continue;

            CompilationUnitSyntax root = (CompilationUnitSyntax)await syntaxTree.GetRootAsync();

            CompilationUnits.Add(root);
        }
    }

    public BaseNamespaceDeclarationSyntax? GetGeneralNodeNamespaceByName(string generalNodeName)
    {
        if (CompilationUnits == null)
            throw new NullReferenceException(nameof(CompilationUnits));

        foreach (SyntaxNode interfaceDeclaration in GetGeneralSyntaxNodesByName(generalNodeName))
            if (interfaceDeclaration.Parent is BaseNamespaceDeclarationSyntax namespaceDeclaration)
                return namespaceDeclaration;

        return null;
    }

    public bool TryGetImplementsByName(string baseGeneralNodeName, out List<ClassDeclarationSyntax>? result)
    {
        if (CompilationUnits == null)
            throw new NullReferenceException(nameof(CompilationUnits));

        result = null;
        SyntaxNode? baseNode = GetGeneralSyntaxNodesByName(baseGeneralNodeName).FirstOrDefault();

        if (baseNode == null)
            return false;

        result = new List<ClassDeclarationSyntax>();

        foreach (ClassDeclarationSyntax node in CompilationUnits.SelectMany(cu => cu.DescendantNodes())
                                                                .OfType<ClassDeclarationSyntax>())
        {
            if (node.BaseList == null)
                continue;

            if (node.IsImplementsTypeByName(baseGeneralNodeName))
                result.Add(node);
        }

        return true;
    }

    public TMemberDeclarationSyntax? GetMemberDeclarationSyntaxByType<TMemberDeclarationSyntax>(TypeSyntax type) where TMemberDeclarationSyntax : MemberDeclarationSyntax
    {
        if (CompilationUnits == null)
            throw new NullReferenceException(nameof(CompilationUnits));

        foreach (CompilationUnitSyntax compilationUnit in CompilationUnits)
        {
            TMemberDeclarationSyntax? result = compilationUnit.DescendantNodes()
                                                              .OfType<TMemberDeclarationSyntax>()
                                                              .FirstOrDefault(m => m.GetName() == type.GetName());

            if (result != null)
                return result;
        }

        return default;
    }

    private List<SyntaxNode> GetGeneralSyntaxNodesByName(string generalNodeName)
    {
        if (CompilationUnits == null)
            throw new NullReferenceException(nameof(CompilationUnits));

        List<SyntaxNode> result = new List<SyntaxNode>();

        foreach (CompilationUnitSyntax compilationUnit in CompilationUnits)
            result.AddRange(compilationUnit.DescendantNodes()
                                           .Where(i => i.GetName() == generalNodeName));

        return result;
    }
}
