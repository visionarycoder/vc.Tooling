Implement the full Roslyn analyzer for category <CATEGORY> in the VisionaryCoder.Tooling suite.

Requirements:
- Namespace: Vc.Analyzers.<CATEGORY>
- Analyzer class: Vc<CATEGORY>Analyzer
- Use DiagnosticDescriptor categories from VcDiagnosticCategories
- Use deterministic rule IDs: VCA<CATEGORY ABBREV>001
- Implement syntax or semantic analysis appropriate for the category
- Use modern Roslyn APIs
- Return only the .cs file contents
