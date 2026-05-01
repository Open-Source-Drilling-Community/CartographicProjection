using MudBlazor;
using MudBlazor.Services;
using NORCE.Drilling.CartographicProjection.WebApp;
using NORCE.Drilling.CartographicProjection.WebPages;

var builder = WebApplication.CreateBuilder(args);

WebPagesHostConfiguration webPagesConfiguration = new()
{
    CartographicProjectionHostURL = builder.Configuration["CartographicProjectionHostURL"] ?? string.Empty,
    GeodeticDatumHostURL = builder.Configuration["GeodeticDatumHostURL"] ?? string.Empty,
    UnitConversionHostURL = builder.Configuration["UnitConversionHostURL"] ?? string.Empty,
};

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 5000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});
builder.Services.AddSingleton<ICartographicProjectionWebPagesConfiguration>(webPagesConfiguration);
builder.Services.AddSingleton<ICartographicProjectionAPIUtils, CartographicProjectionAPIUtils>();
builder.Services.AddExternalWebPages(webPagesConfiguration);

var app = builder.Build();

app.UseForwardedHeaders();

app.Use(async (context, next) => {
    var path = context.Request.Path.Value;
    var pathLower = path.ToLower();
    // Normalize entire path to lowercase for case-insensitive endpoint matching
    if (pathLower.StartsWith("/cartographicprojection/webapp", System.StringComparison.Ordinal))
    {
        context.Request.Path = pathLower;
    }
    await next();
});
var basePath = "/cartographicprojection/webapp";
app.UsePathBase(basePath);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();


