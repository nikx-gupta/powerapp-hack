using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChangeTracking.Clients;

public interface IChangeTrackingClient<T>
{
    Task<List<T>> GetAllRecords();
    Task<List<T>> GetDeltaChanges();
}