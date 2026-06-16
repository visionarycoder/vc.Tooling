[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ConfigurationAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "VC0001";
    private static readonly LocalizableString Title = "Missing configuration";
    private static readonly LocalizableString MessageFormat = "The {0} is missing required configuration.";
    private static readonly LocalizableString Description = "Certain types require specific configuration to function correctly. This diagnostic indicates that such configuration is missing.";
    private const string Category = "Configuration";

    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
        DiagnosticId,
        Title,
        MessageFormat,
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
    }   
}

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class MissingConfigurationAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "VC0002";
    private static readonly LocalizableString Title = "Missing configuration";
    private static readonly LocalizableString MessageFormat = "The {0} is missing required configuration.";
    private static readonly LocalizableString Description = "Certain types require specific configuration to function correctly. This diagnostic indicates that such configuration is missing.";
    private const string Category = "Configuration";

    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
        DiagnosticId,
        Title,
        MessageFormat,
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
    }   
}

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DependencyInjectionAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "VC0003";
    private static readonly LocalizableString Title = "Missing dependency injection configuration";
    private static readonly LocalizableString MessageFormat = "The {0} is missing required dependency injection configuration.";
    private static readonly LocalizableString Description = "Certain types require specific dependency injection configuration to function correctly. This diagnostic indicates that such configuration is missing.";
    private const string Category = "DependencyInjection";

    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
        DiagnosticId,
        Title,
        MessageFormat,
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
    }   
}

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class SerializationAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "VC0004";
    private static readonly LocalizableString Title = "Missing serialization configuration";
    private static readonly LocalizableString MessageFormat = "The {0} is missing required serialization configuration.";
    private static readonly LocalizableString Description = "Certain types require specific serialization configuration to function correctly. This diagnostic indicates that such configuration is missing.";
    private const string Category = "Serialization";

    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
        DiagnosticId,
        Title,
        MessageFormat,
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
    }   
}

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ReflectionAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "VC0005";
    private static readonly LocalizableString Title = "Missing reflection configuration";
    private static readonly LocalizableString MessageFormat = "The {0} is missing required reflection configuration.";
    private static readonly LocalizableString Description = "Certain types require specific reflection configuration to function correctly. This diagnostic indicates that such configuration is missing.";
    private const string Category = "Reflection";

    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
        DiagnosticId,
        Title,
        MessageFormat,
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
    }   
}

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class InteropAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "VC0006";
    private static readonly LocalizableString Title = "Missing interop configuration";
    private static readonly LocalizableString MessageFormat = "The {0} is missing required interop configuration.";
    private static readonly LocalizableString Description = "Certain types require specific interop configuration to function correctly. This diagnostic indicates that such configuration is missing.";
    private const string Category = "Interop";

    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
        DiagnosticId,
        Title,
        MessageFormat,
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
    }   
}