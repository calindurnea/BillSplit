Run from repo root:

- docker compose -f deploy/ENV.docker-compose.yml up -d

For local build you can add --build --force-recreate also

- docker compose -f deploy/local.docker-compose.yml up --build --force-recreate -d

Known ENVs:

- docker compose -f deploy/local.docker-compose.yml up -d
- docker compose -f deploy/prod.docker-compose.yml up -d
