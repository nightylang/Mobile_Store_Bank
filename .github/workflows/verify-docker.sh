# 1. Force down any dead cache structures
docker compose down --remove-orphans

# 2. Re-trigger container isolation mesh build parameters in detached mode
docker compose up --build -d
