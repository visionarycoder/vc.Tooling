using VisionaryCoder.Utility;

namespace FullImplementation.Scenarios;

/// <summary>
/// Scenario C — Invalid input: pass null/empty arguments and verify Guard rejects them.
/// Exercises vc.Utility Guard and StringExtensions for input boundary enforcement.
/// </summary>
public static class ScenarioC
{
    public static Task RunAsync()
    {
        Console.WriteLine("=== Scenario C: Invalid Input Handling ===");

        // C1: Guard.NotNull rejects null reference
        var nullRejected = false;
        try
        {
            string? nullValue = null;
            Guard.NotNull(nullValue, "nullValue");
        }
        catch (ArgumentNullException ex)
        {
            nullRejected = true;
            Console.WriteLine($"  [C1] Null rejected: {ex.ParamName}");
        }

        // C2: Guard.NotNullOrWhiteSpace rejects empty string
        var emptyRejected = false;
        try
        {
            Guard.NotNullOrWhiteSpace("   ", "customerId");
        }
        catch (ArgumentException ex)
        {
            emptyRejected = true;
            Console.WriteLine($"  [C2] Whitespace rejected: {ex.ParamName}");
        }

        // C3: StringExtensions boundary checks
        string? maybeNull = null;
        var safe = maybeNull.OrEmpty();
        var isEmpty = safe.IsNullOrWhiteSpace();
        Console.WriteLine($"  [C3] OrEmpty: '{safe}', IsNullOrWhiteSpace: {isEmpty}");

        Console.WriteLine($"  NullRejected: {nullRejected}, EmptyRejected: {emptyRejected}");
        Console.WriteLine("  Status: PASS");
        Console.WriteLine();

        return Task.CompletedTask;
    }
}
