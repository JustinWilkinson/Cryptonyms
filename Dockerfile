FROM mcr.microsoft.com/dotnet/sdk:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Server/Cryptonyms.Server.csproj", "Server/"]
COPY ["Client/Cryptonyms.Client.csproj", "Client/"]
COPY ["Shared/Cryptonyms.Shared.csproj", "Shared/"]
RUN dotnet restore "Server/Cryptonyms.Server.csproj"
COPY . .
WORKDIR "/src/Server"
RUN dotnet build "Cryptonyms.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Cryptonyms.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cryptonyms.Server.dll"]