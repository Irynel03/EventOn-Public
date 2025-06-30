using EventOn.API.Models;
using EventOn.API.Models.Enums;
using EventOn.API.Models.Stripe;
using EventOn.BusinessLogic.Helpers;
using EventOn.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using System.Net.Http.Headers;
using System.Text;

namespace EventOn.API.Client;

public class EventOnApiService : IEventOnApiService
{
    private const int RetryCount = 1;

    private readonly AsyncRetryPolicy _retryPolicy = Policy.Handle<HttpRequestException>().WaitAndRetryAsync(RetryCount, retryAttempt => TimeSpan.FromSeconds(retryAttempt));
    private readonly HttpClient _client;

    public EventOnApiService(IOptions<EventOnApiSettings> settings)
    {
        _client = new HttpClient()
        {
            BaseAddress = new Uri($"{settings.Value.EventOnApiUri}/")
        };

        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _client.DefaultRequestHeaders.AcceptLanguage.TryParseAdd(Thread.CurrentThread.CurrentUICulture.Name);
    }

    //---------------------------------- Events ------------------------------------------------
    public async Task<Result<Event>> GetEvent(string id)
    {
        var result = new Result<Event>(new());
        await SetAuthorizationHeader();

        var content = new StringContent(JsonConvert.SerializeObject(string.Empty));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await _retryPolicy.ExecuteAsync(async () => await _client.GetAsync($"Event/{id}"));
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            result.AddError(responseContent);
            return result;
        }

        try
        {
            result.Data = JsonConvert.DeserializeObject<Event>(responseContent);
        }
        catch (JsonException ex)
        {
            result.AddError($"Failed to deserialize response: {ex.Message}");
        }

