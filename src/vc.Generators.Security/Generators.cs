using Vc.Generators.Abstractions;

namespace Vc.Generators.Security;

public sealed class VcAuthorizationPolicyGenerator : VcGeneratorBase
{
    protected override string TargetAttributeName => "VcAuthorizationPolicy";
    protected override string Category => "Security";
    protected override string Feature => "AuthorizationPolicy";
}

public sealed class VcAuthenticationFlowGenerator : VcGeneratorBase
{
    protected override string TargetAttributeName => "VcAuthenticationFlow";
    protected override string Category => "Security";
    protected override string Feature => "AuthenticationFlow";
}

public sealed class VcSecretAccessWrapperGenerator : VcGeneratorBase
{
    protected override string TargetAttributeName => "VcSecretAccessWrapper";
    protected override string Category => "Security";
    protected override string Feature => "SecretAccessWrapper";
}

public sealed class VcInputSanitizationGenerator : VcGeneratorBase
{
    protected override string TargetAttributeName => "VcInputSanitization";
    protected override string Category => "Security";
    protected override string Feature => "InputSanitization";
}

public sealed class VcJwtValidationGenerator : VcGeneratorBase
{
    protected override string TargetAttributeName => "VcJwtValidation";
    protected override string Category => "Security";
    protected override string Feature => "JwtValidation";
}
