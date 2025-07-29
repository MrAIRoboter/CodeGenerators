using CodeGenerators.Example.Generator.Elements.RequestAuthorizers;

public class Program
{
    public static async Task Main(string[] args)
    {
        RequestAuthorizersCodeGenerator generator = new RequestAuthorizersCodeGenerator(applicationProjectFilePath: @"..\..\..\..\CodeGenerators.Example.Application\CodeGenerators.Example.Application.csproj",
                                                                                        contractsProjectFilePath: @"..\..\..\..\CodeGenerators.Example.Contracts\CodeGenerators.Example.Contracts.csproj",
                                                                                        authorizersSaveDirectoryPath: @"..\..\..\..\CodeGenerators.Example.Application\Generated\Authorizers");
        await generator.Run();

        // Потихоньку пишем BuildAuthorizer?
    }
}