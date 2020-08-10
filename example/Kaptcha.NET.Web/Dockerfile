#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
RUN apt-get update
RUN apt-get install -y apt-utils
RUN apt-get install -y libc6-dev 
RUN apt-get install -y libgdiplus
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["example/Kaptcha.NET.Web/Kaptcha.NET.Web.csproj", "example/Kaptcha.NET.Web/"]
COPY ["src/Kaptcha.NET/Kaptcha.NET.csproj", "src/Kaptcha.NET/"]
RUN dotnet restore "example/Kaptcha.NET.Web/Kaptcha.NET.Web.csproj"
COPY . .
WORKDIR "/src/example/Kaptcha.NET.Web"
RUN dotnet build "Kaptcha.NET.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Kaptcha.NET.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Kaptcha.NET.Web.dll"]