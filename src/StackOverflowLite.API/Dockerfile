FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/StackOverflowLite.API/StackOverflowLite.API.csproj", "src/StackOverflowLite.API/"]
COPY ["src/StackOverflowLite.Application/StackOverflowLite.Application.csproj", "src/StackOverflowLite.Application/"]
COPY ["src/StackOverflowLite.Infrastructure/StackOverflowLite.Infrastructure.csproj", "src/StackOverflowLite.Infrastructure/"]
COPY ["src/StackOverflowLite.Domain/StackOverflowLite.Domain.csproj", "src/StackOverflowLite.Domain/"]

RUN dotnet restore "src/StackOverflowLite.API/StackOverflowLite.API.csproj"

COPY . .
RUN dotnet publish "src/StackOverflowLite.API/StackOverflowLite.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "StackOverflowLite.API.dll"]