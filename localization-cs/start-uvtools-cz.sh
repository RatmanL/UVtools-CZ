#!/bin/sh
# Unofficial UVtools Czech edition; keep this script with the application files.
set -eu
uvtools_app_dir=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)
cd "$uvtools_app_dir"
exec "$uvtools_app_dir/UVtools" "$@"
