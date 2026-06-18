using FullImplementation.Scenarios;
using VisionaryCoder.Architecture;
using VisionaryCoder.Architecture.Vbd;

// Mark the entry assembly with VBD boundary metadata
[assembly: Boundary(boundaryType: BoundaryType.Tooling)]

Console.WriteLine(value: "VisionaryCoder.Tooling — Full Implementation Sample");
Console.WriteLine(value: "====================================================");
Console.WriteLine(value: "Exercises all five source projects: Architecture, Ifx, Runtime, Tooling, Utility");
Console.WriteLine();

await ScenarioA.RunAsync();   // Happy path
await ScenarioB.RunAsync();   // Edge condition
await ScenarioC.RunAsync();   // Invalid input
await ScenarioD.RunAsync();   // Integration failure

Console.WriteLine(value: "====================================================");
Console.WriteLine(value: "All scenarios completed.");
