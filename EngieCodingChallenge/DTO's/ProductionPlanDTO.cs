namespace EngieCodingChallenge.DTO_s
{
    public class ProductionPlanDTO
    {
        public double Load { get; set; }
        public IDictionary<string, double>? Fuels { get; set; }
        public List<PowerPlantsDTO>? Powerplants { get; set; }

    }
}
