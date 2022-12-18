FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /gibusersync
EXPOSE 5005

COPY . ./

RUN dotnet restore /gibusersync/src
# Build and publish a release
RUN dotnet publish /gibusersync/src/GibUsers.Api -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /todoapi
COPY --from=build-env /gibusersync/out .

ENV ASPNETCORE_ENVIRONMENT=Development
ENTRYPOINT ["dotnet", "GibUsers.Api.dll"]