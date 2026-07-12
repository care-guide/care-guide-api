using CareGuide.Models.DTOs.Shared;

namespace CareGuide.Tests;

public class PagedResultTests
{
    [Fact(DisplayName = "HasNext: more items exist beyond the current page returns true")]
    public void HasNext_MoreItemsBeyondCurrentPage_ReturnsTrue()
    {
        var result = new PagedResult<int>(Items: [1, 2, 3], Page: 1, PageSize: 10, Total: 25);

        result.HasNext.Should().BeTrue();
    }

    [Fact(DisplayName = "HasNext: on the exact last page returns false")]
    public void HasNext_OnLastPage_ReturnsFalse()
    {
        var result = new PagedResult<int>(Items: [1, 2, 3, 4, 5], Page: 3, PageSize: 10, Total: 25);

        result.HasNext.Should().BeFalse();
    }

    [Fact(DisplayName = "HasNext: total exactly fills the current page returns false")]
    public void HasNext_TotalExactlyFillsPage_ReturnsFalse()
    {
        var result = new PagedResult<int>(Items: Enumerable.Range(1, 10).ToList(), Page: 1, PageSize: 10, Total: 10);

        result.HasNext.Should().BeFalse();
    }

    [Fact(DisplayName = "HasNext: empty result set returns false")]
    public void HasNext_EmptyResultSet_ReturnsFalse()
    {
        var result = new PagedResult<int>(Items: [], Page: 1, PageSize: 10, Total: 0);

        result.HasNext.Should().BeFalse();
    }

    [Fact(DisplayName = "HasNext: page beyond the last page returns false")]
    public void HasNext_PageBeyondLastPage_ReturnsFalse()
    {
        var result = new PagedResult<int>(Items: [], Page: 5, PageSize: 10, Total: 25);

        result.HasNext.Should().BeFalse();
    }

    [Fact(DisplayName = "PagedResult: exposes items, page, pageSize and total as provided")]
    public void PagedResult_ExposesConstructorValues()
    {
        var items = new List<string> { "a", "b" };

        var result = new PagedResult<string>(items, Page: 2, PageSize: 5, Total: 12);

        result.Items.Should().BeSameAs(items);
        result.Page.Should().Be(2);
        result.PageSize.Should().Be(5);
        result.Total.Should().Be(12);
    }
}
