namespace MandateParlamentare2024.Models
{
    public class VoturiJudet
    {
        public string Judet { get; set; }
        public List<VoturiCandidat> Voturi { get; set; }
        public int MandateDistribuite { get; set; }
        public int MandateRamase { get; set; }
        public double RepartitorJudet { get; set; }
        public int SectiiNumarate { get; set; }
        public int VoturiExprimate { get; set; }
    }
}
