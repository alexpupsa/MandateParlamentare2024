using MandateParlamentare2024.Models;
using MandateParlamentare2024.Services;
using Newtonsoft.Json;

namespace MandateParlamentare2024
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var counties = new List<string>
            {
                "ab", "ar", "ag", "bc", "bh", "bn", "br", "bt", "bv", "bz",
                "cs", "cl", "cj", "ct", "cv", "db", "dj", "gl", "gr", "gj",
                "hr", "hd", "il", "is", "if", "mm", "mh", "ms", "nt", "ot",
                "ph", "sj", "sm", "sb", "sv", "tr", "tm", "tl", "vs", "vl",
                "vn", "s1", "s2", "s3", "s4", "s5", "s6", "sr"
            };
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
                        var voturiJudetCD = DataMapper.MapJsonDataToVoturiJudet(data, TipVot.CameraDeputatilor);
                        var voturiJudetS = DataMapper.MapJsonDataToVoturiJudet(data, TipVot.Senat);

                        countiesVotesCD.Add(voturiJudetCD);
                        countiesVotesS.Add(voturiJudetS);
                    }
                }
            }

            if (countiesVotesCD.Count > 0 || countiesVotesS.Count > 0)
            {
                var results = await ResultsService.GetResults(countiesVotesCD, countiesVotesS);

                var jsonResults = JsonConvert.SerializeObject(results, Formatting.Indented);

                File.WriteAllText(@"C:\rezultate-parlamentare.json", jsonResults);
            }
        }
    }
}
