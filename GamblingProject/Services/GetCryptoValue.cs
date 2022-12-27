using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GamblingProject.Services
{
    public static class GetCryptoValue
    {
        public static async Task<double> GetEthPriceAsync()
        {
            // Replace YOUR_API_KEY with your actual API key
            var apiKey = "6091ddea491dd192c595a45ad7a6489d63fccd93c37bda8a7199254be48c8254";

            // Construct the API URL
            var apiUrl = $"https://min-api.cryptocompare.com/data/price?fsym=ETH&tsyms=USD&api_key={apiKey}";

            // Create an HTTP client
            using (var client = new HttpClient())
            {
                // Make the GET request to the API
                var response = await client.GetAsync(apiUrl);

                // Check that the request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Read the response content as a string
                    var responseContent = await response.Content.ReadAsStringAsync();

                    // Parse the response content as a JSON object
                    dynamic json = JsonConvert.DeserializeObject(responseContent);

                    // Extract the ETH price from the JSON object
                    double ethPrice = json.USD;

                    // Return the ETH price
                    return ethPrice;
                }

                throw new Exception("API request failed");
            }
        }

        //get btc price
        public static async Task<double> GetBtcPriceAsync()
        {
            // Replace YOUR_API_KEY with your actual API key
            var apiKey = "6091ddea491dd192c595a45ad7a6489d63fccd93c37bda8a7199254be48c8254";

            // Construct the API URL
            var apiUrl = $"https://min-api.cryptocompare.com/data/price?fsym=BTC&tsyms=USD&api_key={apiKey}";

            // Create an HTTP client
            using (var client = new HttpClient())
            {
                // Make the GET request to the API
                var response = await client.GetAsync(apiUrl);

                // Check that the request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Read the response content as a string
                    var responseContent = await response.Content.ReadAsStringAsync();

                    // Parse the response content as a JSON object
                    dynamic json = JsonConvert.DeserializeObject(responseContent);

                    // Extract the ETH price from the JSON object
                    double ethPrice = json.USD;

                    // Return the ETH price
                    return ethPrice;
                }

                throw new Exception("API request failed");
            }
        }
    }
}