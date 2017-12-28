using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OrderProcessor.Tests
{
    [TestClass]
    public class ExpressionTest
    {
        [TestMethod]
        public void TestWithTiming()
        {
            var watch = new Stopwatch();

            var calculator = new Calculator();

            MethodInfo methodInfo = typeof(Calculator).GetMethod(nameof(Calculator.Calculate), new[] { typeof(int) });

            if (methodInfo == null)
                throw new InvalidOperationException();

            ParameterExpression param = Expression.Parameter(typeof(int), "i");

            var thisParameter = Expression.Constant(calculator);

            MethodCallExpression call = Expression.Call(thisParameter, methodInfo, param);

            Expression<Func<int, string>> lambda =
                Expression.Lambda<Func<int, string>>(call, param);

            Func<int, string> func = lambda.Compile();
            
            watch.Start();

            for (int i = 0; i < 1000000; i++)
            {
                string result = func(i);
            }

            watch.Stop();

            var timeWithExp = watch.ElapsedMilliseconds;

            watch.Reset();
            watch.Start();

            for (int i = 0; i < 1000000; i++)
            {
                string result = calculator.Calculate(i);
            }

            watch.Stop();

            var timeWithoutExp = watch.ElapsedMilliseconds;

            Assert.IsTrue(true);
        }
    }

    public class Calculator
    {
        public string Calculate(int i)
        {
            DateTime nowDateTime = DateTime.Now;
            DateTime next = nowDateTime.AddDays(i);

            var result = next.ToString();

            return result;
        }
    }
}