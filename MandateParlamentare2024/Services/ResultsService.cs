using MandateParlamentare2024.Models;

namespace MandateParlamentare2024.Services
{
    public class ResultsService
    {
        public static async Task<RezultatNational> GetResults(List<VoturiJudet> voturiDeputati, List<VoturiJudet> voturiSenatori)
        {
            var rezultatDeputati = GetResultsByTipVot(TipVot.CameraDeputatilor, voturiDeputati, false);
            var rezultatSenatori = GetResultsByTipVot(TipVot.Senat, voturiSenatori, false);

            var judete = rezultatDeputati.Select(x => x.Judet).Distinct().OrderBy(x => x);

            var rezultatJudete = new List<RezultatJudet>();
            foreach (var judet in judete)
            {
                var rezultatDeputatiJudet = rezultatDeputati.First(x => x.Judet == judet);
                var rezultatSenatoriJudet = rezultatSenatori.First(x => x.Judet == judet);
                var rezultatJudet = new RezultatJudet
                {
                    Judet = judet,
                    TotalVoturiDeputat = rezultatDeputatiJudet.Voturi.Sum(x => x.Voturi),
                    TotalVoturiSenator = rezultatSenatoriJudet.Voturi.Sum(x => x.Voturi),
                    RezultatePartide = new List<RezultatPartid>()
                };
                var partide = rezultatDeputatiJudet.Voturi.Where(x => x.MandateObtinuteFaza1 + x.MandateObtinuteFaza2 + x.MandateObtinuteFaza2b > 0).Select(x => x.Candidat)
                    .Concat(rezultatSenatoriJudet.Voturi.Where(x => x.MandateObtinuteFaza1 + x.MandateObtinuteFaza2 + x.MandateObtinuteFaza2b > 0).Select(x => x.Candidat)).Distinct();
                foreach (var partid in partide)
                {
                    var rezultatPartid = new RezultatPartid
                    {
                        Partid = partid
                    };
                    var voturiPartidDeputati = rezultatDeputatiJudet.Voturi.FirstOrDefault(x => x.Candidat == partid);
                    if (voturiPartidDeputati != null)
                    {
                        rezultatPartid.MandateDeputatFaza1 = voturiPartidDeputati.MandateObtinuteFaza1;
                        rezultatPartid.MandateDeputatFaza2 = voturiPartidDeputati.MandateObtinuteFaza2;
                        rezultatPartid.MandateDeputatFaza2b = voturiPartidDeputati.MandateObtinuteFaza2b;
                        rezultatPartid.VoturiDeputat = voturiPartidDeputati.Voturi;
                    }
                    var voturiPartidSenatori = rezultatSenatoriJudet.Voturi.FirstOrDefault(x => x.Candidat == partid);
                    if (voturiPartidSenatori != null)
                    {
                        rezultatPartid.MandateSenatorFaza1 = voturiPartidSenatori.MandateObtinuteFaza1;
                        rezultatPartid.MandateSenatorFaza2 = voturiPartidSenatori.MandateObtinuteFaza2;
                        rezultatPartid.MandateSenatorFaza2b = voturiPartidSenatori.MandateObtinuteFaza2b;
                        rezultatPartid.VoturiSenator = voturiPartidSenatori.Voturi;
                    }
                    rezultatJudet.RezultatePartide.Add(rezultatPartid);
                }
                rezultatJudete.Add(rezultatJudet);
            }

            return new RezultatNational
            {
                RezultateJudete = rezultatJudete,
                Partide = rezultatJudete.SelectMany(x => x.RezultatePartide.Select(x => x.Partid)).Distinct().ToList(),
                MandateRamaseDeputat = rezultatDeputati.Sum(x => x.MandateRamase),
                MandateRamaseSenator = rezultatSenatori.Sum(x => x.MandateRamase)
            };
        }

