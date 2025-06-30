using Microsoft.AspNetCore.Components;

namespace EventOn.Components;

public partial class Toast
{
    [Parameter]
    public string Message { get; set; } = string.Empty;

    private bool _isVisible = false;
    private bool _isError = true;

    public async void ShowToast(string message, bool isError = true)
    {
        Message = message;
        _isError = isError;
        _isVisible = true;
        StateHasChanged();

        await Task.Delay(5000);

        _isVisible = false;
        StateHasChanged();
        Message = string.Empty;
    }

    public async void ShowToast(List<string> messages, bool isError = true)
    {
        foreach (var message in messages)
            Message += message + "\n";
        Message = Message.Trim();

        _isError = isError;
        _isVisible = true;
        StateHasChanged();

        await Task.Delay(5000);

        _isVisible = false;
        StateHasChanged();
        Message = string.Empty;
    }
}