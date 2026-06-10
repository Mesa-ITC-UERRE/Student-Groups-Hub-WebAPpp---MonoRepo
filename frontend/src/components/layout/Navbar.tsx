import { Menu, X, Bell, ChevronDown, User, LogOut, LayoutDashboard } from "lucide-react";
import { Link, useNavigate } from "react-router-dom";
import { useState, useRef, useEffect } from "react";
import { useIsAuthenticated } from "@azure/msal-react";
import { Button } from "@/components/ui/button";
import { useAuth } from "@/context/AuthContext";

const navLinks = [
  { label: "Inicio", href: "/" },
  { label: "Grupos", href: "/groups" },
  { label: "Eventos", href: "/events" },
];

export function Navbar() {
  const [mobileOpen, setMobileOpen] = useState(false);
  const [userMenuOpen, setUserMenuOpen] = useState(false);
  const isAuthenticated = useIsAuthenticated();
  const { user, signOut } = useAuth();
  const menuRef = useRef<HTMLDivElement>(null);
  const navigate = useNavigate();

  // Close user menu on outside click
  useEffect(() => {
    function handleClick(e: MouseEvent) {
      if (menuRef.current && !menuRef.current.contains(e.target as Node)) {
        setUserMenuOpen(false);
      }
    }
    document.addEventListener("mousedown", handleClick);
    return () => document.removeEventListener("mousedown", handleClick);
  }, []);

  const initials = user?.displayName
    ? user.displayName.split(" ").map((n) => n[0]).join("").slice(0, 2).toUpperCase()
    : user?.email.charAt(0).toUpperCase() ?? "?";

  return (
    <nav className="sticky top-0 z-50 border-b border-border/60 bg-white/80 backdrop-blur-xl">
      <div className="mx-auto flex h-16 max-w-[1200px] items-center justify-between px-6">
        {/* Logo */}
        <Link to="/" className="flex items-center gap-2.5">
          <div className="flex size-9 items-center justify-center rounded-lg bg-violet-700">
            <span className="font-heading text-sm font-extrabold leading-none text-white">U</span>
          </div>
          <div className="flex flex-col">
            <span className="font-heading text-sm font-bold leading-tight text-zinc-900">Student Groups</span>
            <span className="text-[10px] font-medium uppercase tracking-widest text-zinc-400">U-ERRE</span>
          </div>
        </Link>

        {/* Desktop links */}
        <div className="hidden items-center gap-1 md:flex">
          {navLinks.map((link) => (
            <Link
              key={link.href}
              to={link.href}
              className="rounded-md px-3.5 py-2 text-sm font-medium text-zinc-700 transition-colors hover:bg-violet-50 hover:text-violet-700"
            >
              {link.label}
            </Link>
          ))}
        </div>

        {/* Desktop right section */}
        <div className="hidden items-center gap-3 md:flex">
          {isAuthenticated && user ? (
            <>
              {/* Notification bell — placeholder for Phase 6 */}
              <button
                type="button"
                className="relative flex size-9 items-center justify-center rounded-lg text-zinc-500 transition-colors hover:bg-violet-50 hover:text-violet-700"
                aria-label="Notificaciones"
              >
                <Bell className="size-5" />
              </button>

              {/* User menu */}
              <div className="relative" ref={menuRef}>
                <button
                  type="button"
                  onClick={() => setUserMenuOpen(!userMenuOpen)}
                  className="flex items-center gap-2 rounded-lg px-2 py-1 transition-colors hover:bg-violet-50"
                  aria-expanded={userMenuOpen}
                  aria-label="Menú de usuario"
                >
                  {/* Avatar */}
                  <div className="flex size-8 items-center justify-center rounded-full bg-violet-700 text-xs font-bold text-white">
                    {user.avatarUrl ? (
                      <img src={user.avatarUrl} alt={user.displayName ?? user.email} className="size-8 rounded-full object-cover" />
                    ) : (
                      initials
                    )}
                  </div>
                  <span className="max-w-[120px] truncate text-sm font-medium text-zinc-700">
                    {user.displayName ?? user.email.split("@")[0]}
                  </span>
                  <ChevronDown className={`size-3.5 text-zinc-400 transition-transform ${userMenuOpen ? "rotate-180" : ""}`} />
                </button>

                {/* Dropdown */}
                {userMenuOpen && (
                  <div className="animate-fade-in absolute right-0 top-full mt-2 w-52 rounded-xl border border-border bg-white py-1.5 shadow-lg">
                    <div className="border-b border-border/60 px-4 pb-2.5 pt-1">
                      <p className="truncate text-sm font-semibold text-zinc-900">{user.displayName ?? user.email.split("@")[0]}</p>
                      <p className="truncate text-xs text-zinc-400">{user.email}</p>
                      {user.role === "admin" && (
                        <span className="mt-1 inline-flex items-center rounded-full bg-amber-100 px-2 py-0.5 text-[10px] font-semibold text-amber-700">
                          Administrador
                        </span>
                      )}
                    </div>
                    <div className="py-1">
                      <button
                        type="button"
                        onClick={() => { setUserMenuOpen(false); navigate("/dashboard"); }}
                        className="flex w-full items-center gap-2.5 px-4 py-2 text-sm text-zinc-700 transition-colors hover:bg-violet-50 hover:text-violet-700"
                      >
                        <LayoutDashboard className="size-4" /> Mi Panel
                      </button>
                      <button
                        type="button"
                        onClick={() => { setUserMenuOpen(false); navigate("/profile"); }}
                        className="flex w-full items-center gap-2.5 px-4 py-2 text-sm text-zinc-700 transition-colors hover:bg-violet-50 hover:text-violet-700"
                      >
                        <User className="size-4" /> Mi Perfil
                      </button>
                    </div>
                    <div className="border-t border-border/60 py-1">
                      <button
                        type="button"
                        onClick={() => { setUserMenuOpen(false); signOut(); }}
                        className="flex w-full items-center gap-2.5 px-4 py-2 text-sm text-red-600 transition-colors hover:bg-red-50"
                      >
                        <LogOut className="size-4" /> Cerrar Sesión
                      </button>
                    </div>
                  </div>
                )}
              </div>
            </>
          ) : (
            <Link to="/login">
              <Button className="rounded-lg bg-violet-700 px-5 font-heading text-sm font-semibold shadow-[0_4px_14px_rgba(91,33,182,0.3)] transition-all hover:bg-violet-800">
                Iniciar Sesión
              </Button>
            </Link>
          )}
        </div>

        {/* Mobile toggle */}
        <button
          type="button"
          onClick={() => setMobileOpen(!mobileOpen)}
          className="inline-flex size-10 items-center justify-center rounded-lg text-zinc-700 transition-colors hover:bg-violet-50 md:hidden"
          aria-label={mobileOpen ? "Cerrar menú" : "Abrir menú"}
        >
          {mobileOpen ? <X className="size-5" /> : <Menu className="size-5" />}
        </button>
      </div>

      {/* Mobile menu */}
      {mobileOpen && (
        <div className="animate-fade-in border-t border-border/60 bg-white px-6 pb-6 pt-4 md:hidden">
          <div className="flex flex-col gap-1">
            {navLinks.map((link) => (
              <Link
                key={link.href}
                to={link.href}
                onClick={() => setMobileOpen(false)}
                className="rounded-lg px-4 py-2.5 text-sm font-medium text-zinc-700 transition-colors hover:bg-violet-50 hover:text-violet-700"
              >
                {link.label}
              </Link>
            ))}
            <div className="mt-3 border-t border-border/60 pt-4">
              {isAuthenticated && user ? (
                <div className="flex flex-col gap-2">
                  <div className="flex items-center gap-3 px-2 py-1">
                    <div className="flex size-8 items-center justify-center rounded-full bg-violet-700 text-xs font-bold text-white">
                      {initials}
                    </div>
                    <div>
                      <p className="text-sm font-semibold text-zinc-900">{user.displayName ?? user.email.split("@")[0]}</p>
                      <p className="text-xs text-zinc-400">{user.email}</p>
                    </div>
                  </div>
                  <Link to="/dashboard" onClick={() => setMobileOpen(false)}>
                    <Button variant="outline" className="w-full rounded-lg font-heading text-sm">Mi Panel</Button>
                  </Link>
                  <Button onClick={() => { setMobileOpen(false); signOut(); }} variant="ghost" className="w-full rounded-lg font-heading text-sm text-red-600 hover:bg-red-50 hover:text-red-700">
                    Cerrar Sesión
                  </Button>
                </div>
              ) : (
                <Link to="/login" onClick={() => setMobileOpen(false)}>
                  <Button className="w-full rounded-lg bg-violet-700 font-heading text-sm font-semibold shadow-[0_4px_14px_rgba(91,33,182,0.3)] hover:bg-violet-800">
                    Iniciar Sesión
                  </Button>
                </Link>
              )}
            </div>
          </div>
        </div>
      )}
    </nav>
  );
}
