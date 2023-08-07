FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app
COPY . .
RUN dotnet publish src/Feeds/Quotes/FeedR.Feeds.Quotes -c release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
# Copy files from build stage from /app/out folder to our workdir /app folder
COPY --from=build /app/out .
ENV ASPNETCORE_URLS http://*:80
ENV ASPNETCORE_ENVIRONMENT docker
ENTRYPOINT dotnet FeedR.Feeds.Quotes.dll