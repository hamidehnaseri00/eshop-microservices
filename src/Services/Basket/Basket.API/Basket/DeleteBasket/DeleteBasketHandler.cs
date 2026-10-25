using FluentValidation;

namespace Basket.API.Basket.DeleteBasket
{

    public record DeleteBasketCommand(string Username) : ICommand<DeleteBasketResult>;

    public record DeleteBasketResult(bool IsSuccess);
    public class DeleteBasketCommandValidator
    : AbstractValidator<DeleteBasketCommand>
    {
        public DeleteBasketCommandValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("username is required");
        }
    }
    public class DeleteBaskeCommandHandler(IBasketRepository repository) : ICommandHandler<DeleteBasketCommand, DeleteBasketResult>
    {
        public async Task<DeleteBasketResult> Handle(DeleteBasketCommand command, CancellationToken cancellationToken)
        {
            var basket =await repository.DeleteBasketAsync(command.Username);
            return new DeleteBasketResult(basket);

        }
    }
}
