using System;

namespace ChangeTracking.Entities
{
    public class SqlTable : ChangeTrackingAttribute
    {
    }

    public class DataverseTable : ChangeTrackingAttribute
    {
    }

    public class SqlChangeTrackingModifiedDate : ChangeTrackingAttribute
    {
    }

    public class ChangeTrackingAttribute : Attribute
    {
        public string Name { get; set; }
    }
}