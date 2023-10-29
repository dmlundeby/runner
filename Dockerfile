FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
COPY ["*.csproj", "./"]
RUN dotnet restore
COPY . .
ARG configuration=Release
RUN dotnet build -c $configuration -o /app/build
RUN dotnet publish -c $configuration -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:8.0-alpine as final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "runner.dll"]
