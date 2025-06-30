namespace EventOn.BusinessLogic.Helpers;

public class Result
{
    private ErrorSeverity _errorSeverity = ErrorSeverity.None;
    public bool HasErrors => ErrorMessages.Count != 0;
    public List<string> ErrorMessages = [];
    public ErrorSeverity ErrorSeverity
    {
        get => _errorSeverity;
        private set => _errorSeverity = (value > _errorSeverity) ? value : _errorSeverity;
    }

    public void AddError(string message, ErrorSeverity severity = ErrorSeverity.Error)
    {
        ErrorMessages.Add(message);
        ErrorSeverity = severity;
    }

    public override string ToString()
    {
        return string.Join(Environment.NewLine, ErrorMessages);
    }

    public void Concat(Result res)
    {
        ErrorSeverity = res.ErrorSeverity;
        ErrorMessages.AddRange(res.ErrorMessages);
    }
}

public class Result<T> : Result
{
    public T Data { get; set; }

    public Result(T data)
    {
        Data = data;
    }
}
