FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore Graduation_Project.Api/Graduation_Project.Api.csproj
RUN dotnet publish Graduation_Project.Api/Graduation_Project.Api.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

COPY --from=build /app .

ENTRYPOINT ["dotnet", "Graduation_Project.Api.dll"]