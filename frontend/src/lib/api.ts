import { msalInstance, apiRequest, loginRequest } from "@/lib/msal";
import type {
  User,
  Group,
  GroupMember,
  PendingMember,
  JoinGroupResponse,
  GroupRegistrationRequest,
  GroupTerm,
  Event,
  EventRsvp,
  Post,
  Comment,
  Notification,
  Report,
  RoleAssignment,
  PaginatedResponse,
} from "@/types";

const API_URL = import.meta.env.VITE_API_URL ?? "http://localhost:8080";

// ─── Token acquisition ────────────────────────────────────────────────────────

async function getAccessToken(): Promise<string> {
  const account = msalInstance.getActiveAccount();

  if (!account) {
    await msalInstance.loginRedirect(loginRequest);
    throw new Error("Redirecting to login");
  }

  try {
    const result = await msalInstance.acquireTokenSilent({ ...apiRequest, account });
    return result.accessToken;
  } catch {
    await msalInstance.acquireTokenRedirect({ ...apiRequest, account });
    throw new Error("Redirecting to acquire token");
  }
}

// ─── Core fetch wrappers ──────────────────────────────────────────────────────

async function apiFetch<T>(path: string, options: RequestInit = {}): Promise<T> {
  const token = await getAccessToken();
  const res = await fetch(`${API_URL}${path}`, {
    ...options,
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
      ...(options.headers ?? {}),
    },
  });
  if (!res.ok) {
    const body = await res.text().catch(() => "Unknown error");
    throw new Error(`[${res.status}] ${body}`);
  }
  if (res.status === 204) return undefined as T;
  return res.json() as Promise<T>;
}

async function publicFetch<T>(path: string, options: RequestInit = {}): Promise<T> {
  const res = await fetch(`${API_URL}${path}`, {
    ...options,
    headers: { "Content-Type": "application/json", ...(options.headers ?? {}) },
  });
  if (!res.ok) {
    const body = await res.text().catch(() => "Unknown error");
    throw new Error(`[${res.status}] ${body}`);
  }
  if (res.status === 204) return undefined as T;
  return res.json() as Promise<T>;
}

// ─── Users ────────────────────────────────────────────────────────────────────

export const userApi = {
  me: () => apiFetch<User>("/api/users/me"),
  getById: (id: string) => apiFetch<User>(`/api/users/${id}`),
  getByEntraOid: (oid: string) => apiFetch<User>(`/api/users/by-entra/${oid}`),
  update: (data: { displayName?: string; avatarUrl?: string }) =>
    apiFetch<User>("/api/users/me", { method: "PATCH", body: JSON.stringify(data) }),
  getNotifications: () => apiFetch<Notification[]>("/api/users/me/notifications"),
  markAllNotificationsRead: () =>
    apiFetch<void>("/api/users/me/notifications/read-all", { method: "PATCH" }),
};

// ─── Groups ───────────────────────────────────────────────────────────────────

export const groupApi = {
  getAll: (params?: { search?: string; category?: string; page?: number; pageSize?: number }) => {
    const qs = new URLSearchParams(
      Object.fromEntries(Object.entries(params ?? {}).filter(([, v]) => v !== undefined)) as Record<string, string>
    ).toString();
    return publicFetch<PaginatedResponse<Group>>(`/api/groups${qs ? `?${qs}` : ""}`);
  },
  getCategories: () => publicFetch<string[]>("/api/groups/categories"),
  getBySlug: (slug: string) => publicFetch<Group>(`/api/groups/${slug}`),
  getMembers: (groupId: string) => publicFetch<GroupMember[]>(`/api/groups/${groupId}/members`),
  getPendingMemberships: (groupId: string) =>
    apiFetch<PendingMember[]>(`/api/groups/${groupId}/memberships/pending`),
  join: (groupId: string) =>
    apiFetch<JoinGroupResponse>(`/api/groups/${groupId}/join`, { method: "POST" }),
  approveMembership: (groupId: string, membershipId: string, decisionNotes?: string) =>
    apiFetch<GroupMember>(`/api/groups/${groupId}/memberships/${membershipId}/approve`, {
      method: "PATCH",
      body: JSON.stringify({ decisionNotes }),
    }),
  rejectMembership: (groupId: string, membershipId: string, decisionNotes?: string) =>
    apiFetch<GroupMember>(`/api/groups/${groupId}/memberships/${membershipId}/reject`, {
      method: "PATCH",
      body: JSON.stringify({ decisionNotes }),
    }),
};

// ─── Group Registration Requests ─────────────────────────────────────────────

export const groupRegistrationApi = {
  create: (data: { proposedGroupName: string; proposedDescription?: string; contactEmail: string }) =>
    apiFetch<GroupRegistrationRequest>("/api/group-registration-requests", {
      method: "POST",
      body: JSON.stringify(data),
    }),
  getMine: () => apiFetch<GroupRegistrationRequest[]>("/api/group-registration-requests/mine"),
  getById: (id: string) =>
    apiFetch<GroupRegistrationRequest>(`/api/group-registration-requests/${id}`),
  getAllPending: () => apiFetch<GroupRegistrationRequest[]>("/api/group-registration-requests"),
  approve: (id: string, decisionNotes?: string) =>
    apiFetch<GroupRegistrationRequest>(`/api/group-registration-requests/${id}/approve`, {
      method: "PATCH",
      body: JSON.stringify({ decisionNotes }),
    }),
  reject: (id: string, decisionNotes?: string) =>
    apiFetch<GroupRegistrationRequest>(`/api/group-registration-requests/${id}/reject`, {
      method: "PATCH",
      body: JSON.stringify({ decisionNotes }),
    }),
};

