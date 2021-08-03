FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["general-shortener/general-shortener.csproj", "general-shortener/"]
RUN dotnet restore "general-shortener/general-shortener.csproj"
COPY . .
WORKDIR "/src/general-shortener"
RUN dotnet build "general-shortener.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "general-shortener.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "general-shortener.dll"]
