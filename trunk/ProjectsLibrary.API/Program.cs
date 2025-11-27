using Microsoft.EntityFrameworkCore;
using ProjectsLibrary.API.Middlewares;
using ProjectsLibrary.CompositionRoot;
using ProjectsLibrary.DAL;
using ProjectsLibrary.Domain.Models.Options;
using ProjectsLibrary.Mapping;

var builder = WebApplication.CreateBuilder(args);

// Adding DbContext
builder.Services.AddDbContext<ProjectsLibraryDbContext>(options =>
{
    options.UseSqlServer(connectionString: builder.Configuration.GetConnectionString("DefaultConnectionAPI"));
});
// Adding ProjectsLibrary.Services and ProjectsLibrary.DAL Dependencies
builder.Services.AddDataAccessDependencies();
builder.Services.AddServicesDependencies();
// Adding AutoMapperProfile
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
// Adding JWT Authentication & Authorization
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(nameof(JwtOptions)));
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddJwtAuthorization();

builder.Services.AddProblemDetails();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
