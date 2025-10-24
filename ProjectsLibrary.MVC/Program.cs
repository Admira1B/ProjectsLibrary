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
    options.UseSqlServer(connectionString: builder.Configuration.GetConnectionString("DefaultConnectionMVC"));
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
builder.Services.AddControllersWithViews();

builder.Services.AddMvc()
    .AddViewOptions(options => {
        options.HtmlHelperOptions.ClientValidationEnabled = true;
    });

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStatusCodePages(context =>
{
    var response = context.HttpContext.Response;

    if (response.StatusCode == 401)
    {
        response.Redirect("/Home/Login");
    }

    return Task.CompletedTask;
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
