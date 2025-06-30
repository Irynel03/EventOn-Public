using EventOn.Interfaces;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace EventOn.BusinessLogic.Services;

public class OneSignalService : IOneSignalService
{
    public async Task<List<string>> GetPlayerIdsByExternalIdAsync(string externalUserId)
    {
        var url = $"https://onesignal.com/api/v1/players?app_idkjkjkjjjjjjjjjjjjjjjjjjj";
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "os_v2_app_mjnzxbi3qbenxe6l5yb3xxwrdwz2jtgm25guonn7rcg4apsj5pj7vfkfwibtxch7gt776cmyqjd6ve32xmoaa75ifslkxa6vp4mixwq");

        var response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Failed to retrieve devices: " + response.StatusCode);
        }
        var content = await response.Content.ReadAsStringAsync();
        var devicesResponse = JsonConvert.DeserializeObject<OneSignalDevicesResponse>(content);

        var devices = devicesResponse.players.Where(p => p.external_user_id == externalUserId);
        return devices.Select(d => d.id).ToList();
    }

    public class OneSignalDevice
    {
        public string id { get; set; }
        public string external_user_id { get; set; }
    }

    public class OneSignalDevicesResponse
    {
        public List<OneSignalDevice> players { get; set; }
    }
}