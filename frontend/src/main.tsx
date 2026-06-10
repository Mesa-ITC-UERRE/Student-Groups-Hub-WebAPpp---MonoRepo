import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { EventType, type EventMessage, type AccountInfo } from "@azure/msal-browser";
import { msalInstance } from "./lib/msal";
import "./index.css";
import App from "./App.tsx";

// Initialize MSAL before rendering React.
// This calls handleRedirectPromise() internally — it processes the Entra
// redirect response from the URL exactly once, stores tokens, and populates
// the account cache. React renders only after this completes.
msalInstance.initialize().then(() => {
  // Set active account from cache if already signed in
  const accounts = msalInstance.getAllAccounts();
  if (accounts.length > 0) {
    msalInstance.setActiveAccount(accounts[0]);
  }

  // Keep active account in sync whenever login succeeds
  msalInstance.addEventCallback((event: EventMessage) => {
    if (event.eventType === EventType.LOGIN_SUCCESS && event.payload) {
      const account = event.payload as AccountInfo;
      msalInstance.setActiveAccount(account);
    }
  });

  createRoot(document.getElementById("root")!).render(
    <StrictMode>
      <App />
    </StrictMode>,
  );
});
