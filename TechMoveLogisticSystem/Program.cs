using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TechMoveLogisticSystem.Data;
using TechMoveLogisticSystem.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Currency Service
builder.Services.AddHttpClient<CurrencyService>();

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Reads the backend API base URL from appsettings.json
builder.Services.Configure<ApiSettings>(
    builder.Configuration.GetSection("ApiSettings"));

// This HttpClient will be used by the MVC frontend to call the backend API
builder.Services.AddHttpClient("TechMoveApi", client =>
{
    var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];

    if (string.IsNullOrWhiteSpace(apiBaseUrl))
    {
        throw new InvalidOperationException("ApiSettings:BaseUrl is missing from appsettings.json.");
    }

    client.BaseAddress = new Uri(apiBaseUrl);
});

// These services allow the MVC frontend to call the backend API
builder.Services.AddScoped<IApiAuthService, ApiAuthService>();
builder.Services.AddScoped<IApiClientService, ApiClientService>();
builder.Services.AddScoped<IApiContractService, ApiContractService>();
builder.Services.AddScoped<IApiServiceRequestService, ApiServiceRequestService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


// helps me add decimal costs, aligns frontend with backend
var cultureInfo = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
