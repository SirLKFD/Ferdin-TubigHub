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
                    prompt,
                    model = "text-davinci-002" // Adjust model as needed
                };

                string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);

                StringContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(ApiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
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
