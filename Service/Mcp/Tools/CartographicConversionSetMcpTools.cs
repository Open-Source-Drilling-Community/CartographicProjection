using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NORCE.Drilling.CartographicProjection.Service.Controllers;
using NORCE.Drilling.CartographicProjection.Service.Managers;
using CartographicConversionSetModel = NORCE.Drilling.CartographicProjection.Model.CartographicConversionSet;

namespace NORCE.Drilling.CartographicProjection.Service.Mcp.Tools;

internal abstract class CartographicConversionSetToolBase : IMcpTool
{
    private protected readonly ILoggerFactory LoggerFactory;
    private protected readonly SqlConnectionManager ConnectionManager;

    protected CartographicConversionSetToolBase(ILoggerFactory loggerFactory, SqlConnectionManager connectionManager)
    {
        LoggerFactory = loggerFactory;
        ConnectionManager = connectionManager;
    }

    public abstract string Name { get; }

    public abstract string Description { get; }

    public abstract JsonNode? InputSchema { get; }

    public abstract Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken);

    protected CartographicConversionSetController CreateController()
    {
        return new CartographicConversionSetController(
            LoggerFactory.CreateLogger<CartographicConversionSetManager>(),
            ConnectionManager);
    }

    protected static bool TryDeserialize(JsonObject? arguments, out CartographicConversionSetModel conversionSet, out JsonNode? error)
    {
        conversionSet = default!;
        error = null;

        if (arguments?["cartographicConversionSet"] is not JsonNode conversionSetNode)
        {
            error = McpToolResponses.CreateValidationError("Argument 'cartographicConversionSet' is required.");
            return false;
        }

        try
        {
            conversionSet = conversionSetNode.Deserialize<CartographicConversionSetModel>(JsonSettings.Options) ?? throw new InvalidOperationException();
            return true;
        }
        catch (Exception ex) when (ex is JsonException or InvalidOperationException)
        {
            error = McpToolResponses.CreateValidationError("Argument 'cartographicConversionSet' could not be deserialized.");
            return false;
        }
    }

    protected static JsonObject CreateConversionSetSchema(bool includeId)
    {
        var properties = new JsonObject
        {
            ["cartographicConversionSet"] = new JsonObject
            {
                ["type"] = "object"
            }
        };
        var required = new JsonArray
        {
            "cartographicConversionSet"
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

internal sealed class GetAllCartographicConversionSetIdsMcpTool : CartographicConversionSetToolBase
{
    public GetAllCartographicConversionSetIdsMcpTool(ILoggerFactory loggerFactory, SqlConnectionManager connectionManager)
        : base(loggerFactory, connectionManager) { }

    public override string Name => "cartographic_conversion_set.get_all_ids";

    public override string Description => "Retrieve all cartographic conversion set identifiers.";

    public override JsonNode? InputSchema => null;

    public override Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var response = McpActionResultConverter.FromActionResult(CreateController().GetAllCartographicConversionSetId());
        return Task.FromResult<JsonNode?>(response);
    }
}

internal sealed class GetAllCartographicConversionSetMetaInfoMcpTool : CartographicConversionSetToolBase
{
    public GetAllCartographicConversionSetMetaInfoMcpTool(ILoggerFactory loggerFactory, SqlConnectionManager connectionManager)
        : base(loggerFactory, connectionManager) { }

    public override string Name => "cartographic_conversion_set.get_all_meta_info";

    public override string Description => "Retrieve metadata for all cartographic conversion sets.";

    public override JsonNode? InputSchema => null;

    public override Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var response = McpActionResultConverter.FromActionResult(CreateController().GetAllCartographicConversionSetMetaInfo());
        return Task.FromResult<JsonNode?>(response);
    }
}

internal sealed class GetCartographicConversionSetByIdMcpTool : CartographicConversionSetToolBase
{
    public GetCartographicConversionSetByIdMcpTool(ILoggerFactory loggerFactory, SqlConnectionManager connectionManager)
        : base(loggerFactory, connectionManager) { }

    public override string Name => "cartographic_conversion_set.get_by_id";

    public override string Description => "Retrieve a cartographic conversion set by identifier.";

    public override JsonNode? InputSchema => McpToolArgumentHelpers.CreateGuidSchema("id");

