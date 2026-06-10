import {
  ArrowRight, BookOpen, Calendar, Clock, Code,
  MapPin, Music, Sparkles, Users,
} from "lucide-react";
import { Link } from "react-router-dom";
import { useEffect, useState } from "react";
import { Footer } from "@/components/layout/Footer";
import { Navbar } from "@/components/layout/Navbar";
import { Button } from "@/components/ui/button";
import { groupApi } from "@/lib/api";
import type { Group } from "@/types";

const GRADIENT_COLORS = [
  "from-violet-600 to-indigo-700",
  "from-rose-500 to-pink-700",
  "from-amber-500 to-orange-600",
  "from-emerald-500 to-teal-700",
  "from-sky-500 to-blue-700",
  "from-fuchsia-500 to-purple-700",
];

const upcomingEvents = [
  { title: "Hackathon Spring 2026", group: "Robótica U-ERRE", date: "12 Abr", time: "09:00 AM", location: "Lab de Innovación", spots: 15 },
  { title: "Concierto Acústico", group: "Ensamble Musical", date: "18 Abr", time: "07:00 PM", location: "Auditorio Principal", spots: 80 },
  { title: "Workshop: Figma Avanzado", group: "Club de Diseño", date: "22 Abr", time: "04:00 PM", location: "Sala Creativa B3", spots: 25 },
  { title: "Torneo de Debate Interno", group: "Debate & Oratoria", date: "28 Abr", time: "10:00 AM", location: "Aula Magna", spots: 40 },
];

const stats = [
  { value: "120+", label: "Grupos Activos" },
  { value: "3,400", label: "Estudiantes" },
  { value: "280", label: "Eventos este año" },
  { value: "95%", label: "Satisfacción" },
];

