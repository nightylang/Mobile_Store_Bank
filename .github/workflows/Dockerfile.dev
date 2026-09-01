# Stage 1: Build Core Environment Context
FROM ://microsoft.com AS build-env
WORKDIR /app

# Cache dependency layer mappings
COPY *.csproj ./
RUN dotnet restore

# Copy entire repository source tree matrix and compile release binaries
COPY . ./
RUN dotnet publish -c Release -o out

# Stage 2: Minimalist Lightweight Runtime Production Output
FROM ://microsoft.com
WORKDIR /app
COPY --from=build-env /app/out .

# Force the production runtime engine to operate exclusively on plaintext HTTP port 5000
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

ENTRYPOINT ["dotnet", "StoreMobile.dll"]
