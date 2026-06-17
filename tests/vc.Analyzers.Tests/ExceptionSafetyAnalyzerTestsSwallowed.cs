using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Vc.Analyzers.Design;
using VisionaryCoder.Tooling.Analyzers.Common;
using Xunit;

namespace Vc.Analyzers.Tests;

public sealed partial class ExceptionSafetyAnalyzerTests
{
    [Fact]
    public async Task ExceptionSafetyAnalyzer_ShouldReportDiagnostic_WhenCatchSwallowsException()
    {
        var source = """
            namespace Sample.Design
            {
                using System;

                public sealed class ErrorHandler
                {
                    public string SafeParse(string input)
                    {
                        try
                        {
                            return int.Parse(input).ToString();
                        }
                        catch
                        {
                            return "failed";
                        }
                    }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.Contains(diagnostics, diagnostic => diagnostic.Id == DiagnosticIds.DesignExceptionSafetySwallowed);
    }

    [Fact]
    public async Task ExceptionSafetyAnalyzer_ShouldNotReportDiagnostic_WhenExceptionIsRethrown()
    {
        var source = """
            namespace Sample.Design
            {
                using System;

                public sealed class ErrorHandler
                {
                    public string SafeParse(string input)
                    {
                        try
                        {
                            return int.Parse(input).ToString();
                        }
                        catch (Exception ex)
                        {
                            throw new InvalidOperationException("Parse failed", ex);
                        }
                    }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.DoesNotContain(diagnostics, diagnostic => diagnostic.Id == DiagnosticIds.DesignExceptionSafetySwallowed);
    }
}
