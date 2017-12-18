using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Moq;

namespace OrderProcessor.Tests
{
    public static class EntityFrameworkMockHelper
    {
        public static MockedDbContext<T> GetMockedDbContext<T>() where T : DbContext
        {
            var instance = new MockedDbContext<T>();
            instance.MockTables();
            return instance;
        }

        public static void MockTables<T>(this MockedDbContext<T> mockedContext) where T : DbContext
        {
            Type contextType = typeof(T);

            var dbSetProperties = contextType.GetProperties().Where(prop =>
                prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));

            foreach (var prop in dbSetProperties)
            {
                var dbSetGenericType = prop.PropertyType.GetGenericArguments()[0];
                Type listType = typeof(List<>).MakeGenericType(dbSetGenericType);

                var listForFakeTable = Activator.CreateInstance(listType);
                var parameter = Expression.Parameter(contextType);
                var body = Expression.PropertyOrField(parameter, prop.Name);
                var lambdaExpression = Expression.Lambda<Func<T, object>>(body, parameter);
                var method = typeof(EntityFrameworkMockHelper).GetMethod("MockDbSet")
                    ?.MakeGenericMethod(dbSetGenericType);

                mockedContext.Setup(lambdaExpression).Returns(method.Invoke(null, new[] {listForFakeTable}));
                mockedContext.Tables.Add(prop.Name, listForFakeTable);
            }
        }

        public static DbSet<T> MockDbSet<T>(List<T> table)
            where T : class
        {
            var dbSet = new Mock<DbSet<T>>();

            dbSet.As<IQueryable<T>>().Setup(q => q.Provider).Returns(() => table.AsQueryable().Provider);
            dbSet.As<IQueryable<T>>().Setup(q => q.Expression).Returns(() => table.AsQueryable().Expression);
            dbSet.As<IQueryable<T>>().Setup(q => q.ElementType).Returns(() => table.AsQueryable().ElementType);
            dbSet.As<IQueryable<T>>().Setup(q => q.GetEnumerator()).Returns(() => table.AsQueryable().GetEnumerator());

            dbSet.Setup(set => set.Add(It.IsAny<T>())).Callback<T>(table.Add);
            dbSet.Setup(set => set.AddRange(It.IsAny<IEnumerable<T>>())).Callback<IEnumerable<T>>(table.AddRange);
            dbSet.Setup(set => set.Remove(It.IsAny<T>())).Callback<T>(t => table.Remove(t));
            dbSet.Setup(set => set.RemoveRange(It.IsAny<IEnumerable<T>>())).Callback<IEnumerable<T>>(ts =>
            {
                foreach (var t in ts)
                {
                    table.Remove(t);
                }
            });

            return dbSet.Object;
        }
    }
}
