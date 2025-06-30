using EventOn.API.Models;
using EventOn.API.Models.Enums;
using EventOn.API.Models.Stripe;
using EventOn.BusinessLogic.Helpers;

namespace EventOn.Interfaces;

public interface IEventOnApiService
{
    //Events
    Task<Result<List<Event>>> GetAllEvents();
    Task<Result<List<Event>>> GetAllUpcomingEvents();
    Task<Result<List<Event>>> GetAllEventsFromUser(string username);
    Task<Result<Event>> GetEvent(string id);
    Task<Result<List<Event>>> GetFavoriteEvents(string username);
    Task<Result> CreateEvent(Event e, List<byte[]> photos);
    Task<Result> DeleteEvent(string id);
    //Task<List<Event>> GetEventsOfCategoryAsync(EventCategory category);
    //Task UpdateEventAsync(string id, Event updatedEvent);
    Task<Result> LikeEvent(string eventId, string userName);
    Task<Result> LikeComment(string eventId, string userName, string commentId);
    Task<Result> PostComment(string eventId, string userName, string message);
    Task<Result> BuyTicket(PaymentRequest paymentRequest);

    //Orders
    Task<Result<List<Order>>> GetAllOrdersForUser(string username);
    Task<Result<List<Order>>> GetAllOrdersForEvent(string eventId);

    //Users
    Task<Result> CreateUser(User user);
    Task<Result<List<User>>> GetAllUsers();
    Task<Result<List<User>>> GetUsersOfTypeAsync(TypeOfUser userType);
    Task<Result<User>> GetUser(string username);
    Task<Result> AddMoneyToUser(string username, decimal amount);
    Task<Result> AddUserOneSignalId(string username, string playerId);
    Task<Result> UpdateUserPassword(string username, string newPassword);
    Task<Result> UpdateUserUsername(string oldUsername, string newUsername);
    Task<Result<LoginResponse>> LoginUser(UserLoginRequestData userRequestData);
    Task<Result> AddToFavourites(string eventId, string userName);
    Task<Result> TogglePerformerFollow(string username, string performerUsername);
    Task<Result> RequestPasswordChange(string username);
    Task<Result> ConfirmNewPassword(string username, PasswordResetRequest passwordResetRequest);
    Task<Result<UserPreferences>> GetUserPreferences(string username);
    Task<Result> UpdateUserPreferences(string username, UserPreferences userPreferences);

    //Stripe
    Task<Result> CreateCard(string username);

    //GoogleGemini
    Task<Result<Event>> GetRecommendedEvent(string username);
    Task<Result<string>> GetChatbotResponse(string username, List<string> conversation);

    //MLRecomandations
    Task<Result<Event>> GetMLRecommendedEvent(string username);
}