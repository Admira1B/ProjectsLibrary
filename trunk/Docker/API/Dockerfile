FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["ProjectsLibrary.API/ProjectsLibrary.API.csproj", "ProjectsLibrary.API/"]
COPY ["ProjectLibrary.Domain/ProjectsLibrary.Domain.csproj", "ProjectLibrary.Domain/"]
COPY ["ProjectsLibrary.DAL/ProjectsLibrary.DAL.csproj", "ProjectsLibrary.DAL/"]
COPY ["ProjectsLibrary.DI/ProjectsLibrary.CompositionRoot.csproj", "ProjectsLibrary.DI/"]
COPY ["ProjectsLibrary.DTOs/ProjectsLibrary.DTOs.csproj", "ProjectsLibrary.DTOs/"]
COPY ["ProjectsLibrary.Mapping/ProjectsLibrary.Mapping.csproj", "ProjectsLibrary.Mapping/"]
COPY ["ProjectsLibrary.Services/ProjectsLibrary.Services.csproj", "ProjectsLibrary.Services/"]

RUN dotnet restore "ProjectsLibrary.API/ProjectsLibrary.API.csproj"

COPY ["ProjectLibrary.Domain/", "ProjectLibrary.Domain/"]
COPY ["ProjectsLibrary.DAL/", "ProjectsLibrary.DAL/"]
COPY ["ProjectsLibrary.DI/", "ProjectsLibrary.DI/"]
COPY ["ProjectsLibrary.DTOs/", "ProjectsLibrary.DTOs/"]
COPY ["ProjectsLibrary.Mapping/", "ProjectsLibrary.Mapping/"]
COPY ["ProjectsLibrary.Services/", "ProjectsLibrary.Services/"]
COPY ["ProjectsLibrary.API/", "ProjectsLibrary.API/"]

WORKDIR "/src/ProjectsLibrary.API"
RUN dotnet build "ProjectsLibrary.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ProjectsLibrary.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProjectsLibrary.API.dll"]