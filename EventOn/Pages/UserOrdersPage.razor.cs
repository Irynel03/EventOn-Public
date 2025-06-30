using EventOn.API.Models;
using EventOn.BusinessLogic.Helpers;
using EventOn.Components;
using EventOn.Interfaces;
using Microsoft.AspNetCore.Components;
using SkiaSharp;
using ZXing;
using ZXing.Common;
using ZXing.SkiaSharp.Rendering;

namespace EventOn.Pages;

public partial class UserOrdersPage
{
    [Inject]
    private IUserLocalDataService _localDataService { get; set; }

    private Toast toast;
    private List<Order> _orders;
    private bool _showQrCodePrompt = false;
    private string _selectedOrderId;
    private string _qrCodeImageSource;
    private string _qrCodeError;

    protected override async Task OnInitializedAsync()
    {
        _orders = [.. _localDataService.GetOrders().OrderByDescending(order => order.Date)];
    }

    private void ShowQrCodePrompt(string orderId)
    {
        var orderToShowQr = _orders.FirstOrDefault(o => o.Id == orderId);
        if (orderToShowQr != null && !string.IsNullOrEmpty(orderToShowQr.QrContent))
        {
            _selectedOrderId = orderId;
            _showQrCodePrompt = true;
            _qrCodeImageSource = null;
            _qrCodeError = null;
            StateHasChanged();

            GenerateQrCode(orderToShowQr.QrContent);
        }
        else
        {
            toast.ShowToast("QR code content not available for this order.");
        }
    }

    private void GenerateQrCode(string qrContent)
    {
        try
        {
            var writer = new BarcodeWriter<SKBitmap>
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Width = 300,
                    Height = 300,
                    Margin = 1
                },
                Renderer = new SKBitmapRenderer()
            };

            using var bitmap = writer.Write(qrContent);
            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            var base64 = Convert.ToBase64String(data.ToArray());

            _qrCodeImageSource = $"data:image/png;base64,{base64}";
            StateHasChanged();
        }
        catch
        {
            toast.ShowToast("Can't show qr code");
        }
    }

    private void CloseQrCodePrompt()
    {
        _showQrCodePrompt = false;
        _selectedOrderId = null;
        _qrCodeImageSource = null;
        _qrCodeError = null;
        StateHasChanged();
    }

    private void NavigateToEvent(string eventId)
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            toast.ShowToast("No Internet");
            return;
        }

        if (string.IsNullOrWhiteSpace(UserData.Instance.Username))
        {
            toast.ShowToast("Not authorized");
            return;
        }

        Navigation.NavigateTo($"/event/{eventId}");
    }
}