#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
#RUN apt-get update && apt-get install curl -y && rm -rf /var/lib/apt/lists/*
ENV TZ=Europe/Vienna
WORKDIR /app
EXPOSE 5000
#HEALTHCHECK --interval=1m --timeout=5s --start-period=10s --retries=3 CMD curl --fail http://localhost:5000/health || exit 1

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
ARG skipNugetSetup
RUN curl -sL https://deb.nodesource.com/setup_14.x | bash - \
 && apt-get install -y --no-install-recommends nodejs \
 && echo "node version: $(node --version)" \
 && echo "npm version: $(npm --version)" \
 && rm -rf /var/lib/apt/lists/*
WORKDIR /src
COPY ["Web/Web.csproj", "Web/"]
RUN dotnet restore "Web/Web.csproj"
COPY . .
WORKDIR "/src/Web"
RUN dotnet build "Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PhishingTraining.Web.dll"]