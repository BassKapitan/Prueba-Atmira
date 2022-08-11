using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prueba_Atmira.Models;

namespace Prueba_Atmira.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class NasaController : ControllerBase
    {
        private readonly IConfiguration _config;
        static HttpClient client = new HttpClient();

        public NasaController(IConfiguration config)
        {
            _config = config;

        }

        // GET api/<NasaController>/5
        [HttpGet("{dayNumber}")]
        public async Task<object> Get(int dayNumber)
        {
            var startDate = DateTime.Today;
            var endDate = startDate.AddDays(dayNumber);
            var apiKey = _config["Api_Key"];


            try
            {
                var url = "https://api.nasa.gov/neo/rest/v1/feed?start_date=" + startDate.ToString("yyyy-MM-dd") + "&end_date=" + endDate.ToString("yyyy-MM-dd") + "&api_key=" + apiKey;
                var response = await client.GetStringAsync(url);

                Asteroids allDaysAsteroidsInfo = JsonConvert.DeserializeObject<Asteroids>(response);
                var asteroidsList = GetAsteroidList(allDaysAsteroidsInfo);

                return JsonConvert.SerializeObject(asteroidsList);
            }
            catch (Exception ex)
            {
                return String.Format("El rango de números permitido es de 1 a 7. Error:", ex.Message);
            }
        }

        private List<Asteroid> GetAsteroidList(Asteroids allDaysAsteroidsInfo)
        {
            var singleDayAsteroidsList = allDaysAsteroidsInfo.NearEarthObjects.Values.ToList();
            List<Asteroid> asteroidsList = new List<Asteroid>();
            foreach (var asteroids in singleDayAsteroidsList)
            {
                foreach (var asteroid in asteroids)
                {
                    var diameter = (asteroid.EstimatedDiameter.Kilometers.EstimatedDiameterMax + asteroid.EstimatedDiameter.Kilometers.EstimatedDiameterMin) / 2;

                    if (asteroid.IsPotentiallyHazardousAsteroid)
                    {
                        var ast = new Asteroid()
                        {
                            Name = asteroid.Name,
                            Speed = asteroid.CloseApproachData[0].RelativeVelocity.KilometersPerHour,
                            Date = asteroid.CloseApproachData[0].CloseApproachDate.Date,
                            Diameter = diameter,
                            Planet = asteroid.CloseApproachData[0].OrbitingBody.ToString(),
                        };

                        if (asteroidsList.Count() == 3 && asteroidsList.Min(x => x.Diameter) < diameter)
                        {
                            asteroidsList.RemoveAll(x => x.Diameter == asteroidsList.Min(x => x.Diameter));
                        }

                        if (asteroidsList.Count() < 3)
                        {
                            asteroidsList.Add(ast);
                        }
                    }
                }
            }
            return asteroidsList;
        }

    }
}
