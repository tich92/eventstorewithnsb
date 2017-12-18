using System;
using System.Collections.Generic;
using System.Data.Entity;
using Moq;

namespace OrderProcessor.Tests
{
    public class MockedDbContext<T> : Mock<T> where T : DbContext
    {
        private Dictionary<string, object> tables;

        public Dictionary<string, object> Tables => tables ?? (tables = new Dictionary<string, object>());
    }
}