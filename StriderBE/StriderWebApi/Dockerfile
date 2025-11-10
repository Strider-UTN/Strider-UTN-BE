# Dockerfile para deploy del backend
# Puede usarse con Railway, Render, o cualquier servicio que soporte Docker

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["StriderWebApi.csproj", "./"]
RUN dotnet restore "StriderWebApi.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "StriderWebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StriderWebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StriderWebApi.dll"]

