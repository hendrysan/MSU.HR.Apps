#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["WebClient/WebClient.csproj", "WebClient/"]
COPY ["Commons/Commons.csproj", "Commons/"]
COPY ["Models/Models.csproj", "Models/"]
RUN dotnet restore "./WebClient/WebClient.csproj"
COPY . .
WORKDIR "/src/WebClient"
RUN dotnet build "WebClient.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebClient.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebClient.dll"]