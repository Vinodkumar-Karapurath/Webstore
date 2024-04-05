using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;

namespace Webstore.Models
{
    public class CircuitServicesAccessor
    {
        static readonly AsyncLocal<IServiceProvider> blazorServices = new();

        public IServiceProvider? Services
        {
            get => blazorServices.Value;
            set => blazorServices.Value = value;
        }
    }

    public class ServicesAccessorCircuitHandler : CircuitHandler
    {
        readonly IServiceProvider services;
        readonly CircuitServicesAccessor circuitServicesAccessor;

        public ServicesAccessorCircuitHandler(IServiceProvider services,
            CircuitServicesAccessor servicesAccessor)
        {
            this.services = services;
            this.circuitServicesAccessor = servicesAccessor;
        }

        public override Func<CircuitInboundActivityContext, Task> CreateInboundActivityHandler(
            Func<CircuitInboundActivityContext, Task> next)
        {
            return async context =>
            {
                circuitServicesAccessor.Services = services;
                await next(context);
                circuitServicesAccessor.Services = null;
            };
        }
    }

    public static class CircuitServicesServiceCollectionExtensions
    {
        public static IServiceCollection AddCircuitServicesAccessor(
            this IServiceCollection services)
        {
            services.AddScoped<CircuitServicesAccessor>();
            services.AddScoped<CircuitHandler, ServicesAccessorCircuitHandler>();

            return services;
        }
    }

    public class AuthenticationStateHandler : DelegatingHandler
    {
        readonly CircuitServicesAccessor circuitServicesAccessor;

        public AuthenticationStateHandler(
            CircuitServicesAccessor circuitServicesAccessor)
        {
            this.circuitServicesAccessor = circuitServicesAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authStateProvider = circuitServicesAccessor.Services
                .GetRequiredService<AuthenticationStateProvider>();
            var authState = await authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity is not null && user.Identity.IsAuthenticated)
            {
                request.Headers.Add("X-USER-IDENTITY-NAME", user.Identity.Name);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
