using EventOn.API.Models;
using EventOn.API.Models.Stripe;
using EventOn.BusinessLogic.Helpers;
using EventOn.Components;
using EventOn.Interfaces;
using Microsoft.AspNetCore.Components;
using System.Diagnostics;
using System.Globalization;

namespace EventOn.Pages;

public partial class EventPage
{
    [Inject]
    private IEventOnApiService _eventOnApiService { get; set; }
    [Inject]
    private IUserLocalDataService _localDataService { get; set; }
    [Inject]
    private NavigationManager NavigationManager { get; set; }

    [Parameter]
    public string EventId { get; set; }

    private string NewCommentContent { get; set; } = string.Empty;
    private Toast toast;
    private bool _performersCollapsed = true;
    private bool _isBuyingTicket = false;
    private Event _selectedEvent;
    private bool _isFavorite;

    private static string GetFormattedDate(DateTime start, DateTime end) => $"{start:MMM dd, yyyy} - {end:MMM dd, yyyy}";

    protected override async Task OnParametersSetAsync()
    {
        var getEventResult = await _eventOnApiService.GetEvent(EventId);
        if (getEventResult.Data == null)
        {
            toast.ShowToast(getEventResult.ErrorMessages);
            return;
        }

        _selectedEvent = getEventResult.Data;
        var getUserResult = await _eventOnApiService.GetUser(UserData.Instance.Username);
        if (getUserResult.Data == null)
        {
            toast.ShowToast(getUserResult.ErrorMessages);
            return;
        }

        _isFavorite = getUserResult.Data.FollowedEvents.Contains(EventId);
    }

    private async Task OpenLocationInMap()
    {
        var platform = DeviceInfo.Current.Platform;

        if (platform == DevicePlatform.WinUI || platform == DevicePlatform.macOS || platform == DevicePlatform.MacCatalyst)
        {
            var urlToOpen = $"https://www.google.com/maps/place/?q=place_id:{Uri.EscapeDataString(_selectedEvent.Location.PlaceId)}";

            try
            {
                NavigationManager.NavigateTo(urlToOpen, forceLoad: true);
            }
            catch (Exception ex)
            {
                toast.ShowToast($"Could not open map link in browser: {ex.Message}");
            }
        }
        else if (platform == DevicePlatform.Android || platform == DevicePlatform.iOS)
        {
            try
            {
                await Map.OpenAsync(_selectedEvent.Location.Latitude, _selectedEvent.Location.Longitude, new MapLaunchOptions
                {
                    Name = _selectedEvent.Location.Name,
                    NavigationMode = NavigationMode.None
                });
            }
            catch (Exception ex)
            {
                toast.ShowToast($"Could not open map application: {ex.Message}");
            }
        }
        else
        {
            toast.ShowToast("Map opening not supported on this platform.");
        }
    }

    private async Task AddToFavourites()
    {
        ToggleFavorite();
        var addToFavouritesResult = await _eventOnApiService.AddToFavourites(EventId, UserData.Instance.Username);
        if (!addToFavouritesResult.HasErrors)
        {
            toast.ShowToast("Added To favourites", false);
        }
        else
        {
            toast.ShowToast(addToFavouritesResult.ErrorMessages);
            ToggleFavorite();
        }
    }

    private async Task PostComment()
    {
        if (string.IsNullOrWhiteSpace(NewCommentContent))
        {
            toast.ShowToast("Comment cannot be empty");
            return;
        }

        var result = await _eventOnApiService.PostComment(EventId, UserData.Instance.Username, NewCommentContent);
        if (result.HasErrors)
        {
            toast.ShowToast(result.ErrorMessages);
            return;
        }

        var newComment = new Comment()
        {
            Content = NewCommentContent,
            UserName = UserData.Instance.Username
        };

        _selectedEvent.Comments.Add(newComment);
        NewCommentContent = string.Empty;
    }

    private async Task LikeComment(Comment comment)
    {
        var likeCommentResult = await _eventOnApiService.LikeComment(EventId, UserData.Instance.Username, comment.Id);
        if (likeCommentResult.HasErrors)
        {
            toast.ShowToast(likeCommentResult.ErrorMessages);
            return;
        }

        if (comment.LikedByUsers.Contains(UserData.Instance.Username))
            comment.LikedByUsers.Remove(UserData.Instance.Username);
        else
            comment.LikedByUsers.Add(UserData.Instance.Username);
    }

    private async Task BuyTicket()
    {
        if (!UserData.Instance.HasCard)
        {
            toast.ShowToast("You need to add a card first");
            return;
        }

        _isBuyingTicket = true;

        var paymentRequest = new PaymentRequest()
        {
            Username = UserData.Instance.Username,
            EventId = EventId,
            Amount = (decimal)_selectedEvent.Price,
        };

        var buyTicketResult = await _eventOnApiService.BuyTicket(paymentRequest);
        if (!buyTicketResult.HasErrors)
            toast.ShowToast("Ticket bought\n check email", false);
        else
            toast.ShowToast(buyTicketResult.ErrorMessages);

        var getOrdersResult = await _eventOnApiService.GetAllOrdersForUser(UserData.Instance.Username);
        if (!getOrdersResult.HasErrors)
        {
            _localDataService.ClearOrders();
            _localDataService.SaveOrders(getOrdersResult.Data);
        }

        _isBuyingTicket = false;
    }

    private async void ToggleFollowPerformer(string performerName)
    {
        var result = await _eventOnApiService.TogglePerformerFollow(UserData.Instance.Username, performerName);
        if (!result.HasErrors)
        {
            if (!UserData.Instance.FollowedPerformers.Remove(performerName))
            {
                UserData.Instance.FollowedPerformers.Add(performerName);
                StateHasChanged();
            }
        }
        else
            toast.ShowToast(result.ErrorMessages);
    }

    private void ToggleFavorite()
    {
        _isFavorite = !_isFavorite;
    }

    private void TogglePerformers()
    {
        _performersCollapsed = !_performersCollapsed;
    }
}