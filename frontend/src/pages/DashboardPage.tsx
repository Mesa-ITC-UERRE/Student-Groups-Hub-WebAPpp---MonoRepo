import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { Bell, Users, ExternalLink, CheckCircle2, Clock, XCircle } from "lucide-react";
import { groupRegistrationApi, notificationApi } from "@/lib/api";
import type { GroupRegistrationRequest, Notification } from "@/types";
import { Navbar } from "@/components/layout/Navbar";
import { Footer } from "@/components/layout/Footer";
import { Button } from "@/components/ui/button";
import { useAuth } from "@/context/AuthContext";

const STATUS_STYLES: Record<string, string> = {
  approved: "bg-green-100 text-green-700",
  pending: "bg-amber-100 text-amber-700",
  rejected: "bg-red-100 text-red-700",
};

const STATUS_ICONS: Record<string, React.ElementType> = {
  approved: CheckCircle2,
  pending: Clock,
  rejected: XCircle,
};

export default function DashboardPage() {
  const navigate = useNavigate();
  const { user, isAuthenticated, isLoading: authLoading, signOut } = useAuth();

  const [registrationRequests, setRegistrationRequests] = useState<GroupRegistrationRequest[]>([]);
  const [notifications, setNotifications] = useState<Notification[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [markingAll, setMarkingAll] = useState(false);

  useEffect(() => {
    if (authLoading) return;
    if (!isAuthenticated) {
      navigate("/login?returnTo=/dashboard", { replace: true });
      return;
    }
    async function load() {
      try {
        const [reqs, notifs] = await Promise.all([
          groupRegistrationApi.getMine(),
          notificationApi.getMine(),
        ]);
        setRegistrationRequests(reqs);
        setNotifications(notifs);
      } catch (err: unknown) {
        setError(err instanceof Error ? err.message : "Error al cargar el panel");
      } finally {
        setLoading(false);
      }
    }
    load();
  }, [isAuthenticated, authLoading, navigate]);

  async function handleMarkAllRead() {
    setMarkingAll(true);
    try {
      await notificationApi.markAllRead();
      setNotifications((prev) => prev.map((n) => ({ ...n, read: true })));
    } catch { /* silent */ } finally { setMarkingAll(false); }
  }

  async function handleMarkOneRead(id: string) {
    try {
      await notificationApi.markRead(id);
      setNotifications((prev) => prev.map((n) => (n.id === id ? { ...n, read: true } : n)));
    } catch { /* silent */ }
  }

  function handleSignOut() {
    signOut();
  }  const unreadCount = notifications.filter((n) => !n.read).length;

  return (
    <div className="flex min-h-screen flex-col">
      <Navbar />
      <main className="flex-1 bg-zinc-50">
        {/* Header */}
        <section className="bg-gradient-to-br from-[#4C1D95] via-[#7C3AED] to-[#8B5CF6] py-10 text-white">
          <div className="mx-auto max-w-5xl px-6">
            {loading ? (
              <div className="h-8 w-48 animate-pulse rounded-lg bg-white/20" />
            ) : user ? (
              <div className="flex items-center justify-between">
                <div className="flex items-center gap-4">
                  <div className="flex size-14 shrink-0 items-center justify-center rounded-full bg-white/15 text-xl font-bold">
                    {user.displayName?.charAt(0).toUpperCase() ?? user.email.charAt(0).toUpperCase()}
                  </div>
                  <div>
                    <h1 className="font-heading text-2xl font-extrabold">{user.displayName ?? user.email}</h1>
                    <p className="mt-0.5 text-sm text-white/60">{user.email}</p>
                    {user.isPlatformAdmin && (
                      <span className="mt-1.5 inline-flex items-center rounded-full bg-amber-400/20 px-2.5 py-0.5 text-xs font-semibold text-amber-300">Administrador</span>
                    )}
                  </div>
                </div>
                <Button variant="ghost" onClick={handleSignOut} className="text-xs text-white/60 hover:bg-white/10 hover:text-white">
                  Cerrar sesión
                </Button>
              </div>
            ) : null}
          </div>
        </section>

        {error && (
          <div className="mx-auto max-w-5xl px-6 py-6">
            <div className="rounded-xl border border-red-200 bg-red-50 px-6 py-4 text-sm text-red-700">{error}</div>
          </div>
        )}

        <div className="mx-auto max-w-5xl space-y-10 px-6 py-10">
          {/* Quick actions */}
          <div className="grid gap-4 sm:grid-cols-2">
            <Link to="/groups" className="flex items-center gap-4 rounded-2xl border border-border bg-white p-5 shadow-sm transition-all hover:-translate-y-0.5 hover:shadow-md">
              <div className="flex size-11 shrink-0 items-center justify-center rounded-xl bg-violet-100">
                <Users className="size-5 text-violet-600" />
              </div>
              <div>
                <p className="text-sm font-bold text-zinc-900">Explorar Grupos</p>
                <p className="mt-0.5 text-xs text-zinc-500">Descubre y únete a nuevos grupos</p>
              </div>
              <ExternalLink className="ml-auto size-4 text-zinc-300" />
            </Link>
            <Link to="/groups/register" className="flex items-center gap-4 rounded-2xl border border-border bg-white p-5 shadow-sm transition-all hover:-translate-y-0.5 hover:shadow-md">
              <div className="flex size-11 shrink-0 items-center justify-center rounded-xl bg-emerald-100">
                <Users className="size-5 text-emerald-600" />
              </div>
              <div>
                <p className="text-sm font-bold text-zinc-900">Crear Grupo</p>
                <p className="mt-0.5 text-xs text-zinc-500">Solicitar registro de un nuevo grupo</p>
              </div>
              <ExternalLink className="ml-auto size-4 text-zinc-300" />
            </Link>
          </div>

          {/* Notifications */}
          <section>
            <div className="mb-4 flex items-center justify-between">
              <div className="flex items-center gap-2">
                <Bell className="size-5 text-zinc-700" />
                <h2 className="font-heading text-base font-bold text-zinc-900">
                  Notificaciones
                  {unreadCount > 0 && (
                    <span className="ml-2 inline-flex size-5 items-center justify-center rounded-full bg-violet-600 text-[10px] font-bold text-white">{unreadCount}</span>
                  )}
                </h2>
              </div>
              {unreadCount > 0 && (
                <button type="button" onClick={handleMarkAllRead} disabled={markingAll} className="text-xs font-medium text-violet-600 transition-colors hover:text-violet-800 disabled:opacity-50">
                  Marcar todas como leídas
                </button>
              )}
            </div>

            {loading ? (
              <div className="space-y-3">{[1,2,3].map((i) => <div key={i} className="h-14 animate-pulse rounded-xl bg-zinc-200" />)}</div>
            ) : notifications.length === 0 ? (
              <div className="rounded-xl border border-border bg-white px-6 py-8 text-center text-sm text-zinc-400">No tienes notificaciones.</div>
            ) : (
              <div className="space-y-2">
                {notifications.map((notif) => (
                  <div key={notif.id} className={`flex items-start gap-4 rounded-xl border border-border bg-white px-5 py-4 shadow-sm ${!notif.read ? "border-l-4 border-l-violet-500" : ""}`}>
                    <div className="min-w-0 flex-1">
                      <p className={`text-sm font-semibold ${notif.read ? "text-zinc-500" : "text-zinc-900"}`}>{notif.title}</p>
                      {notif.body && <p className="mt-0.5 truncate text-xs text-zinc-400">{notif.body}</p>}
                    </div>
                    <div className="flex shrink-0 items-center gap-3">
                      {notif.href && <Link to={notif.href} className="text-xs text-violet-600 hover:underline">Ver</Link>}
                      {!notif.read && (
                        <button type="button" onClick={() => handleMarkOneRead(notif.id)} className="text-xs text-zinc-400 hover:text-zinc-700">Leída</button>
                      )}
                    </div>
                  </div>
                ))}
              </div>
            )}
          </section>

          {/* My group registration requests */}
          <section>
            <h2 className="mb-4 font-heading text-base font-bold text-zinc-900">Mis Solicitudes de Grupo</h2>
            {loading ? (
              <div className="space-y-3">{[1,2].map((i) => <div key={i} className="h-16 animate-pulse rounded-xl bg-zinc-200" />)}</div>
            ) : registrationRequests.length === 0 ? (
              <div className="rounded-xl border border-border bg-white px-6 py-8 text-center">
                <p className="text-sm text-zinc-400">No has enviado ninguna solicitud de grupo.</p>
                <Link to="/groups/register" className="mt-3 inline-block text-sm font-semibold text-violet-600 hover:underline">Solicitar un grupo →</Link>
              </div>
            ) : (
              <div className="space-y-3">
                {registrationRequests.map((req) => {
                  const StatusIcon = STATUS_ICONS[req.status] ?? Clock;
                  return (
                    <div key={req.id} className="flex items-start gap-4 rounded-xl border border-border bg-white px-5 py-4 shadow-sm">
                      <StatusIcon className={`mt-0.5 size-5 shrink-0 ${req.status === "approved" ? "text-green-500" : req.status === "rejected" ? "text-red-400" : "text-amber-500"}`} />
                      <div className="min-w-0 flex-1">
                        <p className="text-sm font-bold text-zinc-900">{req.proposedGroupName}</p>
                        {req.proposedDescription && <p className="mt-0.5 line-clamp-1 text-xs text-zinc-400">{req.proposedDescription}</p>}
                        {req.decisionNotes && <p className="mt-1 text-xs italic text-zinc-400">Notas: {req.decisionNotes}</p>}
                      </div>
                      <span className={`shrink-0 rounded-full px-2.5 py-0.5 text-xs font-semibold capitalize ${STATUS_STYLES[req.status] ?? "bg-zinc-100 text-zinc-600"}`}>
                        {req.status === "approved" ? "Aprobada" : req.status === "rejected" ? "Rechazada" : "Pendiente"}
                      </span>
                    </div>
                  );
                })}
              </div>
            )}
          </section>
        </div>
      </main>
      <Footer />
    </div>
  );
}
