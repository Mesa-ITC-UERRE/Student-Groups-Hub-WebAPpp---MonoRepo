import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { msalInstance } from "./lib/msal";
import "./index.css";
import App from "./App.tsx";

msalInstance.initialize().then(() => {
  // If already signed in from a previous session, restore active account
  const accounts = msalInstance.getAllAccounts();
  if (accounts.length > 0) {
    msalInstance.setActiveAccount(accounts[0]);
  }

  createRoot(document.getElementById("root")!).render(
    <StrictMode>
      <App />
    </StrictMode>,
  );
});
