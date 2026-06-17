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
    public async Task ExceptionSafetyAnalyzer_ShouldReportDiagnostic_WhenCatchBlockCatchesException()
    {
        var source = """
            namespace Sample.Design
            {
                using System;

                public sealed class ErrorHandler
                {
                    public void Handle()
                    {
                        try
                        {
                            var x = 1 / int.Parse("0");
                        }
                        catch (Exception ex)
                        {
                            // Broad catch
                        }
                    }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.Contains(diagnostics, diagnostic => diagnostic.Id == DiagnosticIds.DesignExceptionSafetyBroadCatch);
    }

    [Fact]
    public async Task ExceptionSafetyAnalyzer_ShouldNotReportDiagnostic_WhenCatchBlockCatchesSpecificException()
    {
        var source = """
            namespace Sample.Design
            {
                using System;

                public sealed class ErrorHandler
                {
                    public void Handle()
                    {
                        try
                        {
                            var x = 1 / int.Parse("0");
                        }
                        catch (FormatException ex)
                        {
                            // Specific catch
                        }
                    }
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.DoesNotContain(diagnostics, diagnostic => diagnostic.Id == DiagnosticIds.DesignExceptionSafetyBroadCatch);
    }
}
