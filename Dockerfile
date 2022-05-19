FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Dashboard-Backend.csproj", "./"]
RUN dotnet restore "Dashboard-Backend.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "Dashboard-Backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Dashboard-Backend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dashboard-Backend.dll"]
