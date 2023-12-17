# Define the base image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Define the build stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy and restore project dependencies
COPY ["CurrencyConvertor.csproj", "CurrencyConvertor/"]
RUN dotnet restore "CurrencyConvertor/CurrencyConvertor.csproj"


# Copy the entire project and build
COPY . .
WORKDIR /src

# Create the output directory and grant permissions
RUN mkdir -p /app/build && chmod -R 777 /app/build


RUN dotnet build "CurrencyConvertor.csproj" -c Release -o /app/build


# Define the publish stage
FROM build AS publish
RUN dotnet publish "CurrencyConvertor.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Copy the exchangeRates.json
COPY Data/exchangeRates.json /app/publish

# Create the final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CurrencyConvertor.dll"]