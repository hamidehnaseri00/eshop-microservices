using BuildingBlocks.CQRS;
using FluentValidation;
using MediatR;

namespace BuildingBlocks.Behaviors
{//ValidationBehavior
    public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : ICommand<TResponse>
    {
        //تمام Validatorهای مربوط به نوع درخواست را دریافت کنید:
        //IEnumerable<IValidator<TRequest>> validators
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
               validators.Select(v => v.ValidateAsync(context, cancellationToken))
             );

            var failures = validationResults
                   .SelectMany(result => result.Errors)
                   .Where(error => error != null)
                   .ToList();
            if (failures.Any())
            {
                throw new ValidationException(failures);
            }
            //این مرحله فقط در صورت عبور موفق درخواست از اعتبارسنجی اجرا می‌شود
            return await next();
        }
    }
}
