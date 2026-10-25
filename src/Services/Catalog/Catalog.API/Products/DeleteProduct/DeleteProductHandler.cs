namespace Catalog.API.Products.DeleteProduct
{
   public record class DeleteProductCommand(string Id) : ICommand<DeleteProductResult>;

    public class DeleteProductCommandValidator
    : AbstractValidator<DeleteProductCommand>
    {
        public DeleteProductCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();
        }
    }

    public record DeleteProductResult(bool IsSuccess);


    internal class DeleteProductCommandHandler(IDocumentSession session) : ICommandHandler<DeleteProductCommand, DeleteProductResult>
    {
        public async Task<DeleteProductResult> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
        {

            var product = await session.LoadAsync<Product>(command.Id, cancellationToken);
            if (product == null)
            {
                throw new ProductNotFoundException(command.Id);
            }
            session.Delete<Product>(command.Id);
            await session.SaveChangesAsync(cancellationToken);
            return new DeleteProductResult(true);
        }
    }

}
