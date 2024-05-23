FROM private-docker.aebank.lan/devops/dockerimages/master/dotnetsdk:dp-support AS build-env
WORKDIR /app

ARG MOD
ENV MOD=${MOD}

# Copy csproj and restore as distinct layers
COPY MessageService.sln .
COPY NuGet.config .
COPY MessageService.Abstractions/*.csproj ./MessageService.Abstractions/
COPY MessageService.Core/*.csproj ./MessageService.Core/
COPY MessageService.Web/*.csproj ./MessageService.Web/

RUN dotnet restore MessageService.Web/MessageService.Web.csproj

# Copy everything else and build
COPY . ./
RUN dotnet publish MessageService.Web/MessageService.Web.csproj -c $MOD -o ../out

# Build runtime image
FROM private-docker.aebank.lan/devops/dockerimages/master/aspnet:6.0
WORKDIR /app
COPY --from=build-env --chown=appuser:appuser /out .
ENTRYPOINT ["dotnet", "MessageService.Web.dll"]
