using System.Collections.Generic;

namespace ChangeTracking.Clients
{
    public interface IOutputWriter
    {
        void Write<T>(T objData);
        void WriteBatch<T>(List<T> data);
        void Flush();
    }
}