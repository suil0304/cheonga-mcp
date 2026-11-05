using cheongdo_mcp.Education;
using ModelContextProtocol.Server;

var builder = WebApplication.CreateBuilder(args);
var useMockData = builder.Configuration.GetValue("Data:UseMock", true);

if (builder.Environment.IsProduction() && useMockData)
{
    throw new InvalidOperationException("Mock data cannot be enabled in Production.");
}

if (!useMockData)
{
    throw new InvalidOperationException(
        "A production education data provider is not configured. Do not serve mock data as official material.");
}

builder.Services.AddSingleton<IEducationDataProvider, MockEducationDataProvider>();
builder.Services.AddMcpServer()
    .WithHttpTransport(options => options.Stateless = true)
    .WithTools<EducationTools>();

var app = builder.Build();

app.MapGet("/healthz", () => Results.Ok(new { status = "ok" }));
app.MapMcp();

app.Run();
