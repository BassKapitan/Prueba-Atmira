using Newtonsoft.Json;

namespace Prueba_Atmira.Models
{
    public class Asteroid
    {
        public string Name { get; set; }
        public double Diameter { get; set; }
        public string Speed { get; set; }
        public DateTime Date { get; set; }
        public string Planet { get; set; }
    }
}