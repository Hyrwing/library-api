using FluentValidation;

namespace LibraryApi.Api.Infrastructure;

public class ValidationFilter<T> : IEndpointFilter where T : class
{
    private readonly IValidator<T> _validator;
    private readonly ILogger<ValidationFilter<T>> _logger;

    public ValidationFilter(IValidator<T> validator, ILogger<ValidationFilter<T>> logger)
    {
        _validator = validator;
        _logger = logger;
    }

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var argument = context.Arguments.OfType<T>().FirstOrDefault();
        if (argument is null)
            return TypedResults.BadRequest("Invalid request body.");

        var result = await _validator.ValidateAsync(argument);
        if (!result.IsValid)
        {
            _logger.LogWarning("Validation failed for {Type}: {Errors}",
                typeof(T).Name, string.Join("; ", result.Errors.Select(e => e.ErrorMessage)));

            var errors = result.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            return TypedResults.ValidationProblem(errors);
        }

        return await next(context);
    }
}
