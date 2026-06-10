import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { Users } from "lucide-react";
import { groupApi } from "@/lib/api";
import type { Group } from "@/types";
import { Navbar } from "@/components/layout/Navbar";
import { Footer } from "@/components/layout/Footer";

export default function GroupsPage() {
  const [groups, setGroups] = useState<Group[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    groupApi.getAll()
      .then(setGroups)
      .catch((err: Error) => setError(err.message))
      .finally(() => setLoading(false));
  }, []);

  return (
    <div className="flex min-h-screen flex-col">
      <Navbar />
      <main className="flex-1 bg-zinc-50">
        <section className="bg-gradient-to-br from-[#4C1D95] via-[#7C3AED] to-[#8B5CF6] py-16 text-white">
          <div className="mx-auto max-w-[1200px] px-6">
            <h1 className="font-heading text-3xl font-extrabold sm:text-4xl">Grupos Estudiantiles</h1>
            <p className="mt-3 max-w-xl text-base text-white/70">
              Explora todos los grupos activos de la Universidad Regiomontana y encuentra tu comunidad.
            </p>
          </div>
        </section>

        <section className="mx-auto max-w-[1200px] px-6 py-12">
          {loading && (
            <div className="flex items-center justify-center py-24">
              <div className="h-8 w-8 animate-spin rounded-full border-4 border-violet-600 border-t-transparent" />
            </div>
          )}
          {error && (
            <div className="rounded-xl border border-red-200 bg-red-50 px-6 py-5 text-sm text-red-700">
              No se pudo cargar los grupos: {error}
            </div>
          )}
          {!loading && !error && groups.length === 0 && (
            <p className="py-16 text-center text-zinc-500">No hay grupos activos por el momento.</p>
          )}
          {!loading && !error && groups.length > 0 && (
            <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
              {groups.map((group) => (
                <Link
                  key={group.id}
                  to={`/groups/${group.slug}`}
                  className="group flex flex-col rounded-2xl border border-border bg-white p-6 shadow-sm transition-all hover:-translate-y-0.5 hover:shadow-md"
                >
                  <div className="mb-4 flex size-12 items-center justify-center rounded-xl bg-violet-100">
                    {group.logoUrl ? (
                      <img src={group.logoUrl} alt={group.name} className="size-10 rounded-lg object-cover" />
                    ) : (
                      <Users className="size-6 text-violet-600" />
                    )}
                  </div>
                  <h3 className="font-heading text-base font-bold text-zinc-900 transition-colors group-hover:text-violet-700">{group.name}</h3>
                  {group.description && (
                    <p className="mt-2 line-clamp-3 text-sm leading-relaxed text-zinc-500">{group.description}</p>
                  )}
                  <span className="mt-auto pt-4 text-xs font-semibold text-violet-600 group-hover:underline">Ver grupo →</span>
                </Link>
              ))}
            </div>
          )}
        </section>
      </main>
      <Footer />
    </div>
  );
}
