using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NORCE.Drilling.CartographicProjection.Service.Controllers;
using NORCE.Drilling.CartographicProjection.Service.Managers;
using CartographicProjectionModel = NORCE.Drilling.CartographicProjection.Model.CartographicProjection;

namespace NORCE.Drilling.CartographicProjection.Service.Mcp.Tools;

internal abstract class CartographicProjectionToolBase : IMcpTool
{
    private protected readonly ILoggerFactory LoggerFactory;
    private protected readonly SqlConnectionManager ConnectionManager;

    protected CartographicProjectionToolBase(ILoggerFactory loggerFactory, SqlConnectionManager connectionManager)
    {
        LoggerFactory = loggerFactory;
        ConnectionManager = connectionManager;
    }

    public abstract string Name { get; }

    public abstract string Description { get; }

    public abstract JsonNode? InputSchema { get; }

    public abstract Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken);

    protected CartographicProjectionController CreateController()
    {
        return new CartographicProjectionController(
            LoggerFactory.CreateLogger<CartographicProjectionManager>(),
            ConnectionManager);
    }

    protected static bool TryDeserialize(JsonObject? arguments, out CartographicProjectionModel projection, out JsonNode? error)
    {
        projection = default!;
        error = null;

        if (arguments?["cartographicProjection"] is not JsonNode projectionNode)
        {
            error = McpToolResponses.CreateValidationError("Argument 'cartographicProjection' is required.");
            return false;
        }

        try
        {
            projection = projectionNode.Deserialize<CartographicProjectionModel>(JsonSettings.Options) ?? throw new InvalidOperationException();
            return true;
        }
        catch (Exception ex) when (ex is JsonException or InvalidOperationException)
        {
            error = McpToolResponses.CreateValidationError("Argument 'cartographicProjection' could not be deserialized.");
            return false;
        }
    }

    protected static JsonObject CreateProjectionSchema(bool includeId)
    {
        var properties = new JsonObject
        {
            ["cartographicProjection"] = new JsonObject
            {
                ["type"] = "object"
            }
        };
        var required = new JsonArray
        {
            "cartographicProjection"
        };

        if (includeId)
        {
            properties["id"] = new JsonObject
            {
                ["type"] = "string",
                ["format"] = "uuid"
            };
            required.Add("id");
        }

        return new JsonObject
        {
            ["type"] = "object",
            ["properties"] = properties,
            ["required"] = required,
            ["additionalProperties"] = false
        };
    }
}

internal sealed class GetAllCartographicProjectionIdsMcpTool : CartographicProjectionToolBase
{
    public GetAllCartographicProjectionIdsMcpTool(ILoggerFactory loggerFactory, SqlConnectionManager connectionManager)
        : base(loggerFactory, connectionManager) { }

    public override string Name => "cartographic_projection.get_all_ids";

    public override string Description => "Retrieve all cartographic projection identifiers.";

    public override JsonNode? InputSchema => null;

    public override Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var response = McpActionResultConverter.FromActionResult(CreateController().GetAllCartographicProjectionId());
        return Task.FromResult<JsonNode?>(response);
    }
}

internal sealed class GetAllCartographicProjectionMetaInfoMcpTool : CartographicProjectionToolBase
{
    public GetAllCartographicProjectionMetaInfoMcpTool(ILoggerFactory loggerFactory, SqlConnectionManager connectionManager)
        : base(loggerFactory, connectionManager) { }

    public override string Name => "cartographic_projection.get_all_meta_info";

    public override string Description => "Retrieve metadata for all cartographic projections.";

    public override JsonNode? InputSchema => null;

    public override Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var response = McpActionResultConverter.FromActionResult(CreateController().GetAllCartographicProjectionMetaInfo());
        return Task.FromResult<JsonNode?>(response);
    }
}

internal sealed class GetCartographicProjectionByIdMcpTool : CartographicProjectionToolBase
{
    public GetCartographicProjectionByIdMcpTool(ILoggerFactory loggerFactory, SqlConnectionManager connectionManager)
        : base(loggerFactory, connectionManager) { }

    public override string Name => "cartographic_projection.get_by_id";

    public override string Description => "Retrieve a cartographic projection by identifier.";

    public override JsonNode? InputSchema => McpToolArgumentHelpers.CreateGuidSchema("id");

