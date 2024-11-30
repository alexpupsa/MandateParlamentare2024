namespace MandateParlamentare2024.Models
{
    public class RezultatJudet
    {
        public string Judet { get; set; }
        public List<RezultatPartid> RezultatePartide { get; set; }
        public int TotalVoturiDeputat { get; set; }
        public int TotalVoturiSenator { get; set; }
    }
}
