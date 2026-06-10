CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260610220943_InitialSchema') THEN
    CREATE TABLE users (
        "Id" uuid NOT NULL,
        "EntraOid" character varying(128) NOT NULL,
        "Email" character varying(255) NOT NULL,
        "DisplayName" character varying(255),
        "AvatarUrl" text,
        "Role" character varying(32) NOT NULL DEFAULT 'student',
        "Status" character varying(32) NOT NULL DEFAULT 'active',
        "CreatedAt" timestamp with time zone NOT NULL,
        "UpdatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_users" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260610220943_InitialSchema') THEN
    CREATE TABLE groups (
        "Id" uuid NOT NULL,
        "Slug" character varying(128) NOT NULL,
        "Name" character varying(200) NOT NULL,
        "Description" text,
        "Category" character varying(100),
        "LogoUrl" text,
        "BannerUrl" text,
        "ContactEmail" character varying(255),
        "ContactInfo" text,
        "Status" character varying(32) NOT NULL DEFAULT 'pending',
        "CreatedByUserId" uuid,
        "CreatedAt" timestamp with time zone NOT NULL,
        "UpdatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_groups" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_groups_users_CreatedByUserId" FOREIGN KEY ("CreatedByUserId") REFERENCES users ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260610220943_InitialSchema') THEN
    CREATE TABLE notifications (
        "Id" uuid NOT NULL,
        "UserId" uuid NOT NULL,
        "Kind" character varying(64) NOT NULL,
        "Title" character varying(255) NOT NULL,
        "Body" text,
        "Href" text,
        "Read" boolean NOT NULL DEFAULT FALSE,
        "ReferenceId" uuid,
        "ReferenceType" character varying(64),
        "CreatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_notifications" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_notifications_users_UserId" FOREIGN KEY ("UserId") REFERENCES users ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260610220943_InitialSchema') THEN
    CREATE TABLE reports (
        "Id" uuid NOT NULL,
        "ReportedByUserId" uuid NOT NULL,
        "TargetType" character varying(64) NOT NULL,
        "TargetId" uuid NOT NULL,
        "Reason" character varying(255) NOT NULL,
        "Details" text,
        "Status" character varying(32) NOT NULL DEFAULT 'open',
        "ReviewedByUserId" uuid,
        "ReviewedAt" timestamp with time zone,
        "CreatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_reports" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_reports_users_ReportedByUserId" FOREIGN KEY ("ReportedByUserId") REFERENCES users ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_reports_users_ReviewedByUserId" FOREIGN KEY ("ReviewedByUserId") REFERENCES users ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260610220943_InitialSchema') THEN
    CREATE TABLE events (
        "Id" uuid NOT NULL,
        "GroupId" uuid NOT NULL,
        "CreatedByUserId" uuid NOT NULL,
        "Title" character varying(200) NOT NULL,
        "Description" text,
        "Location" character varying(300),
        "BannerUrl" text,
        "StartAt" timestamp with time zone NOT NULL,
        "EndAt" timestamp with time zone NOT NULL,
        "Timezone" character varying(64) NOT NULL DEFAULT 'America/Monterrey',
        "Capacity" integer,
        "Status" character varying(32) NOT NULL DEFAULT 'published',
        "Visibility" character varying(32) NOT NULL DEFAULT 'public',
        "CreatedAt" timestamp with time zone NOT NULL,
        "UpdatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_events" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_events_groups_GroupId" FOREIGN KEY ("GroupId") REFERENCES groups ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_events_users_CreatedByUserId" FOREIGN KEY ("CreatedByUserId") REFERENCES users ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260610220943_InitialSchema') THEN
    CREATE TABLE group_registration_requests (
        "Id" uuid NOT NULL,
        "RequestedByUserId" uuid NOT NULL,
        "ProposedGroupName" character varying(200) NOT NULL,
        "ProposedDescription" text,
        "ContactEmail" character varying(255) NOT NULL,
        "Status" character varying(32) NOT NULL DEFAULT 'pending',
        "DecisionNotes" text,
        "ReviewedByUserId" uuid,
        "ReviewedAt" timestamp with time zone,
        "CreatedGroupId" uuid,
        "CreatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_group_registration_requests" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_group_registration_requests_groups_CreatedGroupId" FOREIGN KEY ("CreatedGroupId") REFERENCES groups ("Id"),
        CONSTRAINT "FK_group_registration_requests_users_RequestedByUserId" FOREIGN KEY ("RequestedByUserId") REFERENCES users ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_group_registration_requests_users_ReviewedByUserId" FOREIGN KEY ("ReviewedByUserId") REFERENCES users ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260610220943_InitialSchema') THEN
    CREATE TABLE memberships (
        "Id" uuid NOT NULL,
        "UserId" uuid NOT NULL,
        "GroupId" uuid NOT NULL,
        "Status" character varying(32) NOT NULL DEFAULT 'pending',
        "Notes" text,
        "RequestedAt" timestamp with time zone NOT NULL,
        "RespondedAt" timestamp with time zone,
        CONSTRAINT "PK_memberships" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_memberships_groups_GroupId" FOREIGN KEY ("GroupId") REFERENCES groups ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_memberships_users_UserId" FOREIGN KEY ("UserId") REFERENCES users ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260610220943_InitialSchema') THEN
    CREATE TABLE role_assignments (
        "Id" uuid NOT NULL,
        "GroupId" uuid NOT NULL,
        "UserId" uuid NOT NULL,
        "PermissionRole" character varying(32) NOT NULL,
        "DisplayRole" character varying(100),
        "StartDate" date,
        "EndDate" date,
        "CreatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_role_assignments" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_role_assignments_groups_GroupId" FOREIGN KEY ("GroupId") REFERENCES groups ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_role_assignments_users_UserId" FOREIGN KEY ("UserId") REFERENCES users ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260610220943_InitialSchema') THEN
    CREATE TABLE event_participations (
        "Id" uuid NOT NULL,
        "EventId" uuid NOT NULL,
        "UserId" uuid NOT NULL,
        "Status" character varying(32) NOT NULL DEFAULT 'going',
        "RegisteredAt" timestamp with time zone NOT NULL,
        "Attended" boolean,
        CONSTRAINT "PK_event_participations" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_event_participations_events_EventId" FOREIGN KEY ("EventId") REFERENCES events ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_event_participations_users_UserId" FOREIGN KEY ("UserId") REFERENCES users ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260610220943_InitialSchema') THEN
    CREATE UNIQUE INDEX "IX_event_participations_EventId_UserId" ON event_participations ("EventId", "UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260610220943_InitialSchema') THEN
    CREATE INDEX "IX_event_participations_UserId" ON event_participations ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260610220943_InitialSchema') THEN
    CREATE INDEX "IX_events_CreatedByUserId" ON events ("CreatedByUserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260610220943_InitialSchema') THEN
    CREATE INDEX "IX_events_GroupId" ON events ("GroupId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260610220943_InitialSchema') THEN
    CREATE INDEX "IX_group_registration_requests_CreatedGroupId" ON group_registration_requests ("CreatedGroupId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260610220943_InitialSchema') THEN
    CREATE INDEX "IX_group_registration_requests_RequestedByUserId" ON group_registration_requests ("RequestedByUserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260610220943_InitialSchema') THEN
    CREATE INDEX "IX_group_registration_requests_ReviewedByUserId" ON group_registration_requests ("ReviewedByUserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260610220943_InitialSchema') THEN
    CREATE INDEX "IX_groups_CreatedByUserId" ON groups ("CreatedByUserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260610220943_InitialSchema') THEN
    CREATE UNIQUE INDEX "IX_groups_Slug" ON groups ("Slug");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260610220943_InitialSchema') THEN
    CREATE INDEX "IX_memberships_GroupId" ON memberships ("GroupId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260610220943_InitialSchema') THEN
    CREATE UNIQUE INDEX "IX_memberships_UserId_GroupId" ON memberships ("UserId", "GroupId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260610220943_InitialSchema') THEN
    CREATE INDEX "IX_notifications_UserId" ON notifications ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260610220943_InitialSchema') THEN
    CREATE INDEX "IX_reports_ReportedByUserId" ON reports ("ReportedByUserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260610220943_InitialSchema') THEN
    CREATE INDEX "IX_reports_ReviewedByUserId" ON reports ("ReviewedByUserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260610220943_InitialSchema') THEN
    CREATE INDEX "IX_role_assignments_GroupId" ON role_assignments ("GroupId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260610220943_InitialSchema') THEN
    CREATE INDEX "IX_role_assignments_UserId" ON role_assignments ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260610220943_InitialSchema') THEN
    CREATE UNIQUE INDEX "IX_users_Email" ON users ("Email");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260610220943_InitialSchema') THEN
    CREATE UNIQUE INDEX "IX_users_EntraOid" ON users ("EntraOid");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260610220943_InitialSchema') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260610220943_InitialSchema', '10.0.9');
    END IF;
END $EF$;
COMMIT;

