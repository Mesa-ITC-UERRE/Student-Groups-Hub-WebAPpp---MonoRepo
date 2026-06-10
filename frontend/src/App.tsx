import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import MsalProviderWrapper from "@/components/providers/MsalProviderWrapper";
import { AuthProvider } from "@/context/AuthContext";

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

export default function App() {
  return (
    // BrowserRouter is outermost so routing context is available everywhere,
    // including inside MsalProviderWrapper and AuthProvider if they ever need
    // to use navigation hooks.
    <BrowserRouter>
      <QueryClientProvider client={queryClient}>
        <MsalProviderWrapper>
          <AuthProvider>
            <Routes>
              <Route path="/" element={<HomePage />} />
              <Route path="/login" element={<LoginPage />} />
              <Route path="/auth/callback" element={<AuthCallbackPage />} />
              <Route path="/groups" element={<GroupsPage />} />
              {/* /groups/register must come BEFORE /groups/:slug */}
              <Route path="/groups/register" element={<RegisterGroupPage />} />
              <Route path="/groups/:slug" element={<GroupDetailPage />} />
              <Route path="/dashboard" element={<DashboardPage />} />
              {/* Catch-all */}
              <Route path="*" element={<Navigate to="/" replace />} />
            </Routes>
          </AuthProvider>
        </MsalProviderWrapper>
      </QueryClientProvider>
    </BrowserRouter>
  );
}
