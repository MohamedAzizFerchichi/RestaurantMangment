using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using RestaurantManagement.Client;
using RestaurantManagement.Client.Services;
using MudBlazor.Services;
using Microsoft.AspNetCore.Components.Authorization;
using RestaurantManagement.Client.Auth;
using Blazored.LocalStorage;
using System.Net.Http.Headers;
using Microsoft.Extensions.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Add MudBlazor services
builder.Services.AddMudServices(config =>
{
    config.PopoverOptions.ThrowOnDuplicateProvider = false; // Disable duplicate provider warning
});

// Add LocalStorage
builder.Services.AddBlazoredLocalStorage();

// Configure HttpClient
builder.Services.AddScoped<AuthenticationHttpMessageHandler>();
builder.Services.AddHttpClient("RestaurantAPI", client => 
{
    client.BaseAddress = new Uri("http://localhost:5239/"); // Updated to match server's port
}).AddHttpMessageHandler<AuthenticationHttpMessageHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
    .CreateClient("RestaurantAPI"));

// Add authorization core
builder.Services.AddAuthorizationCore();

// Add authentication state provider
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

// Add auth service
builder.Services.AddScoped<RestaurantManagement.Client.Auth.IAuthService, RestaurantManagement.Client.Auth.AuthService>();

// Add new services
builder.Services.AddScoped<PlateService>();
builder.Services.AddScoped<OrderService>();

await builder.Build().RunAsync();
