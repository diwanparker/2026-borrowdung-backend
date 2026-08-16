FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY BorrowdungAPI/BorrowdungAPI.csproj BorrowdungAPI/
RUN dotnet restore BorrowdungAPI/BorrowdungAPI.csproj

COPY BorrowdungAPI/ BorrowdungAPI/
WORKDIR /src/BorrowdungAPI
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:5240
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 5240

ENTRYPOINT ["dotnet", "BorrowdungAPI.dll"]
