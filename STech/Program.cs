using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.FileProviders;
using STech.Config;
using STech.Data.Models;
using STech.Services;
using STech.Services.Services;
using Stripe;
using System.Security.Claims;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.DocInclusionPredicate((docName, apiDesc) =>
    {
        return apiDesc.RelativePath != null && apiDesc.RelativePath.StartsWith("api/");
    });
});

builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
{
    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
}));

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = new PathString("/");
    options.AccessDeniedPath = new PathString("/access-denied");
    options.ExpireTimeSpan = TimeSpan.FromDays(90);
})
.AddFacebook(facebookOptions =>
{
    IConfigurationSection facebookAuthNSection = builder.Configuration.GetSection("Authentication:Facebook");

    facebookOptions.AppId = facebookAuthNSection["AppId"] ?? "";
    facebookOptions.AppSecret = facebookAuthNSection["AppSecret"] ?? "";
    facebookOptions.SaveTokens = true;
    facebookOptions.ClaimActions.MapJsonKey("picture", "picture.data.url");
    facebookOptions.Fields.Add("picture");

    facebookOptions.Events = new OAuthEvents
    {
        OnCreatingTicket = context =>
        {
            string picture = context.User.GetProperty("picture").GetProperty("data").GetProperty("url").GetString() ?? "";
            context.Identity?.AddClaim(new Claim("picture", picture));
            return Task.CompletedTask;
        },
        OnAccessDenied = context =>
        {
            context.Response.Redirect("/error/login-error");
            context.HandleResponse();
            return Task.CompletedTask;
        },
        OnRemoteFailure = context =>
        {
            context.Response.Redirect("/error/login-error");
            context.HandleResponse();
            return Task.CompletedTask;
        },
    };

})
.AddGoogle(googleOptions =>
{
    IConfigurationSection facebookAuthNSection = builder.Configuration.GetSection("Authentication:Google");

    googleOptions.ClientId = facebookAuthNSection["ClientId"] ?? "";
    googleOptions.ClientSecret = facebookAuthNSection["ClientSecret"] ?? "";
    googleOptions.SaveTokens = true;
    googleOptions.ClaimActions.MapJsonKey("picture", "picture");

    googleOptions.Events = new OAuthEvents
    {
        OnAccessDenied = context =>
        {
            context.Response.Redirect("/error/login-error");
            context.HandleResponse();
            return Task.CompletedTask;
        },
        OnRemoteFailure = context =>
        {
            context.Response.Redirect("/error/login-error");
            context.HandleResponse();
            return Task.CompletedTask;
        },
    };
});

builder.Services.AddSingleton(sp =>
{
    IConfiguration configuration = builder.Configuration.GetSection("MongoDB");

    string connectionString = configuration["ConnectionString"] ?? "";
    string dbName = configuration["DbName"] ?? "";

    return new StechDbContext(connectionString, dbName);
});

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, STech.Services.Services.ProductService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<ISliderService, SliderService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<ICustomerService, STech.Services.Services.CustomerService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IDeliveryService, DeliveryService>();
builder.Services.AddScoped<IReviewService, STech.Services.Services.ReviewService>();
builder.Services.AddScoped(typeof(Lazy<>), typeof(Lazy<>));

builder.Services.AddSingleton(new AddressService(Path.Combine(builder.Environment.ContentRootPath, "DataFiles", "Address")));

builder.Services.AddHttpClient<IGeocodioService, GeocodioService>(client =>
{
    client.BaseAddress = new Uri("https://api.opencagedata.com/geocode/v1/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddSingleton<IGeocodioService, GeocodioService>(sp =>
    new GeocodioService(sp.GetRequiredService<HttpClient>(), builder.Configuration.GetSection("OpenCageGeocodio")["ApiKey"] ?? "")
);

builder.Services.AddScoped<IAzureService, AzureService>(sp =>
{
    IConfiguration configuration = builder.Configuration.GetSection("Azure");
    return new AzureService(configuration["ConnectionString"] ?? "", configuration["BlobContainerName"] ?? "", configuration["BlobUrl"] ?? "");
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(15);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

StripeConfiguration.ApiKey = builder.Configuration.GetSection("Payments:Stripe")["SecretKey"];

CloudflareTurnstile.SiteKey = builder.Configuration.GetSection("Cloudflare:Turnstile")["SiteKey"];
CloudflareTurnstile.SecretKey = builder.Configuration.GetSection("Cloudflare:Turnstile")["SecretKey"];
CloudflareTurnstile.ApiUrl = builder.Configuration.GetSection("Cloudflare:Turnstile")["ApiUrl"];

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
    //app.UseStatusCodePages();
}


app.UseStaticFiles();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "Areas", "Admin", "wwwroot")),
    RequestPath = "/admin"
});

app.UseRouting();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.UseSession();


app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
