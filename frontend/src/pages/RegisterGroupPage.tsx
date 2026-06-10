import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { ArrowLeft, CheckCircle2 } from "lucide-react";
import { groupRegistrationApi } from "@/lib/api";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Navbar } from "@/components/layout/Navbar";
import { Footer } from "@/components/layout/Footer";
import { useAuth } from "@/context/AuthContext";

export default function RegisterGroupPage() {
  const { isAuthenticated } = useAuth();
  const navigate = useNavigate();

  const [form, setForm] = useState({ proposedGroupName: "", proposedDescription: "", contactEmail: "" });
  const [submitting, setSubmitting] = useState(false);
  const [success, setSuccess] = useState(false);
  const [error, setError] = useState<string | null>(null);

  function handleChange(e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) {
    setForm((prev) => ({ ...prev, [e.target.name]: e.target.value }));
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!isAuthenticated) {
      navigate("/login?returnTo=/groups/register");
      return;
    }
    setSubmitting(true);
    setError(null);
    try {
      await groupRegistrationApi.create({
        proposedGroupName: form.proposedGroupName.trim(),
        proposedDescription: form.proposedDescription.trim() || undefined,
        contactEmail: form.contactEmail.trim(),
      });
      setSuccess(true);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : "Error al enviar la solicitud");
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <div className="flex min-h-screen flex-col">
      <Navbar />
      <main className="flex-1 bg-zinc-50">
        <section className="bg-gradient-to-br from-[#4C1D95] via-[#7C3AED] to-[#8B5CF6] py-14 text-white">
          <div className="mx-auto max-w-3xl px-6">
            <Link to="/groups" className="mb-6 inline-flex items-center gap-1.5 text-xs font-medium text-white/60 transition-colors hover:text-white">
              <ArrowLeft className="size-3.5" /> Grupos
            </Link>
            <h1 className="font-heading text-3xl font-extrabold sm:text-4xl">Solicitar Nuevo Grupo</h1>
            <p className="mt-3 max-w-lg text-sm leading-relaxed text-white/70">
              Completa el formulario y el equipo de U-ERRE revisará tu solicitud. Una vez aprobada, el grupo será creado.
            </p>
          </div>
        </section>

        <section className="mx-auto max-w-3xl px-6 py-12">
          {success ? (
            <div className="rounded-2xl border border-green-200 bg-green-50 px-8 py-10 text-center">
              <CheckCircle2 className="mx-auto size-12 text-green-500" />
              <h2 className="mt-4 font-heading text-xl font-bold text-zinc-900">¡Solicitud enviada!</h2>
              <p className="mt-2 text-sm text-zinc-500">
                Tu solicitud ha sido recibida y está en revisión. Te notificaremos cuando sea aprobada o rechazada.
              </p>
              <div className="mt-6 flex items-center justify-center gap-4">
                <Link to="/dashboard"><Button variant="outline" className="rounded-xl">Ver mis solicitudes</Button></Link>
                <Link to="/groups"><Button className="rounded-xl bg-violet-700 hover:bg-violet-800">Explorar grupos</Button></Link>
              </div>
            </div>
          ) : (
            <form onSubmit={handleSubmit} className="space-y-6 rounded-2xl border border-border bg-white p-8 shadow-sm">
              {error && (
                <div className="rounded-xl border border-red-200 bg-red-50 px-5 py-4 text-sm text-red-700">{error}</div>
              )}
              {!isAuthenticated && (
                <div className="rounded-xl border border-amber-200 bg-amber-50 px-5 py-4 text-sm text-amber-700">
                  Debes{" "}
                  <Link to="/login?returnTo=/groups/register" className="font-semibold underline">iniciar sesión</Link>{" "}
                  para enviar una solicitud.
                </div>
              )}
              <div className="space-y-2">
                <Label htmlFor="proposedGroupName">Nombre del grupo <span className="text-red-500">*</span></Label>
                <Input id="proposedGroupName" name="proposedGroupName" type="text" required placeholder="Ej. Club de Robótica U-ERRE" value={form.proposedGroupName} onChange={handleChange} className="rounded-xl" maxLength={200} />
              </div>
              <div className="space-y-2">
                <Label htmlFor="proposedDescription">Descripción del grupo</Label>
                <textarea id="proposedDescription" name="proposedDescription" rows={4} placeholder="¿De qué trata el grupo? ¿Qué actividades realizarán?" value={form.proposedDescription} onChange={handleChange}
                  className="w-full rounded-xl border border-input bg-background px-3 py-2 text-sm placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring resize-none" />
              </div>
              <div className="space-y-2">
                <Label htmlFor="contactEmail">Correo de contacto <span className="text-red-500">*</span></Label>
                <Input id="contactEmail" name="contactEmail" type="email" required placeholder="tu.nombre@uerre.mx" value={form.contactEmail} onChange={handleChange} className="rounded-xl" />
                <p className="text-xs text-zinc-400">Te contactaremos a este correo con el resultado de la revisión.</p>
              </div>
              <div className="flex items-center justify-end gap-3 pt-2">
                <Link to="/groups"><Button type="button" variant="outline" className="rounded-xl">Cancelar</Button></Link>
                <Button type="submit" disabled={submitting || !isAuthenticated} className="rounded-xl bg-violet-700 font-heading font-bold text-white hover:bg-violet-800 disabled:opacity-60">
                  {submitting ? (
                    <><div className="mr-2 h-4 w-4 animate-spin rounded-full border-2 border-white border-t-transparent" />Enviando...</>
                  ) : "Enviar solicitud"}
                </Button>
              </div>
            </form>
          )}
        </section>
      </main>
      <Footer />
    </div>
  );
}
