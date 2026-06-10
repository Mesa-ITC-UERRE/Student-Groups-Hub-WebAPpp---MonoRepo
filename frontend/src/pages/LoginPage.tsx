import { BookOpen, Calendar, Sparkles, Users } from "lucide-react";
import { Link, useSearchParams } from "react-router-dom";
import { useState } from "react";
import { Button } from "@/components/ui/button";
import { getMsalInstance, loginRequest } from "@/lib/msal";

const highlights = [
  { icon: Users, title: "120+ Grupos", description: "Encuentra tu comunidad entre más de cien grupos activos." },
  { icon: Calendar, title: "Eventos Semanales", description: "Hackathons, conciertos, talleres y más cada semana." },
  { icon: BookOpen, title: "Crece con Otros", description: "Conecta con estudiantes que comparten tus intereses." },
];

export default function LoginPage() {
  const [searchParams] = useSearchParams();
  const returnTo = searchParams.get("returnTo") ?? "/dashboard";
  const [loading, setLoading] = useState(false);

  async function handleMicrosoftLogin() {
    setLoading(true);
    try {
      sessionStorage.setItem("auth_return_to", returnTo);
      // Do NOT override redirectUri here — MSAL uses the absolute URL from
      // msalConfig.auth.redirectUri for both the authorization request AND
      // the token exchange. They must be identical or Entra returns 400.
      await getMsalInstance().loginRedirect({
        ...loginRequest,
        state: returnTo,
      });
    } catch (error) {
      console.error("Login error:", error);
      setLoading(false);
    }
  }

  return (
    <div className="flex min-h-screen">
      {/* Left panel */}
      <div className="relative isolate hidden flex-col bg-gradient-to-br from-[#4C1D95] via-[#7C3AED] to-[#8B5CF6] justify-between overflow-hidden lg:flex lg:w-[55%]">
        <div className="absolute -left-32 -top-32 size-96 rounded-full bg-violet-400/25 blur-3xl" aria-hidden="true" />
        <div className="absolute -bottom-40 -right-40 size-80 rounded-full bg-amber-400/10 blur-3xl" aria-hidden="true" />
        <div className="absolute left-1/2 top-1/3 size-64 rounded-full bg-white/5 blur-[100px]" aria-hidden="true" />
        <div className="absolute left-[15%] top-[20%] size-2 rounded-full bg-amber-400/60" aria-hidden="true" />
        <div className="absolute right-[25%] top-[40%] size-1.5 rounded-full bg-white/30" aria-hidden="true" />
        <div className="absolute left-[40%] bottom-[30%] size-2.5 rounded-full bg-violet-400/50" aria-hidden="true" />

        <div className="relative z-10 flex flex-1 flex-col justify-between p-12 xl:p-16">
          <Link to="/" className="flex items-center gap-3">
            <div className="flex size-11 items-center justify-center rounded-xl bg-white/15 backdrop-blur-sm">
              <span className="font-heading text-lg font-extrabold text-white">U</span>
            </div>
            <div>
              <p className="font-heading text-sm font-bold text-white">Student Groups Hub</p>
              <p className="text-[10px] font-medium uppercase tracking-widest text-white/50">Universidad Regiomontana</p>
            </div>
          </Link>

          <div className="my-auto max-w-lg py-12">
            <div className="animate-fade-up inline-flex items-center gap-2 rounded-full border border-white/15 bg-white/10 px-4 py-1.5 text-xs font-semibold text-white/80 backdrop-blur-sm">
              <Sparkles className="size-3.5" /> Bienvenido a tu comunidad
            </div>
            <h1 className="animate-fade-up delay-100 mt-8 font-heading text-4xl font-extrabold leading-[1.1] text-white xl:text-5xl">
              Donde cada<br />estudiante
              <span className="mt-1 block text-amber-300">encuentra su lugar.</span>
            </h1>
            <p className="animate-fade-up delay-200 mt-6 max-w-sm text-base leading-relaxed text-white/60">
              Únete a la red de grupos estudiantiles más grande de U-ERRE y transforma tu experiencia universitaria.
            </p>
            <div className="animate-fade-up delay-300 mt-10 flex flex-col gap-5">
              {highlights.map((item) => (
                <div key={item.title} className="flex items-start gap-4">
                  <div className="flex size-10 shrink-0 items-center justify-center rounded-xl bg-white/10 backdrop-blur-sm">
                    <item.icon className="size-5 text-white/80" />
                  </div>
                  <div>
                    <p className="text-sm font-bold text-white">{item.title}</p>
                    <p className="mt-0.5 text-sm text-white/50">{item.description}</p>
                  </div>
                </div>
              ))}
            </div>
          </div>

          <div className="animate-fade-up delay-500">
            <blockquote className="border-l-2 border-amber-400/40 pl-4">
              <p className="text-sm italic text-white/50">
                &ldquo;Gracias a Student Groups Hub encontré mi equipo de robótica y ahora competimos juntos a nivel nacional.&rdquo;
              </p>
              <footer className="mt-2 text-xs font-medium text-white/40">— María Alejandra, Ing. Mecatrónica</footer>
            </blockquote>
          </div>
        </div>
      </div>

      {/* Right panel */}
      <div className="flex flex-1 flex-col justify-center bg-white px-6 py-12 sm:px-12 lg:px-16 xl:px-24">
        <div className="mb-10 lg:hidden">
          <Link to="/" className="flex items-center gap-2.5">
            <div className="flex size-9 items-center justify-center rounded-lg bg-violet-700">
              <span className="font-heading text-sm font-extrabold text-white">U</span>
            </div>
            <span className="font-heading text-sm font-bold text-zinc-900">Student Groups Hub</span>
          </Link>
        </div>

        <div className="mx-auto w-full max-w-[400px]">
          <div>
            <h2 className="font-heading text-2xl font-bold text-zinc-900 sm:text-3xl">Bienvenido de vuelta</h2>
            <p className="mt-2 text-sm text-zinc-400">Inicia sesión con tu cuenta institucional de U-ERRE para continuar.</p>
          </div>

          <div className="mt-10">
            <Button
              type="button"
              onClick={handleMicrosoftLogin}
              disabled={loading}
              className="h-12 w-full rounded-xl bg-[#0078D4] font-heading text-sm font-bold text-white shadow-[0_4px_14px_rgba(0,120,212,0.3)] transition-all hover:bg-[#006CBE] hover:shadow-[0_8px_24px_rgba(0,120,212,0.25)] disabled:opacity-60"
            >
              {loading ? (
                <div className="h-4 w-4 animate-spin rounded-full border-2 border-white border-t-transparent mr-2" />
              ) : (
                <svg className="mr-2.5 size-5 shrink-0" viewBox="0 0 21 21" fill="none" aria-hidden="true">
                  <rect x="1" y="1" width="9" height="9" fill="#F25022" />
                  <rect x="11" y="1" width="9" height="9" fill="#7FBA00" />
                  <rect x="1" y="11" width="9" height="9" fill="#00A4EF" />
                  <rect x="11" y="11" width="9" height="9" fill="#FFB900" />
                </svg>
              )}
              {loading ? "Redirigiendo..." : "Continuar con Microsoft"}
            </Button>
          </div>

          <p className="mt-6 text-center text-xs text-zinc-400 leading-relaxed">
            Solo cuentas institucionales <span className="font-medium text-zinc-600">@uerre.mx</span> tienen acceso.
          </p>

          <div className="mt-8 text-center">
            <Link to="/" className="text-xs text-zinc-400 transition-colors hover:text-zinc-900">← Volver al inicio</Link>
          </div>
        </div>
      </div>
    </div>
  );
}
