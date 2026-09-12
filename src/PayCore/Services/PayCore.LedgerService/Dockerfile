FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["FinTech.slnx", "."]
COPY ["src/PayCore/Services/PayCore.LedgerService/PayCore.LedgerService.csproj", "src/PayCore/Services/PayCore.LedgerService/"]
COPY ["src/PayCore/Shared/PayCore.Domain/PayCore.Domain.csproj", "src/PayCore/Shared/PayCore.Domain/"]
COPY ["src/PayCore/Shared/PayCore.BuildingBlocks/PayCore.BuildingBlocks.csproj", "src/PayCore/Shared/PayCore.BuildingBlocks/"]

RUN dotnet restore "src/PayCore/Services/PayCore.LedgerService/PayCore.LedgerService.csproj"

COPY . .
WORKDIR "/src/src/PayCore/Services/PayCore.LedgerService"
RUN dotnet publish "PayCore.LedgerService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PayCore.LedgerService.dll"]
