import { useEffect, useState } from "react";
import { MsalProvider } from "@azure/msal-react";
import { getMsalInstance } from "@/lib/msal";
import type { PublicClientApplication } from "@azure/msal-browser";
import type { ReactNode } from "react";

export default function MsalProviderWrapper({ children }: { children: ReactNode }) {
  const [instance, setInstance] = useState<PublicClientApplication | null>(null);

  useEffect(() => {
    const pca = getMsalInstance();
    // initialize() MUST be called before any MSAL operation.
    // It internally calls handleRedirectPromise(), which processes the
    // Entra ID redirect response and stores tokens + accounts in the cache.
    // Only after this resolves is it safe to render <MsalProvider>.
    pca.initialize().then(() => setInstance(pca));
  }, []);

  if (!instance) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-background">
        <div className="h-8 w-8 animate-spin rounded-full border-4 border-primary border-t-transparent" />
      </div>
    );
  }

  return <MsalProvider instance={instance}>{children}</MsalProvider>;
}
