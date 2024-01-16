FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["RestDesign.csproj", "."]
RUN dotnet restore "RestDesign.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "RestDesign.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "RestDesign.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "RestDesign.dll", "--urls=http://+:80"]