        private static List<VoturiJudet> GetResultsByTipVot(TipVot tipVot, List<VoturiJudet> voturiJudete, bool isLongJudet)
        {
            var mandateJudete = GetNumarMandate(isLongJudet);
            var candidati = voturiJudete.SelectMany(x => x.Voturi).Select(x => x.Candidat).Distinct();
            var voturiCandidatiNational = new List<VoturiCandidat>();
            foreach (var candidat in candidati)
            {
                voturiCandidatiNational.Add(new VoturiCandidat { Candidat = candidat, Voturi = voturiJudete.SelectMany(x => x.Voturi).Where(x => x.Candidat == candidat).Sum(x => x.Voturi) });
            }
            var totalVoturiNationale = voturiJudete.SelectMany(x => x.Voturi).Sum(x => x.Voturi);
            var pragNational = Convert.ToInt32(Math.Floor(0.05d * totalVoturiNationale));
            var candidatiPestePrag = new List<string>();
            foreach (var voturiCandidat in voturiCandidatiNational)
            {
                if (voturiCandidat.Voturi >= pragNational)
                {
                    candidatiPestePrag.Add(voturiCandidat.Candidat);
                }
                else
                {
                    var prag20JudetCount = 0;
                    foreach (var voturiJudet in voturiJudete)
                    {
                        var totalVoturiJudet = voturiJudet.Voturi.Sum(x => x.Voturi);
                        var prag20 = Convert.ToInt32(Math.Floor(0.20d * totalVoturiJudet));
                        var candidatJudet = voturiJudet.Voturi.FirstOrDefault(x => x.Candidat == voturiCandidat.Candidat);
                        if (candidatJudet != null && candidatJudet.Voturi >= prag20)
                        {
                            prag20JudetCount++;
                        }
                    }
                    if (prag20JudetCount >= 4)
                    {
                        candidatiPestePrag.Add(voturiCandidat.Candidat);
                    }
                }
            }

            //distribuire mandate faza 1
            foreach (var voturiJudet in voturiJudete)
            {
                var candidatiJudet = candidatiPestePrag.Concat(voturiJudet.Voturi.Where(x => x.Tip == TipCandidat.Independent).Select(x => x.Candidat));

                var totalVoturiPragJudet = voturiJudet.Voturi.Where(x => candidatiJudet.Contains(x.Candidat)).Sum(x => x.Voturi);
                var totalLocuriJudet = tipVot == TipVot.CameraDeputatilor ? mandateJudete.First(x => x.Judet == voturiJudet.Judet).Deputati : mandateJudete.First(x => x.Judet == voturiJudet.Judet).Senatori;
                var coeficientElectoral = Convert.ToInt32(Math.Floor((double)totalVoturiPragJudet / (double)totalLocuriJudet));

                foreach (var candidat in candidatiJudet)
                {
                    var voturiCandidatJudet = voturiJudet.Voturi.FirstOrDefault(x => x.Candidat == candidat);
                    if (voturiCandidatJudet != null)
                    {
                        if (voturiCandidatJudet.Voturi >= coeficientElectoral)
                        {
                            if (voturiCandidatJudet.Tip == TipCandidat.Independent)
                            {
                                voturiCandidatJudet.MandateObtinuteFaza1 = 1;
                                voturiJudet.MandateDistribuite++;
                            }
                            else
                            {
                                voturiCandidatJudet.MandateObtinuteFaza1 = Convert.ToInt32(Math.Floor((double)voturiCandidatJudet.Voturi / (double)coeficientElectoral));
                                voturiCandidatJudet.VoturiRamase = voturiCandidatJudet.Voturi - voturiCandidatJudet.MandateObtinuteFaza1 * coeficientElectoral;
                                voturiJudet.MandateDistribuite += voturiCandidatJudet.MandateObtinuteFaza1;
                            }
                        }
                        else if (voturiCandidatJudet.Tip != TipCandidat.Independent)
                        {
                            voturiCandidatJudet.VoturiRamase = voturiCandidatJudet.Voturi;
                        }

                    }
                }
                voturiJudet.MandateRamase = totalLocuriJudet - voturiJudet.MandateDistribuite;
            }

            //distribuire mandate faza 2
            var totalMandateNational = mandateJudete.Sum(x => tipVot == TipVot.CameraDeputatilor ? x.Deputati : x.Senatori);
            var totalMandateDistribuiteNational = voturiJudete.Sum(x => x.MandateDistribuite);
            var totalMandateRamaseNational = totalMandateNational - totalMandateDistribuiteNational;
            var caturi = new List<double>();
            foreach (var candidat in candidatiPestePrag)
            {
                var voturiRamase = voturiJudete.SelectMany(x => x.Voturi).Where(x => x.Candidat == candidat).Sum(x => x.VoturiRamase);
                for (var i = 1; i <= totalMandateRamaseNational; i++)
                {
                    caturi.Add((double)voturiRamase / (double)i);
                }
            }
            var coeficientNational = Convert.ToInt32(Math.Floor(caturi.OrderByDescending(x => x).Take(totalMandateRamaseNational).Min()));
            var mandateNationalePartide = new List<MandateNationalePartid>();
            foreach (var candidat in candidatiPestePrag)
            {
                var voturiRamase = voturiJudete.SelectMany(x => x.Voturi).Where(x => x.Candidat == candidat).Sum(x => x.VoturiRamase);
                mandateNationalePartide.Add(new MandateNationalePartid
                {
                    Candidat = candidat,
                    MandateCuvenite = Convert.ToInt32(Math.Floor((double)voturiRamase / (double)coeficientNational))
                });
            }

            var partideFaraMandateCuvenite = mandateNationalePartide.Where(x => x.MandateCuvenite == 0).Select(x => x.Candidat);
            if (partideFaraMandateCuvenite.Any())
            {
                candidatiPestePrag.RemoveAll(x => partideFaraMandateCuvenite.Contains(x));
            }

            //distribuire mandate din faza 2 pe judete
            var datePartideJudete = new List<DatePartidJudet>();
            foreach (var candidat in candidatiPestePrag)
            {
                var voturiRamaseNational = voturiJudete.SelectMany(x => x.Voturi).Where(x => x.Candidat == candidat).Sum(x => x.VoturiRamase);
                var mandateCuvenite = mandateNationalePartide.First(x => x.Candidat == candidat).MandateCuvenite;
                var voturiNationale = voturiCandidatiNational.First(x => x.Candidat == candidat).Voturi;
                foreach (var voturiJudet in voturiJudete)
                {
                    if (voturiJudet.Voturi.Any(x => x.Candidat == candidat))
                    {
                        var candidatJudet = voturiJudet.Voturi.First(x => x.Candidat == candidat);
                        datePartideJudete.Add(new DatePartidJudet
                        {
                            Candidat = candidat,
                            Judet = voturiJudet.Judet,
                            Coeficient = ((double)candidatJudet.VoturiRamase / (double)voturiRamaseNational) * mandateCuvenite,
                            VoturiRamase = candidatJudet.VoturiRamase,
                            VoturiJudet = candidatJudet.Voturi,
                            VoturiNationale = voturiNationale
                        });
                    }
                }
            }

            var judetGroups = datePartideJudete.GroupBy(x => x.Judet);
            foreach (var judetGroup in judetGroups)
            {
                var voturiJudet = voturiJudete.First(x => x.Judet == judetGroup.Key);
                var mandateRamaseJudet = voturiJudet.MandateRamase;
                var dateJudet = judetGroup.ToList().OrderByDescending(x => x.Coeficient).ThenByDescending(x => x.VoturiRamase).ThenByDescending(x => x.VoturiJudet).ThenByDescending(x => x.VoturiNationale).Take(mandateRamaseJudet).LastOrDefault();
                voturiJudet.RepartitorJudet = dateJudet.Coeficient;
            }

            datePartideJudete = datePartideJudete.OrderByDescending(x => x.Coeficient).ThenByDescending(x => x.VoturiRamase).ThenByDescending(x => x.VoturiJudet).ThenByDescending(x => x.VoturiNationale).ToList();
            var mandateRedistribuiteCount = 0;

            do
            {
                var top = datePartideJudete.First();
                var voturiJudet = voturiJudete.First(x => x.Judet == top.Judet);
                var mandate = Convert.ToInt32(Math.Floor(top.Coeficient / voturiJudet.RepartitorJudet));
                if (mandate == 0)
                {
                    mandate = 1;
                }
                var mandateNationaleCuvenitePartid = mandateNationalePartide.First(x => x.Candidat == top.Candidat);
                if (mandateNationaleCuvenitePartid.MandateCuvenite < mandate)
                {
                    mandate = mandateNationaleCuvenitePartid.MandateCuvenite;
                }
                if (voturiJudet.MandateRamase < mandate)
                {
                    mandate = voturiJudet.MandateRamase;
                }

                var voturiPartidJudet = voturiJudet.Voturi.First(x => x.Candidat == top.Candidat);
                voturiPartidJudet.MandateObtinuteFaza2 += mandate;
                mandateRedistribuiteCount += mandate;

                voturiJudet.MandateRamase -= mandate;
                mandateNationaleCuvenitePartid.MandateCuvenite -= mandate;
                datePartideJudete.Remove(top);
                if (mandateNationaleCuvenitePartid.MandateCuvenite == 0)
                {
                    datePartideJudete.RemoveAll(x => x.Candidat == mandateNationaleCuvenitePartid.Candidat);
                }
                if (voturiJudet.MandateRamase == 0)
                {
                    datePartideJudete.RemoveAll(x => x.Judet == voturiJudet.Judet);
                }

            } while (mandateRedistribuiteCount < totalMandateRamaseNational && datePartideJudete.Count > 0);

            if (mandateRedistribuiteCount < totalMandateRamaseNational)
            {
                var candidatiCuMandateCuveniteRamase = mandateNationalePartide.Where(x => x.MandateCuvenite > 0).ToList();
                if (candidatiCuMandateCuveniteRamase.Count == 1)
                {
                    var candidat = candidatiCuMandateCuveniteRamase.First();
                    var mandateCuvenite = candidat.MandateCuvenite;
                    var judeteCuMandateRamase = voturiJudete.Where(x => x.MandateRamase > 0);
                    foreach (var judet in judeteCuMandateRamase)
                    {
                        if (mandateCuvenite > 0)
                        {
                            var mandateRamaseJudet = judet.MandateRamase;
                            var candidatJudet = judet.Voturi.FirstOrDefault(x => x.Candidat == candidat.Candidat);
                            candidatJudet.MandateObtinuteFaza2b += mandateRamaseJudet;
                            mandateCuvenite -= mandateRamaseJudet;
                            judet.MandateRamase -= mandateRamaseJudet;
                        }
                    }
                }
                else
                {
                    //todo mai multe partide cu mandate ramase
                }
            }


            return voturiJudete;
        }

