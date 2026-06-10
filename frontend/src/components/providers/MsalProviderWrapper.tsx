import { MsalProvider } from "@azure/msal-react";
import { getMsalInstance } from "@/lib/msal";
import type { ReactNode } from "react";

// Get the PCA instance synchronously — no initialize() call here.
// MsalProvider calls handleRedirectPromise() internally during its own
// initialization cycle. Calling initialize() before mounting MsalProvider
// causes it to consume the auth code from the URL, so when MsalProvider
// tries to process the redirect it finds nothing and stores no accounts.
const msalInstance = getMsalInstance();

export default function MsalProviderWrapper({ children }: { children: ReactNode }) {
  return <MsalProvider instance={msalInstance}>{children}</MsalProvider>;
}
