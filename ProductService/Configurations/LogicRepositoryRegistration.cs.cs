public static class LogicRepositoryRegistration
{
    public static void RegisterLogicAndRepository(this IServiceCollection services)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        var types = assemblies
            .SelectMany(s => s.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Logic") || t.Name.EndsWith("Repository"));

        foreach (var type in types)
        {
            var interfaceType = type.GetInterface($"I{type.Name}");
            if (interfaceType != null)
            {
                services.AddScoped(interfaceType, type);
            }
        }
    }
}
