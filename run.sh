# !/bin/bash

cd ./app
docker build . -t currency-worker-app

docker run --env-file ../.env currency-worker-app