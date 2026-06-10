import { Menu, X } from "lucide-react";
import { Link } from "react-router-dom";
import { useState } from "react";
import { Button } from "@/components/ui/button";

const navLinks = [
  { label: "Inicio", href: "/" },
  { label: "Grupos", href: "/#grupos" },
  { label: "Eventos", href: "/#eventos" },
  { label: "Comunidad", href: "/#comunidad" },
];

export function Navbar() {
  const [mobileOpen, setMobileOpen] = useState(false);

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

        {/* Desktop CTA */}
        <div className="hidden items-center gap-3 md:flex">
          <Link to="/login">
            <Button className="rounded-lg bg-violet-700 px-5 font-heading text-sm font-semibold shadow-[0_4px_14px_rgba(91,33,182,0.3)] transition-all hover:bg-violet-800">
              Iniciar Sesión
            </Button>
          </Link>
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
              <Link to="/login" onClick={() => setMobileOpen(false)}>
                <Button className="w-full rounded-lg bg-violet-700 font-heading text-sm font-semibold shadow-[0_4px_14px_rgba(91,33,182,0.3)] hover:bg-violet-800">
                  Iniciar Sesión
                </Button>
              </Link>
            </div>
          </div>
        </div>
      )}
    </nav>
  );
}
