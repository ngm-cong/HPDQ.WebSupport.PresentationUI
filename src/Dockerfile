# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Thêm các đối số build
ARG GITHUB_USERNAME
ARG GITHUB_PAT

# Cấu hình NuGet và xác thực với GitHub Packages
# Sử dụng 'dotnet nuget add source' để thêm nguồn gói
RUN dotnet nuget add source https://nuget.pkg.github.com/ngm-cong/index.json -n "github" -u "${GITHUB_USERNAME}" -p "${GITHUB_PAT}" --store-password-in-clear-text
RUN dotnet nuget locals all --clear

# Sao chép các file dự án và restore
COPY ["HPDQ.WebSupport.csproj", "."]
RUN dotnet restore "./HPDQ.WebSupport.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./HPDQ.WebSupport.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./HPDQ.WebSupport.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HPDQ.WebSupport.dll"]