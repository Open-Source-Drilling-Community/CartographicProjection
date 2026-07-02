using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NORCE.Drilling.CartographicProjection.Service.Controllers;

namespace NORCE.Drilling.CartographicProjection.Service.Mcp.Tools;

internal sealed class GetCartographicProjectionUsageStatisticsMcpTool : IMcpTool
{
    private readonly ILoggerFactory _loggerFactory;

    public GetCartographicProjectionUsageStatisticsMcpTool(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    public string Name => "cartographic_projection_usage_statistics.get";

    public string Description => "Retrieve usage statistics for the CartographicProjection microservice.";

    public JsonNode? InputSchema => null;

    public Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var controller = new CartographicProjectionUsageStatisticsController(
            _loggerFactory.CreateLogger<CartographicProjectionUsageStatisticsController>());
        var response = McpActionResultConverter.FromActionResult(controller.GetCartographicProjectionUsageStatistics());
        return Task.FromResult<JsonNode?>(response);
    }
}
