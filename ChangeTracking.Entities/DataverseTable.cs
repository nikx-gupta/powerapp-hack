using System;

namespace ChangeTracking.Entities
{
    public class DataverseTable : Attribute
    {
        public string Name { get; set; }
    }

    public class ChangeTrackingModifiedDate: Attribute
    {
        public string Name { get; set; }
    }
}