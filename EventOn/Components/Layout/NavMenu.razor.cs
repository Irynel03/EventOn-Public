using EventOn.Pages;
using Microsoft.AspNetCore.Components;

namespace EventOn.Components.Layout;

public partial class NavMenu
{
    [Inject]
    private NavigationManager Navigation { get; set; }

    private void OnEventOnClick()
    {
        if (Navigation.Uri.EndsWith("/feed"))
            Navigation.NavigateTo("/feed");
        else
            Navigation.NavigateTo("/feed");
    }
}