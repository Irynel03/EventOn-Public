using EventOn.API.Models;
using EventOn.BusinessLogic.Helpers;
using EventOn.BusinessLogic.Models;
using EventOn.Interfaces;
using Microsoft.AspNetCore.Components;

namespace EventOn.Components;

public partial class EventBrowse
{
    [Inject]
    private NavigationManager Navigation { get; set; }
    [Inject]
    private IEventOnApiService _eventOnApiService { get; set; }

    [Parameter]
    public EventBrowseModel EventData { get; set; } = new EventBrowseModel();
    [Parameter]
    public Event Event
    {
        get => _event;
        set
        {
            _event = value;
            if (_event != null)
            {
                EventData.SetDataFromEvent(_event);
                _eventLiked = _event.UsersWhoLiked?.Contains(UserData.Instance.Username) ?? false;
            }
        }
    }
    [Parameter]
    public bool IsRecommended { get; set; } = false;

    private Event _event;
    private Toast toast;
    private bool _eventLiked;

    protected override async Task OnInitializedAsync()
    {
        EventData.SetDataFromEvent(Event);
        _eventLiked = Event.UsersWhoLiked.Contains(UserData.Instance.Username);
    }

    private async Task LikePost()
    {
        ToggleLike();

        var result = await _eventOnApiService.LikeEvent(Event.Id, UserData.Instance.Username);
        if (result.HasErrors)
        {
            toast.ShowToast(result.ErrorMessages);
            ToggleLike();
        }
    }

    private void ToggleLike()
    {
        if (!_eventLiked)
        {
            EventData.Likes++;
            _eventLiked = true;
        }
        else
        {
            EventData.Likes--;
            _eventLiked = false;
        }
    }

    private void NavigateToEventPage()
    {
        try
        {
            Navigation.NavigateTo($"/event/{EventData.Id}");
        }
        catch (Exception ex)
        {
            toast.ShowToast(ex.Message);
        }
    }

    private static string GetFormattedDate(DateTime startDate, DateTime endDate)
    {
        var currentYear = DateTime.Now.Year;

        if (startDate.Date == endDate.Date)
        {
            return startDate.Year == currentYear
                ? $"{startDate:d MMMM}"
                : $"{startDate:d MMMM yyyy}";
        }
        else if (startDate.Month == endDate.Month)
        {
            return startDate.Year == currentYear
                ? $"{startDate:d}-{endDate:d} {startDate:MMMM}"
                : $"{startDate:d}-{endDate:d} {startDate:MMMM yyyy}";
        }
        else
        {
            return startDate.Year == currentYear
                ? $"{startDate:d MMMM} - {endDate:d MMMM}"
                : $"{startDate:d MMMM yyyy} - {endDate:d MMMM yyyy}";
        }
    }
}