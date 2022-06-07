# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY *.csproj ./aspnetapp/
WORKDIR /source/aspnetapp
RUN dotnet restore

# copy everything else and build app
COPY ./. ./aspnetapp/
WORKDIR /source/aspnetapp
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
WORKDIR /app
COPY --from=build /app ./
COPY --from=build /app/. ./

WORKDIR /
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "app/Sophie.dll"]

# docker build --no-cache -t aspnetcore_sophie:v1.0 -f Dockerfile . 
# docker run -it --rm --name aspnetcore_sophie aspnetcore_sophie:v1.0 -p 5005:80
# docker run -d --name aspnetcore_sophie aspnetcore_sophie:v1.0 -p 5005:80

# dotnet publish -c release -o ./app
# dotnet run ./app/Sophie.dll
