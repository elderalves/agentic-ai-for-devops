using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using SharedKernel;
using Xunit;

namespace OrderService.Tests;

public sealed class OrderServiceIntegrationTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public OrderServiceIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task HealthEndpoint_ReturnsOk()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var content = await response.Content.ReadAsStreamAsync();
        var body = await JsonSerializer.DeserializeAsync<HealthResponse>(
            content, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        Assert.NotNull(body);
        Assert.Equal("ok", body.Status);
    }

    [Fact]
    public async Task OrdersEndpoint_ReturnsOrders()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/orders");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var content = await response.Content.ReadAsStreamAsync();
        var orders = await JsonSerializer.DeserializeAsync<List<OrderDto>>(
            content, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        Assert.NotNull(orders);
        Assert.Equal(3, orders.Count);
        Assert.Contains(orders, o => o.Id == "ORD-1001");
    }
}
