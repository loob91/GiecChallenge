FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
ENV ASPNETCORE_ENVIRONMENT Docker
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ENV ASPNETCORE_ENVIRONMENT Docker
WORKDIR /src-backend
COPY ./backend/GiecChallenge/GiecChallenge.csproj .
RUN dotnet restore GiecChallenge.csproj
COPY ./backend/GiecChallenge .
RUN dotnet build GiecChallenge.csproj -c Docker -o /app/build

FROM build AS publish
RUN dotnet publish GiecChallenge.csproj -c Docker -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GiecChallenge.dll"]