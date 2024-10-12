using alert_service.Models;
using Microsoft.EntityFrameworkCore;


namespace alert_service.Repositories
{
    public class NotifyRepository: INotifyRepository
    {
        private readonly DBContext _context;
        public NotifyRepository(DBContext context)
        {
            _context = context;
        }
        

        public async Task Add(Notify notify)
        {
            Console.WriteLine("Dans le repo :" + notify);
            _context.Notifys.Add(notify);
            await _context.SaveChangesAsync();
           
        }

        public async Task Delete(int id)
        {
            var notify = await _context.Notifys.FirstOrDefaultAsync(a => a.IdNotify == id);
            if(notify != null){
                _context.Notifys.Remove(notify);
                await _context.SaveChangesAsync();
            }
        }
             

        public async Task Update(Notify notify)
        {   
                _context.Notifys.Update(notify);
                await _context.SaveChangesAsync();
        
        }

        public async Task<IEnumerable<Notify>> GetAll()
        {
            return await _context.Notifys.Include(a => a.Alert).ToListAsync();
 
            
        }

        public async Task<Notify?> GetOneById(int id)
        {
            return await _context.Notifys.Include(a => a.Alert).FirstOrDefaultAsync(m => m.IdNotify == id);
        }
    }
}
