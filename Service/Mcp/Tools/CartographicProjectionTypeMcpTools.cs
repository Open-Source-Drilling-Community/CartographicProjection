using System;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NORCE.Drilling.CartographicProjection.Model;
using NORCE.Drilling.CartographicProjection.Service.Controllers;

namespace NORCE.Drilling.CartographicProjection.Service.Mcp.Tools;

internal abstract class CartographicProjectionTypeToolBase : IMcpTool
{
    private readonly ILoggerFactory _loggerFactory;

    protected CartographicProjectionTypeToolBase(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    public abstract string Name { get; }

    public abstract string Description { get; }

    public abstract JsonNode? InputSchema { get; }

    public abstract Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken);

    protected CartographicProjectionTypeController CreateController()
    {
        return new CartographicProjectionTypeController(_loggerFactory.CreateLogger<CartographicProjectionType>());
    }
}

internal sealed class GetAllCartographicProjectionTypeIdsMcpTool : CartographicProjectionTypeToolBase
{
    public GetAllCartographicProjectionTypeIdsMcpTool(ILoggerFactory loggerFactory)
        : base(loggerFactory) { }

    public override string Name => "cartographic_projection_type.get_all_ids";

    public override string Description => "Retrieve all supported cartographic projection type identifiers.";

    public override JsonNode? InputSchema => null;

    public override Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var response = McpActionResultConverter.FromActionResult(CreateController().GetAllCartographicProjectionTypeId());
        return Task.FromResult<JsonNode?>(response);
    }
}

internal sealed class GetCartographicProjectionTypeByIdMcpTool : CartographicProjectionTypeToolBase
{
    public GetCartographicProjectionTypeByIdMcpTool(ILoggerFactory loggerFactory)
        : base(loggerFactory) { }

    public override string Name => "cartographic_projection_type.get_by_id";

    public override string Description => "Retrieve a cartographic projection type prototype by identifier.";

    public override JsonNode? InputSchema => McpToolArgumentHelpers.CreateStringSchema("id");

    public override Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!McpToolArgumentHelpers.TryParseString(arguments, "id", out string id, out JsonNode? error))
        {
            return Task.FromResult<JsonNode?>(error);
        }

        var response = McpActionResultConverter.FromActionResult(CreateController().GetCartographicProjectionTypeById(id));
        return Task.FromResult<JsonNode?>(response);
    }
}

internal sealed class GetAllCartographicProjectionTypeMcpTool : CartographicProjectionTypeToolBase
{
    public GetAllCartographicProjectionTypeMcpTool(ILoggerFactory loggerFactory)
        : base(loggerFactory) { }

    public override string Name => "cartographic_projection_type.get_all";

    public override string Description => "Retrieve all supported cartographic projection type prototypes.";

    public override JsonNode? InputSchema => null;

    public override Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var response = McpActionResultConverter.FromActionResult(CreateController().GetAllCartographicProjectionType());
        return Task.FromResult<JsonNode?>(response);
    }
}
