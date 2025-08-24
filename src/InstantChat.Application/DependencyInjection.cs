using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using InstantChat.Application.Common.Behaviors;
using InstantChat.Application.Features.Chat.Commands.SendMessage;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace InstantChat.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<SendMessageCommandValidator>();

        //For global model validation
        //builder.Services.AddControllers(options =>
        //{
        //    options.Filters.Add<ValidationFilter>();
        //});
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}
