using alert_service.DTOs;
using alert_service.Models;
using alert_service.Repositories;
using Microsoft.EntityFrameworkCore;


namespace alert_service.Services
{
    public interface IEventService
    {
        public Task CreateAndSubscribeAlert(SubscribeDto subscribeDto);
        public Task UnsubscribeAlert(UnsubscribeDto unsubscribeDto);
        public Task<IEnumerable<ReturnUserDto>> AlertUpdated(int idAlert);
    }

    public class EventService : IEventService
    {
        private readonly DBContext _context;

        public EventService(DBContext context)
        {
            _context = context;
        }

        public async Task CreateAndSubscribeAlert(SubscribeDto subscribeDto)
        {
            EntityDetails? data = await _context.EntityDetails.FirstOrDefaultAsync(e => e.IdDMEntity == subscribeDto.IdDMEntity);
            if (data == null)
            {
                EntityDetails entityDetails = new EntityDetails() { IdDMEntity = subscribeDto.IdDMEntity, Name = subscribeDto.Name, Type = subscribeDto.Type };
                _context.EntityDetails.Add(entityDetails);
                await _context.SaveChangesAsync();
                int idED = entityDetails.IdEntityDetails;
                Alert alert = new Alert() { IdEntityDetails = idED };
                _context.Alerts.Add(alert);
                await _context.SaveChangesAsync();
                int idA = alert.IdAlert;
                _context.Notifys.Add(new Notify { IdAlert = idA, IdDMUser = subscribeDto.IdDMUser ?? default(string) });
                await _context.SaveChangesAsync();
            }
            else
            {
                Alert? alert = await _context.Alerts.FirstOrDefaultAsync(a => a.IdEntityDetails == data.IdEntityDetails);
                if (alert != null)
                {
                    _context.Notifys.Add(new Notify { IdAlert = alert.IdAlert, IdDMUser = subscribeDto.IdDMUser ?? default(string) });
                    await _context.SaveChangesAsync();
                }
                else
                {
                    Alert newAlert = new Alert { IdEntityDetails = data.IdEntityDetails };
                    _context.Alerts.Add(newAlert);
                    await _context.SaveChangesAsync();
                    _context.Notifys.Add(new Notify { IdAlert = newAlert.IdAlert, IdDMUser = subscribeDto.IdDMUser ?? default(string) });
                    await _context.SaveChangesAsync();

                }

            }
        }

        public async Task UnsubscribeAlert(UnsubscribeDto unsubscribeDto)
        {
            Notify? notify = await _context.Notifys.FirstOrDefaultAsync(a => a.IdDMUser == unsubscribeDto.IdDMUser);
            if (notify != null)
            {
                _context.Notifys.Remove(notify);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ReturnUserDto>> AlertUpdated(int idDMEntity)
        {
            List<ReturnUserDto> listDto = [];
            EntityDetails? entityDetails = await _context.EntityDetails.FirstOrDefaultAsync(e => e.IdEntityDetails == idDMEntity);
            if (entityDetails != null)
            {
                Alert? alert = await _context.Alerts.FirstOrDefaultAsync(a => a.IdEntityDetails == entityDetails.IdEntityDetails);
                if (alert != null)
                {
                    List<Notify>? list = await _context.Notifys.Where(n => n.IdAlert == alert.IdAlert).ToListAsync();
                    if (list != null)
                    {
                        foreach (Notify notify in list)
                        {
                            listDto.Add(new ReturnUserDto
                            {
                                IdUser = notify.IdDMUser
                            });
                        }
                    }

                }
            }
            return listDto;

        }
    }
}