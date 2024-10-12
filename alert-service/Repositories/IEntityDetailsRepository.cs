using alert_service.Models;
using Microsoft.AspNetCore.Mvc;

namespace alert_service.Repositories
{
    public interface IEntityDetailsRepository
    {
        Task<EntityDetails?> GetOneById(int id);
        Task<IEnumerable<EntityDetails>> GetAll();
        Task Add(EntityDetails entityDetails);
        Task Update(EntityDetails entityDetails);
        Task Delete(int id);
    }
}