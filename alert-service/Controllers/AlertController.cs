using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using alert_service.Models;
using alert_service.Repositories;

namespace alert_service.Controllers
{
    [Route("alert/[controller]")]
    [ApiController]
    public class AlertController : ControllerBase
    {
        private readonly AlertRepository _alertRepo;
        public AlertController(AlertRepository alertRepo){
            _alertRepo = alertRepo;
        }

        // Route pour un entity_details par son id : GET: alert/Alert/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Alert>> GetAlert(int id)
        {
            
            try{
                // récupération du entity_details (detail_annonce) correspondant à l'id
                var Alert = await _alertRepo.GetOneById(id);

                // si rien n'a été trouvé
                if(Alert == null){
                    return NotFound();
                }
                return Ok(Alert);

            }catch(Exception e){
                // gestion des exceptions
                Console.WriteLine("GetOne route :"+e);
                return StatusCode(400);
            }
            
        }

        // Route pour tous les entity_details : GET: alert/Alert/
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Alert>>> GetAllAlert()
        {
            
            try{
                // récupération de la table en entier sous forme de list
                var listAlert = await _alertRepo.GetAll();
               
                // si rien n'a été trouvé retourner une erreur NotFound
                if(listAlert == null){
                    return NotFound();
                }
                return Ok(listAlert);

            }catch(Exception e){
                // gestion des exception de la promesse
                Console.WriteLine("Get all route :"+e);
                return StatusCode(400);
            }
            
        }

        // Route pour ajouter un entity_details : POST: alert/Alert/{id}
        [HttpPost]
        public async Task<IActionResult> PostAlert([FromBody]Alert Alert)
        {

            try{
                if (ModelState.IsValid)
                {
                    await _alertRepo.Add(Alert);            
                    return Ok();
                }else{
                    return BadRequest();
                }
            }catch (DbUpdateException /* ex */){
                    ModelState.AddModelError("", "l'ajout d'un élément adDetail a échoué");
                    return StatusCode(400);
            }
        }
        
        // Route modifier un entity_details avec son id: PUT: alert/Alert/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<Alert>> EditAlert(int id, [FromBody]Alert alert)
        {
            if (alert == null)
            {
                return NotFound();
            }
            var data = await _alertRepo.GetOneById(id);
            if(data == null){
                return NotFound();
            }
            try
            {
                data.IdEntityDetails = alert.IdEntityDetails;
       
                await _alertRepo.Update(data);
                return Ok(alert.IdAlert + "a bien été modifié");
            }
            catch (DbUpdateException /* ex */)
            {
                
                ModelState.AddModelError("", "la modification de l'élément adDetail a échoué");
                return StatusCode(400);
            }
            

        }

        [HttpDelete("{id}")]
        // le parametre saveChangesError était inclus dans la doc. Il supprime le retour d'erreur de la methode saveChangeAsync() 
        // la méthode étant déjà dans une promesse try, ce n'"tait pas nécessaire de conserver se retour d'erreur
        public async Task<IActionResult> DeleteAlert(int id, bool? saveChangesError = false)
        {
            try
            {
                await _alertRepo.Delete(id);

                return Ok("Alert n°" + id+ " a été correctement supprimé");
            }
            catch (DbUpdateException /* ex */)
            {
                // traitement des exceptions
                return BadRequest();
            }
        }
    }
}

