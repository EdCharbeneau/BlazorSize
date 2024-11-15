using Microsoft.Playwright;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class Tests : PageTest
{
    [Test]
    public async Task IndexPageLoadsWithBlazorSizeAndDisplaysInitialPageSizeOf1920By1080()
    // This test method verifies that BlazorSize is working by exercising the browser's resize method. It performs the following steps:
    // 1. Navigates to the index page of the Blazor application.
    // 2. Sets the viewport size to 1920 by 1080.
    // 3. Expects that the element with the title "browserWidth" contains the text "1920".
    // 4. Expects that the element with the title "browserHeight" contains the text "1080".
    // 5. Expects that the element with the title "isSmallMedia" contains the text "True".
    {

        await Page.GotoAsync(TestHelpers.GetUrl("/"));
        await Page.SetViewportSizeAsync(1080, 720);
        await Page.SetViewportSizeAsync(1920, 1080);
        await Expect(Page.GetByTitle("browserWidth")).ToContainTextAsync("1920");
        await Expect(Page.GetByTitle("browserHeight")).ToContainTextAsync("1080");
        await Expect(Page.GetByTitle("isSmallMedia")).ToContainTextAsync("True");
        Page.PageError += (_, exception) => throw new Exception($"Page errors occurred {exception}");
    }

}

