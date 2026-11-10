FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["BankApp.API/BankApp.API.csproj", "BankApp.API/"]
COPY ["BankApp/BankApp.csproj", "BankApp/"]
RUN dotnet restore "BankApp.API/BankApp.API.csproj"

COPY . .
WORKDIR "/src/BankApp.API"
RUN dotnet publish "BankApp.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "BankApp.API.dll"]
