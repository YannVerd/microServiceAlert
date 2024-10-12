using alert_service.Models;
using alert_service.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace alert_service.Controllers
{
    [Route("alert/[controller]")]
    [ApiController]
    public class NotifyController(DBContext context) : ControllerBase
    {
        private readonly DBContext _context = context;

        // Route pour récupérer un notify: POST: alert/notify/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<IList<Notify>>> GetNotify(int? id)
        {
            // test du parametre
            if (id == null)
            {
                return BadRequest();
            }
            try
            {
                // récupération du entity_details (detail_annonce) correspondant à l'id
                var notify = await _context.Notifys.FirstOrDefaultAsync(m => m.IdNotify == id);
                // si rien n'a été trouvé
                if (notify == null)
                {
                    return NotFound();
                }
                return Ok(notify);
            }
            catch (Exception e)
            {
                // gestion des exceptions
                Console.WriteLine("GetOne route :" + e);
                return StatusCode(400);
            }
        }

        // Route pour récupérer tous les notify: POST: alert/notify/
        [HttpGet]
        public ActionResult<IEnumerable<Notify>> GetAllNotifys()
        {
            try
            {
                // Récupération de la table en entier sous forme de liste
                var listNotify = _context.Notifys.ToList();

                // Si rien n'a été trouvé (la liste est vide), retourner une erreur NotFound
                if (listNotify.Count == 0) 
                {
                    return NotFound();
                }

                return Ok(listNotify);
            }
            catch (Exception e)
            {
                // Gestion des exceptions
                Console.WriteLine("Get all route :" + e);
                return StatusCode(400);
            }
        }

        // Route pour ajouter un notify: POST: alert/notify/{id}
        [HttpPost]
        public async Task<IActionResult> PostNotify([FromBody] Notify notify)
        {
            Console.WriteLine(notify);
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Notifys.Add(notify);
                    await _context.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (DbUpdateException /* ex */
            )
            {
                ModelState.AddModelError("", "l'ajout d'un élément notify a échoué");
                return StatusCode(400);
            }
        }

        // Route pour modifier un notify: PUT: alert/notify/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<Notify>> EditNotify(int id, [FromBody] Notify notify)
        {
            if (notify == null)
            {
                return NotFound();
            }
            var notifyUpdate = await _context.Notifys.FirstOrDefaultAsync(inDb =>
                inDb.IdNotify == id
            );

            if (notifyUpdate == null)
            {
                return NotFound();
            }
            // Ne fonctionne pas sans ce if. J'admet que c'est un peu flou
            if (
                await TryUpdateModelAsync<Notify>(
                    notifyUpdate,
                    "",
                    target => target.IdAlert,
                    target => target.IdDMUser
                )
            )
            {
                try
                {
                    notifyUpdate.IdAlert = notify.IdAlert;
                    notifyUpdate.IdDMUser = notify.IdDMUser;

                    await _context.SaveChangesAsync();
                    return Ok("la liste de diffusion a bien été modifiée");
                }
                catch (DbUpdateException /* ex */
                )
                {
                    ModelState.AddModelError("", "la modification de l'élément notify a échoué");
                }
            }

            return notifyUpdate;
        }

        // Route pour supprimer un notify: DELETE: alert/notify/
        [HttpDelete("{id}")]
        // le parametre saveChangesError était inclus dans la doc. Il supprime le retour d'erreur de la methode saveChangeAsync()
        // la méthode étant déj dans une promesse try, ce n'"tait pas nécessaire de conserver se retour d'erreur
        public async Task<IActionResult> DeleteNotify(int? id, bool? saveChangesError = false)
        {
            var notify = await _context.Notifys.FindAsync(id);
            if (notify == null)
            {
                return NotFound();
            }

            try
            {
                _context.Notifys.Remove(notify);
                await _context.SaveChangesAsync();
                return Ok("Notify n°" + id + " a été correctement supprimé");
            }
            catch (DbUpdateException /* ex */
            )
            {
                return BadRequest();
            }
        }
    }
}
