using System;

namespace Contracts
{
    /// <summary>
    /// Base command class
    /// </summary>
    public abstract class BaseCommand
    {
        public DateTime CreatedDate { get; set; }
    }
}