export default function HomePage() {
  const [featuredGroups, setFeaturedGroups] = useState<Group[]>([]);

  useEffect(() => {
    groupApi.getAll().then((groups) => setFeaturedGroups(groups.slice(0, 6))).catch(() => {});
  }, []);

  return (
    <div className="min-h-screen">
      <Navbar />

      {/* ━━━ HERO ━━━ */}
      <section className="relative isolate overflow-hidden bg-gradient-to-br from-[#4C1D95] via-[#7C3AED] to-[#8B5CF6]">
        <div className="absolute -right-24 -top-24 size-[480px] rounded-full bg-violet-400/20 blur-3xl" aria-hidden="true" />
        <div className="absolute -bottom-32 -left-32 size-[360px] rounded-full bg-amber-400/10 blur-3xl" aria-hidden="true" />
        <div className="absolute right-1/4 top-1/3 size-3 rounded-full bg-amber-400" aria-hidden="true" />
        <div className="absolute left-1/3 bottom-1/4 size-2 rounded-full bg-white/40" aria-hidden="true" />

        <div className="relative z-10 mx-auto max-w-[1200px] px-6 pb-24 pt-20 md:pb-32 md:pt-28">
          <div className="grid items-center gap-12 lg:grid-cols-2">
            <div className="max-w-xl">
              <div className="animate-fade-up inline-flex items-center gap-2 rounded-full border border-white/20 bg-white/10 px-4 py-1.5 text-xs font-semibold uppercase tracking-wider text-white/90 backdrop-blur-sm">
                <Sparkles className="size-3.5" />
                Para estudiantes, por estudiantes
              </div>
              <h1 className="animate-fade-up delay-100 mt-7 font-heading text-4xl font-extrabold leading-[1.1] tracking-tight text-white sm:text-5xl lg:text-6xl">
                Tu comunidad<br />universitaria
                <span className="mt-1 block text-amber-300">empieza aquí.</span>
              </h1>
              <p className="animate-fade-up delay-200 mt-6 max-w-md text-base leading-relaxed text-white/75 sm:text-lg">
                Descubre grupos, únete a eventos y conecta con estudiantes que comparten tus intereses en U-ERRE.
              </p>
              <div className="animate-fade-up delay-300 mt-9 flex flex-wrap items-center gap-4">
                <Link to="/groups">
                  <Button size="lg" className="h-12 rounded-xl bg-white px-7 font-heading text-sm font-bold text-violet-700 shadow-lg transition-all hover:bg-white/90 hover:shadow-xl">
                    Explorar Grupos <ArrowRight className="ml-1 size-4" />
                  </Button>
                </Link>
                <Link to="/login">
                  <Button size="lg" variant="outline" className="h-12 rounded-xl border-white/25 bg-white/10 px-7 font-heading text-sm font-bold text-white backdrop-blur-sm transition-all hover:bg-white/20">
                    Crear Cuenta
                  </Button>
                </Link>
              </div>
              <div className="animate-fade-up delay-400 mt-10 flex items-center gap-3">
                <div className="flex -space-x-2.5">
                  {["bg-rose-400","bg-sky-400","bg-amber-400","bg-emerald-400"].map((bg, i) => (
                    <div key={bg} className={`flex size-8 items-center justify-center rounded-full border-2 border-white/30 ${bg} text-[10px] font-bold text-white`}>
                      {["MA","LC","RG","JP"][i]}
                    </div>
                  ))}
                </div>
                <p className="text-sm text-white/70">
                  <span className="font-semibold text-white">+3,400</span> estudiantes ya son parte
                </p>
              </div>
            </div>

            {/* Floating cards */}
            <div className="relative hidden lg:block" aria-hidden="true">
              <div className="animate-float absolute -right-4 top-4 w-64 rounded-2xl border border-white/10 bg-white/10 p-5 shadow-2xl backdrop-blur-md">
                <div className="flex items-center gap-3">
                  <div className="flex size-10 items-center justify-center rounded-xl bg-gradient-to-br from-violet-500 to-indigo-600">
                    <Code className="size-5 text-white" />
                  </div>
                  <div>
                    <p className="text-sm font-bold text-white">Robótica U-ERRE</p>
                    <p className="text-xs text-white/60">48 miembros</p>
                  </div>
                </div>
                <div className="mt-3.5 flex gap-2">
                  <span className="rounded-full bg-amber-400/20 px-2.5 py-0.5 text-[10px] font-semibold text-amber-300">Hackathon pronto</span>
                </div>
              </div>
              <div className="animate-float absolute left-8 top-36 w-56 rounded-2xl border border-white/10 bg-white/10 p-5 shadow-2xl backdrop-blur-md" style={{ animationDelay: "1.5s" }}>
                <div className="flex items-center gap-3">
                  <div className="flex size-10 items-center justify-center rounded-xl bg-gradient-to-br from-rose-400 to-pink-600">
                    <Music className="size-5 text-white" />
                  </div>
                  <div>
                    <p className="text-sm font-bold text-white">Ensamble Musical</p>
                    <p className="text-xs text-white/60">32 miembros</p>
                  </div>
                </div>
              </div>
              <div className="animate-float absolute -right-8 bottom-0 w-48 rounded-2xl border border-white/10 bg-white/10 p-4 shadow-2xl backdrop-blur-md" style={{ animationDelay: "3s" }}>
                <div className="flex items-center gap-2.5">
                  <div className="flex size-9 items-center justify-center rounded-lg bg-gradient-to-br from-emerald-400 to-teal-600">
                    <BookOpen className="size-4 text-white" />
                  </div>
                  <div>
                    <p className="text-xs font-bold text-white">Debate</p>
                    <p className="text-[10px] text-white/60">35 miembros</p>
                  </div>
                </div>
              </div>
              <div className="absolute left-1/2 top-1/2 size-64 -translate-x-1/2 -translate-y-1/2 rounded-full bg-violet-400/20 blur-[80px]" />
            </div>
          </div>
        </div>

        <div className="absolute inset-x-0 -bottom-px">
          <svg viewBox="0 0 1440 64" fill="none" className="w-full" preserveAspectRatio="none" aria-hidden="true">
            <path d="M0 64h1440V32C1280 0 1120 56 960 40S640 0 480 20 160 64 0 32v32z" fill="white" />
          </svg>
        </div>
      </section>

      {/* ━━━ STATS BAND ━━━ */}
      <section className="relative -mt-px bg-white py-4">
        <div className="mx-auto max-w-[1200px] px-6">
          <div className="relative overflow-hidden rounded-2xl bg-gradient-to-br from-[#1E1B4B] to-[#0F0A2A] px-8 py-10 shadow-xl sm:px-12">
            <div className="absolute -right-16 -top-16 size-48 rounded-full bg-violet-700/30 blur-3xl" aria-hidden="true" />
            <div className="relative grid grid-cols-2 gap-8 sm:grid-cols-4">
              {stats.map((stat, i) => (
                <div key={stat.label} className="text-center">
                  <p className="animate-count-up font-heading text-3xl font-extrabold text-white sm:text-4xl" style={{ animationDelay: `${i * 150}ms` }}>
                    {stat.value}
                  </p>
                  <p className="mt-1.5 text-xs font-medium uppercase tracking-wider text-zinc-400">{stat.label}</p>
                </div>
              ))}
            </div>
          </div>
        </div>
      </section>

      {/* ━━━ FEATURED GROUPS ━━━ */}
      <section id="grupos" className="bg-zinc-50 py-20 sm:py-24">
        <div className="mx-auto max-w-[1200px] px-6">
          <div className="mx-auto max-w-2xl text-center">
            <span className="inline-block rounded-full bg-violet-50 px-4 py-1 text-xs font-semibold uppercase tracking-wider text-violet-700">Comunidad</span>
            <h2 className="mt-4 font-heading text-3xl font-bold tracking-tight text-zinc-900 sm:text-4xl">Grupos Destacados</h2>
            <p className="mt-3 text-base text-zinc-700">Encuentra tu lugar. Únete a grupos donde podrás crecer, aprender y compartir.</p>
          </div>

          <div className="mt-14 grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
            {featuredGroups.length === 0 ? (
              <p className="col-span-3 py-8 text-center text-sm text-zinc-400">No hay grupos activos por el momento.</p>
            ) : (
              featuredGroups.map((group, i) => (
                <Link key={group.id} to={`/groups/${group.slug}`}
                  className="group relative overflow-hidden rounded-2xl border border-border bg-white p-6 shadow-sm transition-all duration-300 hover:-translate-y-1 hover:shadow-lg"
                >
                  <div className={`absolute inset-x-0 top-0 h-1 bg-gradient-to-r ${GRADIENT_COLORS[i % GRADIENT_COLORS.length]}`} />
                  <div className="flex items-start gap-4">
                    <div className={`flex size-12 shrink-0 items-center justify-center rounded-xl bg-gradient-to-br ${GRADIENT_COLORS[i % GRADIENT_COLORS.length]} shadow-md`}>
                      <Users className="size-6 text-white" />
                    </div>
                    <div className="min-w-0">
                      <h3 className="font-heading text-base font-bold text-zinc-900">{group.name}</h3>
                    </div>
                  </div>
                  {group.description && (
                    <p className="mt-4 line-clamp-3 text-sm leading-relaxed text-zinc-700">{group.description}</p>
                  )}
                  <div className="mt-5 flex items-center justify-end">
                    <span className="inline-flex items-center gap-1 text-xs font-semibold text-violet-700 opacity-0 transition-opacity group-hover:opacity-100">
                      Ver más <ArrowRight className="size-3" />
                    </span>
                  </div>
                </Link>
              ))
            )}
          </div>

          <div className="mt-12 text-center">
            <Link to="/groups">
              <Button variant="outline" className="rounded-xl border-violet-700/30 px-6 font-heading text-sm font-semibold text-violet-700 hover:bg-violet-50">
                Ver todos los grupos <ArrowRight className="ml-1 size-4" />
              </Button>
            </Link>
          </div>
        </div>
      </section>

      {/* ━━━ UPCOMING EVENTS ━━━ */}
      <section id="eventos" className="bg-white py-20 sm:py-24">
        <div className="mx-auto max-w-[1200px] px-6">
          <div className="mx-auto max-w-2xl text-center">
            <span className="inline-block rounded-full bg-amber-50 px-4 py-1 text-xs font-semibold uppercase tracking-wider text-amber-700">Próximamente</span>
            <h2 className="mt-4 font-heading text-3xl font-bold tracking-tight text-zinc-900 sm:text-4xl">Eventos que Vienen</h2>
            <p className="mt-3 text-base text-zinc-700">No te pierdas lo que está pasando en tu campus.</p>
          </div>

          <div className="mt-14 grid gap-5 sm:grid-cols-2">
            {upcomingEvents.map((event) => (
              <article key={event.title} className="group flex gap-5 rounded-2xl border border-border bg-white p-5 shadow-sm transition-all duration-300 hover:-translate-y-0.5 hover:shadow-md">
                <div className="flex size-16 shrink-0 flex-col items-center justify-center rounded-xl bg-gradient-to-br from-amber-400 to-amber-500 shadow-md">
                  <span className="text-[10px] font-bold uppercase leading-none text-white/80">{event.date.split(" ")[1]}</span>
                  <span className="mt-0.5 font-heading text-xl font-extrabold leading-none text-white">{event.date.split(" ")[0]}</span>
                </div>
                <div className="min-w-0 flex-1">
                  <h3 className="font-heading text-base font-bold text-zinc-900">{event.title}</h3>
                  <p className="mt-0.5 text-xs font-medium text-violet-700">{event.group}</p>
                  <div className="mt-3 flex flex-wrap items-center gap-x-4 gap-y-1.5 text-xs text-zinc-400">
                    <span className="flex items-center gap-1"><Clock className="size-3" />{event.time}</span>
                    <span className="flex items-center gap-1"><MapPin className="size-3" />{event.location}</span>
                    <span className="flex items-center gap-1"><Users className="size-3" />{event.spots} lugares</span>
                  </div>
                </div>
                <div className="hidden shrink-0 self-center sm:block">
                  <Button size="sm" className="rounded-lg bg-violet-700 px-4 font-heading text-xs font-semibold shadow-[0_4px_14px_rgba(91,33,182,0.3)] hover:bg-violet-800">
                    RSVP
                  </Button>
                </div>
              </article>
            ))}
          </div>

          <div className="mt-12 text-center">
            <Link to="/events">
              <Button variant="outline" className="rounded-xl border-amber-300 px-6 font-heading text-sm font-semibold text-amber-700 hover:bg-amber-50">
                <Calendar className="mr-1 size-4" /> Ver calendario completo
              </Button>
            </Link>
          </div>
        </div>
      </section>

      {/* ━━━ CTA BANNER ━━━ */}
      <section className="relative isolate overflow-hidden bg-gradient-to-br from-[#4C1D95] via-[#7C3AED] to-[#8B5CF6] py-20 sm:py-24">
        <div className="absolute -left-32 -top-32 size-72 rounded-full bg-amber-400/10 blur-3xl" aria-hidden="true" />
        <div className="absolute -bottom-24 -right-24 size-60 rounded-full bg-white/5 blur-3xl" aria-hidden="true" />
        <div className="relative z-10 mx-auto max-w-[1200px] px-6 text-center">
          <h2 className="font-heading text-3xl font-extrabold text-white sm:text-4xl lg:text-5xl">
            ¿Listo para ser parte<br />de algo increíble?
          </h2>
          <p className="mx-auto mt-5 max-w-lg text-base text-white/70 sm:text-lg">
            Crea tu cuenta gratuita y comienza a explorar la comunidad estudiantil más activa de U-ERRE.
          </p>
          <div className="mt-9 flex flex-wrap items-center justify-center gap-4">
            <Link to="/login">
              <Button size="lg" className="h-12 rounded-xl bg-white px-8 font-heading text-sm font-bold text-violet-700 shadow-lg hover:bg-white/90 hover:shadow-xl">
                Crear Cuenta Gratis <ArrowRight className="ml-1 size-4" />
              </Button>
            </Link>
            <Link to="/groups">
              <Button size="lg" variant="outline" className="h-12 rounded-xl border-white/25 bg-white/10 px-8 font-heading text-sm font-bold text-white backdrop-blur-sm hover:bg-white/20">
                Explorar Primero
              </Button>
            </Link>
          </div>
        </div>
      </section>

      <Footer />
    </div>
  );
}
