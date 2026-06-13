namespace Vc.Analyzers.Design
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class ExceptionSafetyAnalyzer : DiagnosticAnalyzer
    {
        public const string EmptyCatchId = "VCEX001";
        public const string SwallowedExceptionId = "VCEX002";
        public const string GeneralCatchId = "VCEX003";
        public const string AsyncVoidId = "VCEX004";

        private static readonly DiagnosticDescriptor EmptyCatchRule =
            new DiagnosticDescriptor(
                EmptyCatchId,
                "Empty catch block",
                "Catch block does not handle or rethrow the exception",
                "ExceptionSafety",
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor SwallowedExceptionRule =
            new DiagnosticDescriptor(
                SwallowedExceptionId,
                "Exception is swallowed",
                "Exception is caught but not logged, wrapped, or rethrown",
                "ExceptionSafety",
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor GeneralCatchRule =
            new DiagnosticDescriptor(
                GeneralCatchId,
                "Overly general catch",
                "Catching System.Exception or without specifying an exception type is discouraged",
                "ExceptionSafety",
                DiagnosticSeverity.Info,
                isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor AsyncVoidRule =
            new DiagnosticDescriptor(
                AsyncVoidId,
                "Avoid async void",
                "Async void methods should be avoided except for event handlers",
                "ExceptionSafety",
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(EmptyCatchRule, SwallowedExceptionRule, GeneralCatchRule, AsyncVoidRule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeCatchClause, SyntaxKind.CatchClause);
            context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
        }

        private static void AnalyzeCatchClause(SyntaxNodeAnalysisContext context)
        {
            var catchClause = (CatchClauseSyntax)context.Node;

            // General catch (no declaration or System.Exception)
            if (catchClause.Declaration is null ||
                IsSystemException(context.SemanticModel, catchClause.Declaration, context.CancellationToken))
            {
                context.ReportDiagnostic(Diagnostic.Create(GeneralCatchRule, catchClause.CatchKeyword.GetLocation()));
            }

            // Empty catch block
            if (catchClause.Block is { Statements.Count: 0 })
            {
                context.ReportDiagnostic(Diagnostic.Create(EmptyCatchRule, catchClause.Block.GetLocation()));
                return;
            }

            // Swallowed exception: no throw, no logging, no wrapping
            if (!ContainsThrow(catchClause.Block) && !ContainsLogging(context.SemanticModel, catchClause.Block, context.CancellationToken))
            {
                context.ReportDiagnostic(Diagnostic.Create(SwallowedExceptionRule, catchClause.CatchKeyword.GetLocation()));
            }
        }

        private static bool IsSystemException(SemanticModel semanticModel, CatchDeclarationSyntax declaration, System.Threading.CancellationToken cancellationToken)
        {
            var type = semanticModel.GetTypeInfo(declaration.Type, cancellationToken).Type;
            if (type is null)
                return false;

            return type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == "global::System.Exception";
        }

        private static bool ContainsThrow(BlockSyntax block)
        {
            return block.DescendantNodes().OfType<ThrowStatementSyntax>().Any();
        }

        private static bool ContainsLogging(SemanticModel semanticModel, BlockSyntax block, System.Threading.CancellationToken cancellationToken)
        {
            foreach (var invocation in block.DescendantNodes().OfType<InvocationExpressionSyntax>())
            {
                var symbol = semanticModel.GetSymbolInfo(invocation, cancellationToken).Symbol as IMethodSymbol;
                if (symbol is null)
                    continue;

                var name = symbol.Name;
                if (name.Contains("Log", StringComparison.OrdinalIgnoreCase) ||
                    name.Contains("Trace", StringComparison.OrdinalIgnoreCase) ||
                    name.Contains("Error", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var method = (MethodDeclarationSyntax)context.Node;

            if (!method.Modifiers.Any(SyntaxKind.AsyncKeyword))
                return;

            if (method.ReturnType is PredefinedTypeSyntax pts &&
                pts.Keyword.IsKind(SyntaxKind.VoidKeyword))
            {
                // Allow event handlers: (object sender, EventArgs e) or derived
                if (IsEventHandlerSignature(context.SemanticModel, method, context.CancellationToken))
                    return;

                context.ReportDiagnostic(Diagnostic.Create(AsyncVoidRule, method.Identifier.GetLocation()));
            }
        }

        private static bool IsEventHandlerSignature(SemanticModel semanticModel, MethodDeclarationSyntax method, System.Threading.CancellationToken cancellationToken)
        {
            var parameters = method.ParameterList.Parameters;
            if (parameters.Count != 2)
                return false;

            var firstType = semanticModel.GetTypeInfo(parameters[0].Type!, cancellationToken).Type;
            var secondType = semanticModel.GetTypeInfo(parameters[1].Type!, cancellationToken).Type;

            if (firstType is null || secondType is null)
                return false;

            var isObject = firstType.SpecialType == SpecialType.System_Object;
            var isEventArgs = InheritsFromEventArgs(secondType);

            return isObject && isEventArgs;

            static bool InheritsFromEventArgs(ITypeSymbol type)
            {
                while (type is not null)
                {
                    if (type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == "global::System.EventArgs")
                        return true;

                    type = type.BaseType!;
                }

                return false;
            }
        }
    }
}
