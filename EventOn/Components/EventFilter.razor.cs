using EventOn.API.Models.Enums;
using EventOn.BusinessLogic.Models;
using Microsoft.AspNetCore.Components;

namespace EventOn.Components;

public partial class EventFilter
{
    private EventCategory? _selectedCategory;
    private List<EventCategory> _categories = Enum.GetValues<EventCategory>().ToList(); // TODO filtrare categorie
    private DateTime? _startDate;
    private DateTime? _endDate;
    private string? _location;
    private double? _minPrice;
    private double? _maxPrice;

    private double? MinPrice
    {
        get => _minPrice;
        set => _minPrice = (value.HasValue && value.Value < 0) ? 0 : value;
    }
    private double? MaxPrice
    {
        get => _maxPrice;
        set => _maxPrice = (value.HasValue && value.Value < 0) ? 0 : value;
    }

    [Parameter]
    public EventFilterCriteria FilterCriteria { get; set; }
    [Parameter]
    public EventCallback<EventFilterCriteria> OnFilterApplied { get; set; }
    [Parameter]
    public EventCallback OnCloseFilter { get; set; }

    private async Task ApplyFilters()
    {
        FilterCriteria.Category = _selectedCategory;
        FilterCriteria.StartDate = _startDate;
        FilterCriteria.EndDate = _endDate;
        FilterCriteria.Location = _location;
        FilterCriteria.MinPrice = _minPrice;
        FilterCriteria.MaxPrice = _maxPrice;

        await OnFilterApplied.InvokeAsync(FilterCriteria);
    }
}