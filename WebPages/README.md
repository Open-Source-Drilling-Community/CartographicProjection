# NORCE.Drilling.CartographicProjection.WebPages

Reusable Razor class library for the Cartographic Projection web UI.

It contains the `CartographicProjection`, `CartographicProjectionEdit`, `CartographicConverter`, and `StatisticsMain` pages together with the supporting API and UI utility code they depend on.

## Package contents

- Cartographic projection list and edit pages
- Cartographic conversion page
- Usage statistics page
- Host-configurable API access through injected configuration

## Dependencies

- `OSDC.DotnetLibraries.Drilling.WebAppUtils`
- `MudBlazor`
- `OSDC.UnitConversion.DrillingRazorMudComponents`
- `ModelSharedOut`

## Host integration

The consuming app should:

1. Reference this package.
2. Provide an implementation of `ICartographicProjectionWebPagesConfiguration`.
3. Register that configuration and `ICartographicProjectionAPIUtils` in DI.
4. Add the `WebPages` assembly to the Blazor router `AdditionalAssemblies`.

## Required configuration

- `CartographicProjectionHostURL`
- `GeodeticDatumHostURL`
- `UnitConversionHostURL`
