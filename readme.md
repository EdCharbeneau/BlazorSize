BlazorSize is a JavaScript interop library for Blazor that is used to detect the Broswer's current size, change in size, and test media queries.

BlazorSize was designed to allow Razor Components to implment adaptive rendering. Be sure to exhaust media queries as an option before using BlazorSize as your method of handling mobile/desktop rendering changes.

Usage:

index.html / _hosts.cshtml
```
...
<script src="_content/BlazorSize/blazorSize.js"></script>
```
startup.cs
```
public void ConfigureServices(IServiceCollection services)
{
	...
    services.AddScoped<ResizeListener>();
}

```
FetchData.razor
```
@inject ResizeListener listener
@implements IDisposable
@page "/fetchdata"

@using BlazorSize.Example.Data
@inject WeatherForecastService ForecastService

<h1>Weather forecast</h1>

<p>This component demonstrates adaptive rendering of a Blazor UI.</p>

@if (IsSmallMedia)
{
	<!-- Display a full data grid on larger devices -->
    <WeatherGrid Data="forecasts"></WeatherGrid>
}
else
{
	<!-- Display a card layout on small devices -->
    <WeatherCards Data="forecasts"></WeatherCards>
}

@code {
    WeatherForecast[] forecasts;

	// We can also capture the browser's widht / height if needed. We hold the value here.
    BrowserWindowSize browser = new BrowserWindowSize();

    bool IsSmallMedia = false;

    protected override async Task OnInitializedAsync()
    {
        forecasts = await ForecastService.GetForecastAsync(DateTime.Now);
    }

    protected override void OnAfterRender(bool firstRender)
    {

        if (firstRender)
        {
			// Subscribe to the OnResized event. This will do work when the browser is resized.
            listener.OnResized += WindowResized;
        }
    }

    void IDisposable.Dispose()
    {
		// Always use IDisposable in your component to unsubscribe from the event.
		// Be a good citizen and leave things how you found them. 
		// This way event handlers aren't called when nobody is listening.
        listener.OnResized -= WindowResized;
    }

	// This method will be called when the window resizes.
	// It is ONLY called when the user stops dragging the window's edge. (It is already throttled to protect your app from perf. nightmares)
    async void WindowResized(object _, BrowserWindowSize window)
    {
		// Get the broser's width / height
		// We're not making direct use of this in the example, but it is possible to retrive the values from the event arguments.
        browser = window;

		// Check a media query to see if it was matched. We can do this at any time, but it's best to check on each resize
        IsSmallMedia = await listener.MatchMedia("(min-width: 768px)");

		// We're outside of the component's lifecycle, be sure to let it know it has to re-render.
        StateHasChanged();
    }

}
```