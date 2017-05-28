#!/usr/bin/env bash
set -eu

dotnet restore
dotnet build

# Tests
dotnet test NBT.Standard.Test/NBT.Standard.Test.csproj