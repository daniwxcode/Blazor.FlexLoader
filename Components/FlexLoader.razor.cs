using Blazor.FlexLoader.Services;

using Microsoft.AspNetCore.Components;

namespace Blazor.FlexLoader.Components;

public partial class FlexLoader : ComponentBase, IDisposable
{
    [Inject] public LoaderService LoaderService { get; set; } = null!;

    [Parameter] public string? ImagePath { get; set; }
    [Parameter] public string BackgroundColor { get; set; } = "rgba(255,255,255,0.75)";
    [Parameter] public string? Text { get; set; } = "Chargement...";
    [Parameter] public string TextColor { get; set; } = "#333";
    [Parameter] public string? ImageHeight { get; set; } = "120px";
    [Parameter] public bool UseAbsolutePosition { get; set; } = true;
    [Parameter] public RenderFragment? CustomContent { get; set; }
    [Parameter] public bool AutoClose { get; set; } = true;
    [Parameter] public int AutoCloseDelay { get; set; } = 300; // ms
    [Parameter] public bool CloseOnOverlayClick { get; set; } = false;

    private string OverlayStyle =>
        $"position:{(UseAbsolutePosition ? "absolute" : "fixed")};" +
        "top:0;left:0;width:100%;height:100%;" +
        $"background-color:{BackgroundColor};" +
        "display:flex;align-items:center;justify-content:center;z-index:999999;";

    private string ContentStyle =>
        "display:flex;flex-direction:column;align-items:center;justify-content:center;text-align:center;";

    private string ImageStyle => $"height:{ImageHeight};width:auto;";

    private string TextStyle => $"color:{TextColor};font-size:1.1rem;margin-top:10px;";

    protected override void OnInitialized()
    {
        LoaderService.OnChange += OnLoaderChange;
    }

    private async void OnLoaderChange(object? sender, EventArgs e)
    {
        try
        {
            // Mise à jour immédiate
            await InvokeAsync(StateHasChanged);

            // Permet un délai court pour éviter flicker si AutoClose true
            if (AutoClose && !LoaderService.IsLoading)
            {
                await Task.Delay(AutoCloseDelay);
                await InvokeAsync(StateHasChanged);
            }
        }
        catch (ObjectDisposedException)
        {
            // Composant déjà disposé, ignorer
        }
    }

    private void OnOverlayClick()
    {
        if (CloseOnOverlayClick)
        {
            LoaderService.Reset();
        }
    }

    public void Dispose()
    {
        LoaderService.OnChange -= OnLoaderChange;
    }
}
