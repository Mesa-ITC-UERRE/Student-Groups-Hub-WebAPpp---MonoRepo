import { useEffect, useState } from "react";
import { useParams, Link } from "react-router-dom";
import { ArrowLeft, Users, Mail, Tag } from "lucide-react";
import { groupApi } from "@/lib/api";
import type { Group, GroupMember } from "@/types";
import { Navbar } from "@/components/layout/Navbar";
import { Footer } from "@/components/layout/Footer";
import JoinButton from "@/components/JoinButton";

export default function GroupDetailPage() {
  const { slug } = useParams<{ slug: string }>();
  const [group, setGroup] = useState<Group | null>(null);
  const [members, setMembers] = useState<GroupMember[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!slug) return;
    async function load() {
      try {
        const g = await groupApi.getBySlug(slug!);
        setGroup(g);
        const m = await groupApi.getMembers(g.id);
        setMembers(m);
      } catch (err: unknown) {
        setError(err instanceof Error ? err.message : "Error desconocido");
      } finally {
        setLoading(false);
      }
    }
    load();
  }, [slug]);

  return (
    <div className="flex min-h-screen flex-col">
      <Navbar />
      <main className="flex-1 bg-zinc-50">

        {loading && (
          <div className="flex items-center justify-center py-32">
            <div className="h-8 w-8 animate-spin rounded-full border-4 border-violet-600 border-t-transparent" />
          </div>
        )}

        {!loading && error && (
          <div className="mx-auto max-w-3xl px-6 py-16">
            <div className="rounded-xl border border-red-200 bg-red-50 px-6 py-5 text-sm text-red-700">
              No se pudo cargar el grupo: {error}
            </div>
          </div>
        )}

        {!loading && !error && group && (
          <>
            {/* Hero banner */}
            <section
              className="relative bg-gradient-to-br from-[#4C1D95] via-[#7C3AED] to-[#8B5CF6] py-12 text-white"
              style={group.bannerUrl ? {
                backgroundImage: `linear-gradient(to bottom right, rgba(76,29,149,0.85), rgba(124,58,237,0.85)), url(${group.bannerUrl})`,
                backgroundSize: "cover",
                backgroundPosition: "center",
              } : undefined}
            >
              <div className="mx-auto max-w-5xl px-6">
                <Link
                  to="/groups"
                  className="mb-6 inline-flex items-center gap-1.5 text-xs font-medium text-white/60 transition-colors hover:text-white"
                >
                  <ArrowLeft className="size-3.5" /> Todos los grupos
                </Link>

                <div className="flex flex-col gap-5 sm:flex-row sm:items-start">
                  {/* Logo */}
                  <div className="flex size-16 shrink-0 items-center justify-center rounded-2xl bg-white/15 backdrop-blur-sm">
                    {group.logoUrl ? (
                      <img src={group.logoUrl} alt={group.name} className="size-14 rounded-xl object-cover" />
                    ) : (
                      <Users className="size-8 text-white/80" />
                    )}
                  </div>

                  <div className="flex-1">
                    <div className="flex flex-wrap items-start gap-3">
                      <h1 className="font-heading text-2xl font-extrabold sm:text-3xl">{group.name}</h1>
                      {group.category && (
                        <span className="mt-1 inline-flex items-center gap-1 rounded-full bg-white/15 px-3 py-0.5 text-xs font-semibold text-white/90 backdrop-blur-sm">
                          <Tag className="size-3" /> {group.category}
                        </span>
                      )}
                    </div>

                    {group.description && (
                      <p className="mt-3 max-w-xl text-sm leading-relaxed text-white/70">{group.description}</p>
                    )}

                    <div className="mt-4 flex flex-wrap items-center gap-5">
                      <span className="flex items-center gap-1.5 text-sm text-white/70">
                        <Users className="size-4" />
                        <span className="font-bold text-white">{members.length}</span>{" "}
                        {members.length === 1 ? "miembro" : "miembros"}
                      </span>
                      {group.contactEmail && (
                        <a
                          href={`mailto:${group.contactEmail}`}
                          className="flex items-center gap-1.5 text-sm text-white/70 transition-colors hover:text-white"
                        >
                          <Mail className="size-4" /> {group.contactEmail}
                        </a>
                      )}
                    </div>
                  </div>

                  <div className="shrink-0 sm:pt-1">
                    <JoinButton groupId={group.id} />
                  </div>
                </div>
              </div>
            </section>

            {/* Members list */}
            <section className="mx-auto max-w-5xl px-6 py-12">
              <h2 className="mb-6 font-heading text-lg font-bold text-zinc-900">
                Miembros ({members.length})
              </h2>

              {members.length === 0 ? (
                <div className="rounded-xl border border-border bg-white px-6 py-8 text-center text-sm text-zinc-400">
                  Este grupo aún no tiene miembros aprobados.
                </div>
              ) : (
                <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-3">
                  {members.map((member) => (
                    <div
                      key={member.membershipId}
                      className="flex items-center gap-3 rounded-xl border border-border bg-white px-4 py-3 shadow-sm"
                    >
                      <div className="flex size-10 shrink-0 items-center justify-center rounded-full bg-violet-100 text-sm font-bold text-violet-700">
                        {member.avatarUrl ? (
                          <img
                            src={member.avatarUrl}
                            alt={member.displayName ?? member.email}
                            className="size-10 rounded-full object-cover"
                          />
                        ) : (
                          (member.displayName ?? member.email).charAt(0).toUpperCase()
                        )}
                      </div>
                      <div className="min-w-0">
                        <p className="truncate text-sm font-semibold text-zinc-900">
                          {member.displayName ?? member.email}
                        </p>
                        <p className="truncate text-xs text-zinc-400">{member.email}</p>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </section>
          </>
        )}
      </main>
      <Footer />
    </div>
  );
}
