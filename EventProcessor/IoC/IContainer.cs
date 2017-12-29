using System;

namespace EventProcessor.IoC
{
    /// <summary>
    /// IoC container
    /// </summary>
    public interface IContainer
    {
        /// <summary>
        /// Register a type mapping
        /// </summary>
        /// <param name="from">Type that will be requested</param>
        /// <param name="to">Type that will actually be returned</param>
        /// <param name="instanceName">Instance name (optional)</param>
        void Register(Type from, Type to, string instanceName = null);

        /// <summary>
        /// Register a type mapping
        /// </summary>
        /// <typeparam name="TFrom">Type that will be requested</typeparam>
        /// <typeparam name="TTo">Type that will actually be returned</typeparam>
        /// <param name="instanceName">Instance name (optional)</param>
        void Register<TFrom, TTo>(string instanceName = null);

        /// <summary>
        /// Register a type mapping
        /// </summary>
        /// <param name="type">Type that will be requested</param>
        /// <param name="createInstanceDelegate">A delegate that will be used to
        /// create an instance of the requested object</param>
        /// <param name="instanceName">Instance name (optional)</param>
        void Register(Type type, Func<object> createInstanceDelegate, string instanceName = null);

        /// <summary>
        /// Register a type mapping
        /// </summary>
        /// <typeparam name="T">Type that will be requested</typeparam>
        /// <param name="createInstanceDelegate">A delegate that will be used to
        /// create an instance of the requested object</param>
        /// <param name="instanceName">Instance name (optional)</param>
        void Register<T>(Func<T> createInstanceDelegate, string instanceName = null);

        /// <summary>
        /// Check if a particular type/instance name has been registered with the container
        /// </summary>
        /// <param name="type">Type to check registration for</param>
        /// <param name="instanceName">Instance name (optional)</param>
        /// <returns><c>true</c>if the type/instance name has been registered
        /// with the container; otherwise <c>false</c></returns>
        bool IsRegistered(Type type, string instanceName = null);

        /// <summary>
        /// Check if a particular type/instance name has been registered with the container
        /// </summary>
        /// <typeparam name="T">Type to check registration for</typeparam>
        /// <param name="instanceName">Instance name (optional)</param>
        /// <returns><c>true</c>if the type/instance name has been registered
        /// with the container; otherwise <c>false</c></returns>
        bool IsRegistered<T>(string instanceName = null);

        /// <summary>
        /// Resolve an instance of the requested type from the container.
        /// </summary>
        /// <param name="type">Requested type</param>
        /// <param name="instanceName">Instance name (optional)</param>
        /// <returns>The retrieved object</returns>
        object Resolve(Type type, string instanceName = null);

        /// <summary>
        /// Resolve an instance of the requested type from the container.
        /// </summary>
        /// <typeparam name="T">Requested type</typeparam>
        /// <param name="instanceName">Instance name (optional)</param>
        /// <returns>The retrieved object</returns>
        T Resolve<T>(string instanceName = null);
    }
}