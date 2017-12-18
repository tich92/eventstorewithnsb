using EventStore.ClientAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace EventStoreContext.Helpers
{
    public static class ParsingHelper
    {
        private static Type GetTypeByFullName(string fullName)
        {
            return GetReferencedAssemblies().SelectMany(t => t.GetTypes()).First(t => t.FullName == fullName);
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

        public static object ParseEvent(this RecordedEvent @event)
        {
            if(@event == null)
                throw new ArgumentNullException(nameof(@event));

            var data = Encoding.UTF8.GetString(@event.Data);

            return JsonConvert.DeserializeObject(data, GetTypeByFullName(@event.EventType));
        }
    }
}