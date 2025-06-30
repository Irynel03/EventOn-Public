using EventOn.API.Models;
using Microsoft.AspNetCore.Components;
using EventOn.Interfaces;
using EventOn.Components;
using SkiaSharp;
using System.Runtime.InteropServices;
using ZXing.Common;
using ZXing;
using EventOn.BusinessLogic.Helpers;

namespace EventOn.Pages;

public partial class UserCreatedEventsPage
{
    [Inject]
    private IEventOnApiService _eventOnApiService { get; set; }
    [Inject]
    private NavigationManager Navigation { get; set; }

    private List<Event> _fullEvents = [];
    private static bool _dataLoaded = false;

    private Toast toast;

    protected override async Task OnInitializedAsync()
    {
        _dataLoaded = false;
        var getEventsResult = await _eventOnApiService.GetAllEventsFromUser(UserData.Instance.Username);
        if (getEventsResult.HasErrors)
        {
            toast.ShowToast(getEventsResult.ToString());
            return;
        }

        _fullEvents = getEventsResult.Data;
        _fullEvents.Reverse();
        _dataLoaded = true;
    }

    private void NavigateToEvent(string eventId)
    {
        Navigation.NavigateTo($"/event/{eventId}");
    }

    private async Task DeleteEvent(string eventId)
    {
        var eventToRemove = _fullEvents.FirstOrDefault(e => e.Id == eventId);
        if (eventToRemove == null)
        {
            toast.ShowToast("Can't find the event.");
            return;
        }

        var deleteResult = await _eventOnApiService.DeleteEvent(eventId);
        if (deleteResult.HasErrors)
        {
            toast.ShowToast(deleteResult.ToString());
            return;
        }

        _fullEvents.Remove(eventToRemove);
    }

    private async Task ScanQRCode(string eventId)
    {
        var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
        if (status != PermissionStatus.Granted)
            status = await Permissions.RequestAsync<Permissions.Camera>();

        if (status == PermissionStatus.Granted)
        {
            try
            {
                FileResult photo = await MediaPicker.CapturePhotoAsync();
                if (photo != null)
                {
                    var qrCodeResult = await ProcessPhotoForQRCode(photo);
                    if (string.IsNullOrEmpty(qrCodeResult) || qrCodeResult.Contains("not found"))
                    {
                        toast.ShowToast("No QR Code found.");
                        return;
                    }

                    var isValid = await IsQrCodeValid(qrCodeResult, eventId);
                    if (isValid)
                        toast.ShowToast("Qr Code Valid", false);
                    else
                        toast.ShowToast("Qr code is not valid.");

                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error capturing photo: {ex.Message}");
                toast.ShowToast("Error accessing camera");
            }
        }
        else
        {
            Console.WriteLine("Camera permission not granted");
            toast.ShowToast("Camera permission not granted");
        }
    }

    private static async Task<string> ProcessPhotoForQRCode(FileResult photo)
    {
        if (photo == null)
            return "No photo provided.";

        try
        {
            using var stream = await photo.OpenReadAsync();
            using var skStream = new SKManagedStream(stream);
            using var bitmap = SKBitmap.Decode(skStream);
            if (bitmap == null)
                return "Failed to load image.";

            int width = bitmap.Width;
            int height = bitmap.Height;

            var pixmap = bitmap.PeekPixels();
            if (pixmap == null)
                return "Failed to extract pixel data.";

            int rowBytes = pixmap.RowBytes;
            int totalBytes = rowBytes * pixmap.Height;
            byte[] pixelData = new byte[totalBytes];

            Marshal.Copy(pixmap.GetPixels(), pixelData, 0, totalBytes);

            var luminanceSource = new RGBLuminanceSource(
                pixelData,
                width,
                height,
                RGBLuminanceSource.BitmapFormat.BGRA32
            );

            var reader = new BarcodeReaderGeneric
            {
                Options = new DecodingOptions
                {
                    TryHarder = true,
                    PossibleFormats = [BarcodeFormat.QR_CODE]
                }
            };

            var result = reader.Decode(luminanceSource);
            return result?.Text ?? "QR Code not found.";
        }
        catch (Exception)
        {
            return "Error processing QR Code.";
        }
    }

    private async Task<bool> IsQrCodeValid(string qrCodeResult, string eventId)
    {
        var getOrdersResult = await _eventOnApiService.GetAllOrdersForEvent(eventId);
        if (getOrdersResult.HasErrors)
        {
            toast.ShowToast(getOrdersResult.ErrorMessages);
            return false;
        }

        return getOrdersResult.Data.FirstOrDefault(o => o.QrContent == qrCodeResult) != null;
    }
}