    public override Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!McpToolArgumentHelpers.TryParseGuid(arguments, "id", out Guid id, out JsonNode? error))
        {
            return Task.FromResult<JsonNode?>(error);
        }

        var response = McpActionResultConverter.FromActionResult(CreateController().GetCartographicConversionSetById(id));
        return Task.FromResult<JsonNode?>(response);
    }
}

internal sealed class GetAllCartographicConversionSetLightMcpTool : CartographicConversionSetToolBase
{
    public GetAllCartographicConversionSetLightMcpTool(ILoggerFactory loggerFactory, SqlConnectionManager connectionManager)
        : base(loggerFactory, connectionManager) { }

    public override string Name => "cartographic_conversion_set.get_all_light";

    public override string Description => "Retrieve all cartographic conversion sets as lightweight records.";

    public override JsonNode? InputSchema => null;

    public override Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var response = McpActionResultConverter.FromActionResult(CreateController().GetAllCartographicConversionSetLight());
        return Task.FromResult<JsonNode?>(response);
    }
}

internal sealed class GetAllCartographicConversionSetMcpTool : CartographicConversionSetToolBase
{
    public GetAllCartographicConversionSetMcpTool(ILoggerFactory loggerFactory, SqlConnectionManager connectionManager)
        : base(loggerFactory, connectionManager) { }

    public override string Name => "cartographic_conversion_set.get_all";

    public override string Description => "Retrieve all cartographic conversion sets with full data.";

    public override JsonNode? InputSchema => null;

    public override Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var response = McpActionResultConverter.FromActionResult(CreateController().GetAllCartographicConversionSet());
        return Task.FromResult<JsonNode?>(response);
    }
}

internal sealed class PostCartographicConversionSetMcpTool : CartographicConversionSetToolBase
{
    private static readonly JsonObject Schema = CreateConversionSetSchema(includeId: false);

    public PostCartographicConversionSetMcpTool(ILoggerFactory loggerFactory, SqlConnectionManager connectionManager)
        : base(loggerFactory, connectionManager) { }

    public override string Name => "cartographic_conversion_set.create";

    public override string Description => "Calculate and create a cartographic conversion set.";

    public override JsonNode? InputSchema => Schema;

    public override async Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!TryDeserialize(arguments, out CartographicConversionSetModel conversionSet, out JsonNode? error))
        {
            return error;
        }

        var response = McpActionResultConverter.FromActionResult(await CreateController().PostCartographicConversionSet(conversionSet).ConfigureAwait(false));
        return response;
    }
}

internal sealed class PutCartographicConversionSetByIdMcpTool : CartographicConversionSetToolBase
{
    private static readonly JsonObject Schema = CreateConversionSetSchema(includeId: true);

    public PutCartographicConversionSetByIdMcpTool(ILoggerFactory loggerFactory, SqlConnectionManager connectionManager)
        : base(loggerFactory, connectionManager) { }

    public override string Name => "cartographic_conversion_set.update_by_id";

    public override string Description => "Calculate and update an existing cartographic conversion set identified by id.";

    public override JsonNode? InputSchema => Schema;

    public override async Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!McpToolArgumentHelpers.TryParseGuid(arguments, "id", out Guid id, out JsonNode? idError))
        {
            return idError;
        }
        if (!TryDeserialize(arguments, out CartographicConversionSetModel conversionSet, out JsonNode? conversionSetError))
        {
            return conversionSetError;
        }

        var response = McpActionResultConverter.FromActionResult(await CreateController().PutCartographicConversionSetById(id, conversionSet).ConfigureAwait(false));
        return response;
    }
}

internal sealed class DeleteCartographicConversionSetByIdMcpTool : CartographicConversionSetToolBase
{
    public DeleteCartographicConversionSetByIdMcpTool(ILoggerFactory loggerFactory, SqlConnectionManager connectionManager)
        : base(loggerFactory, connectionManager) { }

    public override string Name => "cartographic_conversion_set.delete_by_id";

    public override string Description => "Delete a cartographic conversion set by identifier.";

    public override JsonNode? InputSchema => McpToolArgumentHelpers.CreateGuidSchema("id");

    public override Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!McpToolArgumentHelpers.TryParseGuid(arguments, "id", out Guid id, out JsonNode? error))
        {
            return Task.FromResult<JsonNode?>(error);
        }

        var response = McpActionResultConverter.FromActionResult(CreateController().DeleteCartographicConversionSetById(id));
        return Task.FromResult<JsonNode?>(response);
    }
}
