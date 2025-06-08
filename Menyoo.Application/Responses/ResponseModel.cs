namespace Menyoo.Application.Responses
{
    //public partial class AuthResponse : AuthResult
    //public partial class ResponseModel
    //{
    //    public dynamic? Data { get; set; }

    //    public int? Result { get; set; }
    //    public List<ValidationError>? Errors { get; set; }
    //    public string? Message { get; set; }
    //}

    public class ResponseModel<T>
    {
        public T? Data { get; set; }
        public int Result { get; set; } = 1;
        public List<ValidationError>? Errors { get; set; }
        public string? Message { get; set; }

        public static ResponseModel<T> Success(T data, string? message = null) =>
            new() { Data = data, Result = 1, Message = message };

        public static ResponseModel<T> Fail(List<ValidationError> errors, string? message = null) =>
            new() { Result = 0, Errors = errors, Message = message };
    }

    public partial class ValidationError
    {
        public string? Field { get; set; }
        public string? Message { get; set; }

        public ValidationError(string field, string message)
        {
            Field = field != string.Empty ? field : null;
            Message = message;
        }

    }
}
