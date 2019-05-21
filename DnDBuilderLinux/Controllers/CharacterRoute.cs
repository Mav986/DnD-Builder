using System.Net;
using System.Net.Http;
using System.Web.Http;
using DnDBuilderLinux.Handlers;
using Newtonsoft.Json.Linq;

namespace DnDBuilderLinux.Controllers
{
    public class CharacterRoute : ApiController
    {
        private readonly CharacterHandler _charHandler;
        
        public CharacterRoute()
        {
            _charHandler = new CharacterHandler();
        }

        [HttpPost]
        [Route("add/character")]
        public void AddCharacter([FromBody] JObject charData)
        {
            try
            {
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