using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DnDBuilderLinux.Database;
using DnDBuilderLinux.Handlers;
using DnDBuilderLinux.Models;
using Newtonsoft.Json.Linq;

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
                if (!ModelState.IsValid) throw new CharacterException("Adding character failed, invalid data");
                _charHandler.AddCharacter(charData);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Unable to create character. Please check your data and try again. " +
                    "If the problem persists, contact a server administrator"));
            }
        }

        [HttpGet]
        [Route("view/all")]
        public JArray GetAllCharacters()
        {
            try
            {
                return _charHandler.GetAllCharacters();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError,
                    "Unable to get characters. Please contact a server administrator."));
            }
        }

        [HttpGet]
        [Route("view/{name}")]
        public JObject GetCharacter(string name)
        {
            try
            {
                return _charHandler.GetCharacter(name);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Unable to get character. Please check your data and try again. " +
                    "If the problem persists, contact a server administrator"));
            }
        }

        [HttpPut]
        [Route("update")]
        public void UpdateCharacter([FromBody] JObject charData)
        {
            try
            {
                _charHandler.UpdateCharacter(charData);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Unable to update character. Please check your data and try again. " +
                    "If the problem persists, contact a server administrator"));
            }
        }

        [HttpDelete]
        [Route("delete/{name}")]
        public void DeleteCharacter(string name)
        {
            try
            {
                _charHandler.DeleteCharacter(name);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Unable to delete character. Please check the name and try again. " +
                    "If the problem persists, contact a server administrator"));
            }
        }
    }
}