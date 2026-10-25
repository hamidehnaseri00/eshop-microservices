

namespace Catalog.API.Products.CreateProduct
{

    public record CreateProductRequest(string Name,List<string> Category,string Descripti,string ImageFile,
    decimal Price);

    public record CreateProductResponse(Guid Id);


    public class CreateProductEndpoint : ICarterModule
    {
        // کدهای مربوط به پیاده‌سازی اینترفیس اینجا قرار می‌گیرد
        public void AddRoutes(IEndpointRouteBuilder app)
        {

            app.MapPost("/products", async (CreateProductRequest request, ISender sender
                ) =>
                {
                    var command = request.Adapt<CreateProductCommand>();
                    var result = await sender.Send(command);

                    var response = result.Adapt<CreateProductResponse>();
                    return Results.Created( $"/products/{response.Id}",response);
                });
        }
    }

}
