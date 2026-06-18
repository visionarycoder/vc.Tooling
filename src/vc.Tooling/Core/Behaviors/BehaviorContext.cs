namespace VisionaryCoder.Tooling.Core.Behaviors
{
    public sealed class BehaviorContext(
        string operationName,
        object? request,
        IDictionary<string, object>? metadata = null)
    {
        /// <summary>
        /// The name of the tooling operation being executed.
        /// </summary>
        public string OperationName { get; } = operationName;

        /// <summary>
        /// The request payload for the operation (may be null).
        /// </summary>
        public object? Request { get; } = request;

        /// <summary>
        /// Arbitrary metadata for behaviors to read/write.
        /// </summary>
        public IDictionary<string, object> Metadata { get; } = metadata ?? new Dictionary<string, object>();

        /// <summary>
        /// Adds or updates a metadata value.
        /// </summary>
        public void Set(string key, object value)
        {
            Metadata[key: key] = value;
        }

        /// <summary>
        /// Attempts to retrieve a metadata value.
        /// </summary>
        public bool TryGet<T>(string key, out T? value)
        {
            if (Metadata.TryGetValue(key: key, value: out var raw) && raw is T typed)
            {
                value = typed;
                return true;
            }

            value = default;
            return false;
        }
    }
}
