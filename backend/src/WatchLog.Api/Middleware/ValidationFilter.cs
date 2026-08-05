using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WatchLog.Api.Middleware;

/// <summary>
/// Runs any registered FluentValidation `IValidator&lt;T&gt;` against every matched action argument
/// before the action executes, short-circuiting with a 400 + field error map on failure. Applied
/// globally (see `Program.cs`) so controllers never call validators by hand.
/// </summary>
public class ValidationFilter(IServiceProvider serviceProvider) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null) continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
            if (serviceProvider.GetService(validatorType) is not IValidator validator) continue;

            var validationContext = new ValidationContext<object>(argument);
            var result = await validator.ValidateAsync(validationContext);
            if (result.IsValid) continue;

            var errors = result.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            context.Result = new BadRequestObjectResult(new ValidationProblemDetails(errors));
            return;
        }

        await next();
    }
}
