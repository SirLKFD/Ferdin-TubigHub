using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Ferdin_TB_Hub.Classes
{
    internal class ChatGPTIntegration
    {
        private const string ApiKey = "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";
        private const string ApiUrl = "https://api.openai.com/v1/completions";

        public async Task<string> GetChatResponseAsync(string prompt)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");

                var requestData = new
                {
                    prompt = prompt,
                    model = "text-davinci-002" // Adjust model as needed
                };

                var jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);

                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(ApiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    // Parse jsonResponse to extract the generated text
                    return jsonResponse;
                }
                else
                {
                    // Handle error response
                    return null;
                }
            }
        }
    }
}
