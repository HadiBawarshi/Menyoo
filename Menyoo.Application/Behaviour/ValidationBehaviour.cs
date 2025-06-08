using FluentValidation;
using MediatR;
using Menyoo.Application.Responses;

namespace Menyoo.Application.Behaviour
{
    public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);
                var validationResults = await Task.WhenAll(
                    _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

                var failures = validationResults.SelectMany(e => e.Errors).Where(f => f != null).ToList();

                if (failures.Count > 0)
                {
                    var validationErrors = failures
                        .Select(e => new ValidationError(e.PropertyName, e.ErrorMessage))
                        .ToList();

                    var responseType = typeof(TResponse);
                    if (responseType.IsGenericType &&
                        responseType.GetGenericTypeDefinition() == typeof(ResponseModel<>))
                    {
                        var genericArg = responseType.GetGenericArguments().First();
                        var failMethod = typeof(ResponseModel<>)
                            .MakeGenericType(genericArg)
                            .GetMethod(nameof(ResponseModel<object>.Fail), new[] { typeof(List<ValidationError>), typeof(string) });

                        var response = failMethod!.Invoke(null, new object[] { validationErrors, "Validation failed" });
                        return (TResponse)response!;
                    }

                    throw new InvalidOperationException("ValidationBehavior only supports ResponseModel<T> responses.");
                }
            }

            return await next();
        }
    }

}
