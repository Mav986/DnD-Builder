using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using DnDBuilderLinux.Exceptions;
using DnDBuilderLinux.Handlers;
using Newtonsoft.Json.Linq;

namespace DnDBuilderLinux.Controllers
{
    [RoutePrefix("character")]
    public class CharacterController : ApiController
    {
        private readonly CharacterHandler _charHandler;

        private const string CheckData = " Please check your data and try again.";
        private const string ContactAdmin = " If the problem persists, contact a server administrator";
        
        public CharacterController()
        {
            _charHandler = new CharacterHandler();
        }

        /// <summary>
        ///     Add a Character to DnDBuilder
        /// </summary>
        /// <param name="character">A JSON object containing all required character parameters</param>
        /// <returns>
        ///     201 Created if sucecssful
        ///     400 Bad Request otherwise
        /// </returns>
        [HttpPost]
        [Route("add")]
        public HttpResponseMessage AddCharacter([FromBody] JObject character)
        {
            try
            {
                _charHandler.AddCharacter(character);
                return Request.CreateResponse(HttpStatusCode.Created, "Character successfully added!");
            }
            catch (CharacterException e)
            {
                Console.WriteLine(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message + CheckData + ContactAdmin);
            }
        }

        /// <summary>
        ///     CachedGet all characters in DnDBuilder
        /// </summary>
        /// <returns>
        ///     200 OK if successful
        ///     500 Internal Server Error otherwise
        /// </returns>
        [HttpGet]
        [Route("view/all")]
        public HttpResponseMessage GetAllCharacters()
        {
            try
            {
                JArray characters = _charHandler.GetAllCharacters();
                return Request.CreateResponse(HttpStatusCode.OK, characters);
            }
            catch (CharacterException e)
            {
                Console.WriteLine(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message + ContactAdmin);
            }
        }

        /// <summary>
        ///     CachedGet a single character from within DnDBuilder
        /// </summary>
        /// <param name="characterName">The name of the character</param>
        /// <returns>
        ///     200 OK if successful
        ///     404 Not Found if the character doesn't exist
        ///     400 Bad Request otherwise
        /// </returns>
        [HttpGet]
        [Route("view/{characterName}")]
        public HttpResponseMessage GetCharacter(string characterName)
        {
            try
            {
                JObject character = _charHandler.GetCharacter(characterName);
                return Request.CreateResponse(HttpStatusCode.OK, character);
            }
            catch (NotFoundException e)
            {
                Console.WriteLine(e);
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, e.Message + CheckData + ContactAdmin);
            }
            catch (CharacterException e)
            {
                Console.WriteLine(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message + CheckData + ContactAdmin);
            }
        }

        /// <summary>
        ///     Update a single character within DnDBuilder
        /// </summary>
        /// <param name="character">JSON containing the character's name to be updated, and any updated fields</param>
        /// <returns>
        ///     204 OK if successful
        ///     404 Not Found if the character doesn't exist
        ///     400 Bad Request otherwise
        /// </returns>
        [HttpPut]
        [Route("update")]
        public HttpResponseMessage UpdateCharacter([FromBody] JObject character)
        {
            try
            {
                JObject updatedChar = _charHandler.UpdateCharacter(character);
                return Request.CreateResponse(HttpStatusCode.OK, updatedChar);
            }
            catch (NotFoundException e)
            {
                Console.WriteLine(e);
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, e.Message + CheckData + ContactAdmin);
            }
            catch (CharacterException e)
            {
                Console.WriteLine(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message + CheckData + ContactAdmin);
            }
        }
        
        /// <summary>
        ///     deleteReq a single character within DnDBuilder
        /// </summary>
        /// <param name="characterName">Name of the character to be deleted</param>
        /// <returns>
        ///     200 OK if the character was deleted
        ///     404 Not Found if the character doesn't exist
        ///     500 Internal Server Error if there's a problem processing the request 
        /// </returns>
        [HttpDelete]
        [Route("delete/{characterName}")]
        public HttpResponseMessage DeleteCharacter(string characterName)
        {
            try
            {
                _charHandler.DeleteCharacter(characterName);
                return Request.CreateResponse(HttpStatusCode.OK, "Character successfully deleted");
            }
            catch (NotFoundException e)
            {
                Console.WriteLine(e);
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, e.Message + CheckData + ContactAdmin);
            }
            catch (CharacterException e)
            {
                Console.WriteLine(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message + CheckData + ContactAdmin);
            }
        }

        /// <summary>
        ///     Download an XML file detailing a single character
        /// </summary>
        /// <param name="characterName">Name of the character to download xml file for</param>
        /// <returns>
        ///     200 OK if the file was generated and sent to the client
        ///     404 Not Found if the character doesn't exist
        ///     500 Internal Server Error if there's a problem processing the request
        /// </returns>
        [HttpGet]
        [Route("xml/{characterName}")]
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
                    new ContentDispositionHeaderValue("attachment") {FileName = filename};
                responseMsg.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");

                return responseMsg;
            }
            catch (NotFoundException e)
            {
                Console.WriteLine(e);
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, e.Message + CheckData + ContactAdmin);
            }
            catch (CharacterException e)
            {
                Console.WriteLine(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message + CheckData + ContactAdmin);
            }
        }
    }
}