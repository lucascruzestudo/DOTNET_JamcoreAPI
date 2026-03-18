FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.sln . 
COPY source/WebApi/Project.WebApi.csproj ./source/WebApi/
COPY source/Application/Project.Application.csproj ./source/Application/
COPY source/Infrastructure/Project.Infrastructure.csproj ./source/Infrastructure/
COPY source/Domain/Project.Domain.csproj ./source/Domain/
RUN dotnet restore

RUN dotnet tool install --global dotnet-ef

ENV PATH="${PATH}:/root/.dotnet/tools"

COPY . .

WORKDIR /app/source/WebApi
RUN dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install Redis
RUN apt-get update && apt-get install -y redis-server && rm -rf /var/lib/apt/lists/*

COPY --from=build /out .

# Create entrypoint script
RUN echo '#!/bin/bash\nredis-server --daemonize yes\ndotnet Project.WebApi.dll' > /app/entrypoint.sh && chmod +x /app/entrypoint.sh

EXPOSE 5000

ENTRYPOINT ["/app/entrypoint.sh"]
