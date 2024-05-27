FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

ARG MOD
ENV MOD=${MOD}

# Copy csproj and restore as distinct layers
COPY MessageService.sln .
COPY NuGet.config .
COPY MessageService.Abstractions/*.csproj ./MessageService.Abstractions/
COPY MessageService.Client2/*.csproj ./MessageService.Client2/

RUN dotnet restore MessageService.Client2/MessageService.Client2.csproj

# Copy everything else and build
COPY . ./
RUN dotnet publish MessageService.Client2/MessageService.Client2.csproj -c $MOD -o ../out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env --chown=appuser:appuser /out .
ENTRYPOINT ["dotnet", "MessageService.Client2.dll"]
