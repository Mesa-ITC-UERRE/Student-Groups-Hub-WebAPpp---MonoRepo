import { BookOpen, Calendar, Sparkles, Users, Eye, EyeOff } from "lucide-react";
import { Link, useSearchParams, useNavigate } from "react-router-dom";
import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { supabase } from "@/lib/supabase";

const highlights = [
  { icon: Users, title: "120+ Grupos", description: "Encuentra tu comunidad entre más de cien grupos activos." },
  { icon: Calendar, title: "Eventos Semanales", description: "Hackathons, conciertos, talleres y más cada semana." },
  { icon: BookOpen, title: "Crece con Otros", description: "Conecta con estudiantes que comparten tus intereses." },
];

export default function LoginPage() {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const returnTo = searchParams.get("returnTo") ?? "/dashboard";

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [mode, setMode] = useState<"login" | "register">("login");

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setLoading(true);
    setError(null);

    try {
      if (mode === "login") {
        const { error } = await supabase.auth.signInWithPassword({ email, password });
        if (error) throw error;
      } else {
        const { error } = await supabase.auth.signUp({ email, password });
        if (error) throw error;
      }
      navigate(returnTo, { replace: true });
    } catch (err: unknown) {
      const msg = err instanceof Error ? err.message : "Error de autenticación";
      // Translate common Supabase error messages to Spanish
      if (msg.includes("Invalid login credentials")) {
        setError("Correo o contraseña incorrectos.");
      } else if (msg.includes("User already registered")) {
        setError("Ya existe una cuenta con este correo. Inicia sesión.");
      } else if (msg.includes("Password should be at least")) {
        setError("La contraseña debe tener al menos 6 caracteres.");
      } else {
        setError(msg);
      }
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="flex min-h-screen">
      {/* Left panel — branding */}
      <div className="relative isolate hidden flex-col bg-gradient-to-br from-[#4C1D95] via-[#7C3AED] to-[#8B5CF6] justify-between overflow-hidden lg:flex lg:w-[55%]">
        <div className="absolute -left-32 -top-32 size-96 rounded-full bg-violet-400/25 blur-3xl" aria-hidden="true" />
        <div className="absolute -bottom-40 -right-40 size-80 rounded-full bg-amber-400/10 blur-3xl" aria-hidden="true" />

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
              Únete a la red de grupos estudiantiles más grande de U-ERRE.
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

          <blockquote className="border-l-2 border-amber-400/40 pl-4">
            <p className="text-sm italic text-white/50">
              &ldquo;Gracias a Student Groups Hub encontré mi equipo de robótica y ahora competimos juntos a nivel nacional.&rdquo;
            </p>
            <footer className="mt-2 text-xs font-medium text-white/40">— María Alejandra, Ing. Mecatrónica</footer>
          </blockquote>
        </div>
      </div>

      {/* Right panel — form */}
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
          <h2 className="font-heading text-2xl font-bold text-zinc-900 sm:text-3xl">
            {mode === "login" ? "Bienvenido de vuelta" : "Crear cuenta"}
          </h2>
          <p className="mt-2 text-sm text-zinc-400">
            {mode === "login"
              ? "Inicia sesión con tu correo institucional."
              : "Regístrate para explorar los grupos de U-ERRE."}
          </p>

          <form onSubmit={handleSubmit} className="mt-8 space-y-4">
            {error && (
              <div className="rounded-xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
                {error}
              </div>
            )}

            <div className="space-y-1.5">
              <Label htmlFor="email">Correo electrónico</Label>
              <Input
                id="email"
                type="email"
                required
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                placeholder="tu.nombre@uerre.mx"
                className="rounded-xl h-11"
                autoComplete="email"
              />
            </div>

            <div className="space-y-1.5">
              <Label htmlFor="password">Contraseña</Label>
              <div className="relative">
                <Input
                  id="password"
                  type={showPassword ? "text" : "password"}
                  required
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  placeholder={mode === "register" ? "Mínimo 6 caracteres" : "Tu contraseña"}
                  className="rounded-xl h-11 pr-10"
                  autoComplete={mode === "login" ? "current-password" : "new-password"}
                />
                <button
                  type="button"
                  onClick={() => setShowPassword(!showPassword)}
                  className="absolute right-3 top-1/2 -translate-y-1/2 text-zinc-400 hover:text-zinc-600"
                  tabIndex={-1}
                >
                  {showPassword ? <EyeOff className="size-4" /> : <Eye className="size-4" />}
                </button>
              </div>
            </div>

            <Button
              type="submit"
              disabled={loading}
              className="h-11 w-full rounded-xl bg-violet-700 font-heading text-sm font-bold text-white shadow-[0_4px_14px_rgba(91,33,182,0.3)] hover:bg-violet-800 disabled:opacity-60 mt-2"
            >
              {loading ? (
                <div className="h-4 w-4 animate-spin rounded-full border-2 border-white border-t-transparent mr-2" />
              ) : null}
              {loading
                ? "Procesando..."
                : mode === "login"
                ? "Iniciar Sesión"
                : "Crear Cuenta"}
            </Button>
          </form>

          <p className="mt-6 text-center text-sm text-zinc-500">
            {mode === "login" ? (
              <>
                ¿No tienes cuenta?{" "}
                <button
                  type="button"
                  onClick={() => { setMode("register"); setError(null); }}
                  className="font-semibold text-violet-700 hover:underline"
                >
                  Regístrate
                </button>
              </>
            ) : (
              <>
                ¿Ya tienes cuenta?{" "}
                <button
                  type="button"
                  onClick={() => { setMode("login"); setError(null); }}
                  className="font-semibold text-violet-700 hover:underline"
                >
                  Inicia sesión
                </button>
              </>
            )}
          </p>

          <div className="mt-6 text-center">
            <Link to="/" className="text-xs text-zinc-400 transition-colors hover:text-zinc-900">
              ← Volver al inicio
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
}
