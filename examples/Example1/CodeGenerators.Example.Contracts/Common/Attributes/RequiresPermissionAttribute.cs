namespace CodeGenerators.Example.Contracts.Common.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
public sealed class RequiresPermissionAttribute : Attribute
{
    public string Permission { get; init; }

    public RequiresPermissionAttribute(string permission)
    {
        if (string.IsNullOrWhiteSpace(permission))
            throw new ArgumentException("Permission must be non-empty.", nameof(permission));

        Permission = permission;
    }
}