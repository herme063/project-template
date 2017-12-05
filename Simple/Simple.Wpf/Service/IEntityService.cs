using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Simple.Wpf.Model;

namespace Simple.Wpf.Service
{
    public interface IEntityService
    {
        Task<List<Entity>> AllAsync();
        Task<Entity> SaveAsync(Entity entity);
        Task DeleteAsync(int entityId);
        Task<bool> NameDuplicateAsync(int entityId, string entityName);
    }
}
