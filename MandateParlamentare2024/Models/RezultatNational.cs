namespace MandateParlamentare2024.Models
{
    public class RezultatNational
    {
        public List<RezultatJudet> RezultateJudete { get; set; }
        public List<string> Partide { get; set; }
        public int SectiiDeputati { get; set; }
        public int SectiiSenatori { get; set; }
        public int MandateRamaseDeputat { get; set; }
        public int MandateRamaseSenator { get; set; }
    }
}
