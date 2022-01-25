using System;
using System.Collections.Generic;

namespace ChangeTracking.Clients
{
    public interface IOutputWriter : IDisposable
    {
        void Write<T>(T objData);
        void WriteBatch<T>(List<T> data);
    }
}