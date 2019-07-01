FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
RUN apt-get update
RUN apt-get install -y apt-utils
RUN apt-get install -y libc6-dev 
RUN apt-get install -y libgdiplus
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["Kaptcha.NET.Test/Kaptcha.NET.Test.csproj", "Kaptcha.NET.Test/"]
RUN dotnet restore "Kaptcha.NET.Test/Kaptcha.NET.Test.csproj"
COPY . .
WORKDIR "/src/Kaptcha.NET.Test"
RUN dotnet build "Kaptcha.NET.Test.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Kaptcha.NET.Test.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Kaptcha.NET.Test.dll"]