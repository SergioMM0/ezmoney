FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["deployable/UserRepository/UserRepository.csproj", "deployable/UserRepository/"]
COPY ["shared/Messages/Messages.csproj", "shared/Messages/"]
COPY ["shared/Domain/Domain.csproj", "shared/Domain/"]
COPY ["library/RPC/RPC.csproj", "library/RPC/"]
RUN dotnet restore "deployable/UserRepository/UserRepository.csproj"
COPY . .
WORKDIR "/src/deployable/UserRepository"
RUN dotnet build "UserRepository.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "UserRepository.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserRepository.dll"]
