using EngieCodingChallenge.DTO_s;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EngieCodingChallenge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductionPlanController : ControllerBase
    {

        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;
        private readonly ILogger<ProductionPlanController> _logger;

        public ProductionPlanController(Microsoft.AspNetCore.Hosting.IHostingEnvironment environment, ILogger<ProductionPlanController> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        //Just for initial testing purposes (Not needed for the challenge)
        [HttpGet]
        public ProductionPlanDTO GetJson()
        {
            var rootPath = _environment.ContentRootPath;
            var fullPath = Path.Combine(rootPath, "Payloads\\payload1.json");
            var jsonData = System.IO.File.ReadAllText(fullPath);
            if (string.IsNullOrWhiteSpace(jsonData)) { return null; }
            var payloads = JsonConvert.DeserializeObject<ProductionPlanDTO>(jsonData);
            if(payloads == null) { return null; }
            return payloads;
        }

        [HttpPost]
        [Route("productionplan")]
        public ProductionPlanDTO GenerateProductionPlan([FromBody] ProductionPlanDTO json)
        {
            var loads = json.Load;
            var fuels = json.Fuels;
            var powerplants = json.Powerplants;
            var windturbine = json.Powerplants.Select(p => p.Type = "windturbine").FirstOrDefault();

            //Merit order
            powerplants = (List<PowerPlantsDTO>?)powerplants.OrderByDescending(p => p.Type = windturbine);

            FuelCost(json);

            //Needs to return the optimal production plan 
            return json;

        }

        public double FuelCost(ProductionPlanDTO json)
        {

            double cost = 0;
            var powerplant = json.Powerplants.FirstOrDefault();

            //I need to deserialize the json-values of the fuels into the different existing fuels
            var FuelPrices = JsonConvert.DeserializeObject<List<IDictionary<string, double>>>(json.Fuels);

            //Fuels
            double value = 0;            
            var fuels = json.Fuels;
            var gasFuel = fuels.TryGetValue("gas(euro/MWh)", out value); 
            var kerosineFuel = fuels.TryGetValue("kerosine(euro/MWh)", out value);
            var co2Fuel = fuels.TryGetValue("co2(euro/ton)", out value);
            var windFuel = fuels.TryGetValue("wind(%)", out value);

            //Types
            var gasType = json.Powerplants.FirstOrDefault(p => p.Type == "gasfired"); 
            var turobojetType = json.Powerplants.FirstOrDefault(p => p.Type == "turbojet");
            var windturbineType = json.Powerplants.FirstOrDefault(p => p.Type == "windturbine");

            //Efficiency  
            var efficiency = powerplant.Efficency;

            //Pmax
            var pMax = powerplant.PMax;


            if(powerplant.Type == gasType.ToString())
            {
                cost = gasFuel * efficiency * pMax;
            }
            else if(powerplant.Type == turobojetType.ToString())
            {
                cost = kerosineFuel * efficiency * pMax;
            }
            else if(powerplant.Type ==  windturbineType.ToString())
            {
                cost = windFuel * efficiency * pMax;
            }

            return cost;
        }
    }
}
