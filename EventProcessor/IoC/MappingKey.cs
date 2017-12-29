using System;

namespace EventProcessor.IoC
{
    internal class MappingKey
    {
        public Type Type { get; protected set; }
        public string InstanceName { get; protected set; }

        public MappingKey(Type type, string instanceName)
        {
            Type = type;
            InstanceName = instanceName;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                const int multiplayer = 31;
                int hash = GetType().GetHashCode();

                hash = hash * multiplayer + Type.GetHashCode();
                hash = hash * multiplayer + (InstanceName == null ? 0 : InstanceName.GetHashCode());

                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            MappingKey compareTo = obj as MappingKey;

            if (ReferenceEquals(this, compareTo))
                return true;

            if (compareTo == null)
                return false;

            return Type.Equals(compareTo.Type) && string.Equals(InstanceName, compareTo.InstanceName,
                       StringComparison.InvariantCultureIgnoreCase);
        }

        public override string ToString()
        {
            const string format = "Instance name: {0} ({1})";

            return string.Format(format, InstanceName ?? "[null]", Type.FullName);
        }
    }
}