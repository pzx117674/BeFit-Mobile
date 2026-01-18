namespace BeFit.Mobile;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        webView.Navigating += OnNavigating;
        webView.Navigated += OnNavigated;
    }

    private void OnNavigating(object? sender, WebNavigatingEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"Navigating to: {e.Url}");
    }

    private void OnNavigated(object? sender, WebNavigatedEventArgs e)
    {
        if (e.Result != WebNavigationResult.Success)
        {
            System.Diagnostics.Debug.WriteLine($"Navigation failed: {e.Result}");
            webView.Source = new HtmlWebViewSource
            {
                Html = $"<html><body style='font-family:sans-serif;padding:20px;'><h1>Nie można połączyć</h1><p>Błąd: {e.Result}</p><p>Upewnij się, że aplikacja webowa BeFit jest uruchomiona.</p></body></html>"
            };
        }
    }
}
