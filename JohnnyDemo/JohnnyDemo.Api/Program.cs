using JohnnyDemo.Api.Options;
using JohnnyDemo.BusinessLogic;
using JohnnyDemo.Repository;
using JohnnyDemo.Repository.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction())
{
    // TODO. Here we can add additional application settings provider. ex. AWS ParamterStore or Azure KeyVault
}

// Add config options
builder.Services.Configure<AuthOptions>(builder.Configuration.GetSection(AuthOptions.Key));

// Add Database context
builder.Services.AddDbContext<JohnnyDemoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add repository services
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

// Add business logic services
builder.Services.AddScoped<ICustomerQueryManager, CustomerQueryManager>();
builder.Services.AddScoped<ICustomerCommandManager, CustomerCommandManager>();
builder.Services.AddScoped<IOrderQueryManager, OrderQueryManager>();
builder.Services.AddScoped<IOrderCommandManager, OrderCommandManager>();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swagger =>
{
    swagger.SwaggerDoc("v1", new OpenApiInfo { Title = "Johnny Demo API " });
    var xmlfile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    swagger.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlfile));
    swagger.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "API Key",
        Type = SecuritySchemeType.ApiKey,
        Name = "X-API-Key",
        In = ParameterLocation.Header,
        Scheme = "ApiKeyScheme"
    });
    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "ApiKey",
                    Type = ReferenceType.SecurityScheme
                }
            }, new List<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Add data migration
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<JohnnyDemoContext>();
    dbContext.Database.Migrate();
}

app.Run();
