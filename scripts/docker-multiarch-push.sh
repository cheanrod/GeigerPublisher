#!/bin/bash

docker build -t cheanrod/geigerpublisher:amd64 -f Dockerfile .
docker build -t cheanrod/geigerpublisher:arm32v7 --build-arg DOTNET_RUNTIME=linux-arm \
 --build-arg IMAGE_TAG=3.1-buster-slim-arm32v7 -f Dockerfile .
docker build -t cheanrod/geigerpublisher:arm64v8 --build-arg DOTNET_RUNTIME=linux-arm64 \
 --build-arg IMAGE_TAG=3.1-buster-slim-arm64v8 -f Dockerfile .

docker push cheanrod/geigerpublisher:amd64
docker push cheanrod/geigerpublisher:arm32v7
docker push cheanrod/geigerpublisher:arm64v8

docker manifest create --amend cheanrod/geigerpublisher:latest cheanrod/geigerpublisher:amd64 cheanrod/geigerpublisher:arm32v7 cheanrod/geigerpublisher:arm64v8

docker manifest annotate cheanrod/geigerpublisher:latest cheanrod/geigerpublisher:arm32v7 --os linux --arch arm --variant v7
docker manifest annotate cheanrod/geigerpublisher:latest cheanrod/geigerpublisher:arm64v8 --os linux --arch arm64 --variant v8

docker manifest inspect cheanrod/geigerpublisher:latest

docker manifest push --purge cheanrod/geigerpublisher:latest
