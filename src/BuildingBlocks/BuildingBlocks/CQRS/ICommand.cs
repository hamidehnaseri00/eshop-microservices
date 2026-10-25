using MediatR;

namespace BuildingBlocks.CQRS;
//تعریف قرارداد Command
public interface ICommand<out TResponse> : IRequest<TResponse>
{
}

public interface ICommand:IRequest<Unit>
{

}

