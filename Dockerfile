FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
RUN apk update && apk add curl && apk add openjdk11-jre 
WORKDIR /app
ENV PATH="$PATH:/root/.dotnet/tools"
RUN dotnet tool install --global --version 5.2.1 dotnet-reportgenerator-globaltool
COPY . .

RUN dotnet restore
RUN dotnet build -c Release --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
