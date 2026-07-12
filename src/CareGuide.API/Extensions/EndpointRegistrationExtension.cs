using CareGuide.API.Endpoints;
using CareGuide.API.Endpoints.Shared;

namespace CareGuide.API.Extensions
{
    public static class EndpointRegistrationExtension
    {
        public static void MapAllEndpoints(this IEndpointRouteBuilder endpoints)
        {
            var endpointDefinitions = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(IEndpoint).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                .Where(type => type != typeof(HealthCheckEndpoints))
                .Select(type => Activator.CreateInstance(type) as IEndpoint)
                .Where(endpoint => endpoint is not null);

            foreach (var endpoint in endpointDefinitions)
            {
                endpoint!.RegisterEndpoints(endpoints);
            }
        }

        public static RouteGroupBuilder WithDefaultProblemResponses(this RouteGroupBuilder group)
        {
            group.ProducesProblem(StatusCodes.Status500InternalServerError);
            group.ProducesProblem(StatusCodes.Status401Unauthorized);
            return group;
        }
    }
}
