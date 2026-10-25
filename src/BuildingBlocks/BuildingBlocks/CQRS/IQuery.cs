using MediatR;

namespace BuildingBlocks.CQRS;
//تعریف قرارداد Query و نوع پاسخ
public interface IQuery<out TResponse> : IRequest<TResponse> where TResponse : notnull
{
}

