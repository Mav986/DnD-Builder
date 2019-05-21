using System.Net;
using System.Net.Http;
using System.Web.Http;
using DnDBuilderLinux.Database;
using DnDBuilderLinux.Handlers;
using DnDBuilderLinux.Models;

namespace DnDBuilderLinux.Controllers
{
    [RoutePrefix("character")]
    public class CharacterController : ApiController
    {
        private readonly CharacterHandler _charHandler;
        
        public CharacterController()
        {
            DatabaseHandler db = new DatabaseHandler();
            _charHandler = new CharacterHandler(db);
        }

        [HttpPost]
        [Route("add")]
        public void AddCharacter([FromBody] Character charData)
        {
            try
            {
                if (!ModelState.IsValid) throw new CharacterException("Character invalid");
                _charHandler.AddCharacter(charData);
            }
            catch (CharacterException e)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Error: " + e.Message));
            }
        }
    }
}