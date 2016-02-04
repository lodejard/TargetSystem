using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.PlatformAbstractions;

namespace TargetSystem
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class Program
    {
        public static int Main(string[] args)
        {
            var serviceProvider = BuildServiceProvider();

            var targetManager = serviceProvider.GetRequiredService<ITargetManager>();
            if (args.Length == 0)
            {
                targetManager.ExecuteByName("Default");
            }
            else
            {
                foreach (var arg in args)
                {
                    targetManager.ExecuteByName(arg);
                }
            }

            return 0;
        }

        public static IServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();

            DiscoverServices(services);

            return services.BuildServiceProvider();
        }

        private static void DiscoverServices(ServiceCollection services)
        {
            foreach (var type in GetAssemblies()
                .SelectMany(a => a.ExportedTypes))
            {
                if (type.GetTypeInfo().IsAbstract)
                {
                    continue;
                }

                foreach (var itf in type.GetInterfaces())
                {
                    if (itf.GetInterfaces().Contains(typeof(ITargetService)))
                    {
                        services.AddScoped(itf, type);
                    }

                    if (itf == typeof(ITargetService))
                    {
                        services.AddScoped(type);
                    }
                }
            }
        }

        public static IEnumerable<Assembly> GetAssemblies()
        {
            try
            {
                return DnxPlatformServices.Default.LibraryManager
                    .GetLibraries()
                    .SelectMany(l => l.Assemblies)
                    .Select(Assembly.Load)
                    .OrderBy(a => a, new AssemblyComparer());
            }
            catch
            {
                return DependencyContext.Default
                    .RuntimeLibraries
                    .SelectMany(l => l.Assemblies)
                    .Select(ra => ra.Name)
                    .Select(Assembly.Load)
                    .OrderBy(a => a, new AssemblyComparer());
            }
        }

        internal class AssemblyComparer : IComparer<Assembly>
        {
            public int Compare(Assembly x, Assembly y)
            {
                var xName = x.GetName();
                var yName = y.GetName();
                return string.Compare(yName.Name, xName.Name);
            }
        }
    }
}
