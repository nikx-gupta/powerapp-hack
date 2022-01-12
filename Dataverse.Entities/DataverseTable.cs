using System;

namespace Dataverse.Entities
{
    public class DataverseTable : Attribute
    {
        public string Name { get; set; }
    }
}