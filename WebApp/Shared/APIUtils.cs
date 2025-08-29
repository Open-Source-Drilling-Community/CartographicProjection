using NORCE.Drilling.CartographicProjection.ModelShared;

public static class APIUtils
{
    // API parameters
    public static readonly string HostNameCartographicProjection = NORCE.Drilling.CartographicProjection.WebApp.Configuration.CartographicProjectionHostURL!;
    public static readonly string HostBasePathCartographicProjection = "CartographicProjection/api/";
    public static readonly HttpClient HttpClientCartographicProjection = SetHttpClient(HostNameCartographicProjection, HostBasePathCartographicProjection);
    public static readonly string HostNameGeodeticDatum = NORCE.Drilling.CartographicProjection.WebApp.Configuration.GeodeticDatumHostURL!;
    public static readonly string HostBasePathGeodeticDatum = "GeodeticDatum/api/";
    public static readonly HttpClient HttpClientGeodeticDatum = SetHttpClient(HostNameGeodeticDatum, HostBasePathGeodeticDatum);
    public static readonly Client ClientCartographicProjection = new Client(HttpClientCartographicProjection.BaseAddress!.ToString(), HttpClientCartographicProjection);
    public static readonly Client ClientGeodeticDatum = new Client(APIUtils.HttpClientGeodeticDatum.BaseAddress!.ToString(), HttpClientGeodeticDatum);

    public static readonly string HostNameUnitConversion = NORCE.Drilling.CartographicProjection.WebApp.Configuration.UnitConversionHostURL!;
    public static readonly string HostBasePathUnitConversion = "UnitConversion/api/";

    // API utility methods
    public static HttpClient SetHttpClient(string host, string microServiceUri)
    {
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }; // temporary workaround for testing purposes: bypass certificate validation (not recommended for production environments due to security risks)
        HttpClient httpClient = new(handler)
        {
            BaseAddress = new Uri(host + microServiceUri)
        };
        httpClient.DefaultRequestHeaders.Accept.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        return httpClient;
    }
}