using alert_service.Models;
using Microsoft.EntityFrameworkCore;


namespace alert_service.Repositories
{
    public class AlertRepository: IAlertRepository
    {
        private readonly DBContext _context;
        public AlertRepository(DBContext context)
        {
            _context = context;
        }
        

        public async Task Add(Alert alert)
        {
            Console.WriteLine("Dans le repo :" +alert);
            _context.Alerts.Add(alert);
            await _context.SaveChangesAsync();
           
        }

        public async Task Delete(int id)
        {
            var alert = await _context.Alerts.FirstOrDefaultAsync(a => a.IdAlert == id);
            if(alert != null){
                _context.Alerts.Remove(alert);
                await _context.SaveChangesAsync();
            }
        }
             

        public async Task Update(Alert alert)
        {   
                _context.Alerts.Update(alert);
                await _context.SaveChangesAsync();
        
        }

        public async Task<IEnumerable<Alert>> GetAll()
        {
            return await _context.Alerts.Include(e=>e.Notifys).Include(e=>e.EntityDetails).ToListAsync();
 
            
        }

        public async Task<Alert?> GetOneById(int id)
        {
            return await _context.Alerts.Include(e=>e.Notifys).Include(e=>e.EntityDetails).FirstOrDefaultAsync(m => m.IdAlert == id);
        }
    }
}