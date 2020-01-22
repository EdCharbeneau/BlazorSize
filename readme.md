BlazorSize is a JavaScript interop library for Blazor that is used to detect the Broswer's current size, change in size, and test media queries.

BlazorSize was designed to allow Razor Components to implment adaptive rendering. Be sure to exhaust media queries as an option before using BlazorSize as your method of handling mobile/desktop rendering changes.

## Add the NuGet package 

**Package Manager**
```bash
Install-Package BlazorPro.BlazorSize -Version 1.2.0
```

**CLI**
```bash
dotnet add package BlazorPro.BlazorSize --version 1.2.0
```
**.csproj**
```html
<PackageReference Include="BlazorPro.BlazorSize" Version="1.2.0" />
```

## Reference the JavaScript interop

Add the JavaScript interop to your application's index.html or _hosts.cshtml
```html
    <!-- <script src="_content/BlazorPro.BlazorSize/blazorSize.js"></script> -->
    <script src="_content/BlazorPro.BlazorSize/blazorSize.min.js"></script>
```

## Configure DI

In startup.cs register ResizeListener with the applications service collection.


```csharp
services.AddScoped<ResizeListener>();
//services.AddResizeListener(options =>
//                            {
//                                options.ReportRate = 300;
//                                options.EnableLogging = true;
//                                options.SuppressInitEvent = true;
//                            });
```

## Import the namespace

Add a reference to the namespace in your _Imports.razor or at the top of a page.

```html
@using BlazorPro.BlazorSize
```

## Usage

This example shows how to get the browsers width/height and check for media query matches. Depending on the matched media query the view can toggle between two components WeatherGrid or WeatherCards.

```html
@inject ResizeListener listener
@implements IDisposable
@page "/fetchdata"

@using BlazorSize.Example.Data
@inject WeatherForecastService ForecastService

<h1>Weather forecast</h1>

<p>This component demonstrates adaptive rendering of a Blazor UI.</p>

<h3>Height: @browser.Height</h3>
<h3>Width: @browser.Width</h3>
<h3>MQ: @IsSmallMedia</h3>

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

	// We can also capture the browser's width / height if needed. We hold the value here.
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
		// Get the browsers's width / height
        browser = window;

		// Check a media query to see if it was matched. We can do this at any time, but it's best to check on each resize
        IsSmallMedia = await listener.MatchMedia("(min-width: 768px)");

		// We're outside of the component's lifecycle, be sure to let it know it has to re-render.
        StateHasChanged();
    }

}
```

## Helpers

```csharp

The Breakpoints class defines common media query strings.

/// @media(min-width: 576px) { ... }
/// Small devices (landscape phones, 576px and up)
IsSmallUpMedia = await listener.MatchMedia(Breakpoints.SmallUp);

/// @media(min-width: 768px) { ... }
/// Medium devices (tablets, 768px and up)
Breakpoints.MediumUp
IsMediumUpMedia = await listener.MatchMedia(Breakpoints.MediumUp);

// Large devices (desktops, 992px and up)
/// @media(min-width: 992px) { ... }
Breakpoints.LargeUp
IsLargeUpMedia = await listener.MatchMedia(Breakpoints.LargeUp);

/// Extra large devices (large desktops, 1200px and up)
/// @media(min-width: 1200px) { ... }
Breakpoints.XLargeUp
IsXLargeUpMedia = await listener.MatchMedia(Breakpoints.XLargeUp);

/// Extra small devices (portrait phones, less than 576px)
/// @media(max-width: 575.98px) { ... }
Breakpoints.XSmallDown

/// Small devices (landscape phones, less than 768px)
/// @media(max-width: 767.98px) { ... }
Breakpoints.SmallDown

/// Medium devices (tablets, less than 992px)
/// @media(max-width: 991.98px) { ... }
Breakpoints.MediumDown

/// Large devices (desktops, less than 1200px)
/// @media(max-width: 1199.98px) { ... }
Breakpoints.LargeDown

/// Small devices (landscape phones, 576px and up)
/// @media(min-width: 576px) and(max-width: 767.98px) { ... }
Breakpoints.OnlySmall

/// Medium devices (tablets, 768px and up)
/// @media(min-width: 768px) and(max-width: 991.98px) { ... }
Breakpoints.OnlyMedium

/// Large devices (desktops, 992px and up)
/// @media(min-width: 992px) and(max-width: 1199.98px) { ... }
Breakpoints.OnlyLarge

/// <summary>
/// Combines two media queries with the `and` keyword.
/// Values must include parenthesis.
/// Ex: (min-width: 992px) and (max-width: 1199.98px)
Between(string min, string max)

Example:
string BetweenMediumAndLargeOnly => Between(Breakpoints.MediumUp, LargeDown);

// out: "(min-width: 768px) and (max-width: 1199.98px)"

```
