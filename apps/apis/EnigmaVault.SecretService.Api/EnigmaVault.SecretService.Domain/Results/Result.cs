namespace EnigmaVault.SecretService.Domain.Results
{
    public class Result<TValue> : Result
    {
        public TValue? Value { get; }

        private Result(TValue? value, bool isSuccess, List<Error> errors) : base(isSuccess, errors)
        {
            Value = value;
        }

        public static Result<TValue> Success(TValue value) => new Result<TValue>(value, true, new List<Error>());

        public new static Result<TValue> Failure(Error error) => new Result<TValue>(default, false, new List<Error> { error });

        public new static Result<TValue> Failure(IEnumerable<Error> errors) => new Result<TValue>(default, false, errors.ToList());
    }

    public class Result
    {
        public bool IsSuccess { get; }

        private readonly List<Error> _errors;
        public IReadOnlyList<Error> Errors => _errors.AsReadOnly();

        protected Result(bool isSuccess, List<Error> errors)
        {
            IsSuccess = isSuccess;
            _errors = errors;
        }

        public static Result Success() => new Result(true, new List<Error>());

        public static Result Failure(Error error) => new Result(false, new List<Error> { error });

        public static Result Failure(IEnumerable<Error> errors) => new Result(false, errors.ToList());
    }
}