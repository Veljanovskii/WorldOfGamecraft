
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AccountService.csproj", "./"]
RUN dotnet restore "AccountService.csproj"
COPY . .
RUN dotnet build "AccountService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AccountService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY wait-for-it.sh ./wait-for-it.sh
RUN chmod +x ./wait-for-it.sh

COPY ["aspnetapp.pfx", "/https/"]
ENV ASPNETCORE_URLS=https://+:443
ENV ASPNETCORE_Kestrel__Certificates__Default__Password="WorldOfGamecraft"
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx

USER app
ENTRYPOINT ["./wait-for-it.sh", "sqlserver:1433", "--timeout=30", "--", "dotnet", "AccountService.dll"]