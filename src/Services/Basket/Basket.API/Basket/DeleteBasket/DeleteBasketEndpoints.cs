using Basket.API.Basket.GetBasket;

namespace Basket.API.Basket.DeleteBasket
{
    public record DeleteBasketRequest(string Username);
    public record DeleteBasketResponse(bool IsSuccess);

    public class DeleteBasketEndpoints : ICarterModule
    {

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/basket/{username}", async(string userName,ISender sender) =>
            {
                //var command = request.Adapt<DeleteBasketRequest>();

                var result = await sender.Send(new DeleteBasketCommand(userName));

                var response = result.Adapt<DeleteBasketResponse>();

                return Results.Ok(response);

             }).WithName("DeleteBasket")
                .Produces<DeleteBasketResponse>(StatusCodes.Status200OK)
                .WithSummary("Delete Basket")
                .WithDescription("Delete Basket")
                .ProducesProblem(StatusCodes.Status400BadRequest);

        }
    }
}
