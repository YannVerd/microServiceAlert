using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using alert_service.Models;
using alert_service.Repositories;

namespace alert_service.Controllers
{
    [Route("alert/[controller]")]
    [ApiController]
    public class EntityDetailsController : ControllerBase
    {
        private readonly EntityDetailsRepository _entityDetailsRepo;
        public EntityDetailsController(EntityDetailsRepository entityDetailsRepo){
            _entityDetailsRepo = entityDetailsRepo;
        }

        // Route pour un entity_details par son id : GET: alert/entitydetails/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<EntityDetails>> GetEntityDetails(int id)
        {
            
            try{
                // récupération du entity_details (detail_annonce) correspondant à l'id
                var entityDetails = await _entityDetailsRepo.GetOneById(id);
                Console.WriteLine(entityDetails);
                // si rien n'a été trouvé
                if(entityDetails == null){
                    return NotFound();
                }
                return Ok(entityDetails);

            }catch(Exception e){
                // gestion des exceptions
                Console.WriteLine("GetOne route :"+e);
                return StatusCode(400);
            }
            
        }

        // Route pour tous les entity_details : GET: alert/entitydetails/
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EntityDetails>>> GetAllEntityDetails()
        {
            
            try{
                // récupération de la table en entier sous forme de list
                var listEntityDetails = await _entityDetailsRepo.GetAll();
               
                // si rien n'a été trouvé retourner une erreur NotFound
                if(listEntityDetails == null){
                    return NotFound();
                }
                return Ok(listEntityDetails);

            }catch(Exception e){
                // gestion des exception de la promesse
                Console.WriteLine("Get all route :"+e);
                return StatusCode(400);
            }
            
        }

        // Route pour ajouter un entity_details : POST: alert/entitydetails/{id}
        [HttpPost]
        public async Task<IActionResult> PostEntityDetails([FromBody]EntityDetails entityDetails)
        {

            try{
                if (ModelState.IsValid)
                {
                    Console.WriteLine("Du body "+entityDetails);
                    await _entityDetailsRepo.Add(entityDetails);            
                    return Ok();
                }else{
                    return BadRequest();
                }
            }catch (DbUpdateException /* ex */){
                    ModelState.AddModelError("", "l'ajout d'un élément adDetail a échoué");
                    return StatusCode(400);
            }
        }
        
        // Route modifier un entity_details avec son id: PUT: alert/entitydetails/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<EntityDetails>> EditEntityDetails(int id, [FromBody]EntityDetails entityDetails)
        {
            if (entityDetails == null)
            {
                return NotFound();
            }
            var data = await _entityDetailsRepo.GetOneById(id);
            if(data == null){
                return NotFound();
            }
            try
            {
                data.IdDMEntity = entityDetails.IdDMEntity;
                data.Name = entityDetails.Name;
                data.Type = entityDetails.Type;
                data.IdAlert = entityDetails.IdAlert;
                await _entityDetailsRepo.Update(data);
                return Ok(entityDetails.IdEntityDetails + "a bien été modifié");
            }
            catch (DbUpdateException /* ex */)
            {
                
                ModelState.AddModelError("", "la modification de l'élément adDetail a échoué");
                return StatusCode(400);
            }
            

        }

        [HttpDelete("{id}")]
        // le parametre saveChangesError était inclus dans la doc. Il supprime le retour d'erreur de la methode saveChangeAsync() 
        // la méthode étant déj dans une promesse try, ce n'"tait pas nécessaire de conserver se retour d'erreur
        public async Task<IActionResult> DeleteEntityDetails(int id, bool? saveChangesError = false)
        {
            try
            {
                await _entityDetailsRepo.Delete(id);

                return Ok("entityDetails n°" + id+ " a été correctement supprimé");
            }
            catch (DbUpdateException /* ex */)
            {
                // traitement des exceptions
                return BadRequest();
            }
        }
    }
}
