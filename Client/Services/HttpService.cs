using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Cryptonyms.Client.Services
{
    public interface IHttpService
    {
        Task<T> GetAsync<T>(string requestUri);

        Task<T> PostAsync<T>(string requestUri, object postObject);

        Task PostAsync(string requestUri, object postObject);

        Task<T> PutAsync<T>(string requestUri, object putObject);

        Task PutAsync(string requestUri, object putObject);

        Task DeleteAsync(string requestUri);
    }


    public class HttpService : IHttpService
    {
        private readonly HttpClient _client;

        public HttpService(string baseUri)
        {
            _client = new HttpClient { BaseAddress = new Uri(baseUri) };
        }

        public async Task<T> GetAsync<T>(string requestUri)
        {
            var responseContent = await _client.GetStringAsync(requestUri).ConfigureAwait(false);
            try
            {
                return JsonConvert.DeserializeObject<T>(responseContent);
            }
            catch
            {
                throw new HttpRequestException($"Unexpected response content received on GET Request to {requestUri}. Content: {responseContent}");
            }
        }

        public async Task<T> PostAsync<T>(string requestUri, object postObject)
        {
            var response = await _client.PostAsync(requestUri, GetJsonContent(postObject)).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                return await DeserializeResponse<T>(response);
            }
            else
            {
                throw new HttpRequestException($"An error occurred during a POST Request. Received Status Code: {response.StatusCode} from {requestUri}.");
            }
        }

        public async Task PostAsync(string requestUri, object postObject) => await _client.PostAsync(requestUri, GetJsonContent(postObject)).ConfigureAwait(false);

        public async Task<T> PutAsync<T>(string requestUri, object putObject)
        {
            var response = await _client.PutAsync(requestUri, GetJsonContent(putObject)).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                return await DeserializeResponse<T>(response);
            }
            else
            {
                throw new HttpRequestException($"An error occurred during a PUT Request. Received Status Code: {response.StatusCode} from {requestUri}.");
            }
        }

        public async Task PutAsync(string requestUri, object putObject) => await _client.PutAsync(requestUri, GetJsonContent(putObject)).ConfigureAwait(false);

        public async Task DeleteAsync(string requestUri) => await _client.DeleteAsync(requestUri);

        private JsonContent GetJsonContent(object content) => JsonContent.Create(content ?? "null", content?.GetType() ?? typeof(string));

        private async Task<T> DeserializeResponse<T>(HttpResponseMessage response) => JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
    }
}