        return result;
    }

    public async Task<Result<List<Event>>> GetFavoriteEvents(string username)
    {
        var result = new Result<List<Event>>([]);
        await SetAuthorizationHeader();

        var response = await _retryPolicy.ExecuteAsync(async () => await _client.GetAsync($"Event/favorite-events/{username}"));
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            result.AddError(responseContent);
            return result;
        }

        try
        {
            result.Data = JsonConvert.DeserializeObject<List<Event>>(responseContent);
        }
        catch (JsonException ex)
        {
            result.AddError($"Failed to deserialize response: {ex.Message}");
        }

        return result;
    }

    public async Task<Result<List<Event>>> GetAllEvents()
    {
        var result = new Result<List<Event>>([]);
        await SetAuthorizationHeader();

        var response = await _retryPolicy.ExecuteAsync(async () => await _client.GetAsync("Event/events"));
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            result.AddError(responseContent);
            return result;
        }

        try
        {
            result.Data = JsonConvert.DeserializeObject<List<Event>>(responseContent);
        }
        catch (JsonException ex)
        {
            result.AddError($"Failed to deserialize response: {ex.Message}");
        }

        return result;
    }

    public async Task<Result<List<Event>>> GetAllUpcomingEvents()
    {
        var result = new Result<List<Event>>([]);
        await SetAuthorizationHeader();

        var response = await _retryPolicy.ExecuteAsync(async () => await _client.GetAsync("Event/upcoming-events"));
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            result.AddError(responseContent);
            return result;
        }

        try
        {
            result.Data = JsonConvert.DeserializeObject<List<Event>>(responseContent);
        }
        catch (JsonException ex)
        {
            result.AddError($"Failed to deserialize response: {ex.Message}");
        }

        return result;
    }

    public async Task<Result<List<Event>>> GetAllEventsFromUser(string username)
    {
        var result = new Result<List<Event>>([]);
        await SetAuthorizationHeader();

        var response = await _retryPolicy.ExecuteAsync(async () => await _client.GetAsync($"Event/user/{username}"));
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            result.AddError(responseContent);
            return result;
        }

        try
        {
            result.Data = JsonConvert.DeserializeObject<List<Event>>(responseContent);
        }
        catch (JsonException ex)
        {
            result.AddError($"Failed to deserialize response: {ex.Message}");
        }

        return result;
    }

    public async Task<Result> CreateEvent(Event e, List<byte[]> photos)
    {
        var result = new Result();
        await SetAuthorizationHeader();

        using var multiContent = new MultipartFormDataContent();

        var eventJson = JsonConvert.SerializeObject(e);
        var jsonContent = new StringContent(eventJson, Encoding.UTF8, "application/json");
        multiContent.Add(jsonContent, "event");

        int index = 0;
        foreach (var photo in photos)
        {
            var imageContent = new ByteArrayContent(photo);
            imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
            multiContent.Add(imageContent, "photos", $"photo{index++}.jpg");
        }

        var response = await _retryPolicy.ExecuteAsync(async () => await _client.PostAsync("Event", multiContent));
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            result.AddError(responseContent);
        }

        return result;
    }

    public async Task<Result> DeleteEvent(string id)
    {
        var result = new Result();
        await SetAuthorizationHeader();

        var response = await _retryPolicy.ExecuteAsync(async () => await _client.DeleteAsync($"Event/{id}"));
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            result.AddError(responseContent);
        }

        return result;
    }

    public async Task<Result> LikeEvent(string eventId, string userName)
    {
        var result = new Result();
        await SetAuthorizationHeader();

        var content = new StringContent(JsonConvert.SerializeObject(string.Empty));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await _retryPolicy.ExecuteAsync(async () => await _client.PostAsync($"Event/like-event/{eventId}/{userName}", content));
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            result.AddError(responseContent);
        }

        return result;
    }

    public async Task<Result> LikeComment(string eventId, string userName, string commentId)
    {
        var result = new Result();
        await SetAuthorizationHeader();

        var content = new StringContent(JsonConvert.SerializeObject(string.Empty));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await _retryPolicy.ExecuteAsync(async () => await _client.PostAsync($"Event/like-comment/{eventId}/{userName}/{commentId}", content));
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            result.AddError(responseContent);
        }

        return result;
    }

    public async Task<Result> PostComment(string eventId, string userName, string message)
    {
        var result = new Result();
        await SetAuthorizationHeader();

        var content = new StringContent(JsonConvert.SerializeObject(string.Empty));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await _retryPolicy.ExecuteAsync(async () => await _client.PostAsync($"Event/post-comment/{eventId}/{userName}/{message}", content));
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            result.AddError(responseContent);
        }

        return result;
    }

    public async Task<Result> BuyTicket(PaymentRequest paymentRequest)
    {
        var result = new Result();
        await SetAuthorizationHeader();

        var content = new StringContent(JsonConvert.SerializeObject(paymentRequest));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await _retryPolicy.ExecuteAsync(async () => await _client.PostAsync($"Event/buy-ticket", content));
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            result.AddError(responseContent);
        }

        return result;
    }

    //------------------------------- Orders-----------------------

    public async Task<Result<List<Order>>> GetAllOrdersForUser(string username)
    {
        var result = new Result<List<Order>>([]);
        await SetAuthorizationHeader();

        var response = await _retryPolicy.ExecuteAsync(async () => await _client.GetAsync($"Order/user/{username}"));
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            result.AddError(responseContent);
            return result;
        }

        try
        {
            result.Data = JsonConvert.DeserializeObject<List<Order>>(responseContent);
        }
        catch (JsonException ex)
        {
            result.AddError($"Failed to deserialize response: {ex.Message}");
        }

        return result;
    }

    public async Task<Result<List<Order>>> GetAllOrdersForEvent(string eventId)
    {
        var result = new Result<List<Order>>([]);
        await SetAuthorizationHeader();

        var response = await _retryPolicy.ExecuteAsync(async () => await _client.GetAsync($"Order/event/{eventId}"));
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            result.AddError(responseContent);
            return result;
        }

        try
        {
            result.Data = JsonConvert.DeserializeObject<List<Order>>(responseContent);
        }
        catch (JsonException ex)
        {
            result.AddError($"Failed to deserialize response: {ex.Message}");
        }

        return result;
    }

//----------------------------------- Users --------------------------------------------

