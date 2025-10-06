using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;

namespace LearningTracker.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (!_validators.Any())
        {
            return await next(ct);
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, ct)));

        var errors = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (errors.Any())
        {
            var errorMessage = string.Join("; ", errors.Select(e => e.ErrorMessage));

            var resultType = typeof(TResponse);
            if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Result<>))
            {
                var valueType = resultType.GetGenericArguments()[0];
                var failureMethod = typeof(Result)
                    .GetMethod(nameof(Result.Failure))!
                    .MakeGenericMethod(valueType);

                return (TResponse)failureMethod.Invoke(null, [errorMessage])!;
            }
            else
            {
                return (TResponse)(object)Result.Failure(errorMessage);
            }
        }

        return await next(ct);
    }
}
