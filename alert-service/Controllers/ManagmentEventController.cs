using Microsoft.AspNetCore.Mvc;
using alert_service.Services;
using alert_service.DTOs;

namespace alert_service.Controllers
{
    [Route("alert/[controller]")]
    [ApiController]
    public class ManagmentEventController(EventService eventService) : ControllerBase
    {
        private readonly EventService _eventService = eventService;


        [HttpPost]
        public async Task<ActionResult> CreateAndSubscribeAlert([FromBody] SubscribeDto subscribeDto)
        {
            Console.WriteLine("Dans le controleur Sub");
            try
            {
                await _eventService.CreateAndSubscribeAlert(subscribeDto);
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(400);
            }
        }

        [HttpDelete]
        public async Task<ActionResult> UnsubscribeAlert([FromBody] UnsubscribeDto unsubscribeDto)
        {
            Console.WriteLine("Dans le controleur Unsub");
            try
            {
                await _eventService.UnsubscribeAlert(unsubscribeDto);
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(400);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<ReturnUserDto>>> AlertUpdated(int id)
        {
            Console.WriteLine("Dans le controleur AlertUpdate");
            try
            {
                IEnumerable<ReturnUserDto> list = await _eventService.AlertUpdated(id);
                return Ok(list);
            }
            catch (Exception)
            {
                return StatusCode(400);
            }
        }

    }

}