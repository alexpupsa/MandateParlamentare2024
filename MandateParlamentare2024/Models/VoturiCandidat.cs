namespace MandateParlamentare2024.Models
{
    public class VoturiCandidat
    {
        private static readonly string[] PARTIDE = [ "ALIANȚA", "ALTERNATIVA", "PARTIDUL", "UNIUNEA", "BLOCUL", "ASOCIAȚIA", "FORUMUL", "FEDERAȚIA", "COMUNITATEA",
            "DREPTATE", "FORȚA", "SĂNĂTATE", "PATRIOȚII", "LIGA", "REÎNNOIM", "SOCIALISTĂ" ];
        public string Candidat { get; set; }
        public int Voturi { get; set; }
        public TipCandidat Tip
        {
            get
            {
                if (Candidat == "ROMÂNIA SOCIALISTĂ")
                {
                    return TipCandidat.Alianta;
                }
                else if (PARTIDE.Any(x => Candidat.Contains(x)))
                {
                    return TipCandidat.Partid;
                }
                return TipCandidat.Independent;
            }
            private set { }
        }
        public int MandateObtinuteFaza1 { get; set; }
        public int MandateObtinuteFaza2 { get; set; }
        public int MandateObtinuteFaza2b { get; set; }
        public int VoturiRamase { get; set; }
    }
}
