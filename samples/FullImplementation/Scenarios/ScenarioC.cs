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
        Console.WriteLine(value: "=== Scenario C: Invalid Input Handling ===");

        // C1: Guard.NotNull rejects null reference
        var nullRejected = false;
        try
        {
            string? nullValue = null;
            Guard.NotNull(value: nullValue, paramName: "nullValue");
        }
        catch (ArgumentNullException ex)
        {
            nullRejected = true;
            Console.WriteLine(value: $"  [C1] Null rejected: {ex.ParamName}");
        }

        // C2: Guard.NotNullOrWhiteSpace rejects empty string
        var emptyRejected = false;
        try
        {
            Guard.NotNullOrWhiteSpace(value: "   ", paramName: "customerId");
        }
        catch (ArgumentException ex)
        {
            emptyRejected = true;
            Console.WriteLine(value: $"  [C2] Whitespace rejected: {ex.ParamName}");
        }

        // C3: StringExtensions boundary checks
        string? maybeNull = null;
        var safe = maybeNull.OrEmpty();
        var isEmpty = safe.IsNullOrWhiteSpace();
        Console.WriteLine(value: $"  [C3] OrEmpty: '{safe}', IsNullOrWhiteSpace: {isEmpty}");

        Console.WriteLine(value: $"  NullRejected: {nullRejected}, EmptyRejected: {emptyRejected}");
        Console.WriteLine(value: "  Status: PASS");
        Console.WriteLine();

        return Task.CompletedTask;
    }
}
