using CSharpFunctionalExtensions;
using LearningTracker.Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LearningTracker.Application.Configuration.Pipelines;

internal class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse> where TResponse : IResult 
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

    public TransactionBehavior(IUnitOfWork unitOfWork, ILogger<TransactionBehavior<TRequest, TResponse>> logger) 
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct) 
    {
        if (_unitOfWork.TransactionOpened) 
        {
            var response = await next();
            if (response.IsSuccess) 
            {
                await _unitOfWork.FlushChanges(ct);
            }

            return response;
        }
        await _unitOfWork.BeginTransaction(ct);
        try 
        {
            var response = await next();
            if (response.IsSuccess) 
            {
                await _unitOfWork.FlushChanges(ct);
                await _unitOfWork.CommitAsync(CancellationToken.None);
            }
            else 
            {
                await _unitOfWork.RollbackAsync(CancellationToken.None);
            }

            return response;
        }
        catch (TaskCanceledException e) 
        {
            _logger.LogWarning(e,"Operation was cancelled by the client");
            throw ;
        }
        catch (OperationCanceledException e) 
        {
            _logger.LogWarning(e,"Operation was cancelled by the client");
            throw;
        }
        catch (Exception e) 
        {
            _logger.LogCritical(e, "Error");
            throw ;
        }
    }
}