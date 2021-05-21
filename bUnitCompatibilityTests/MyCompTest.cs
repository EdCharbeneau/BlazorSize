using BlazorPro.BlazorSize;
using Bunit;
using Xunit;

namespace bUnitCompatibilityTests
{
    public class MyCompTest
    {
        [Fact]
        public void Shows_Correct_Content_Based_On_MediaQuery()
        {
            // Arrange
            using var ctx = new TestContext();
            var blazorSizeState = ctx.AddBlazorSize();
            blazorSizeState.SetActiveBreakPoint(Breakpoints.LargeDown);

            // Act
            var cut = ctx.RenderComponent<Sample>();

            // Assert
            cut.MarkupMatches("DOES NOT MATCH");

            // Change breakpoint
            blazorSizeState.SetActiveBreakPoint(Breakpoints.SmallDown);

            // Assert
            cut.MarkupMatches("MATCHES");
        }
    }
}
