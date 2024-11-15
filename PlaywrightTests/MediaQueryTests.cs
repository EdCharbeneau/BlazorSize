using Microsoft.Playwright;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class MediaQueryTests : PageTest
{
    [Test]
    public async Task MyNewPlaywrightTest()
    {
        // Act
        await Page.SetViewportSizeAsync(767, 800);
        await Page.GotoAsync(TestHelpers.GetUrl("/fetchdata"));

        // Assert
        await Expect(Page.GetByTestId("weather-card-0")).ToBeVisibleAsync();
        await Expect(Page.GetByTestId("weather-grid")).Not.ToBeVisibleAsync();

        // Act
        await Page.SetViewportSizeAsync(768, 1200);
        await Page.GotoAsync(TestHelpers.GetUrl("/fetchdata"));

        // Assert
        await Expect(Page.GetByTestId("weather-card-0")).ToBeVisibleAsync();
        await Expect(Page.GetByTestId("weather-grid")).Not.ToBeVisibleAsync();

    }
}

