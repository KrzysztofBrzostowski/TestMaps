using NUnit.Framework;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TEST.Model.LukeSkywalkerModel;

namespace TEST
{
    [TestFixture]
    class LukeSkywalkerTests
    {

        private HttpClient _httpClient;


        [SetUp]
        public void SetUp()
        {
            _httpClient = new HttpClient();
        }


        [Test]
        public async Task CheckLukeSkywalkerHomeworld()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("https://swapi.dev/api/people/1/");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            var luke = JsonSerializer.Deserialize<Person>(responseBody);
            //var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);

            response = await _httpClient.GetAsync(luke.homeworld);
            response.EnsureSuccessStatusCode();
            responseBody = await response.Content.ReadAsStringAsync();
        
            var planete = JsonSerializer.Deserialize<Planete>(responseBody);

            Assert.AreEqual("Tatooine", planete.name);
        }


    }
}