        private static List<MandateJudet> GetNumarMandate(bool isLongJudet)
        {
            return new List<MandateJudet>
            {
                new MandateJudet { Judet = "AB", Senatori = 2, Deputati = 5 },
                new MandateJudet { Judet = "AR", Senatori = 3, Deputati = 7 },
                new MandateJudet { Judet = "AG", Senatori = 4, Deputati = 9 },
                new MandateJudet { Judet = "BC", Senatori = 4, Deputati = 10 },
                new MandateJudet { Judet = "BH", Senatori = 4, Deputati = 9 },
                new MandateJudet { Judet = "BN", Senatori = 2, Deputati = 5 },
                new MandateJudet { Judet = "BT", Senatori = 3, Deputati = 6 },
                new MandateJudet { Judet = "BV", Senatori = 4, Deputati = 9 },
                new MandateJudet { Judet = "BR", Senatori = 2, Deputati = 5 },
                new MandateJudet { Judet = "BZ", Senatori = 3, Deputati = 7 },
                new MandateJudet { Judet = "CS", Senatori = 2, Deputati = 5 },
                new MandateJudet { Judet = "CL", Senatori = 2, Deputati = 4 },
                new MandateJudet { Judet = "CJ", Senatori = 4, Deputati = 10 },
                new MandateJudet { Judet = "CT", Senatori = 5, Deputati = 11 },
                new MandateJudet { Judet = "CV", Senatori = 2, Deputati = 4 },
                new MandateJudet { Judet = "DB", Senatori = 3, Deputati = 7 },
                new MandateJudet { Judet = "DJ", Senatori = 4, Deputati = 10 },
                new MandateJudet { Judet = "GL", Senatori = 4, Deputati = 9 },
                new MandateJudet { Judet = "GR", Senatori = 2, Deputati = 4 },
                new MandateJudet { Judet = "GJ", Senatori = 2, Deputati = 5 },
                new MandateJudet { Judet = "HR", Senatori = 2, Deputati = 5 },
                new MandateJudet { Judet = "HD", Senatori = 3, Deputati = 6 },
                new MandateJudet { Judet = "IL", Senatori = 2, Deputati = 4 },
                new MandateJudet { Judet = "IS", Senatori = 5, Deputati = 12 },
                new MandateJudet { Judet = "IF", Senatori = 2, Deputati = 5 },
                new MandateJudet { Judet = "MM", Senatori = 3, Deputati = 7 },
                new MandateJudet { Judet = "MH", Senatori = 2, Deputati = 4 },
                new MandateJudet { Judet = "MS", Senatori = 4, Deputati = 8 },
                new MandateJudet { Judet = "NT", Senatori = 3, Deputati = 8 },
                new MandateJudet { Judet = "OT", Senatori = 3, Deputati = 6 },
                new MandateJudet { Judet = "PH", Senatori = 5, Deputati = 11 },
                new MandateJudet { Judet = "SM", Senatori = 2, Deputati = 5 },
                new MandateJudet { Judet = "SJ", Senatori = 2, Deputati = 4 },
                new MandateJudet { Judet = "SB", Senatori = 3, Deputati = 6 },
                new MandateJudet { Judet = "SV", Senatori = 4, Deputati = 10 },
                new MandateJudet { Judet = "TR", Senatori = 2, Deputati = 5 },
                new MandateJudet { Judet = "TM", Senatori = 4, Deputati = 10 },
                new MandateJudet { Judet = "TL", Senatori = 2, Deputati = 4 },
                new MandateJudet { Judet = "VS", Senatori = 3, Deputati = 7 },
                new MandateJudet { Judet = "VL", Senatori = 2, Deputati = 6 },
                new MandateJudet { Judet = "VN", Senatori = 2, Deputati = 5 },
                new MandateJudet { Judet = "B", Senatori = 13, Deputati = 29 },
                new MandateJudet { Judet = "DI", Senatori = 2, Deputati = 4 }
            };
        }
    }
}
