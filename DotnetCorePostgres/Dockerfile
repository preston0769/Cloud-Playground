FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 9229

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["DotnetCorePostgres.csproj", "./"]
RUN dotnet restore "./DotnetCorePostgres.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "DotnetCorePostgres.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "DotnetCorePostgres.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "DotnetCorePostgres.dll"]
