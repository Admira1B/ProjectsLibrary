using Microsoft.EntityFrameworkCore;
using ProjectsLibrary.CompositionRoot;
using ProjectsLibrary.DAL;
using ProjectsLibrary.Mapping;
using ProjectsLibrary.Domain.Models.Options;
using ProjectsLibrary.MinimalAPI.Extensions;
using ProjectsLibrary.MinimalAPI.Middlewares;

namespace ProjectsLibrary.MinimalAPI {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            // Adding DbContext
            builder.Services.AddDbContext<ProjectsLibraryDbContext>(options => {
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

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.MapApplicationEndpoints();

            app.Run();
        }
    }
}
