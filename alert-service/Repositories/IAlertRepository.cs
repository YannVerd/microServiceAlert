using alert_service.Models;
using Microsoft.AspNetCore.Mvc;

namespace alert_service.Repositories
{
    public interface IAlertRepository
    {
        Task<Alert?> GetOneById(int id);
        Task<IEnumerable<Alert>> GetAll();
        Task Add(Alert alert);
        Task Update(Alert alert);
        Task Delete(int id);
    }
}