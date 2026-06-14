Implement a State Machine source generator in VisionaryCoder.Tooling.



Project:

\- Vc.Generators.Domain (or Vc.Generators.Design if you prefer)



Requirements:

\- Attribute: \[StateMachine] and \[State] (define in Vc.Tooling.Abstractions)

\- Input:

&#x20; - Partial classes annotated with \[StateMachine]

&#x20; - Nested or related types annotated with \[State]

\- Output:

&#x20; - Strongly typed state enum

&#x20; - Transition methods (e.g., CanTransition, TryTransition)

&#x20; - Optional async handlers for transitions

&#x20; - Generated under Vc.Generated.Domain (or .Design)

Use Incremental generator + TypeMetadata + IndentedStringBuilder. Emit diagnostics for invalid transitions. Return only the .cs file contents.



