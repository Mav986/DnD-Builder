using System;
using System.Net.Http;
using System.Web.Http;
using DnDBuilder.Handlers;
using Newtonsoft.Json.Linq;

namespace DnDBuilder.Controllers
{
    public class DnDController : ApiController
    {
        private const string BaseUri = "http://www.dnd5eapi.co/api/";
        
        private readonly Race _raceHandler;
        
        public DnDController()
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseUri)
            };
            _raceHandler = new Race(httpClient);
        }
        
        [HttpGet]
        [Route("races/all")]
        public JObject Get()
        { 
            return _raceHandler.GetRaces(); ;
        }
    }
}