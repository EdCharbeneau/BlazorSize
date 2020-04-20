BlazorSize is a JavaScript interop library for Blazor that is used to detect the Browser's current size, change in size, and test media queries.

BlazorSize was designed to allow Razor Components to implement adaptive rendering. Handle mobile/desktop rendering changes at runtime with ease and complement your apps CSS media queries with programmatic work flows.

## Add the NuGet package 

**Package Manager**
```bash
Install-Package BlazorPro.BlazorSize 
```

**CLI**
```bash
dotnet add package BlazorPro.BlazorSize 
```
**.csproj**
```html
<PackageReference Include="BlazorPro.BlazorSize" Version="2.*" />
```

## Reference the JavaScript interop

Add the JavaScript interop to your application's index.html or _hosts.cshtml
```html
    <!--<script src="_content/BlazorPro.BlazorSize/blazorSize.js" type="module"></script>-->
    <script src="_content/BlazorPro.BlazorSize/blazorSize.min.js"></script>
```

## Import the namespace

Add a reference to the namespace in your _Imports.razor or at the top of a page.

```razor
@using BlazorPro.BlazorSize
```

## Add the MediaQueryList

Add a MediaQueryList to your application's main layout or root. The MediaQueryList is responsponsible for communicating with all MediaQuery components in your app. The MediaQueryList component will consolidate redundant media queries and manage resources so that unused event listeners are disposed of properly.

```razor
<MediaQueryList>
    <div class="sidebar">
        <NavMenu />
    </div>

    <div class="main">
        <div class="top-row px-4">
            <a href="http://blazor.net" target="_blank" class="ml-md-auto">About</a>
        </div>

        <div class="content px-4">
            @Body
        </div>
    </div>
</MediaQueryList>
```

### Add a MediaQuery and bind

Using the @bind-Matches directive we can easily bind to a browser's media query and respond to it.

```razor
@if (IsSmall)
{
    <WeatherCards Data="forecasts"></WeatherCards>
}
else
{
    <WeatherGrid Data="forecasts"></WeatherGrid>
}

@if (IsMedium)
{
    <span>Medium</span>
}

<MediaQuery Media="@Breakpoints.OnlyMedium" @bind-Matches="IsMedium" />
<MediaQuery Media="@Breakpoints.SmallDown" @bind-Matches="IsSmall" />

@code {
    WeatherForecast[] forecasts;

    bool IsMedium = false;
    bool IsSmall = false;

    protected override async Task OnInitializedAsync()
    {
        forecasts = await Http.GetFromJsonAsync<WeatherForecast[]>("sample-data/weather.json");
    }
}
```

### No-Code Templates

The MediaQuery component can also use templates instead of the @bind directive. Templates are useful for swapping out UI bits when the screen size changes.

```razor

<MediaQuery Media="@Breakpoints.SmallDown">
    <Matched>
        <WeatherCards Data="forecasts"></WeatherCards>
    </Matched>
    <Unmatched>
        <WeatherGrid Data="forecasts"></WeatherGrid>
    </Unmatched>
</MediaQuery>

```

## Helpers

Common media queries are already included as helpers to keep you out of the Bootstrap docs. Stay in your code longer and write cleaner statements too!

```csharp

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
IsXSmallDown = await listener.MatchMedia(Breakpoints.XSmallDown);

/// Small devices (landscape phones, less than 768px)
/// @media(max-width: 767.98px) { ... }
IsSmallDown = = await listener.MatchMedia(Breakpoints.SmallDown);

/// Medium devices (tablets, less than 992px)
/// @media(max-width: 991.98px) { ... }
IsMediumDown = = await listener.MatchMedia(Breakpoints.MediumDown);

/// Large devices (desktops, less than 1200px)
/// @media(max-width: 1199.98px) { ... }
LargeDown = = await listener.MatchMedia(Breakpoints.LargeDown);

/// Small devices (landscape phones, 576px and up)
/// @media(min-width: 576px) and(max-width: 767.98px) { ... }
IsSmallOnly = = await listener.MatchMedia(Breakpoints.OnlySmall);

/// Medium devices (tablets, 768px and up)
/// @media(min-width: 768px) and(max-width: 991.98px) { ... }
IsMediumOnly = = await listener.MatchMedia(Breakpoints.OnlyMedium);

/// Large devices (desktops, 992px and up)
/// @media(min-width: 992px) and(max-width: 1199.98px) { ... }
IsOnlyLarge = = await listener.MatchMedia(Breakpoints.OnlyLarge);

/// <summary>
/// Combines two media queries with the `and` keyword.
/// Values must include parenthesis.
/// Ex: (min-width: 992px) and (max-width: 1199.98px)
Breakpoints.Between(string min, string max)

Example:
string BetweenMediumAndLargeOnly => Breakpoints.Between(Breakpoints.MediumUp, Breakpoints.LargeDown);
// out: "(min-width: 768px) and (max-width: 1199.98px)"

IsBetweenMediumAndLargeOnly = await listener.MatchMedia(BetweenMediumAndLargeOnly);

```
## Listening for the Resize event

The ResizeListener is a service that allows your application to listen for the browser's resize event. The ResizeListener is a throttled to improve performance and can be adjusted thru configuration. If you only need to respond the user's device or screen size the **MediaQueryList & MediaQuery** components provide a **more performant** experience.

### Configure DI

In startup.cs register ResizeListener with the applications service collection.


```razor
services.AddScoped<ResizeListener>();
//services.AddResizeListener(options =>
//                            {
//                                options.ReportRate = 300;
//                                options.EnableLogging = true;
//                                options.SuppressInitEvent = true;
//                            });
```

### Usage

This example shows how to get the browsers width/height and check for media query matches. Depending on the matched media query the view can toggle between two components WeatherGrid or WeatherCards.

```razor
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
	<!-- Display a card layout on small devices -->
    <WeatherCards Data="forecasts"></WeatherCards>
}
else
{
	<!-- Display a full data grid on larger devices -->
    <WeatherGrid Data="forecasts"></WeatherGrid>
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
        IsSmallMedia = await listener.MatchMedia(Breakpoints.SmallDown);

		// We're outside of the component's lifecycle, be sure to let it know it has to re-render.
        StateHasChanged();
    }

}
```
