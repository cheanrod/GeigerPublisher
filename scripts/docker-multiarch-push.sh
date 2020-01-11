#!/bin/bash

docker build -t cheanrod/geigerpublisher:amd64 -f Dockerfile .
docker build -t cheanrod/geigerpublisher:arm32v7 -f Dockerfile.arm32v7 .
docker build -t cheanrod/geigerpublisher:arm64v8 -f Dockerfile.arm64v8 .

docker push cheanrod/geigerpublisher:amd64
docker push cheanrod/geigerpublisher:arm32v7
docker push cheanrod/geigerpublisher:arm64v8

docker manifest create cheanrod/geigerpublisher:latest cheanrod/geigerpublisher:amd64 cheanrod/geigerpublisher:arm32v7 cheanrod/geigerpublisher:arm64v8

docker manifest annotate cheanrod/geigerpublisher:latest cheanrod/geigerpublisher:arm32v7 --os linux --arch arm --variant v7
docker manifest annotate cheanrod/geigerpublisher:latest cheanrod/geigerpublisher:arm64v8 --os linux --arch arm64 --variant v8

docker manifest inspect cheanrod/geigerpublisher:latest

docker manifest push cheanrod/geigerpublisher:latest
