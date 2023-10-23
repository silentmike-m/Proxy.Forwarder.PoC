FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /Proxy.Forwarder.PoC.Client

COPY Proxy.Forwarder.PoC.Client.csproj Proxy.Forwarder.PoC.Client.csproj
RUN dotnet restore

COPY . .

RUN dotnet build -c Release -o /app/build
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app

COPY --from=build /app/publish .

RUN groupadd -g 10000 dotnet && useradd -u 10000 -g dotnet -m -d /app dotnet && chown -R dotnet:dotnet /app
USER dotnet:dotnet

ENV ASPNETCORE_URLS http://*:30000
EXPOSE 30000

ENTRYPOINT ["dotnet", "Proxy.Forwarder.PoC.Client.dll"]
