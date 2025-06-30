using EventOn.API.Models;
using EventOn.BusinessLogic.Helpers;
using EventOn.BusinessLogic.Models;
using EventOn.Components;
using EventOn.Interfaces;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace EventOn.Pages;

public partial class Feed
{
    [Inject]
    private IEventOnApiService _eventOnApiService { get; set; }

    private static Toast toast;
    private string _searchTerm = string.Empty;
    //private static Event? _recommendedEvent;
    private static List<Event> _fullEvents = [];
    private static List<Event> _filteredEvents = [];
    private EventFilterCriteria _filterCriteria = new();
    private bool _filterApplied = false;
    private bool _isFilterVisible = false;
    private static bool _dataLoaded = false;
    private static Feed? _instance;

    protected override async Task OnInitializedAsync()
    {
        _dataLoaded = false;
        _instance = this;

        var getEventsResult = await _eventOnApiService.GetAllUpcomingEvents();
        if (getEventsResult.HasErrors)
        {
            toast.ShowToast(getEventsResult.ToString());
            return;
        }

        _fullEvents = getEventsResult.Data;
        _filteredEvents = _fullEvents;

        var getRecommendedEventResult = await _eventOnApiService.GetMLRecommendedEvent(UserData.Instance.Username);
        if (!getRecommendedEventResult.HasErrors)
        {
            var recommendedEvent = _filteredEvents.FirstOrDefault(e => e.Id == getRecommendedEventResult.Data.Id);
            if (recommendedEvent != null)
            {
                recommendedEvent.IsRecommended = true;
                _filteredEvents.Sort((x, y) =>
                {
                    if (x.IsRecommended && !y.IsRecommended) return -1;
                    if (!x.IsRecommended && y.IsRecommended) return 1;
                    return x.StartDate.CompareTo(y.StartDate);
                });
            }
        }

        _dataLoaded = true;
    }

    private void OnSearch()
    {
        _filteredEvents = [.. _fullEvents.Where(e => e.Name.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase))];
    }

    private void OnFilterApplied(EventFilterCriteria criteria)
    {
        _filterApplied = true;
        _filterCriteria = criteria;


        _filteredEvents = [.. _fullEvents.Where(e =>
            (criteria.Category == null || e.Category == criteria.Category) &&
            (!criteria.StartDate.HasValue || e.StartDate >= criteria.StartDate) &&
            (!criteria.EndDate.HasValue || e.EndDate <= criteria.EndDate) &&
            (string.IsNullOrWhiteSpace(criteria.Location) || RemoveRomanianSpecificCharacters(e.Location.Name).Contains(RemoveRomanianSpecificCharacters(criteria.Location), StringComparison.OrdinalIgnoreCase)) &&
            (!criteria.MinPrice.HasValue || e.Price >= criteria.MinPrice) &&
            (!criteria.MaxPrice.HasValue || e.Price <= criteria.MaxPrice))];

        ToggleFilter();
    }

    private static string RemoveRomanianSpecificCharacters(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        return text
            .Replace('ă', 'a')
            .Replace('â', 'a')
            .Replace('î', 'i')
            .Replace('ș', 's')
            .Replace('ț', 't')
            .Replace('Ă', 'A')
            .Replace('Â', 'A')
            .Replace('Î', 'I')
            .Replace('Ș', 'S')
            .Replace('Ț', 'T');
    }

    private void ToggleFilter()
    {
        _isFilterVisible = !_isFilterVisible;
    }

    public static void RefreshFeed()
    {
        if (_instance == null)
            return;

        _instance.InvokeAsync(async () =>
        {
            var eventsList = _filteredEvents;
            _filteredEvents = null;
            _instance.StateHasChanged();
            eventsList.Reverse();
            _filteredEvents = eventsList;
            _instance.StateHasChanged();
        });
    }
}