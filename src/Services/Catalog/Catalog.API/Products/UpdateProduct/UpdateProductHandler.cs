using JasperFx.Events.Daemon;

namespace Catalog.API.Products.UpdateProduct
{
    public record UpdateProductCommand(
    string Id,
    string Name,
    List<string> Category,
    string Description,
    string ImageFile,
    decimal Price
) : ICommand<UpdateProductResult>;
    public class UpdateProductCommandValidator: AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.Id)
        .NotEmpty();

            RuleFor(x => x.Name)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(100);

            RuleFor(x => x.Price)
                .GreaterThan(0);
        }
    }
    public record UpdateProductResult(bool IsSuccess);


    internal class UpdateProductCommandHandler(IDocumentSession session) : ICommandHandler<UpdateProductCommand, UpdateProductResult>
    {
        public async Task<UpdateProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            var product =await session.LoadAsync<Product>(command.Id, cancellationToken);
            if (product==null)
            {
                throw new ProductNotFoundException(command.Id);
            }
            product.Name = command.Name;
            product.Category = command.Category;
            product.Description = command.Description;
            product.ImageFile = command.ImageFile;
            product.Price = command.Price;
            session.Update(product);
            await session.SaveChangesAsync(cancellationToken);
            return new UpdateProductResult(true);
        }
    }
}
