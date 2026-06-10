import { BrowserRouter, Routes, Route, Navigate, useNavigate, useLocation } from "react-router-dom";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { MsalProvider } from "@azure/msal-react";
import { AuthProvider } from "@/context/AuthContext";
import { msalInstance } from "@/lib/msal";
import { CustomNavigationClient } from "@/lib/NavigationClient";

import HomePage from "@/pages/HomePage";
import LoginPage from "@/pages/LoginPage";
import AuthCallbackPage from "@/pages/AuthCallbackPage";
import GroupsPage from "@/pages/GroupsPage";
import GroupDetailPage from "@/pages/GroupDetailPage";
import RegisterGroupPage from "@/pages/RegisterGroupPage";
import DashboardPage from "@/pages/DashboardPage";

const queryClient = new QueryClient({
  defaultOptions: {
    queries: { retry: 1, staleTime: 1000 * 60 * 5 },
  },
});

// Inner component — has access to router hooks so we can wire
// CustomNavigationClient to MSAL before MsalProvider renders.
function AppWithMsal() {
  const navigate = useNavigate();
  const location = useLocation();

  // Wire React Router's navigate to MSAL so internal redirects
  // (e.g. after login/logout) use client-side navigation.
  msalInstance.setNavigationClient(new CustomNavigationClient(navigate));

  // The /auth/callback route MUST render outside <MsalProvider>.
  // MsalProvider calls handleRedirectPromise() internally — if it is active
  // on the callback page, it consumes the auth code before the redirect
  // bridge can broadcast it, leaving no accounts in the cache.
  if (location.pathname === "/auth/callback") {
    return <AuthCallbackPage />;
  }

  return (
    <MsalProvider instance={msalInstance}>
      <AuthProvider>
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/login" element={<LoginPage />} />
          {/* /groups/register before /groups/:slug to avoid slug capture */}
          <Route path="/groups/register" element={<RegisterGroupPage />} />
          <Route path="/groups/:slug" element={<GroupDetailPage />} />
          <Route path="/groups" element={<GroupsPage />} />
          <Route path="/dashboard" element={<DashboardPage />} />
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </AuthProvider>
    </MsalProvider>
  );
}

export default function App() {
  return (
    <BrowserRouter>
      <QueryClientProvider client={queryClient}>
        <AppWithMsal />
      </QueryClientProvider>
    </BrowserRouter>
  );
}
