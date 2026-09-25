#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
graphify_bin="$repo_root/.tools/graphify/bin/graphify"
python_bin="$repo_root/.tools/graphify/bin/python"

if [[ ! -x "$graphify_bin" ]]; then
  echo "Graphify no está preparado. Ejecuta scripts/setup-graphify.sh." >&2
  exit 1
fi

cd "$repo_root"
"$graphify_bin" --version
"$python_bin" - <<'PY'
import json
from pathlib import Path
from graphify.detect import detect_incremental

required = (
    Path("graphify-out/graph.json"),
    Path("graphify-out/manifest.json"),
    Path("graphify-out/GRAPH_REPORT.md"),
    Path("graphify-out/graph.html"),
)
missing = [str(path) for path in required if not path.is_file()]
if missing:
    raise SystemExit("Faltan artefactos Graphify: " + ", ".join(missing))

graph = json.loads(required[0].read_text(encoding="utf-8"))
manifest = json.loads(required[1].read_text(encoding="utf-8"))
if not graph.get("nodes") or not graph.get("links"):
    raise SystemExit("graph.json no contiene nodos o relaciones")
if not isinstance(manifest, dict) or not manifest:
    raise SystemExit("manifest.json está vacío o no es válido")

print(f"graph.json: {len(graph['nodes'])} nodos, {len(graph['links'])} relaciones")
print(f"manifest.json: {len(manifest)} archivos")

freshness = detect_incremental(
    Path("."),
    manifest_path="graphify-out/manifest.json",
    kind="ast",
)
if freshness["new_total"] or freshness["deleted_files"]:
    raise SystemExit(
        "El grafo estructural requiere actualización: "
        f"{freshness['new_total']} modificados, "
        f"{len(freshness['deleted_files'])} eliminados"
    )
print("manifest.json: grafo estructural actualizado")
PY
"$graphify_bin" query "MembershipService" --budget 200 >/dev/null
echo "Graphify está operativo y el grafo es consultable."
