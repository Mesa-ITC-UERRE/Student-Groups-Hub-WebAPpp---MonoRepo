#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
venv_dir="$repo_root/.tools/graphify"

python3 -m venv "$venv_dir"
"$venv_dir/bin/python" -m pip install -r "$repo_root/requirements-graphify.txt"
"$venv_dir/bin/graphify" --version
