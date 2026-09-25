#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
graphify_bin="$repo_root/.tools/graphify/bin/graphify"

if [[ ! -x "$graphify_bin" ]]; then
  echo "Graphify no está preparado. Ejecuta scripts/setup-graphify.sh." >&2
  exit 1
fi

cd "$repo_root"
exec "$graphify_bin" "$@"
