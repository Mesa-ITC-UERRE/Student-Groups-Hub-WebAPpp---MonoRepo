// ─── Users ───────────────────────────────────────────────────────────────────

export interface User {
  id: string;
  entraOid: string;
  email: string;
  displayName: string | null;
  avatarUrl: string | null;
  role: "student" | "group_leader" | "admin";
  isPlatformAdmin: boolean;
  status: string;
  createdAt: string;
  updatedAt: string;
}

// ─── Groups ───────────────────────────────────────────────────────────────────

export interface Group {
  id: string;
  slug: string;
  name: string;
  description: string | null;
  logoUrl: string | null;
  active: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface GroupMember {
  membershipId: string;
  userId: string;
  email: string;
  displayName: string | null;
  avatarUrl: string | null;
  status: string;
  joinedAt: string | null;
}

export interface PendingMember {
  membershipId: string;
  userId: string;
  email: string;
  displayName: string | null;
  avatarUrl: string | null;
  requestedAt: string;
}

export interface JoinGroupResponse {
  membershipId: string;
  groupId: string;
  userId: string;
  status: string;
  requestedAt: string;
}

// ─── Group Registration Requests ─────────────────────────────────────────────

export interface GroupRegistrationRequest {
  id: string;
  requestedByUserId: string;
  proposedGroupName: string;
  proposedDescription: string | null;
  contactEmail: string;
  status: string;
  createdAt: string;
  reviewedAt: string | null;
  decisionNotes: string | null;
}

// ─── Group Terms ──────────────────────────────────────────────────────────────

export interface GroupTerm {
  id: string;
  groupId: string;
  label: string;
  startDate: string;
  endDate: string;
  status: string;
  createdAt: string;
  updatedAt: string;
}

// ─── Events ───────────────────────────────────────────────────────────────────

export interface Event {
  id: string;
  groupId: string;
  groupName: string;
  createdByUserId: string;
  createdByDisplayName: string | null;
  termId: string | null;
  title: string;
  description: string | null;
  location: string | null;
  startAt: string;
  endAt: string;
  timezone: string;
  status: string;
  visibility: string;
  createdAt: string;
  updatedAt: string;
}

export interface EventRsvp {
  id: string;
  eventId: string;
  userId: string;
  userDisplayName: string | null;
  status: string;
  createdAt: string;
}

// ─── Posts ────────────────────────────────────────────────────────────────────

export interface Post {
  id: string;
  groupId: string;
  authorId: string;
  authorDisplayName: string | null;
  authorAvatarUrl: string | null;
  termId: string | null;
  type: string;
  title: string;
  body: string | null;
  pinned: boolean;
  pinnedAt: string | null;
  visibility: string;
  mediaUrls: string[];
  commentCount: number;
  createdAt: string;
  updatedAt: string;
}

// ─── Comments ─────────────────────────────────────────────────────────────────

export interface Comment {
  id: string;
  postId: string;
  authorId: string;
  authorDisplayName: string | null;
  authorAvatarUrl: string | null;
  body: string;
  createdAt: string;
  updatedAt: string;
}

// ─── Notifications ────────────────────────────────────────────────────────────

export interface Notification {
  id: string;
  kind: string;
  title: string;
  body: string | null;
  href: string | null;
  read: boolean;
  createdAt: string;
}

// ─── Reports ──────────────────────────────────────────────────────────────────

export interface Report {
  id: string;
  reportedByUserId: string;
  targetType: string;
  targetId: string;
  reason: string;
  details: string | null;
  status: string;
  createdAt: string;
  reviewedAt: string | null;
}

// ─── Role Assignments ─────────────────────────────────────────────────────────

export interface RoleAssignment {
  id: string;
  groupId: string;
  termId: string | null;
  userId: string;
  userDisplayName: string | null;
  userEmail: string;
  permissionRole: string;
  displayRole: string | null;
  startDate: string | null;
  endDate: string | null;
  createdAt: string;
}

// ─── API Error ────────────────────────────────────────────────────────────────

export interface ApiError {
  status: number;
  message: string;
  timestamp: string;
}
