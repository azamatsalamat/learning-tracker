using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;

namespace LearningTracker.Application.Configuration.Pipelines;

internal abstract class ValidationBehavior<TRequest, TResponse> : AbstractValidator<TRequest>,
    IPipelineBehavior<TRequest, Result<TResponse>>
    where TRequest : IRequest<Result<TResponse>>
{
    public async Task<Result<TResponse>> Handle(TRequest request, RequestHandlerDelegate<Result<TResponse>> next, CancellationToken ct)
    {
        var fluentValidationResult = await base.ValidateAsync(request, ct);
        if (!fluentValidationResult.IsValid)
        {
            var errorText = string.Join(Environment.NewLine, fluentValidationResult.Errors.Select(x => $"{x.ErrorMessage}"));
            return Result.Failure<TResponse>(errorText);
        }

        var requestValidateResultAsync = await RequestValidateAsync(request, ct);
        if (requestValidateResultAsync.IsFailure)
        {
            return Result.Failure<TResponse>(requestValidateResultAsync.Error);
        }

        return await next();
    }

    protected virtual Task<Result> RequestValidateAsync(TRequest request, CancellationToken ct)
    {
        return Task.FromResult(Result.Success());
    }
}

internal abstract class ValidationBehavior<TRequest> : AbstractValidator<TRequest>,
    IPipelineBehavior<TRequest, Result>
    where TRequest : IRequest<Result>
{
    public async Task<Result> Handle(TRequest request, RequestHandlerDelegate<Result> next, CancellationToken ct)
    {
        var fluentValidationResult = await base.ValidateAsync(request, ct);
        if (!fluentValidationResult.IsValid)
        {
            var errorText = string.Join(Environment.NewLine, fluentValidationResult.Errors.Select(x => $"{x.ErrorMessage}"));
            return Result.Failure(errorText);
        }

        var requestValidateResultAsync = await RequestValidateAsync(request, ct);
        if (requestValidateResultAsync.IsFailure)
        {
            return Result.Failure(requestValidateResultAsync.Error);
        }

        return await next();
    }

    internal virtual Task<Result> RequestValidateAsync(TRequest request, CancellationToken ct)
    {
        return Task.FromResult(Result.Success());
    }
}
