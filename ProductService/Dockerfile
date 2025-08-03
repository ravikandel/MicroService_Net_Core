# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App
COPY *.csproj ./
RUN dotnet restore
COPY . ./
RUN dotnet publish -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /App
ENV ASPNETCORE_URLS=http://+:8002
COPY --from=build-env /App/out .
EXPOSE 8002
ENTRYPOINT ["dotnet", "ProductService.dll"]
