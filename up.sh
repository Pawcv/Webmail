#!/bin/bash
docker-compose -f docker-compose.ci.build.yml up --build
docker-compose -f docker-compose.yml up --build
