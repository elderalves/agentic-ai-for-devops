using SharedKernel;
using Xunit;

namespace OrderService.Tests;

public sealed class ResultTests
{
    [Fact]
    public void Ok_ReturnsSuccessWithNoError()
    {
        var result = Result.Ok();

        Assert.True(result.Success);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Fail_ReturnsFailureWithError()
    {
        var result = Result.Fail("boom");

        Assert.False(result.Success);
        Assert.Equal("boom", result.Error);
    }
}

public sealed class OrderDtoTests
{
    [Fact]
    public void Constructor_SetsProperties()
    {
        var dto = new OrderDto("ORD-1", "Pending", 10.50m);

        Assert.Equal("ORD-1", dto.Id);
        Assert.Equal("Pending", dto.Status);
        Assert.Equal(10.50m, dto.Amount);
    }
}
