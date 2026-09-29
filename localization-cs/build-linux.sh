#!/bin/sh
# Unofficial UVtools Czech community edition, modified 2026-09-29.
# Distributed under the upstream AGPL license; see ../LICENSE.
set -eu
uvtools_source_dir=$(CDPATH= cd -- "$(dirname -- "$0")/.." && pwd)
uvtools_dotnet=${UVTOOLS_DOTNET:-dotnet}
uvtools_publish_dir="$uvtools_source_dir/artifacts/publish/UVtools-CZ-linux-x64"
cd "$uvtools_source_dir"
"$uvtools_dotnet" publish UVtools.UI/UVtools.UI.csproj \
    -c Release -r linux-x64 --self-contained true \
    -p:DebugType=None -p:DebugSymbols=false \
    -p:ContinuousIntegrationBuild=true \
    -o "$uvtools_publish_dir"
cp localization-cs/start-uvtools-cz.sh "$uvtools_publish_dir/"
cp localization-cs/README.md "$uvtools_publish_dir/COMMUNITY-README.md"
cp UVtools.UI/Assets/Icons/UVtools.svg "$uvtools_publish_dir/UVtools.svg"
mkdir -p "$uvtools_publish_dir/screenshots"
cp localization-cs/screenshots/*.png "$uvtools_publish_dir/screenshots/"
chmod 755 "$uvtools_publish_dir/start-uvtools-cz.sh"
printf '\nBuilt community preview in: %s\n' "$uvtools_publish_dir"
