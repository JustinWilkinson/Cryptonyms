using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Cryptonyms.Client.Services
{
    /// <summary>
    /// Provides an interface for interacting with the server over HTTP.
    /// </summary>
    public interface IHttpService
    {
        Task<T> GetAsync<T>(string requestUri);

        Task<T> PostAsync<T>(string requestUri, object postObject);

        Task PostAsync(string requestUri, object postObject);

        Task<T> PutAsync<T>(string requestUri, object putObject);

        Task PutAsync(string requestUri, object putObject);

        Task PatchAsync(string requestUri, object patchObject);

        Task<T> PatchAsync<T>(string requestUri, object patchObject);

        Task DeleteAsync(string requestUri);
    }

    /// <summary>
    /// Class used for interacting with the server over HTTP.
    /// </summary>
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

        public Task PostAsync(string requestUri, object postObject) => _client.PostAsync(requestUri, GetJsonContent(postObject));

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

        public Task PutAsync(string requestUri, object putObject) => _client.PutAsync(requestUri, GetJsonContent(putObject));

        public async Task<T> PatchAsync<T>(string requestUri, object patchObject)
        {
            var response = await _client.PatchAsync(requestUri, GetJsonContent(patchObject)).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                return await DeserializeResponse<T>(response);
            }
            else
            {
                throw new HttpRequestException($"An error occurred during a PATCH Request. Received Status Code: {response.StatusCode} from {requestUri}.");
            }
        }

        public Task PatchAsync(string requestUri, object patchObject) => _client.PatchAsync(requestUri, GetJsonContent(patchObject));

        public Task DeleteAsync(string requestUri) => _client.DeleteAsync(requestUri);

        private static JsonContent GetJsonContent(object content) => JsonContent.Create(content ?? "null", content?.GetType() ?? typeof(string));

        private static async Task<T> DeserializeResponse<T>(HttpResponseMessage response) => JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
    }
}