// ─── Group Terms ──────────────────────────────────────────────────────────────

export const groupTermApi = {
  getAll: (groupId: string) => apiFetch<GroupTerm[]>(`/api/groups/${groupId}/terms`),
  create: (groupId: string, data: { label: string; startDate: string; endDate: string }) =>
    apiFetch<GroupTerm>(`/api/groups/${groupId}/terms`, {
      method: "POST",
      body: JSON.stringify(data),
    }),
};

// ─── Events ───────────────────────────────────────────────────────────────────

export const eventApi = {
  getAll: (params?: { search?: string; groupId?: string; from?: string; to?: string }) => {
    const qs = new URLSearchParams(params as Record<string, string>).toString();
    return publicFetch<Event[]>(`/api/events${qs ? `?${qs}` : ""}`);
  },
  getForGroup: (groupId: string) => publicFetch<Event[]>(`/api/groups/${groupId}/events`),
  getById: (id: string) => publicFetch<Event>(`/api/events/${id}`),
  create: (groupId: string, data: Partial<Event>) =>
    apiFetch<Event>(`/api/groups/${groupId}/events`, {
      method: "POST",
      body: JSON.stringify(data),
    }),
  update: (groupId: string, eventId: string, data: Partial<Event>) =>
    apiFetch<Event>(`/api/groups/${groupId}/events/${eventId}`, {
      method: "PUT",
      body: JSON.stringify(data),
    }),
  delete: (groupId: string, eventId: string) =>
    apiFetch<void>(`/api/groups/${groupId}/events/${eventId}`, { method: "DELETE" }),
};

// ─── Event RSVPs ──────────────────────────────────────────────────────────────

export const rsvpApi = {
  upsert: (eventId: string, status: string) =>
    apiFetch<EventRsvp>(`/api/events/${eventId}/rsvp`, {
      method: "POST",
      body: JSON.stringify({ status }),
    }),
  remove: (eventId: string) =>
    apiFetch<void>(`/api/events/${eventId}/rsvp`, { method: "DELETE" }),
  getAll: (eventId: string) => apiFetch<EventRsvp[]>(`/api/events/${eventId}/rsvp/all`),
};

// ─── Posts ────────────────────────────────────────────────────────────────────

export const postApi = {
  getForGroup: (groupId: string) => apiFetch<Post[]>(`/api/groups/${groupId}/posts`),
  getById: (id: string) => apiFetch<Post>(`/api/posts/${id}`),
  create: (groupId: string, data: {
    title: string; body?: string; type?: string; visibility?: string;
    termId?: string; mediaUrls?: string[];
  }) =>
    apiFetch<Post>(`/api/groups/${groupId}/posts`, {
      method: "POST",
      body: JSON.stringify(data),
    }),
  update: (id: string, data: Partial<Post>) =>
    apiFetch<Post>(`/api/posts/${id}`, { method: "PUT", body: JSON.stringify(data) }),
  delete: (id: string) => apiFetch<void>(`/api/posts/${id}`, { method: "DELETE" }),
};

// ─── Comments ─────────────────────────────────────────────────────────────────

export const commentApi = {
  getForPost: (postId: string) => apiFetch<Comment[]>(`/api/posts/${postId}/comments`),
  create: (postId: string, body: string) =>
    apiFetch<Comment>(`/api/posts/${postId}/comments`, {
      method: "POST",
      body: JSON.stringify({ body }),
    }),
  delete: (id: string) => apiFetch<void>(`/api/comments/${id}`, { method: "DELETE" }),
};

// ─── Notifications ────────────────────────────────────────────────────────────

export const notificationApi = {
  getMine: () => apiFetch<Notification[]>("/api/users/me/notifications"),
  markRead: (id: string) =>
    apiFetch<Notification>(`/api/notifications/${id}/read`, { method: "PATCH" }),
  markAllRead: () =>
    apiFetch<void>("/api/users/me/notifications/read-all", { method: "PATCH" }),
};

// ─── Reports ──────────────────────────────────────────────────────────────────

export const reportApi = {
  create: (data: { targetType: string; targetId: string; reason: string; details?: string }) =>
    apiFetch<Report>("/api/reports", { method: "POST", body: JSON.stringify(data) }),
  getOpen: () => apiFetch<Report[]>("/api/reports"),
};

// ─── Role Assignments ─────────────────────────────────────────────────────────

export const roleApi = {
  getForGroup: (groupId: string) =>
    apiFetch<RoleAssignment[]>(`/api/groups/${groupId}/roles`),
  create: (groupId: string, data: {
    userId: string; permissionRole: string; displayRole?: string;
    startDate?: string; endDate?: string; termId?: string;
  }) =>
    apiFetch<RoleAssignment>(`/api/groups/${groupId}/roles`, {
      method: "POST",
      body: JSON.stringify(data),
    }),
  delete: (groupId: string, roleId: string) =>
    apiFetch<void>(`/api/groups/${groupId}/roles/${roleId}`, { method: "DELETE" }),
};
