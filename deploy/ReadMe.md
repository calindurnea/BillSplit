Run from repo root:

# Local

- docker compose -p bill-split-local -f deploy/bill-split.local.yml up -d
- docker compose -p bill-split-local -f deploy/bill-split.local.yml up --build --force-recreate -d

# Prod

- docker compose -p bill-split -f deploy/bill-split.yml up -d
