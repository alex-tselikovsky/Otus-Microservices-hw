FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["UserProfile.csproj", "UserProfile/"]
RUN dotnet restore "UserProfile/UserProfile.csproj"
WORKDIR "/src/UserProfile"
COPY . .
RUN dotnet build "UserProfile.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UserProfile.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserProfile.dll"]
