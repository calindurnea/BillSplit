# BillSplit

Run from repo root:

# Local

- docker compose -p bill-split-local -f deploy/bill-split.local.yml up -d
- docker compose -p bill-split-local -f deploy/bill-split.local.yml up --build --force-recreate -d

# Prod

- docker compose -p bill-split -f deploy/bill-split.yml up -d

Desired flow
![image](https://user-images.githubusercontent.com/17986810/211364839-f15d75cd-ed9a-441d-ad5f-24c4d1fa37d6.png)

Current DB schema
![image](https://user-images.githubusercontent.com/17986810/211364929-de877c49-4178-4a73-bec6-cd669d2083c2.png)
