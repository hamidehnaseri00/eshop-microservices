namespace BuildingBlocks.Behaviors
{
    using MediatR;
    using Microsoft.Extensions.Logging;
    using System.Diagnostics;

    public class LoggingBehavior<TRequest, TResponse>(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull, IRequest<TResponse>
        where TResponse : notnull
    {
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            ////توکن لغو عملیات
            CancellationToken cancellationToken)
        {
            //ثبت نام و اطلاعات درخواست ورودی با LogInformation.
            logger.LogInformation(
                "[Start] Handle request={Request} - Response={Response} -RequestDate={RequestDate}",
                typeof(TRequest).Name, typeof(TResponse).Name,request);

            //شروع زمان‌سنج با Stopwatch.StartNew()
            var timer = Stopwatch.StartNew();
            //next ادامه خط لوله و اجرای Handler.
            var response = await next();
            //توقف زمان‌سنج و محاسبه مدت‌زمان اجرا.
            timer.Stop();
            var timeTaken = timer.Elapsed;
            //اگر مدت‌زمان اجرا بیشتر از ۳۰ ثانیه بود، ثبت هشدار با LogWarning.
            if (timeTaken.Seconds > 3)
            {
                logger.LogWarning(
                    "[PERFORMANCE] the request{Request}took {timeTaken})",
                    typeof(TRequest).Name,
                    timeTaken.Seconds);
            }
            //ثبت پایان پردازش و زمان اجرا
            logger.LogInformation(
                "[END] Handle {Request} with {Response}",
                typeof(TRequest).Name, typeof(TResponse).Name);
            //بازگرداندن پاسخ با return response.
            return response;
        }
    }
}
