FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
RUN mkdir -p /var/server/data
WORKDIR /App

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /App
COPY --from=build-env /App/out .
ENTRYPOINT ["dotnet", "EWS_NetCore_Scheduler.dll"]
