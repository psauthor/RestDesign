FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ArestMe.csproj", "."]
RUN dotnet restore "ArestMe.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "ArestMe.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ArestMe.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ArestMe.dll"]
