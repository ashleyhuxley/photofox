using System;

namespace PhotoFox.Core.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string entityType, string entityId)
        {
            this.EntityType = entityType;
            this.EntityId = entityId;
        }

        public string EntityType { get; }
        public string EntityId { get; }
    }
}
