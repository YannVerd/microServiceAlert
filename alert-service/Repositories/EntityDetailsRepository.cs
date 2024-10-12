using alert_service.Models;
using Microsoft.EntityFrameworkCore;


namespace alert_service.Repositories
{
    public class EntityDetailsRepository: IEntityDetailsRepository
    {
        private readonly DBContext _context;
        public EntityDetailsRepository(DBContext context)
        {
            _context = context;
        }
        

        public async Task Add(EntityDetails entityDetails)
        {
            Console.WriteLine("Dans le repo :" + entityDetails);
            _context.EntityDetails.Add(entityDetails);
            await _context.SaveChangesAsync();
           
        }

        public async Task Delete(int id)
        {
            var entityDetails = await _context.EntityDetails.FirstOrDefaultAsync(a => a.IdEntityDetails == id);
            if(entityDetails != null){
                _context.EntityDetails.Remove(entityDetails);
                await _context.SaveChangesAsync();
            }
        }
             

        public async Task Update(EntityDetails entityDetails)
        {   
                _context.EntityDetails.Update(entityDetails);
                await _context.SaveChangesAsync();
        
        }

        public async Task<IEnumerable<EntityDetails>> GetAll()
        {
            return await _context.EntityDetails.Include(e=>e.Alert).ToListAsync();
 
            
        }

        public async Task<EntityDetails?> GetOneById(int id)
        {
            return await _context.EntityDetails.Include(e=>e.Alert).FirstOrDefaultAsync(m => m.IdEntityDetails == id);
        }
    }
}