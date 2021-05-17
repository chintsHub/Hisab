FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /app

# Install nodejs
RUN apt-get install --yes curl
RUN curl --silent --location https://deb.nodesource.com/setup_14.x | bash -
RUN apt-get install --yes nodejs

# Restore Nuget 
COPY *.sln .
COPY Hisab.AWS/*.csproj ./Hisab.AWS/
COPY Hisab.BL/*.csproj ./Hisab.BL/
COPY Hisab.Common/*.csproj ./Hisab.Common/
COPY Hisab.Dapper/*.csproj ./Hisab.Dapper/
COPY Hisab.Database.Test/*.csproj ./Hisab.Database.Test/
COPY Hisab.UI/*.csproj ./Hisab.UI/
RUN dotnet restore 

# Restore NPM dependencies
COPY ["Hisab.UI/package.json", "./Hisab.UI/"]
COPY ["Hisab.UI/package-lock.json", "./Hisab.UI/"]
WORKDIR /app/Hisab.UI
RUN npm install

# copy everything else and build app
WORKDIR /app
COPY . .
WORKDIR /app/Hisab.UI
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
WORKDIR /app
COPY --from=build /app/Hisab.UI/out ./
COPY --from=build /app/Hisab.UI/node_modules ./node_modules
ENTRYPOINT ["dotnet", "Hisab.UI.dll"]

