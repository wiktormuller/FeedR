FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app
COPY . .
RUN dotnet publish src/Gateway/FeedR.Gateway -c release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
# Copy files from build stage from /app/out folder to our workdir /app folder
COPY --from=build /app/out .
ENV ASPNETCORE_URLS http://*:80
ENV ASPNETCORE_ENVIRONMENT docker
ENTRYPOINT dotnet FeedR.Gateway.dll

# When build from the src/Gateway location, 
# but then we have an error, because Shared project wasn't copied when building project, because Docker can see only what's beneath
# docker build -t feedr-gateway .

# When build from the FeedR location, specyfing the docker file, also the Shared project will be visible for Docker
# docker build -t feedr-gateway -f src/Gateway/Dockerfile .

# docker run --rm -it -p 5000:80 feedr-gateway
