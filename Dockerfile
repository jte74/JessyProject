# Étape de construction
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-stage
WORKDIR /src

RUN apt-get update && apt-get install -y net-tools

# Configuration du port (optionnel pour l'étape de build)
ENV ASPNETCORE_URLS=http://*:80
EXPOSE 80

# Copie et restauration
COPY ["JessyProject/JessyProject.csproj", "JessyProject/"]
RUN dotnet restore "JessyProject/JessyProject.csproj"

# Copie du code source
COPY . .

# Publication
RUN dotnet publish "JessyProject/JessyProject.csproj" -c Release -o /app/publish

# Étape d'exécution
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Configuration du port pour Render
ENV ASPNETCORE_URLS=http://*:$PORT
EXPOSE $PORT

# CORRECTION ICI : utilisation du bon nom d'étape
COPY --from=build-stage /app/publish .  

ENTRYPOINT ["dotnet", "JessyProject.dll"]