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

# Install ffmpeg for audio processing
RUN apt-get update && apt-get install -y ffmpeg && rm -rf /var/lib/apt/lists/*

# Create a non-root user to run the application
RUN groupadd -r appgroup && useradd -r -g appgroup -s /sbin/nologin appuser

COPY --from=build /out .

# Ensure the app files are owned by the non-root user
RUN chown -R appuser:appgroup /app

# Switch to non-root user
USER appuser

EXPOSE 5000

ENTRYPOINT ["dotnet", "Project.WebApi.dll"]
