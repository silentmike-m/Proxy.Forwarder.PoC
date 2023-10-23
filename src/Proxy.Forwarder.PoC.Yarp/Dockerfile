FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /Dispacher.Proxy.WebApi

COPY Proxy.Forwarder.PoC.Yarp.csproj Proxy.Forwarder.PoC.Yarp.csproj
RUN dotnet restore

COPY . .

RUN dotnet build -c Release -o /app/build
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app

COPY --from=build /app/publish .

RUN groupadd -g 10000 dotnet && useradd -u 10000 -g dotnet -m -d /app dotnet && chown -R dotnet:dotnet /app
USER dotnet:dotnet

ENV ASPNETCORE_URLS http://*:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Proxy.Forwarder.PoC.Yarp.dll"]
