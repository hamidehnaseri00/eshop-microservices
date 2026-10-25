using Catalog.API.Products.GetProduct;

namespace Catalog.API.Products.GetProductById
{
   
    public record GetProductByIdResponse(Product Product);

    public class GetProductByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {

            app.MapGet("/products/{id}", async (Guid
                 id,ISender sender) =>
            {
                var result=await sender.Send(new GetProductByIdQuery(id));
                var response=result.Adapt<GetProductByIdResponse>();
                return Results.Ok(response);
            }
                )
                .WithName("GetProductById")
                .Produces<GetProductsResponse>(StatusCodes.Status200OK)
                .WithSummary("Get Product by id")
                .WithDescription("Get Products by Id")
                .ProducesProblem(StatusCodes.Status400BadRequest);
        }
    }
}
