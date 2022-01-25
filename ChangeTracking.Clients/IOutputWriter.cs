using System;
using System.Collections.Generic;

namespace ChangeTracking.Clients
{
    public interface IOutputWriter : IDisposable
    {
        void Write<T>(T objData) where T : class;
        void WriteBatch<T>(List<T> data) where T : class;
    }
}