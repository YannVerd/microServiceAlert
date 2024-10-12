using alert_service.Models;
using Microsoft.AspNetCore.Mvc;

namespace alert_service.Repositories
{
    public interface INotifyRepository
    {
        Task<Notify?> GetOneById(int id);
        Task<IEnumerable<Notify>> GetAll();
        Task Add(Notify notify);
        Task Update(Notify notify);
        Task Delete(int id);
    }
}