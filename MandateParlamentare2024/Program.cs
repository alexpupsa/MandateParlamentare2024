using MandateParlamentare2024.Models;
using MandateParlamentare2024.Services;
using Newtonsoft.Json;

namespace MandateParlamentare2024
{
    internal class Program
    {
        private static readonly List<string> counties = new List<string>
        {
            "ab", "ar", "ag", "bc", "bh", "bn", "br", "bt", "bv", "bz",
            "cs", "cl", "cj", "ct", "cv", "db", "dj", "gl", "gr", "gj",
            "hr", "hd", "il", "is", "if", "mm", "mh", "ms", "nt", "ot",
            "ph", "sj", "sm", "sb", "sv", "tr", "tm", "tl", "vs", "vl",
            "vn", "s1", "s2", "s3", "s4", "s5", "s6", "sr"
        };
        private static readonly List<string> sectoare = new List<string> { "s1", "s2", "s3", "s4", "s5", "s6" };


        static async Task Main(string[] args)
        {
            var countiesVotesCD = new List<VoturiJudet>();
            var countiesVotesS = new List<VoturiJudet>();

            foreach (var county in counties)
            {
                var json = await DataService.GetJsonFromAEP(county);
                if (json != null)
                {
                    var data = DataService.ParseCountyJson(json);
                    if (data != null)
                    {
                        var judet = county;
                        if (sectoare.Contains(county))
                        {
                            judet = "b";
                        }

                        var voturiJudetCD = DataMapper.MapJsonDataToVoturiJudet(data, TipVot.CameraDeputatilor, county);
                        var voturiJudetS = DataMapper.MapJsonDataToVoturiJudet(data, TipVot.Senat, county);

                        ProcessCountyVotes(voturiJudetCD, judet, ref countiesVotesCD);
                        ProcessCountyVotes(voturiJudetS, judet, ref countiesVotesS);
                    }
                }
                else
                {
                    countiesVotesCD.Add(new VoturiJudet(county));
                    countiesVotesS.Add(new VoturiJudet(county));
                }
            }

            if (countiesVotesCD.Count > 0 || countiesVotesS.Count > 0)
            {
                var results = ResultsService.GetResults(countiesVotesCD, countiesVotesS);

                var jsonResults = JsonConvert.SerializeObject(results, Formatting.Indented);

                File.WriteAllText(@"C:\USR\rezultate-parlamentare.json", jsonResults);
            }
        }

        private static void ProcessCountyVotes(VoturiJudet? countyVotes, string county, ref List<VoturiJudet> allCountiesVotes)
        {
            if (countyVotes != null)
            {
                if (county == "b")
                {
                    var judetB = allCountiesVotes.FirstOrDefault(x => x.Judet == "b");
                    if (judetB == null)
                    {
                        allCountiesVotes.Add(countyVotes);
                    }
                    else
                    {
                        judetB.Voturi.AddRange(countyVotes.Voturi);
                    }
                }
                else
                {
                    allCountiesVotes.Add(countyVotes);
                }
            }
        }
    }
}
