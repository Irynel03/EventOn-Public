using EventOn.BusinessLogic.Helpers;
using EventOn.BusinessLogic.Models;
using EventOn.Interfaces;

namespace EventOn.BusinessLogic.Services;

public class GeolocationService : IGeolocationService
{
    public async Task<Result<LocationCoordinates>> GetLocationFromMauiAsync()
    {
        var result = new Result<LocationCoordinates>(null);
        try
        {
            var geoLocation = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium));

            if (geoLocation != null)
            {
                var locationCoordinates = new LocationCoordinates()
                {
                    Latitude = geoLocation.Latitude,
                    Longitude = geoLocation.Longitude
                };

                result.Data = locationCoordinates;
            }
            else
                result.AddError("Location access denied");
        }
        catch
        {
            result.AddError("Can't access location");
        }

        return result;
    }
}