using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeGenerators.CodeAnalysis.Extensions;
public static class SyntaxExtensions
{
    public static BaseNamespaceDeclarationSyntax? GetNamespace(this CompilationUnitSyntax root) => root.GetNamespaces()
                                                                                                       .FirstOrDefault();

    public static BaseNamespaceDeclarationSyntax? GetNamespace(this MemberDeclarationSyntax source)
    {
        if (source is BaseNamespaceDeclarationSyntax result)
            return result;

        if (source.Parent == null)
            return null;

        if (source.Parent is MemberDeclarationSyntax parent)
            return GetNamespace(parent);

        return null;
    }

    public static IEnumerable<BaseNamespaceDeclarationSyntax> GetNamespaces(this CompilationUnitSyntax root) => root.DescendantNodes()
                                                                                                                    .OfType<BaseNamespaceDeclarationSyntax>();

    public static ClassDeclarationSyntax? GetClass(this BaseNamespaceDeclarationSyntax ns) => ns.DescendantNodes()
                                                                                               .OfType<ClassDeclarationSyntax>()
                                                                                               .FirstOrDefault();

    public static TypeSyntax? GetBaseType(this ClassDeclarationSyntax classDeclaration) => classDeclaration.BaseList?
                                                                                                           .Types
                                                                                                           .FirstOrDefault()?
                                                                                                           .Type;

    public static TypeSyntax? GetBaseTypeByName(this ClassDeclarationSyntax classDeclaration, string baseTypeName) => classDeclaration.BaseList?
                                                                                                                                      .Types
                                                                                                                                      .FirstOrDefault(t => t.GetName() == baseTypeName)?
                                                                                                                                      .Type;

    public static AttributeSyntax? GetAttributeByName(this ClassDeclarationSyntax classDeclaration, string name) => classDeclaration.AttributeLists
                                                                                                                                    .GetAttributeByName(name);

    public static AttributeSyntax? GetAttributeByName(this PropertyDeclarationSyntax property, string name) => property.AttributeLists
                                                                                                                       .GetAttributeByName(name);

    public static AttributeSyntax? GetAttributeByName(this SyntaxList<AttributeListSyntax> attributeLists, string name) => attributeLists.SelectMany(attributesList => attributesList.Attributes)
                                                                                                                                         .FirstOrDefault(attribute => attribute.Name.ToString().Equals(name) == true);

    public static TValue? GetValue<TValue>(this AttributeArgumentSyntax attributeArgument)
    {
        object? objectedValue = ((LiteralExpressionSyntax)attributeArgument.Expression).Token.Value;

        if (objectedValue is null)
            return default;

        if (objectedValue is TValue value)
            return value;

        throw new InvalidCastException(nameof(TValue));
    }

    public static IEnumerable<PropertyDeclarationSyntax> GetProperties(this ClassDeclarationSyntax classDeclaration) => classDeclaration.Members
                                                                                                                                        .OfType<PropertyDeclarationSyntax>();

    public static string? GetName(this SyntaxNode node)
    {
        switch (node)
        {
            case ClassDeclarationSyntax classDeclaration:
                return classDeclaration.Identifier.Text;

            case InterfaceDeclarationSyntax interfaceDeclaration:
                return interfaceDeclaration.Identifier.Text;

            case StructDeclarationSyntax structDeclaration:
                return structDeclaration.Identifier.Text;

            case EnumDeclarationSyntax enumDeclaration:
                return enumDeclaration.Identifier.Text;

            case MethodDeclarationSyntax methodDeclaration:
                return methodDeclaration.Identifier.Text;

            case PropertyDeclarationSyntax propertyDeclaration:
                return propertyDeclaration.Identifier.Text;

            case FieldDeclarationSyntax fieldDeclaration:
                return fieldDeclaration.Declaration.Variables.First().Identifier.Text;

            case BaseTypeSyntax baseType:
                return GetName(baseType.Type);

            default:
                return null;
        }
    }

    public static string GetName(this TypeSyntax type)
    {
        if (type is IdentifierNameSyntax identifierName)
            return identifierName.Identifier.Text;

        if (type is GenericNameSyntax genericName)
            return genericName.Identifier.Text;

        if (type is QualifiedNameSyntax qualifiedName)
            return GetName(qualifiedName.Right);

        return type.ToString();
    }

    public static bool IsImplementsTypeByName(this ClassDeclarationSyntax source, string typeNameForImplement)
    {
        if (source.BaseList == null)
            return false;

        IEnumerable<TypeSyntax> baseList = source.BaseList.Types.Select(bt => bt.Type);

        foreach (TypeSyntax baseType in baseList)
            if (typeNameForImplement == baseType.GetName())
                return true;

        return false;
    }
}
