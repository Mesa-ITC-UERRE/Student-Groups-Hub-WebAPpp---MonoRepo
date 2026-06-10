import { Heart } from "lucide-react";
import { Link } from "react-router-dom";

const footerSections = [
  {
    title: "Plataforma",
    links: [
      { label: "Explorar Grupos", href: "/groups" },
      { label: "Eventos", href: "/events" },
      { label: "Crear Grupo", href: "/groups/register" },
    ],
  },
  {
    title: "Recursos",
    links: [
      { label: "Guía de Inicio", href: "#" },
      { label: "FAQ", href: "#" },
      { label: "Contacto", href: "#" },
      { label: "Soporte", href: "#" },
    ],
  },
  {
    title: "Universidad",
    links: [
      { label: "U-ERRE Website", href: "https://www.u-erre.mx" },
      { label: "Portal Estudiantil", href: "#" },
      { label: "Vida Universitaria", href: "#" },
      { label: "Directorio", href: "#" },
    ],
  },
];

export function Footer() {
  return (
    <footer className="relative overflow-hidden bg-gradient-to-br from-[#1E1B4B] to-[#0F0A2A]">
      <div className="mx-auto max-w-[1200px] px-6 pt-20 pb-10">
        <div className="grid grid-cols-1 gap-12 sm:grid-cols-2 lg:grid-cols-4">
          {/* Brand column */}
          <div className="flex flex-col gap-5">
            <div className="flex items-center gap-2.5">
              <div className="flex size-10 items-center justify-center rounded-xl bg-violet-500">
                <span className="font-heading text-base font-extrabold text-white">U</span>
              </div>
              <div>
                <p className="font-heading text-sm font-bold text-white">Student Groups Hub</p>
                <p className="text-[10px] font-medium uppercase tracking-widest text-zinc-400">Universidad Regiomontana</p>
              </div>
            </div>
            <p className="max-w-[240px] text-sm leading-relaxed text-gray-400">
              La plataforma donde los estudiantes de U-ERRE se conectan, organizan y crean comunidad.
            </p>
          </div>

          {footerSections.map((section) => (
            <div key={section.title}>
              <h4 className="font-heading text-xs font-bold uppercase tracking-wider text-violet-400">
                {section.title}
              </h4>
              <ul className="mt-4 flex flex-col gap-2.5">
                {section.links.map((link) => (
                  <li key={link.label}>
                    {link.href.startsWith("http") ? (
                      <a
                        href={link.href}
                        target="_blank"
                        rel="noopener noreferrer"
                        className="text-sm text-gray-400 transition-colors hover:text-white"
                      >
                        {link.label}
                      </a>
                    ) : (
                      <Link to={link.href} className="text-sm text-gray-400 transition-colors hover:text-white">
                        {link.label}
                      </Link>
                    )}
                  </li>
                ))}
              </ul>
            </div>
          ))}
        </div>

        <div className="mt-14 flex flex-col items-center justify-between gap-4 border-t border-white/10 pt-8 sm:flex-row">
          <p className="text-xs text-gray-500">
            &copy; {new Date().getFullYear()} Student Groups Hub — U-ERRE. Todos los derechos reservados.
          </p>
          <p className="flex items-center gap-1.5 text-xs text-gray-500">
            Hecho con <Heart className="size-3 fill-violet-400 text-violet-400" /> por estudiantes
          </p>
        </div>
      </div>
    </footer>
  );
}
