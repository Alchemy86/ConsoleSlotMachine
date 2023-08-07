FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/SlotMachine.Console/SlotMachine.Console.csproj", "src/SlotMachine.Console/"]
RUN dotnet restore "src/SlotMachine.Console/SlotMachine.Console.csproj"
COPY . .
WORKDIR "/src/src/SlotMachine.Console"
RUN dotnet build "SlotMachine.Console.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SlotMachine.Console.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SlotMachine.Console.dll"]
