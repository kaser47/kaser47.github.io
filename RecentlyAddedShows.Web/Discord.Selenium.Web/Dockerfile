#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["Discord.Selenium.Web/Discord.Selenium.Web.csproj", "Discord.Selenium.Web/"]
COPY ["Discord.Selenium/Discord.Selenium.csproj", "Discord.Selenium/"]
RUN dotnet restore "Discord.Selenium.Web/Discord.Selenium.Web.csproj"
COPY . .
WORKDIR "/src/Discord.Selenium.Web"
RUN dotnet build "Discord.Selenium.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Discord.Selenium.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Discord.Selenium.Web.dll"]