FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

# FIND csproj dynamically (no path guessing)
RUN find . -name "*.csproj"

# THEN publish using correct path (IMPORTANT: replace after you see result)
RUN dotnet publish $(find . -name "*.csproj" | head -n 1) -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "FraudDetection.API.dll"]