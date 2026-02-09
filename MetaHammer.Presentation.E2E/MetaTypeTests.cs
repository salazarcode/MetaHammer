using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace MetaHammer.Presentation.E2E;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class MetaTypeTests : PageTest
{
    private const string BaseUrl = "http://localhost:8080";

    [Test]
    public async Task Dashboard_ShouldShowCounters()
    {
        await Page.GotoAsync(BaseUrl);

        // Wait for the dashboard to load
        await Expect(Page.GetByText("MetaHammer Dashboard")).ToBeVisibleAsync();
        
        // Check if counters are visible
        await Expect(Page.GetByText("Total Types")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Native Types")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Custom Types")).ToBeVisibleAsync();
    }

    [Test]
    public async Task CreateType_ShouldUpdateListAndDashboard()
    {
        // 1. Go to Types page
        await Page.GotoAsync($"{BaseUrl}/types");

        string testTypeName = $"E2E_Entity_{Guid.NewGuid().ToString().Substring(0, 8)}";

        // 2. Fill the form
        await Page.GetByPlaceholder("e.g. Person").FillAsync(testTypeName);
        await Page.GetByLabel("Nature").SelectOptionAsync(new[] { "Entity" });
        
        // 3. Click Create
        await Page.GetByRole(AriaRole.Button, new() { Name = "Create Type" }).ClickAsync();

        // 4. Verify success message
        await Expect(Page.GetByText("Type created successfully")).ToBeVisibleAsync();

        // 5. Verify it appears in the table
        await Expect(Page.GetByRole(AriaRole.Cell, new() { Name = testTypeName })).ToBeVisibleAsync();

        // 6. Go back to Dashboard and verify counter (optional but good)
        await Page.GetByRole(AriaRole.Link, new() { Name = "Dashboard" }).ClickAsync();
        await Expect(Page.GetByText("Custom Types")).ToBeVisibleAsync();
    }
}
