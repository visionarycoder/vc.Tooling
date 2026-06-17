using FullImplementation.Scenarios;
using VisionaryCoder.Architecture;
using VisionaryCoder.Architecture.Vbd;

// Mark the entry assembly with VBD boundary metadata
[assembly: Boundary(BoundaryType.Tooling)]

Console.WriteLine("VisionaryCoder.Tooling — Full Implementation Sample");
Console.WriteLine("====================================================");
Console.WriteLine("Exercises all five source projects: Architecture, Ifx, Runtime, Tooling, Utility");
Console.WriteLine();

await ScenarioA.RunAsync();   // Happy path
await ScenarioB.RunAsync();   // Edge condition
await ScenarioC.RunAsync();   // Invalid input
await ScenarioD.RunAsync();   // Integration failure

Console.WriteLine("====================================================");
Console.WriteLine("All scenarios completed.");
