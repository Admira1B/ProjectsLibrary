using ProjectsLibrary.MinimalAPI.Interfaces;

namespace ProjectsLibrary.MinimalAPI.Extensions {
    public static class EndpointRouteBuilderExtension {
        public static void MapApplicationEndpoints(this IEndpointRouteBuilder app) { 
            var mappers = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(IEndpointsGroup).IsAssignableFrom(type) && type.IsClass && !type.IsInterface && !type.IsAbstract)
                .ToList();

            foreach (var mapper in mappers) {
                try {
                    var endpointInstance = Activator.CreateInstance(mapper) as IEndpointsGroup;

                    endpointInstance?.MapEndpoints(app);
                } catch (Exception) {
                    throw;
                }
            }
        }
    }
}
