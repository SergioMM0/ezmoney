FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["deployable/GroupRepository/GroupRepository.csproj", "deployable/GroupRepository/"]
COPY ["shared/Messages/Messages.csproj", "shared/Messages/"]
COPY ["shared/Domain/Domain.csproj", "shared/Domain/"]
COPY ["library/RPC/RPC.csproj", "library/RPC/"]
RUN dotnet restore "deployable/GroupRepository/GroupRepository.csproj"
COPY . .
WORKDIR "/src/deployable/GroupRepository"
RUN dotnet build "GroupRepository.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "GroupRepository.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GroupRepository.dll"]
