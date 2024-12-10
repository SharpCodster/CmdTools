using CmdTools.Core.Entities;
using CmdTools.Core.UserSettings;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace CmdTools.Core.Helpers
{
    public class CodioApiClient
    {
        private readonly string _baseUrl = "";
        private HttpClient _httpClient;

        public CodioApiClient(string baseUrl, Credential cred)
        {
            _baseUrl = baseUrl;
            Task.WaitAll(Initialize(cred));
        }

        private async Task Initialize(Credential cred)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_baseUrl);

            JsonContent content = JsonContent.Create(cred);

            var res = await _httpClient.PostAsync("auth/api/login", content);

            var tokenJson = await res.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<BarerToken>(tokenJson);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        }

        public HttpClient GetClient()
        {
            return _httpClient;
        }


        public async Task<T> GetAsync<T>(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"HTTP GET failed: {response.StatusCode} - {response.ReasonPhrase}");
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var envelope = JsonConvert.DeserializeObject<Envelope<T>>(jsonResponse);
                return envelope.Data;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error during HTTP GET: {ex.Message}", ex);
            }
        }

        public async Task<T> PostAsync<T, U>(string url, U body)
        {
            try
            {
                JsonContent jsonBody = JsonContent.Create(body);

                var response = await _httpClient.PostAsync(url, jsonBody);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"HTTP POST failed: {response.StatusCode} - {response.ReasonPhrase}");
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var envelope = JsonConvert.DeserializeObject<Envelope<T>>(jsonResponse);
                return envelope.Data;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error during HTTP POST: {ex.Message}", ex);
            }
        }
    }
}
