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
            var countiesVotes = new List<VoturiJudet>();

            foreach (var county in counties)
            {
                var json = await DataService.GetJsonFromAEP(county);
                var data = DataService.ParseCountyJson(json);
                if (data != null)
                {
                    var voturiJudet = DataMapper.MapJsonDataToVoturiJudet(data);
                    countiesVotes.Add(voturiJudet);
                }
            }

            if (countiesVotes.Count > 0)
            {
                var results = await ResultsService.GetResults(countiesVotes, countiesVotes);

                var jsonResults = JsonConvert.SerializeObject(results, Formatting.Indented);

                File.WriteAllText(@"C:\rezultate-parlamentare.json", jsonResults);
            }
        }
    }
}
