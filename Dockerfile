FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
RUN apt update && \
    apt install -y ffmpeg libsodium-dev libopus-dev libopus0 libsodium23
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Capybotta.csproj", "./"]
RUN dotnet restore "Capybotta.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "Capybotta.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Capybotta.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Capybotta.dll"]
