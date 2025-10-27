#!/bin/bash
set -e

# Go into your project folder
cd "Daraz CloneAgain"

# Restore and build the project
dotnet restore
dotnet publish -c Release -o out

# Move into published folder and run the app
cd out
dotnet "Daraz CloneAgain.dll"
