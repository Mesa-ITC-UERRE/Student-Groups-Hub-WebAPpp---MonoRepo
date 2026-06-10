import { useEffect } from "react";
import { broadcastResponseToMainFrame } from "@azure/msal-browser/redirect-bridge";

// This page is intentionally rendered OUTSIDE <MsalProvider>.
// When MsalProvider is active, it calls handleRedirectPromise() and consumes
// the auth code from the URL. This page uses broadcastResponseToMainFrame()
// instead — the MSAL v5 redirect bridge — which posts the auth response back
// to the main frame so MsalProvider can process it normally on next navigation.
export default function AuthCallbackPage() {
  useEffect(() => {
    broadcastResponseToMainFrame().catch((err) => {
      console.error("[AuthCallback] broadcastResponseToMainFrame failed:", err);
    });
  }, []);

  return (
    <div className="flex min-h-screen items-center justify-center bg-background">
      <div className="space-y-3 text-center">
        <div className="mx-auto h-8 w-8 animate-spin rounded-full border-4 border-primary border-t-transparent" />
        <p className="text-sm text-muted-foreground">Iniciando sesión...</p>
      </div>
    </div>
  );
}
