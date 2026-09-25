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
import subprocess
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

manifest_paths = [str(Path(path)) for path in manifest]
ignored = subprocess.run(
    ["git", "check-ignore", "--no-index", "--stdin"],
    input="\n".join(manifest_paths),
    text=True,
    capture_output=True,
    check=False,
)
if ignored.returncode not in (0, 1):
    raise SystemExit("No se pudo verificar .gitignore: " + ignored.stderr.strip())
ignored_paths = [path for path in ignored.stdout.splitlines() if path]
if ignored_paths:
    preview = ", ".join(ignored_paths[:10])
    suffix = " ..." if len(ignored_paths) > 10 else ""
    raise SystemExit(
        "manifest.json contiene archivos ignorados por Git: " + preview + suffix
    )

sensitive_paths = {
    ".env",
    ".env.github",
    ".env.github.example",
    "blazor/appsettings.Development.json",
}
leaked = sorted(sensitive_paths.intersection(manifest_paths))
if leaked:
    raise SystemExit(
        "manifest.json contiene configuración local/sensible: " + ", ".join(leaked)
    )
print("manifest.json: sin archivos ignorados o configuración local sensible")

freshness = detect_incremental(
    Path("."),
    manifest_path="graphify-out/manifest.json",
    kind="ast",
)
if freshness["new_total"] or freshness["deleted_files"] or freshness["excluded_files"]:
    raise SystemExit(
        "El grafo estructural requiere actualización: "
        f"{freshness['new_total']} modificados, "
        f"{len(freshness['deleted_files'])} eliminados, "
        f"{len(freshness['excluded_files'])} ahora excluidos"
    )
print("manifest.json: grafo estructural actualizado")
PY
"$graphify_bin" query "MembershipService" --budget 200 >/dev/null
echo "Graphify está operativo y el grafo es consultable."
