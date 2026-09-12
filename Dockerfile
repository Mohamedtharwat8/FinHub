FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["FinTech.slnx", "."]
COPY ["src/FinHub/FinHub.Api/FinHub.Api.csproj", "src/FinHub/FinHub.Api/"]
COPY ["src/FinHub/FinHub.Application/FinHub.Application.csproj", "src/FinHub/FinHub.Application/"]
COPY ["src/FinHub/FinHub.Domain/FinHub.Domain.csproj", "src/FinHub/FinHub.Domain/"]
COPY ["src/FinHub/FinHub.Infrastructure/FinHub.Infrastructure.csproj", "src/FinHub/FinHub.Infrastructure/"]

RUN dotnet restore "src/FinHub/FinHub.Api/FinHub.Api.csproj"

COPY . .
WORKDIR "/src/src/FinHub/FinHub.Api"
RUN dotnet publish "FinHub.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
ENV PORT=8080
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FinHub.Api.dll"]
