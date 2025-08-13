namespace GPS.CrossCutting
{
    public class Result<T>
    {
        public bool Success { get; private set; }
        public T? Data { get; private set; }
        public List<string> Errors { get; } = new();

        private Result(bool success, T? data, IEnumerable<string>? errors)
        {
            Success = success;
            Data = data;
            if (errors != null) Errors.AddRange(errors);
        }

        public static Result<T> Ok(T data) => new(true, data, null);
        public static Result<T> Fail(params string[] errors) => new(false, default, errors);
        public static Result<T> Fail(IEnumerable<string> errors) => new(false, default, errors);
    }
}
