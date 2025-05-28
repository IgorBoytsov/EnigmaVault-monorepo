namespace EnigmaVault.Domain.Results
{
    public class Result<TValue>
    {
        public TValue? Value { get; }
        public bool IsSuccess { get; }

        private readonly List<Error> _errors;
        public IReadOnlyList<Error> Errors => _errors.AsReadOnly();

        private Result(TValue? value, bool isSuccess, List<Error> errors)
        {
            Value = value;
            IsSuccess = isSuccess;
            _errors = errors;
        }

        public static Result<TValue> Success(TValue value) => new Result<TValue>(value, true, new List<Error>());

        public static Result<TValue> Failure(Error error) => new Result<TValue>(default, false, new List<Error> { error });

        public static Result<TValue> Failure(IEnumerable<Error> errors) => new Result<TValue>(default, false, errors.ToList());
    }
}