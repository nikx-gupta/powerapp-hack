using System;

namespace ChangeTracking.Entities
{
    public interface ISqlTrackingEntity
    {
        public DateTime LastModifiedDate { get; }
    }
}