using Microsoft.OpenApi.Models;
using Serilog;
using SkillBridge.Infrastructure.Ai;
using SkillBridge.Infrastructure.Extensions;
using SkillBridge.Services.GenerateAssignment;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console();
});


builder.Services.AddAuth0(builder.Configuration);
builder.Services.AddWeb();
builder.Services.AddPostgres(builder.Configuration);
//builder.Services.AddOpenAI(builder.Configuration);
builder.Services.AddPerplexity(builder.Configuration);
//builder.Services.AddStripe(builder.Configuration);
builder.Services.AddTransient<IPromptBuilder, PromptBuilder>();
builder.Services.AddScoped<IGenerateAssignmentService, GenerateAssignmentService>();

builder.Services.AddSwaggerGen(s =>
{
    s.SwaggerDoc("v1", new OpenApiInfo { Title = "SkillBridge API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use custom exception handler middleware
app.UseGlobalExceptionHandler();

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(opts =>
{
    opts.WithOrigins("http://localhost:5173").AllowCredentials();
    opts.AllowAnyHeader();
    opts.AllowAnyMethod();
    opts.WithExposedHeaders("X-Pagination");
});
app.UseAuthentication();
app.UseAuthorization();
app.UseEnsureUserProfile();

app.MapControllers();

app.Run();
