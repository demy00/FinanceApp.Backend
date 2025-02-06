# syntax=docker/dockerfile:1
# https://github.com/dotnet/dotnet-docker/blob/main/samples/README.md

# Create a stage for building the application.
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build

WORKDIR /source

COPY ./FinanceApp.sln ./
COPY src/ ./src/
COPY test/ ./test/

# This is the architecture you’re building for, which is passed in by the builder.
# Placing it here allows the previous steps to be cached across architectures.
ARG TARGETARCH

# Restore dependencies 
RUN dotnet nuget locals all --clear && \
    dotnet restore FinanceApp.sln --force

# Build solution
RUN dotnet build FinanceApp.sln -c Release --no-restore

# Test stage
FROM build AS test
WORKDIR /source
RUN dotnet test FinanceApp.sln -c Release --no-build --verbosity normal

# Publish Stage
FROM build AS publish
WORKDIR /app
RUN dotnet publish src/FinanceApp.Api/FinanceApp.Api.csproj \
    -a ${TARGETARCH/amd64/x64} \
    --use-current-runtime \
    --self-contained false \
    -c Release \
    -o .

# Development stage
FROM build AS development
WORKDIR /source/src/FinanceApp.Api
CMD dotnet run --no-launch-profile

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "FinanceApp.Api.dll"]