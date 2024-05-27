FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

ARG MOD
ENV MOD=${MOD}

# Copy csproj and restore as distinct layers
COPY MessageService.sln .
COPY NuGet.config .
COPY MessageService.Abstractions/*.csproj ./MessageService.Abstractions/
COPY MessageService.Connector/*.csproj ./MessageService.Connector/
COPY MessageService.Client/*.csproj ./MessageService.Client/

RUN dotnet restore MessageService.Client/MessageService.Client.csproj

# Copy everything else and build
COPY . ./
RUN dotnet publish MessageService.Client/MessageService.Client.csproj -c $MOD -o ../out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env --chown=appuser:appuser /out .
ENTRYPOINT ["dotnet", "MessageService.Client.dll"]
