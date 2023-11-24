using Newtonsoft.Json;
using System.Text;

namespace PokerOffline.Services.ApiClient
{
    public class ApiClient : IApiClient
    {
        private HttpClient _httpClient;

        public ApiClient(string baseUrl)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);
        }

        public void SetBaseUrl(string baseUrl)
        {
            _httpClient.Dispose();
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);
        }

        public async Task<HttpResponseMessage> GetAsync(string endpoint, Dictionary<string, object> queryParams = null)
        {
            var uriBuilder = new UriBuilder(_httpClient.BaseAddress + endpoint);

            if (queryParams != null && queryParams.Count > 0)
            {
                var query = new StringBuilder();
                foreach (var param in queryParams)
                {
                    if (query.Length > 0)
                    {
                        query.Append('&');
                    }

                    query.Append($"{param.Key}={param.Value}");
                }

                uriBuilder.Query = query.ToString();
            }

            return await _httpClient.GetAsync(uriBuilder.Uri);
        }


        public async Task<HttpResponseMessage> PostAsync(string endpoint, HttpContent content)
        {
            return await _httpClient.PostAsync(endpoint, content);
        }

        public async Task<ICollection<string>> GetRooms()
        {
            var endpoint = "/rooms";

            var response = await GetAsync(endpoint);

            if (!response.IsSuccessStatusCode)
                return new List<string>();

            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ICollection<string>>(result);
        }

        public async Task<bool> CreateRoom(string roomName)
        {
            var endpoint = "/createRoom";

            var param = new Dictionary<string, object>() {
                {"name", roomName},
            };

            var response = await GetAsync(endpoint, param);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> JoinRoom(string roomName)
        {
            var endpoint = "joinRoom";

            var param = new Dictionary<string, object>() {
                {"name", roomName},
            };

            var response = await GetAsync(endpoint, param);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ExitRoom(string roomName)
        {
            var endpoint = "/exitRoom";

            var param = new Dictionary<string, object>() {
                {"name", roomName},
            };

            var response = await GetAsync(endpoint, param);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> StartGame(string roomName)
        {
            var endpoint = "/startGame";

            var param = new Dictionary<string, object>() {
                {"name", roomName},
            };

            var response = await GetAsync(endpoint, param);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RestartGame(string roomName)
        {
            var endpoint = "/restartGame";

            var param = new Dictionary<string, object>() {
                {"name", roomName},
            };

            var response = await GetAsync(endpoint, param);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> FinishGame(string roomName)
        {
            var endpoint = "/finishGame";

            var param = new Dictionary<string, object>() {
                {"name", roomName},
            };

            var response = await GetAsync(endpoint, param);

            return response.IsSuccessStatusCode;
        }

        public async Task<string> GetStatus(string roomName)
        {
            var endpoint = "/getStatus";

            var param = new Dictionary<string, object>() {
                {"name", roomName},
            };

            var response = await GetAsync(endpoint, param);

            if (!response.IsSuccessStatusCode)
                return "";

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetHand(string roomName)
        {
            var endpoint = "/getHand";

            var param = new Dictionary<string, object>() {
                {"name", roomName},
            };

            var response = await GetAsync(endpoint, param);

            if (!response.IsSuccessStatusCode)
                return "";

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetTable(string roomName)
        {
            var endpoint = "/getTable";

            var param = new Dictionary<string, object>() {
                {"name", roomName},
            };

            var response = await GetAsync(endpoint, param);

            if (!response.IsSuccessStatusCode)
                return "";

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<bool> Flop(string roomName)
        {
            var endpoint = "/flop";

            var param = new Dictionary<string, object>() {
                {"name", roomName},
            };

            var response = await GetAsync(endpoint, param);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> Turn(string roomName)
        {
            var endpoint = "/turn";

            var param = new Dictionary<string, object>() {
                {"name", roomName},
            };

            var response = await GetAsync(endpoint, param);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> River(string roomName)
        {
            var endpoint = "/river";

            var param = new Dictionary<string, object>() {
                {"name", roomName},
            };

            var response = await GetAsync(endpoint, param);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DoAction(string roomName, string action)
        {
            var param = new Dictionary<string, object>() {
                {"name", roomName},
            };

            var response = await GetAsync(action, param);

            return response.IsSuccessStatusCode;
        }

        public async Task<int> GetCountPeople(string roomName)
        {
            var endpoint = "getCountPeople";

            var param = new Dictionary<string, object>() {
                {"name", roomName},
            };

            var response = await GetAsync(endpoint, param);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                try
                {
                    return Convert.ToInt32(result);
                }
                catch(Exception){}
            }

            return 0;
        }
    }
}
