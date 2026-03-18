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

# Install Redis and curl for health checks
RUN apt-get update && apt-get install -y redis-server curl && rm -rf /var/lib/apt/lists/*

# Create Redis data directory
RUN mkdir -p /app/redis-data

COPY --from=build /out .

# Create entrypoint script that starts Redis on localhost:6379 and waits for it
RUN printf '#!/bin/bash\nset -e\necho "Starting Redis on localhost:6379..."\nredis-server --daemonize yes --logfile /app/redis.log --loglevel notice --port 6379 --bind 127.0.0.1 --dir /app/redis-data --dbfilename dump.rdb\necho "Waiting for Redis to be ready on localhost:6379..."\nfor i in {1..30}; do\n  if redis-cli -h 127.0.0.1 -p 6379 ping > /dev/null 2>&1; then\n    echo "Redis is ready on localhost:6379!"\n    break\n  fi\n  echo "Attempt $i: Redis not ready, waiting..."\n  sleep 1\ndone\necho "Starting .NET API on port 5000..."\nexec dotnet Project.WebApi.dll' > /app/entrypoint.sh && chmod +x /app/entrypoint.sh

# Redis internal port (not exposed outside container), API port
EXPOSE 5000

ENTRYPOINT ["/app/entrypoint.sh"]