public async Task<Result> CreateUser(User user)
    {
        var result = new Result();

        var content = new StringContent(JsonConvert.SerializeObject(user));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await _retryPolicy.ExecuteAsync(async () => await _client.PostAsync($"User", content));
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            result.AddError(responseContent);
        }

        return result;
    }

    public async Task<Result<List<User>>> GetAllUsers()
    {
        var result = new Result<List<User>>([]);
        await SetAuthorizationHeader();

        var response = await _retryPolicy.ExecuteAsync(async () => await _client.GetAsync("User/users"));
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            result.AddError(responseContent);
            return result;
        }

        try
        {
            result.Data = JsonConvert.DeserializeObject<List<User>>(responseContent);
        }
        catch (JsonException ex)
        {
            result.AddError($"Failed to deserialize response: {ex.Message}");
        }

        return result;
    }

    public async Task<Result<List<User>>> GetUsersOfTypeAsync(TypeOfUser userType)
    {
        var result = new Result<List<User>>([]);
        await SetAuthorizationHeader();

        var response = await _retryPolicy.ExecuteAsync(async () => await _client.GetAsync($"User/users/{userType}"));
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            result.AddError(responseContent);
            return result;
        }

        try
        {
            result.Data = JsonConvert.DeserializeObject<List<User>>(responseContent);
        }
        catch (JsonException ex)
        {
            result.AddError($"Failed to deserialize response: {ex.Message}");
        }

        return result;
    }

    public async Task<Result<User>> GetUser(string username)
    {
        var result = new Result<User>(new());
        await SetAuthorizationHeader();

        var response = await _retryPolicy.ExecuteAsync(async () => await _client.GetAsync($"User/{username}"));
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            result.AddError(responseContent);
            return result;
        }

        try
        {
            result.Data = JsonConvert.DeserializeObject<User>(responseContent);
        }
        catch (JsonException ex)
        {
            result.AddError($"Failed to deserialize response: {ex.Message}");
        }

        return result;
    }

    public async Task<Result> AddUserOneSignalId(string username, string playerId)
    {
        var result = new Result();
        await SetAuthorizationHeader();

        var content = new StringContent(JsonConvert.SerializeObject(string.Empty));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await _retryPolicy.ExecuteAsync(async () => await _client.PostAsync($"User/new-playerId-onesignal/{username}/{playerId}", content));
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            result.AddError(responseContent);
        }

        return result;
    }

    public async Task<Result> UpdateUserPassword(string username, string newPassword)
    {
        var result = new Result();
        await SetAuthorizationHeader();

        var content = new StringContent(JsonConvert.SerializeObject(newPassword));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await _retryPolicy.ExecuteAsync(async () => await _client.PutAsync($"User/newPassword/{username}", content));
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            result.AddError(responseContent);
        }

        return result;
    }

    public async Task<Result> UpdateUserUsername(string oldUsername, string newUsername)
    {
        var result = new Result();
        await SetAuthorizationHeader();

        var content = new StringContent(JsonConvert.SerializeObject(newUsername));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await _retryPolicy.ExecuteAsync(async () => await _client.PutAsync($"User/newUsername/{oldUsername}", content));
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            result.AddError(responseContent);
        }

        return result;
    }

    public async Task<Result> AddMoneyToUser(string username, decimal amount)
    {
        var result = new Result();
        await SetAuthorizationHeader();

        var content = new StringContent(JsonConvert.SerializeObject(amount));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await _retryPolicy.ExecuteAsync(async () => await _client.PostAsync($"User/add-money/{username}", content));
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            result.AddError(responseContent);
        }

        return result;
    }

    public async Task<Result<LoginResponse>> LoginUser(UserLoginRequestData userRequestData)
    {
        var result = new Result<LoginResponse>(new());

        var content = new StringContent(JsonConvert.SerializeObject(userRequestData));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await _retryPolicy.ExecuteAsync(async () => await _client.PostAsync($"User/login", content));
        var responseContent = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            result.AddError(responseContent);
            return result;
        }

        try
        {
            result.Data = JsonConvert.DeserializeObject<LoginResponse>(responseContent);
        }
        catch (JsonException ex)
        {
            result.AddError($"Failed to deserialize response: {ex.Message}");
        }

        return result;
    }

    public async Task<Result> AddToFavourites(string eventId, string userName)
    {
        var result = new Result();
        await SetAuthorizationHeader();

        var content = new StringContent(JsonConvert.SerializeObject(string.Empty));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await _retryPolicy.ExecuteAsync(async () => await _client.PostAsync($"User/add-to-favourites/{eventId}/{userName}", content));
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            result.AddError(responseContent);
        }

        return result;
    }

    public async Task<Result> TogglePerformerFollow(string username, string performerUsername)
    {
        var result = new Result();
        await SetAuthorizationHeader();

        var content = new StringContent(JsonConvert.SerializeObject(string.Empty));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await _retryPolicy.ExecuteAsync(async () => await _client.PostAsync($"User/toggle-performer-follow/{username}/{performerUsername}", content));
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            result.AddError(responseContent);
        }

        return result;
    }

    public async Task<Result> RequestPasswordChange(string username)
    {
        var result = new Result();
        await SetAuthorizationHeader();

        var content = new StringContent(JsonConvert.SerializeObject(string.Empty));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await _retryPolicy.ExecuteAsync(async () => await _client.PostAsync($"User/request-password-change-code/{username}", content));
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            result.AddError(responseContent);
        }

        return result;
    }

    public async Task<Result> ConfirmNewPassword(string username, PasswordResetRequest passwordResetRequest)
    {
        var result = new Result();
        await SetAuthorizationHeader();

        var content = new StringContent(JsonConvert.SerializeObject(passwordResetRequest));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await _retryPolicy.ExecuteAsync(async () => await _client.PostAsync($"User/confirm-new-password-request/{username}", content));
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            result.AddError(responseContent);
        }

        return result;
    }

    public async Task<Result<UserPreferences>> GetUserPreferences(string username)
    {
        var result = new Result<UserPreferences>(null);
        await SetAuthorizationHeader();

        var response = await _retryPolicy.ExecuteAsync(async () => await _client.GetAsync($"User/preferences/{username}"));
        var responseContent = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            result.AddError(responseContent);
            return result;
        }

        try
        {
            result.Data = JsonConvert.DeserializeObject<UserPreferences>(responseContent);
        }
        catch (JsonException ex)
        {
            result.AddError($"Failed to deserialize response: {ex.Message}");
        }

        return result;
    }

    public async Task<Result> UpdateUserPreferences(string username, UserPreferences userPreferences)
    {
        var result = new Result();
        await SetAuthorizationHeader();

        var content = new StringContent(JsonConvert.SerializeObject(userPreferences));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await _retryPolicy.ExecuteAsync(async () => await _client.PutAsync($"User/update-preferences/{username}", content));
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            result.AddError(responseContent);
        }

        return result;
    }

    //---------------------------- Stripe ------------------------------------

    public async Task<Result> CreateCard(string username)
    {
        var result = new Result();
        await SetAuthorizationHeader();

        var content = new StringContent(JsonConvert.SerializeObject(string.Empty));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await _retryPolicy.ExecuteAsync(async () => await _client.PostAsync($"Stripe/create-card/{username}", content));
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            result.AddError(responseContent);
        }

        return result;
    }

    //---------------------------- Google Gemini -----------------------------------

    public async Task<Result<Event>> GetRecommendedEvent(string username)
    {
        var result = new Result<Event>(null);
        await SetAuthorizationHeader();

        var response = await _retryPolicy.ExecuteAsync(async () => await _client.GetAsync($"GoogleGemini/get-recommended-event/{username}"));
        var responseContent = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            result.AddError(responseContent);
        else
        {
            try
            {
                result.Data = JsonConvert.DeserializeObject<Event>(responseContent);
            }
            catch (JsonException ex)
            {
                result.AddError($"Failed to deserialize response: {ex.Message}");
            }
        }

        return result;
    }

    public async Task<Result<string>> GetChatbotResponse(string username, List<string> conversation)
    {
        var result = new Result<string>(string.Empty);
        await SetAuthorizationHeader();

        var content = new StringContent(JsonConvert.SerializeObject(conversation));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await _retryPolicy.ExecuteAsync(async () => await _client.PostAsync($"GoogleGemini/generate-message/{username}", content));
        var responseContent = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            result.AddError(responseContent);

        try
        {
            result.Data = JsonConvert.DeserializeObject<string>(responseContent);
        }
        catch (JsonException ex)
        {
            result.AddError($"Failed to deserialize response: {ex.Message}");
        }

        return result;
    }

    //---------------------------- MLRecomandations -----------------------------------

    public async Task<Result<Event>> GetMLRecommendedEvent(string username)
    {
        var result = new Result<Event>(null);
        await SetAuthorizationHeader();

        var response = await _retryPolicy.ExecuteAsync(async () => await _client.GetAsync($"MLRec/recommended-event/{username}"));
        var responseContent = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            result.AddError(responseContent);
        else
        {
            try
            {
                result.Data = JsonConvert.DeserializeObject<Event>(responseContent);
            }
            catch (JsonException ex)
            {
                result.AddError($"Failed to deserialize response: {ex.Message}");
            }
        }

        return result;
    }

    //-------------- Private methods ---------------

    private async Task SetAuthorizationHeader()
    {
        var token = await SecureStorage.GetAsync("jwt_token");
        if (!string.IsNullOrWhiteSpace(token))
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        else
            throw new UnauthorizedAccessException("No JWT token found. Please log in again.");
    }
}