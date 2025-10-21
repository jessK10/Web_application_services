namespace project_1.Common.Exceptions
{
    /// <summary>
    /// Represents a validation error with a dictionary of field => errors[].
    /// Throw this from a service if you manually validate a DTO.
    /// </summary>
    public class ValidationException : Exception
    {
        public IDictionary<string, string[]> Errors { get; }

        public ValidationException(IDictionary<string, string[]> errors)
            : base("Validation failed")
        {
            Errors = errors;
        }
    }
}