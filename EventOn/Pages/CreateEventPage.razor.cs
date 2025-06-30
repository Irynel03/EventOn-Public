namespace EventOn.Pages;

using EventOn.API.Models;
using EventOn.API.Models.Enums;
using EventOn.BusinessLogic.Helpers;
using EventOn.Components;
using EventOn.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

public partial class CreateEventPage : IDisposable
{
    [Inject]
    private IEventOnApiService _eventOnApiService { get; set; }
    [Inject]
    private IGeolocationService _geolocationService { get; set; }

    [Inject]
    private IJSRuntime JS { get; set; }

    private Toast toast;

    private List<string> _availablePerformers = [];
    private string _eventName = string.Empty;
    private int _capacity;
    private EventCategory _category;
    private List<byte[]> _photos = [];
    private string _description = string.Empty;
    private List<string> _performers = [];
    private double? _price;
    private DateTime _startDate = DateTime.Today;
    private DateTime _endDate = DateTime.Today;
    private Location _location = new();
    private string _performerInput = string.Empty;
    private double? Price
    {
        get => _price;
        set => _price = (value.HasValue && value.Value < 0) ? 0 : value;
    }

    private DotNetObjectReference<CreateEventPage>? _objectReference;

    protected override async Task OnInitializedAsync()
    {
        var getPerformersAsync = await _eventOnApiService.GetUsersOfTypeAsync(TypeOfUser.Performer);
        if (getPerformersAsync.HasErrors)
        {
            toast.ShowToast(getPerformersAsync.ToString());
            return;
        }
        _availablePerformers = [.. getPerformersAsync.Data.Select(p => p.Username)];

        _objectReference = DotNetObjectReference.Create(this);
        await JS.InvokeVoidAsync("initializeMap", _objectReference);

        var getLocationResult = await _geolocationService.GetLocationFromMauiAsync();
        if (getLocationResult.HasErrors)
        {
            toast.ShowToast(getLocationResult.ErrorMessages);
            return;
        }

        try
        {
            await JS.InvokeVoidAsync("loadMap", getLocationResult.Data.Latitude, getLocationResult.Data.Longitude);
        }
        catch (Exception) { }
    }

    private async Task CreateEvent()
    {
        var @event = new Event()
        {
            Name = _eventName,
            Capacity = _capacity,
            Category = _category,
            Description = _description,
            Performers = _performers,
            Price = (double)_price,
            StartDate = _startDate,
            EndDate = _endDate,
            Location = _location,
            OwnerUsername = UserData.Instance.Username
        };

        var eventValidationResult = EventValidator.ValidateNewCreatedEvent(@event, _photos);
        if (eventValidationResult.HasErrors)
        {
            toast.ShowToast(eventValidationResult.ToString());
            return;
        }

        var createEventResult = await _eventOnApiService.CreateEvent(@event, _photos);
        if (createEventResult.HasErrors)
        {
            toast.ShowToast(createEventResult.ToString());
            return;
        }

        toast.ShowToast($"Event {@event.Name} Created", false);
        await Task.Delay(1000);
        Navigation.NavigateTo("/feed");
    }

    private async Task AddPhotosAsync()
    {
        try
        {
            var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Please select a photo"
            });

            if (result != null)
            {
                using var stream = await result.OpenReadAsync();
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);

                byte[] photoBytes = memoryStream.ToArray();
                _photos.Add(photoBytes);
                StateHasChanged();
            }
        }
        catch (Exception ex)
        {
            toast.ShowToast(ex.Message);
        }
    }

    private void AddPerformers()
    {
        if (!string.IsNullOrWhiteSpace(_performerInput))
        {
            if (_availablePerformers.Contains(_performerInput) && !_performers.Contains(_performerInput))
            {
                _performers.Add(_performerInput);
                _performerInput = string.Empty;
            }
            else
                toast.ShowToast("Can't find performer");
        }
    }

    [JSInvokable]
    public void UpdateLocation(string name, string placeId, double lat, double lng)
    {
        _location.Name = name;
        _location.Latitude = lat;
        _location.Longitude = lng;
        _location.PlaceId = placeId;
    }

    public void Dispose() => _objectReference?.Dispose();
}