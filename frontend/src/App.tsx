import { BrowserRouter, Routes, Route, Navigate, useNavigate } from "react-router-dom";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { MsalProvider } from "@azure/msal-react";
import { AuthProvider } from "@/context/AuthContext";
import { msalInstance } from "@/lib/msal";
import { CustomNavigationClient } from "@/lib/NavigationClient";

import HomePage from "@/pages/HomePage";
import LoginPage from "@/pages/LoginPage";
import GroupsPage from "@/pages/GroupsPage";
import GroupDetailPage from "@/pages/GroupDetailPage";
import RegisterGroupPage from "@/pages/RegisterGroupPage";
import DashboardPage from "@/pages/DashboardPage";

const queryClient = new QueryClient({
  defaultOptions: {
    queries: { retry: 1, staleTime: 1000 * 60 * 5 },
  },
});

function AppWithMsal() {
  const navigate = useNavigate();
  msalInstance.setNavigationClient(new CustomNavigationClient(navigate));

  return (
    <MsalProvider instance={msalInstance}>
      <AuthProvider>
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/login" element={<LoginPage />} />
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
