namespace MandateParlamentare2024.Models
{
    public class RezultatPartid
    {
        public string Partid { get; set; }
        public int MandateDeputatFaza1 { get; set; }
        public int MandateDeputatFaza2 { get; set; }
        public int MandateDeputatFaza2b { get; set; }
        public int MandateSenatorFaza1 { get; set; }
        public int MandateSenatorFaza2 { get; set; }
        public int MandateSenatorFaza2b { get; set; }
        public int VoturiDeputat { get; set; }
        public int VoturiSenator { get; set; }
    }
}
