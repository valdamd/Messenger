# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

# Copy project files
COPY ["Services/Message/Message.Api/Message.Api.csproj", "Services/Message/Message.Api/"]
COPY ["Messages/Pingo.Messages/Pingo.Messages.Infrastructure/Pingo.Messages.Infrastructure.csproj", "Messages/Pingo.Messages/Pingo.Messages.Infrastructure/"]
COPY ["Messages/Pingo.Messages/Pingo.Messages.Application/Pingo.Messages.Application.csproj", "Messages/Pingo.Messages/Pingo.Messages.Application/"]
COPY ["Messages/Pingo.Messages/Pingo.Messages.Domain/Pingo.Messages.Domain.csproj", "Messages/Pingo.Messages/Pingo.Messages.Domain/"]

# Restore dependencies
RUN dotnet restore "Services/Message/Message.Api/Message.Api.csproj"

# Copy source code
COPY . .

# Build
WORKDIR "/src/Services/Message/Message.Api"
RUN dotnet build "Message.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Message.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Message.Api.dll"]