    public override Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!McpToolArgumentHelpers.TryParseGuid(arguments, "id", out Guid id, out JsonNode? error))
        {
            return Task.FromResult<JsonNode?>(error);
        }

        var response = McpActionResultConverter.FromActionResult(CreateController().GetCartographicProjectionById(id));
        return Task.FromResult<JsonNode?>(response);
    }
}

internal sealed class GetAllCartographicProjectionLightMcpTool : CartographicProjectionToolBase
{
    public GetAllCartographicProjectionLightMcpTool(ILoggerFactory loggerFactory, SqlConnectionManager connectionManager)
        : base(loggerFactory, connectionManager) { }

    public override string Name => "cartographic_projection.get_all_light";

    public override string Description => "Retrieve all cartographic projections as lightweight records.";

    public override JsonNode? InputSchema => null;

    public override Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var response = McpActionResultConverter.FromActionResult(CreateController().GetAllCartographicProjectionLight());
        return Task.FromResult<JsonNode?>(response);
    }
}

internal sealed class GetAllCartographicProjectionMcpTool : CartographicProjectionToolBase
{
    public GetAllCartographicProjectionMcpTool(ILoggerFactory loggerFactory, SqlConnectionManager connectionManager)
        : base(loggerFactory, connectionManager) { }

    public override string Name => "cartographic_projection.get_all";

    public override string Description => "Retrieve all cartographic projections with full data.";

    public override JsonNode? InputSchema => null;

    public override Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var response = McpActionResultConverter.FromActionResult(CreateController().GetAllCartographicProjection());
        return Task.FromResult<JsonNode?>(response);
    }
}

internal sealed class PostCartographicProjectionMcpTool : CartographicProjectionToolBase
{
    private static readonly JsonObject Schema = CreateProjectionSchema(includeId: false);

    public PostCartographicProjectionMcpTool(ILoggerFactory loggerFactory, SqlConnectionManager connectionManager)
        : base(loggerFactory, connectionManager) { }

    public override string Name => "cartographic_projection.create";

    public override string Description => "Create a cartographic projection.";

    public override JsonNode? InputSchema => Schema;

    public override Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!TryDeserialize(arguments, out CartographicProjectionModel projection, out JsonNode? error))
        {
            return Task.FromResult<JsonNode?>(error);
        }

        var response = McpActionResultConverter.FromActionResult(CreateController().PostCartographicProjection(projection));
        return Task.FromResult<JsonNode?>(response);
    }
}

internal sealed class PutCartographicProjectionByIdMcpTool : CartographicProjectionToolBase
{
    private static readonly JsonObject Schema = CreateProjectionSchema(includeId: true);

    public PutCartographicProjectionByIdMcpTool(ILoggerFactory loggerFactory, SqlConnectionManager connectionManager)
        : base(loggerFactory, connectionManager) { }

    public override string Name => "cartographic_projection.update_by_id";

    public override string Description => "Update an existing cartographic projection identified by id.";

    public override JsonNode? InputSchema => Schema;

    public override Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!McpToolArgumentHelpers.TryParseGuid(arguments, "id", out Guid id, out JsonNode? idError))
        {
            return Task.FromResult<JsonNode?>(idError);
        }
        if (!TryDeserialize(arguments, out CartographicProjectionModel projection, out JsonNode? projectionError))
        {
            return Task.FromResult<JsonNode?>(projectionError);
        }

        var response = McpActionResultConverter.FromActionResult(CreateController().PutCartographicProjectionById(id, projection));
        return Task.FromResult<JsonNode?>(response);
    }
}

internal sealed class DeleteCartographicProjectionByIdMcpTool : CartographicProjectionToolBase
{
    public DeleteCartographicProjectionByIdMcpTool(ILoggerFactory loggerFactory, SqlConnectionManager connectionManager)
        : base(loggerFactory, connectionManager) { }

    public override string Name => "cartographic_projection.delete_by_id";

    public override string Description => "Delete a cartographic projection by identifier.";

    public override JsonNode? InputSchema => McpToolArgumentHelpers.CreateGuidSchema("id");

    public override Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!McpToolArgumentHelpers.TryParseGuid(arguments, "id", out Guid id, out JsonNode? error))
        {
            return Task.FromResult<JsonNode?>(error);
        }

        var response = McpActionResultConverter.FromActionResult(CreateController().DeleteCartographicProjectionById(id));
        return Task.FromResult<JsonNode?>(response);
    }
}
