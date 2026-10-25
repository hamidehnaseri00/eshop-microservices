using BuildingBlocks.CQRS;
using Catalog.API.Models;


namespace Catalog.API.Products.CreateProduct;

public record CreateProductCommand(
    string Name,
    List<string> Category,
    string Description,
    string ImageFile,
    decimal Price
) : ICommand<CreateProductResult>;

public record CreateProductResult(Guid Id);


//مرکز CommandHandler فقط بر منطق اصلی کسب‌وکار و واگذاری اعتبارسنجی به Pipeline.

internal class CreateProductCommandHandler(IDocumentSession session)
    : ICommandHandler<
        CreateProductCommand,
        CreateProductResult>
{
    public async Task<CreateProductResult> Handle(
        CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = command.Name,
             Category = command.Category,
            Description = command.Description,
            ImageFile = command.ImageFile,
            Price = command.Price
        };

        session.Store(product);

        await session.SaveChangesAsync(cancellationToken);

        return new CreateProductResult(product.Id);
    }
}
