using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EventProcessor.IoC
{
    /// <summary>
    /// IoC container
    /// </summary>
    public class Container : IContainer
    {
        private readonly Dictionary<MappingKey, Func<object>> mappings;

        public Container()
        {
            mappings = new Dictionary<MappingKey, Func<object>>();
        }

        public bool IsRegistered(Type type, string instanceName = null)
        {
            if(type == null)
                throw new ArgumentNullException(nameof(type));

            var key = new MappingKey(type, instanceName);
            return mappings.ContainsKey(key);
        }

        public bool IsRegistered<T>(string instanceName = null)
        {
            return IsRegistered(typeof(T), instanceName);
        }

        public void Register(Type from, Type to, string instanceName = null)
        {
            if(to == null)
                throw new ArgumentNullException(nameof(to));

            if (!from.IsAssignableFrom(to))
            {
                string errorMessage =
                    $"Error typing to register instance '{from.FullName}' is not assignable to '{to.FullName}'";

                throw new InvalidOperationException(errorMessage);
            }

            Func<object> createInstance;
            
            ConstructorInfo constructor =
                to.GetConstructors().First();
            ParameterInfo[] parameters = constructor.GetParameters();

            if (!parameters.Any())
            {
                createInstance = () => Activator.CreateInstance(to);
            }
            else
            {
                createInstance = () => constructor.Invoke(ResolvedPararameters(parameters));
            }

            Register(from, createInstance, instanceName);
        }

        public void Register<TFrom, TTo>(string instanceName = null)
        {
            Register(typeof(TFrom), typeof(TTo), instanceName);
        }

        public void Register(Type type, Func<object> createInstanceDelegate, string instanceName = null)
        {
            if(type == null)
                throw new ArgumentNullException(nameof(type));

            if(createInstanceDelegate == null)
                throw new ArgumentNullException(nameof(createInstanceDelegate));

            var key = new MappingKey(type, instanceName);

            if (mappings.ContainsKey(key))
            {
                const string errorMessageFormat = "The requested mapping already exists '{0}'";
                throw new InvalidOperationException(errorMessageFormat);
            }

            mappings.Add(key, createInstanceDelegate);
        }

        public void Register<T>(Func<T> createInstanceDelegate, string instanceName = null)
        {
            if (createInstanceDelegate == null)
                throw new ArgumentNullException(nameof(createInstanceDelegate));

            var createInstance = createInstanceDelegate as Func<object>;

            Register(typeof(T), createInstance, instanceName);
        }

        public object Resolve(Type type, string instanceName = null)
        {
            var obj = LookUpDependency(type, instanceName);

            ConstructorInfo constructorInfo = obj.GetConstructors().FirstOrDefault();

            var parameters = constructorInfo?.GetParameters();

            if (parameters == null || !parameters.Any())
            {
                return Activator.CreateInstance(obj);
            }

            return constructorInfo.Invoke(ResolvedPararameters(parameters));
        }

        public T Resolve<T>(string instanceName = null)
        {
            var instance = Resolve(typeof(T), instanceName);
            return (T) instance;
        }

        public override string ToString()
        {
            return mappings == null ? "No mappings" : string.Join(Environment.NewLine, mappings.Keys);
        }

        private Type LookUpDependency(Type type, string instanceName)
        {
            if(type == null)
                throw new ArgumentNullException(nameof(type));

            var key = new MappingKey(type, instanceName);

            if (mappings.TryGetValue(key, out var createInstance))
            {
                var instance = createInstance();
                return instance.GetType();
            }

            const string errorMessageFormat = "Could not find mapping for type '{0}'";
            throw new InvalidOperationException(string.Format(errorMessageFormat, type.FullName));
        }
        
        private object[] ResolvedPararameters(ParameterInfo[] parameters)
        {
            return parameters.Select(p => Resolve(p.ParameterType)).ToArray();
        }

    }
}