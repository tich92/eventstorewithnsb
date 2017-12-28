using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace EventProcessor.Caching
{
    public class TypeProvider
    {
        public IEnumerable<MethodInfo> Methods { get; set; }

        public TypeProvider()
        {
            Methods = new List<MethodInfo>();
        }

        public IEnumerable<Type> GetReferencedTypes(Func<Type, bool> predicate)
        {
            var assemblies = GetReferencedAssemblies();

            return assemblies.SelectMany(o => o.GetTypes()).Where(o =>
                o.Namespace != null).Where(predicate);
        }

        private static IEnumerable<Assembly> GetReferencedAssemblies()
        {
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var loadedPaths = loadedAssemblies.Where(a => !a.IsDynamic).Select(a => a.Location).ToArray();

            var referencedPaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
            var toLoad = referencedPaths.Where(r => !loadedPaths.Contains(r, StringComparer.InvariantCultureIgnoreCase)).ToList();

            loadedAssemblies.AddRange(toLoad.Select(path => AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(path))));

            return loadedAssemblies;
        }
    }
}
