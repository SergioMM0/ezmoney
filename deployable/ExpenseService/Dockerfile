﻿FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["deployable/ExpenseService/ExpenseService.csproj", "deployable/ExpenseService/"]
COPY ["shared/Messages/Messages.csproj", "shared/Messages/"]
COPY ["shared/Domain/Domain.csproj", "shared/Domain/"]
COPY ["library/RPC/RPC.csproj", "library/RPC/"]
RUN dotnet restore "deployable/ExpenseService/ExpenseService.csproj"
COPY . .
WORKDIR "/src/deployable/ExpenseService"
RUN dotnet build "ExpenseService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ExpenseService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExpenseService.dll"]
