using EventOn.API.Models;
using EventOn.BusinessLogic.Helpers;
using EventOn.Interfaces;
using Microsoft.AspNetCore.Components;

namespace EventOn.Pages;

public partial class FavoriteEventsPage
{
    [Inject]
    private IEventOnApiService _eventOnApiService { get; set; }

    private List<Event> _favoriteEvents;
    private bool _isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        _isLoading = true;
        _favoriteEvents = [];

        var getFollowedEventsResult = await _eventOnApiService.GetFavoriteEvents(UserData.Instance.Username);
        if (getFollowedEventsResult.HasErrors)
        {
            _isLoading = false;
            return;
        }

        _favoriteEvents = getFollowedEventsResult.Data;
        _isLoading = false;
        StateHasChanged();
    }

    private void NavigateToEvent(string eventId)
    {
        Navigation.NavigateTo($"/event/{eventId}");
    }

    private static string GetFormattedDate(DateTime start, DateTime end) => $"{start:MMM dd, yyyy} - {end:MMM dd, yyyy}";
}