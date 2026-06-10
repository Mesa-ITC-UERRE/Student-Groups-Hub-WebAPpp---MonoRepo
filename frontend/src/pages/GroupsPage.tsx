import { useEffect, useState, useCallback } from "react";
import { Link } from "react-router-dom";
import { Search, Users, Plus } from "lucide-react";
import { groupApi } from "@/lib/api";
import type { Group } from "@/types";
import { Navbar } from "@/components/layout/Navbar";
import { Footer } from "@/components/layout/Footer";
import { cn } from "@/lib/utils";

const GRADIENT_COLORS = [
  "from-violet-600 to-indigo-700",
  "from-rose-500 to-pink-700",
  "from-amber-500 to-orange-600",
  "from-emerald-500 to-teal-700",
  "from-sky-500 to-blue-700",
  "from-fuchsia-500 to-purple-700",
];

// Stable gradient per group id
function gradientForId(id: string): string {
  let hash = 0;
  for (let i = 0; i < id.length; i++) hash = id.charCodeAt(i) + ((hash << 5) - hash);
  return GRADIENT_COLORS[Math.abs(hash) % GRADIENT_COLORS.length];
}

export default function GroupsPage() {
  const [groups, setGroups] = useState<Group[]>([]);
  const [categories, setCategories] = useState<string[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [search, setSearch] = useState("");
  const [debouncedSearch, setDebouncedSearch] = useState("");
  const [selectedCategory, setSelectedCategory] = useState<string | null>(null);
  const [total, setTotal] = useState(0);

  // Debounce search input by 350ms
  useEffect(() => {
    const t = setTimeout(() => setDebouncedSearch(search), 350);
    return () => clearTimeout(t);
  }, [search]);

  const loadGroups = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const res = await groupApi.getAll({
        search: debouncedSearch || undefined,
        category: selectedCategory || undefined,
      });
      setGroups(res.data);
      setTotal(res.total);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : "Error al cargar grupos");
    } finally {
      setLoading(false);
    }
  }, [debouncedSearch, selectedCategory]);

  useEffect(() => { loadGroups(); }, [loadGroups]);

  // Load categories once
  useEffect(() => {
    groupApi.getCategories()
      .then((cats) => setCategories(cats))
      .catch(() => setCategories([]));
  }, []);

  return (
    <div className="flex min-h-screen flex-col">
      <Navbar />

      <main className="flex-1 bg-zinc-50">
        {/* Hero header */}
        <section className="bg-gradient-to-br from-[#4C1D95] via-[#7C3AED] to-[#8B5CF6] py-16 text-white">
          <div className="mx-auto max-w-[1200px] px-6">
            <div className="flex flex-col gap-6 sm:flex-row sm:items-end sm:justify-between">
              <div>
                <h1 className="font-heading text-3xl font-extrabold sm:text-4xl">
                  Grupos Estudiantiles
                </h1>
                <p className="mt-2 text-base text-white/70">
                  {total > 0
                    ? `${total} grupo${total !== 1 ? "s" : ""} activo${total !== 1 ? "s" : ""} en U-ERRE`
                    : "Explora todos los grupos activos de la Universidad Regiomontana"}
                </p>
              </div>
              <Link
                to="/groups/register"
                className="inline-flex items-center gap-2 self-start rounded-xl border border-white/25 bg-white/10 px-5 py-2.5 text-sm font-semibold text-white backdrop-blur-sm transition-all hover:bg-white/20 sm:self-auto"
              >
                <Plus className="size-4" /> Crear grupo
              </Link>
            </div>

            {/* Search bar */}
            <div className="relative mt-8 max-w-lg">
              <Search className="absolute left-3.5 top-1/2 size-4 -translate-y-1/2 text-white/40" />
              <input
                type="search"
                value={search}
                onChange={(e) => setSearch(e.target.value)}
                placeholder="Buscar grupos..."
                className="h-11 w-full rounded-xl border border-white/20 bg-white/10 pl-10 pr-4 text-sm text-white placeholder:text-white/40 backdrop-blur-sm outline-none focus:border-white/40 focus:bg-white/15"
              />
            </div>
          </div>
        </section>

        <section className="mx-auto max-w-[1200px] px-6 py-8">
          {/* Category filter chips */}
          {categories.length > 0 && (
            <div className="mb-8 flex flex-wrap gap-2">
              <button
                type="button"
                onClick={() => setSelectedCategory(null)}
                className={cn(
                  "rounded-full px-4 py-1.5 text-xs font-semibold transition-all",
                  selectedCategory === null
                    ? "bg-violet-700 text-white shadow-sm"
                    : "bg-white text-zinc-600 border border-border hover:border-violet-300 hover:text-violet-700"
                )}
              >
                Todos
              </button>
              {categories.map((cat) => (
                <button
                  key={cat}
                  type="button"
                  onClick={() => setSelectedCategory(selectedCategory === cat ? null : cat)}
                  className={cn(
                    "rounded-full px-4 py-1.5 text-xs font-semibold transition-all",
                    selectedCategory === cat
                      ? "bg-violet-700 text-white shadow-sm"
                      : "bg-white text-zinc-600 border border-border hover:border-violet-300 hover:text-violet-700"
                  )}
                >
                  {cat}
                </button>
              ))}
            </div>
          )}

          {/* Loading skeleton */}
          {loading && (
            <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
              {[...Array(6)].map((_, i) => (
                <div key={i} className="h-48 animate-pulse rounded-2xl bg-zinc-200" />
              ))}
            </div>
          )}

          {/* Error */}
          {!loading && error && (
            <div className="rounded-xl border border-red-200 bg-red-50 px-6 py-5 text-sm text-red-700">
              No se pudo cargar los grupos: {error}
            </div>
          )}

          {/* Empty state */}
          {!loading && !error && groups.length === 0 && (
            <div className="flex flex-col items-center gap-4 py-24 text-center">
              <div className="flex size-16 items-center justify-center rounded-2xl bg-violet-100">
                <Users className="size-8 text-violet-400" />
              </div>
              <div>
                <p className="font-heading text-base font-bold text-zinc-700">
                  {debouncedSearch || selectedCategory ? "Sin resultados" : "No hay grupos activos"}
                </p>
                <p className="mt-1 text-sm text-zinc-400">
                  {debouncedSearch || selectedCategory
                    ? "Intenta con otros filtros de búsqueda."
                    : "Sé el primero en crear un grupo."}
                </p>
              </div>
              {(debouncedSearch || selectedCategory) && (
                <button
                  type="button"
                  onClick={() => { setSearch(""); setSelectedCategory(null); }}
                  className="text-sm font-semibold text-violet-700 hover:underline"
                >
                  Limpiar filtros
                </button>
              )}
            </div>
          )}

          {/* Groups grid */}
          {!loading && !error && groups.length > 0 && (
            <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
              {groups.map((group) => {
                const gradient = gradientForId(group.id);
                return (
                  <Link
                    key={group.id}
                    to={`/groups/${group.slug}`}
                    className="group relative flex flex-col overflow-hidden rounded-2xl border border-border bg-white shadow-sm transition-all duration-300 hover:-translate-y-1 hover:shadow-lg"
                  >
                    {/* Gradient accent bar */}
                    <div className={`h-1 w-full bg-gradient-to-r ${gradient}`} />

                    <div className="flex flex-1 flex-col p-6">
                      <div className="flex items-start gap-4">
                        {/* Logo or gradient icon */}
                        <div className={`flex size-12 shrink-0 items-center justify-center rounded-xl bg-gradient-to-br ${gradient} shadow-md`}>
                          {group.logoUrl ? (
                            <img src={group.logoUrl} alt={group.name} className="size-10 rounded-lg object-cover" />
                          ) : (
                            <Users className="size-6 text-white" />
                          )}
                        </div>
                        <div className="min-w-0 flex-1">
                          <h3 className="font-heading text-base font-bold text-zinc-900 transition-colors group-hover:text-violet-700">
                            {group.name}
                          </h3>
                          {group.category && (
                            <span className="mt-1 inline-block rounded-full bg-violet-50 px-2.5 py-0.5 text-[10px] font-semibold text-violet-700">
                              {group.category}
                            </span>
                          )}
                        </div>
                      </div>

                      {group.description && (
                        <p className="mt-4 line-clamp-2 flex-1 text-sm leading-relaxed text-zinc-500">
                          {group.description}
                        </p>
                      )}

                      <div className="mt-5 flex items-center justify-between">
                        <span className="flex items-center gap-1 text-xs text-zinc-400">
                          <Users className="size-3" />
                          {group.memberCount} {group.memberCount === 1 ? "miembro" : "miembros"}
                        </span>
                        <span className="text-xs font-semibold text-violet-600 opacity-0 transition-opacity group-hover:opacity-100">
                          Ver grupo →
                        </span>
                      </div>
                    </div>
                  </Link>
                );
              })}
            </div>
          )}
        </section>
      </main>
      <Footer />
    </div>
  );
}
