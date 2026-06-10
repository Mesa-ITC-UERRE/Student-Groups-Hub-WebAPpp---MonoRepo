import { useEffect, useRef, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useMsal } from "@azure/msal-react";
import { InteractionStatus, EventType } from "@azure/msal-browser";
import type { AuthenticationResult } from "@azure/msal-browser";

export default function AuthCallbackPage() {
  const navigate = useNavigate();
  const { instance, inProgress } = useMsal();
  const returnToRef = useRef<string>("/dashboard");
  const handled = useRef(false);
  const [mounted, setMounted] = useState(false);

  useEffect(() => { setMounted(true); }, []);

  useEffect(() => {
    const callbackId = instance.addEventCallback((event) => {
      if (event.eventType === EventType.LOGIN_SUCCESS && event.payload) {
        const payload = event.payload as AuthenticationResult;
        if (payload.state && payload.state.startsWith("/")) {
          returnToRef.current = payload.state;
        }
      }
    });
    return () => { if (callbackId) instance.removeEventCallback(callbackId); };
  }, [instance]);

  useEffect(() => {
    if (!mounted) return;
    if (inProgress !== InteractionStatus.None) return;
    if (handled.current) return;

    const timer = setTimeout(() => {
      if (handled.current) return;
      const accounts = instance.getAllAccounts();
      if (accounts.length > 0) {
        handled.current = true;
        instance.setActiveAccount(accounts[0]);
        document.cookie = "msal_authenticated=1; path=/; SameSite=Lax; max-age=604800";
        navigate(returnToRef.current, { replace: true });
      } else {
        handled.current = true;
        navigate("/login", { replace: true });
      }
    }, 0);

    return () => clearTimeout(timer);
  }, [mounted, inProgress, instance, navigate]);

  return (
    <div className="flex min-h-screen items-center justify-center bg-background">
      <div className="space-y-3 text-center">
        <div className="mx-auto h-8 w-8 animate-spin rounded-full border-4 border-primary border-t-transparent" />
        <p className="text-sm text-muted-foreground">Iniciando sesión...</p>
      </div>
    </div>
  );
}
