import {
  type Configuration,
  PublicClientApplication,
} from "@azure/msal-browser";

export const msalConfig: Configuration = {
  auth: {
    clientId: import.meta.env.VITE_ENTRA_CLIENT_ID,
    authority: `https://login.microsoftonline.com/${import.meta.env.VITE_ENTRA_TENANT_ID}`,
    redirectUri: "http://localhost:5173/auth/callback",
    postLogoutRedirectUri: "http://localhost:5173/login",
  },
  cache: {
    cacheLocation: "localStorage",
  },
};

export const loginRequest = {
  scopes: ["openid", "profile", "email"],
  prompt: "select_account",
};

export const apiRequest = {
  scopes: [`api://${import.meta.env.VITE_ENTRA_CLIENT_ID}/access_as_user`],
};

// Create and export the PCA instance.
// initialize() is called in main.tsx before React renders — this ensures
// handleRedirectPromise() runs exactly once and BEFORE MsalProvider mounts.
export const msalInstance = new PublicClientApplication(msalConfig);
