import { useEffect, useRef } from "react";
import { useNavigate } from "react-router-dom";
import { useMsal } from "@azure/msal-react";
import { InteractionStatus, EventType } from "@azure/msal-browser";
import type { AuthenticationResult, AuthError } from "@azure/msal-browser";

export default function AuthCallbackPage() {
  const navigate = useNavigate();
  const { instance, inProgress } = useMsal();
  const returnToRef = useRef<string>(
    sessionStorage.getItem("auth_return_to") ?? "/dashboard"
  );
  const handled = useRef(false);

  // Listen for LOGIN_SUCCESS to capture returnTo state,
  // and LOGIN_FAILURE to detect token exchange errors early.
  useEffect(() => {
    const callbackId = instance.addEventCallback((event) => {
      if (event.eventType === EventType.LOGIN_SUCCESS && event.payload) {
        const payload = event.payload as AuthenticationResult;
        if (payload.state && payload.state.startsWith("/")) {
          returnToRef.current = payload.state;
        }
      }
      if (event.eventType === "msal:acquireTokenFailure") {
        const err = event.error as AuthError | undefined;
        console.error("[Auth] Token acquisition failed:", err?.errorCode, err?.errorMessage);
      }
    });
    return () => {
      if (callbackId) instance.removeEventCallback(callbackId);
    };
  }, [instance]);

  useEffect(() => {
    if (inProgress !== InteractionStatus.None) return;
    if (handled.current) return;
    handled.current = true;

    const accounts = instance.getAllAccounts();

    if (accounts.length > 0) {
      instance.setActiveAccount(accounts[0]);
      const storedReturnTo = sessionStorage.getItem("auth_return_to");
      if (storedReturnTo) returnToRef.current = storedReturnTo;
      sessionStorage.removeItem("auth_return_to");
      navigate(returnToRef.current, { replace: true });
    } else {
      // Token exchange failed — clear any stale MSAL interaction state
      // so the next login attempt starts fresh, then send back to login.
      Object.keys(localStorage)
        .filter((k) => k.includes("interaction.status") || k.includes("request.params"))
        .forEach((k) => localStorage.removeItem(k));
      sessionStorage.removeItem("auth_return_to");
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
