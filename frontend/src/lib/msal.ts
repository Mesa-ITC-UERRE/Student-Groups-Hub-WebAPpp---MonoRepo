import {
  type Configuration,
  PublicClientApplication,
  LogLevel,
} from "@azure/msal-browser";

// ---------------------------------------------------------------------------
// E2E test mock support
// ---------------------------------------------------------------------------
export interface MsalMockConfig {
  authenticated: boolean;
  redirectState?: string;
  accounts?: Array<{
    homeAccountId: string;
    localAccountId: string;
    tenantId: string;
    username: string;
    name: string;
  }>;
}

declare global {
  interface Window {
    __MSAL_MOCK__?: MsalMockConfig;
  }
}

function createMockPca(config: MsalMockConfig): PublicClientApplication {
  const fakeAccounts = config.authenticated
    ? (config.accounts ?? [
        {
          homeAccountId: "test-oid.test-tid",
          localAccountId: "test-oid",
          tenantId: "test-tid",
          username: "testuser@uerre.mx",
          name: "Test User",
        },
      ])
    : [];

  const fakeLogger = {
    clone: () => fakeLogger,
    verbose: () => {},
    info: () => {},
    warning: () => {},
    error: () => {},
  };

  let cbCounter = 0;
  const callbacks = new Map<string, (event: unknown) => void>();
  let activeAccount = fakeAccounts[0] ?? null;

  return {
    initializeWrapperLibrary: () => {},
    getLogger: () => fakeLogger,
    addEventCallback: (cb: (event: unknown) => void) => {
      const id = `mock_cb_${++cbCounter}`;
      callbacks.set(id, cb);
      return id;
    },
    removeEventCallback: (id: string) => {
      callbacks.delete(id);
    },
    initialize: () => Promise.resolve(),
    handleRedirectPromise: () => {
      if (config.authenticated && config.redirectState) {
        Promise.resolve().then(() => {
          for (const cb of callbacks.values()) {
            cb({
              eventType: "msal:loginSuccess",
              payload: { state: config.redirectState },
            });
          }
        });
      }
      return Promise.resolve(null);
    },
    getAllAccounts: () => fakeAccounts,
    getActiveAccount: () => activeAccount,
    setActiveAccount: (account: unknown) => {
      activeAccount = account as (typeof fakeAccounts)[0];
    },
    acquireTokenSilent: () =>
      Promise.resolve({ accessToken: "mock-access-token" }),
    acquireTokenRedirect: () => Promise.resolve(),
    loginRedirect: () => Promise.resolve(),
    logoutRedirect: () => Promise.resolve(),
  } as unknown as PublicClientApplication;
}

// ---------------------------------------------------------------------------

export const msalConfig: Configuration = {
  auth: {
    clientId: import.meta.env.VITE_ENTRA_CLIENT_ID,
    authority: `https://login.microsoftonline.com/${import.meta.env.VITE_ENTRA_TENANT_ID}`,
    redirectUri: `${window.location.origin}/auth/callback`,
    postLogoutRedirectUri: `${window.location.origin}/login`,
  },
  cache: {
    cacheLocation: "localStorage",
  },
  system: {
    loggerOptions: {
      loggerCallback: (level, message, containsPii) => {
        if (containsPii) return;
        if (import.meta.env.DEV) {
          console.log(`[MSAL][${LogLevel[level]}] ${message}`);
        }
      },
    },
  },
};

let _msalInstance: PublicClientApplication | null = null;

export function getMsalInstance(): PublicClientApplication {
  if (window.__MSAL_MOCK__ && !_msalInstance) {
    _msalInstance = createMockPca(window.__MSAL_MOCK__);
  }
  if (!_msalInstance) {
    _msalInstance = new PublicClientApplication(msalConfig);
  }
  return _msalInstance;
}

export const loginRequest = {
  scopes: ["openid", "profile", "email"],
};

export const apiRequest = {
  scopes: [`api://${import.meta.env.VITE_ENTRA_CLIENT_ID}/access_as_user`],
};
