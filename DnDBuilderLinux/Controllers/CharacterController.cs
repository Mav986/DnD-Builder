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
                if (!ModelState.IsValid) throw new CharacterException("Character invalid");
                _charHandler.AddCharacter(charData);
            }
            catch (CharacterException)
            {
                // Best practice: log stack trace here
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Something went wrong. Please check your data and try again."));
            }
        }

        [HttpGet]
        [Route("view/all")]
        public JArray GetCharacters()
        {
            try
            {
                return _charHandler.GetAllCharacters();
            }
            catch (CharacterException)
            {
                // Best practice: log stack trace here
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError,
                    "Something went wrong. Please contact a server administrator."));
            }
        }

        [HttpPut]
        [Route("update")]
        public void UpdateCharacter([FromBody] JObject newData)
        {
            try
            {
                _charHandler.UpdateCharacter(newData);
            }
            catch (Exception)
            {
                // Best practice: log stack trace here
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Something went wrong. Please check your data and try again."));
            }
        }
    }
}