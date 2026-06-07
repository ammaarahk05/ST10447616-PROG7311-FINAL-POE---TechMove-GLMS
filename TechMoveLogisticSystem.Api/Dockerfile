FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["TechMoveLogisticSystem.Api/TechMoveLogisticSystem.Api.csproj", "TechMoveLogisticSystem.Api/"]
RUN dotnet restore "TechMoveLogisticSystem.Api/TechMoveLogisticSystem.Api.csproj"

COPY . .
WORKDIR "/src/TechMoveLogisticSystem.Api"
RUN dotnet publish "TechMoveLogisticSystem.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TechMoveLogisticSystem.Api.dll"]