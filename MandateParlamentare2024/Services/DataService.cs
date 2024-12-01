using MandateParlamentare2024.Models;
using Newtonsoft.Json;

namespace MandateParlamentare2024.Services
{
    public class DataService
    {
        public static Root? ParseCountyJson(string json)
        {
            return JsonConvert.DeserializeObject<Root?>(json);
        }

        public static async Task<string?> GetJsonFromAEP(string county)
        {
            var client = new HttpClient();

            var url = $"https://prezenta.roaep.ro/parlamentare01122024/data/json/sicpv/pv/pv_{county}.json";
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            return null;
        }
    }
}
