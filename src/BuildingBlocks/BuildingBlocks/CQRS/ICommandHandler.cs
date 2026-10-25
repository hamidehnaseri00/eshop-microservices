using MediatR;

namespace BuildingBlocks.CQRS;

    public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, TResponse> where TCommand : ICommand<TResponse>
    where TResponse : notnull
    {
    }


    public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Unit> where TCommand : IRequest<Unit>
     {

      }

