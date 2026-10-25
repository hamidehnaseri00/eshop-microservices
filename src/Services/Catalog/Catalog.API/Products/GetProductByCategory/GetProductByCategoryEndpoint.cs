using Catalog.API.Products.GetProduct;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Catalog.API.Products.GetProductByCategory
{

    public record GetProductByCategoryResponse(IEnumerable<Product> Products);

    public class GetProductByCategoryEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/products/category/{categoryName}", async(string categoryName,ISender sender) =>
            {
                var result =await sender.Send(new GetProductByCategoryQuery(categoryName));

                var response = result.Adapt<GetProductByCategoryResponse>();

                return Results.Ok(response);
            })
                .WithName("GetProductByCategory")
                .Produces<GetProductByCategoryResponse>(StatusCodes.Status200OK)
                .WithSummary("Get Product By Category")
                .WithDescription("Get Product By Category")
                .ProducesProblem(StatusCodes.Status400BadRequest);

            ;
        }
    }
}
