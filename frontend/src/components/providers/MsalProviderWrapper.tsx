import { MsalProvider } from "@azure/msal-react";
import { msalInstance } from "@/lib/msal";
import type { ReactNode } from "react";

// Pass the already-initialized PCA instance (initialized in main.tsx).
// No initialize() call here — that runs once before React mounts.
export default function MsalProviderWrapper({ children }: { children: ReactNode }) {
  return <MsalProvider instance={msalInstance}>{children}</MsalProvider>;
}
