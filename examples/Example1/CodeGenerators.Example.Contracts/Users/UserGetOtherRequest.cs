using CodeGenerators.Example.Contracts.Common.Abstracts;
using CodeGenerators.Example.Contracts.Common.Attributes;

namespace CodeGenerators.Example.Contracts.Users;

[RequiresPermission("User.Get.Other.Request")]
public sealed class UserGetOtherRequest : AbstractAuthorizableRequest<UserGetOtherResponse>
{
    [RequiresPermission("User.Get.Other.Request.UserId")]
    public Guid UserId { get; set; }
}
