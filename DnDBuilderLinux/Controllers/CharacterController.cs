using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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

        /// <summary>
        ///     Add a Character to DnDBuilder
        /// </summary>
        /// <param name="character">A valid Character object</param>
        /// <exception cref="HttpResponseException"></exception>
        [HttpPost]
        [Route("add")]
        public void AddCharacter([FromBody] Character character)
        {
            try
            {
                if (!ModelState.IsValid) throw new CharacterException("Adding character failed, invalid data");
                _charHandler.AddCharacter(character);
            }
            catch (CharacterException e)
            {
                Console.WriteLine(e);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Unable to create character. Please check your data and try again. " +
                    "If the problem persists, contact a server administrator"));
            }
        }

        /// <summary>
        ///     Get all characters in DnDBuilder
        /// </summary>
        /// <returns>A JSON array containing JSON with every character's data</returns>
        /// <exception cref="HttpResponseException"></exception>
        [HttpGet]
        [Route("view/all")]
        public JArray GetAllCharacters()
        {
            try
            {
                return _charHandler.GetAllCharacters();
            }
            catch (CharacterException e)
            {
                Console.WriteLine(e);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError,
                    "Unable to get characters. Please contact a server administrator."));
            }
        }

        /// <summary>
        ///     Get a single character from within DnDBuilder
        /// </summary>
        /// <param name="characterName">The name of the character</param>
        /// <returns>JSON containing all the specified character's data</returns>
        /// <exception cref="HttpResponseException"></exception>
        [HttpGet]
        [Route("view/{name}")]
        public JObject GetCharacter(string characterName)
        {
            try
            {
                return _charHandler.GetCharacter(characterName);
            }
            catch (CharacterException e)
            {
                Console.WriteLine(e);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Unable to get character. Please check your data and try again. " +
                    "If the problem persists, contact a server administrator"));
            }
        }

        /// <summary>
        ///     Update a single character within DnDBuilder
        /// </summary>
        /// <param name="character">JSON containing the character's name to be updated, and any updated fields</param>
        /// <exception cref="HttpResponseException"></exception>
        [HttpPut]
        [Route("update")]
        public void UpdateCharacter([FromBody] JObject character)
        {
            try
            {
                _charHandler.UpdateCharacter(character);
            }
            catch (CharacterException e)
            {
                Console.WriteLine(e);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Unable to update character. Please check your data and try again. " +
                    "If the problem persists, contact a server administrator"));
            }
        }
        
        /// <summary>
        ///     Delete a single character within DnDBuilder
        /// </summary>
        /// <param name="characterName">Name of the character to be deleted</param>
        /// <exception cref="HttpResponseException"></exception>
        [HttpDelete]
        [Route("delete/{name}")]
        public void DeleteCharacter(string characterName)
        {
            try
            {
                _charHandler.DeleteCharacter(characterName);
            }
            catch (CharacterException e)
            {
                Console.WriteLine(e);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Unable to delete character. Please check the name and try again. " +
                    "If the problem persists, contact a server administrator"));
            }
        }

        /// <summary>
        ///     Download an XML file detailing a single character
        /// </summary>
        /// <param name="characterName">Name of the character to download xml file for</param>
        /// <returns>An XML file containing character data</returns>
        /// <exception cref="HttpResponseException"></exception>
        [HttpGet]
        [Route("xml/{name}")]
        public HttpResponseMessage GenerateXmlFor(string characterName)
        {
            const string filename = "character.xml";
            
            try
            {
                if (string.IsNullOrEmpty(characterName)) throw new CharacterException("Name is required");
                
                // Generate XML file and add to response content
                _charHandler.CreateCharacterXml(characterName, filename);
                FileStream xmlFile = new FileStream(filename, FileMode.Open);
                HttpResponseMessage responseMsg = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StreamContent(xmlFile)
                };
                responseMsg.Content.Headers.ContentDisposition =
                    new ContentDispositionHeaderValue("attachment") {FileName = "Text.xml"};
                responseMsg.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");

                return responseMsg;
            }
            catch (CharacterException e)
            {
                Console.WriteLine(e);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Unable to generate XML. Please check the name and try again. " +
                    "If the problem persists, contact a server administrator"));
            }
        }
    }
}