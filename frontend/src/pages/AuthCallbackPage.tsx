import { useEffect, useRef } from "react";
import { useNavigate } from "react-router-dom";
import { useMsal } from "@azure/msal-react";
import { InteractionStatus, EventType } from "@azure/msal-browser";
import type { AuthenticationResult } from "@azure/msal-browser";

export default function AuthCallbackPage() {
  const navigate = useNavigate();
  const { instance, inProgress } = useMsal();
  // Pre-populate returnTo from sessionStorage — this was written before the
  // Entra redirect so it's always available even if the LOGIN_SUCCESS event
  // already fired before this component mounted.
  const returnToRef = useRef<string>(
    sessionStorage.getItem("auth_return_to") ?? "/dashboard"
  );
  const handled = useRef(false);

  // Capture returnTo from the MSAL LOGIN_SUCCESS event state param.
  // This fires during initialize() (which runs in MsalProviderWrapper
  // before this component even mounts), so we may have already missed it.
  // As a fallback, we also check sessionStorage below.
  useEffect(() => {
    const callbackId = instance.addEventCallback((event) => {
      if (event.eventType === EventType.LOGIN_SUCCESS && event.payload) {
        const payload = event.payload as AuthenticationResult;
        if (payload.state && payload.state.startsWith("/")) {
          returnToRef.current = payload.state;
          // Persist so the timeout check below can read it
          sessionStorage.setItem("auth_return_to", payload.state);
        }
      }
    });
    return () => {
      if (callbackId) instance.removeEventCallback(callbackId);
    };
  }, [instance]);

  useEffect(() => {
    // Wait until MSAL has finished all in-progress interactions
    if (inProgress !== InteractionStatus.None) return;
    if (handled.current) return;

    handled.current = true;

    const accounts = instance.getAllAccounts();

    if (accounts.length > 0) {
      instance.setActiveAccount(accounts[0]);

      // Prefer event-captured state, then sessionStorage (written before redirect)
      const storedReturnTo = sessionStorage.getItem("auth_return_to");
      if (storedReturnTo) {
        returnToRef.current = storedReturnTo;
      }
      sessionStorage.removeItem("auth_return_to");

      navigate(returnToRef.current, { replace: true });
    } else {
      // No accounts found after redirect — something went wrong, back to login
      navigate("/login", { replace: true });
    }
  }, [inProgress, instance, navigate]);

  return (
    <div className="flex min-h-screen items-center justify-center bg-background">
      <div className="space-y-3 text-center">
        <div className="mx-auto h-8 w-8 animate-spin rounded-full border-4 border-primary border-t-transparent" />
        <p className="text-sm text-muted-foreground">Iniciando sesión...</p>
      </div>
    </div>
  );
}
