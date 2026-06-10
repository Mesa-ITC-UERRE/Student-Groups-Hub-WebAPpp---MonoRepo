import { useState } from "react";
import { Button } from "@/components/ui/button";
import { groupApi } from "@/lib/api";

interface JoinButtonProps {
  groupId: string;
  initialStatus?: string | null;
}

export default function JoinButton({ groupId, initialStatus }: JoinButtonProps) {
  const [status, setStatus] = useState<string | null>(initialStatus ?? null);
  const [loading, setLoading] = useState(false);

  async function handleJoin() {
    setLoading(true);
    try {
      const result = await groupApi.join(groupId);
      setStatus(result.status);
    } catch (error) {
      console.error("Failed to join group:", error);
    } finally {
      setLoading(false);
    }
  }

  if (status === "approved") {
    return (
      <span className="inline-flex items-center gap-1.5 rounded-full bg-green-100 px-4 py-2 text-sm font-semibold text-green-700">
        <span className="size-2 rounded-full bg-green-500" />
        Miembro
      </span>
    );
  }

  if (status === "pending") {
    return (
      <span className="inline-flex items-center gap-1.5 rounded-full bg-amber-100 px-4 py-2 text-sm font-semibold text-amber-700">
        <span className="size-2 rounded-full bg-amber-500" />
        Solicitud pendiente
      </span>
    );
  }

  return (
    <Button
      onClick={handleJoin}
      disabled={loading}
      className="rounded-xl bg-violet-700 font-heading text-sm font-bold text-white shadow-[0_4px_14px_rgba(91,33,182,0.3)] hover:bg-violet-800 disabled:opacity-60"
    >
      {loading && (
        <div className="h-4 w-4 animate-spin rounded-full border-2 border-white border-t-transparent mr-2" />
      )}
      {loading ? "Enviando..." : "Unirse al grupo"}
    </Button>
  );
}
