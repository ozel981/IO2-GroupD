# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY sale-system/*.csproj ./sale-system/
COPY SaleSystemIntegrationTests/*.csproj ./SaleSystemIntegrationTests/
COPY SaleSystemUnitTests/*.csproj ./SaleSystemUnitTests/
RUN dotnet restore -r linux-x64 --disable-parallel

# copy everything else and build app
COPY sale-system/. ./sale-system/
WORKDIR /source/sale-system
RUN dotnet publish -c release -o /app -r linux-x64 --self-contained false --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:5.0-focal-amd64
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["./sale-system"]