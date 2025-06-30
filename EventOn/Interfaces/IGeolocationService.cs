using EventOn.BusinessLogic.Helpers;
using EventOn.BusinessLogic.Models;

namespace EventOn.Interfaces;

public interface IGeolocationService
{
    Task<Result<LocationCoordinates>> GetLocationFromMauiAsync();
}