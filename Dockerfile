FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set working directory
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY *.sln .
COPY WebApiShop/*.csproj ./WebApiShop/
COPY Services/*.csproj ./Services/
COPY Repositories/*.csproj ./Repositories/
COPY DTOs/*.csproj ./DTOs/
COPY Entities/*.csproj ./Entities/
COPY Tests/*.csproj ./Tests/

RUN dotnet restore --verbosity minimal

# Copy everything else and publish
COPY . .

# Build and publish trimmed, ready for production
RUN dotnet publish WebApiShop/EventDressRental.csproj \
    -c Release \
    -r linux-x64 \
    --self-contained false \
    /p:TrimUnusedDependencies=true \
    /p:PublishReadyToRun=true \
    -o /app/publish

### Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Create non-root user for better security
ENV APP_USER=app
RUN adduser --system --group $APP_USER || useradd -m $APP_USER || true

WORKDIR /app

# Copy publish output
COPY --from=build /app/publish ./

# Set environment variables appropriate for production
ENV DOTNET_URLS=http://+:80 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_RUNNING_IN_CONTAINER=true

# Expose the listening port
EXPOSE 80

# Use non-root user
USER $APP_USER

ENTRYPOINT ["dotnet", "EventDressRental.dll"]
