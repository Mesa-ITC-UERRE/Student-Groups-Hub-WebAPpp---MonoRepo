using Microsoft.EntityFrameworkCore;
using StudentGroupsHub.Data;
using StudentGroupsHub.DTOs.Responses;
using StudentGroupsHub.Models;

namespace StudentGroupsHub.Services;

public class GroupRegistrationRequestService(AppDbContext db, GroupService groupService)
{
    public async Task<GroupRegistrationRequest> CreateAsync(
        Guid requestedByUserId, string proposedGroupName,
        string? proposedDescription, string contactEmail)
    {
        var req = new GroupRegistrationRequest
        {
            Id = Guid.NewGuid(),
            RequestedByUserId = requestedByUserId,
            ProposedGroupName = proposedGroupName,
            ProposedDescription = proposedDescription,
            ContactEmail = contactEmail,
            Status = "pending",
            CreatedAt = DateTime.UtcNow,
        };
        db.GroupRegistrationRequests.Add(req);
        await db.SaveChangesAsync();
        return req;
    }

    public async Task<List<GroupRegistrationRequest>> GetByUserAsync(Guid userId)
        => await db.GroupRegistrationRequests
            .Where(r => r.RequestedByUserId == userId)
            .Include(r => r.RequestedBy)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    public async Task<List<GroupRegistrationRequest>> GetAllPendingAsync()
        => await db.GroupRegistrationRequests
            .Where(r => r.Status == "pending")
            .Include(r => r.RequestedBy)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync();

    public async Task<GroupRegistrationRequest?> GetByIdAsync(Guid id)
        => await db.GroupRegistrationRequests
            .Include(r => r.RequestedBy)
            .FirstOrDefaultAsync(r => r.Id == id);

    public async Task<GroupRegistrationRequest?> ApproveAsync(
        Guid requestId, Guid reviewedByUserId, string? decisionNotes)
    {
        var req = await db.GroupRegistrationRequests
            .Include(r => r.RequestedBy)
            .FirstOrDefaultAsync(r => r.Id == requestId);

        if (req is null || req.Status != "pending") return null;

        // Create the group
        var group = await groupService.CreateAsync(
            req.ProposedGroupName,
            req.ProposedDescription,
            null, // category assigned later by leader
            req.ContactEmail,
            req.RequestedByUserId
        );

        // Assign the requester as leader
        db.RoleAssignments.Add(new RoleAssignment
        {
            Id = Guid.NewGuid(),
            GroupId = group.Id,
            UserId = req.RequestedByUserId,
            PermissionRole = "leader",
            DisplayRole = "Líder",
            CreatedAt = DateTime.UtcNow,
        });

        // Promote user to group_leader role if they are still a student
        var user = await db.Users.FindAsync(req.RequestedByUserId);
        if (user is not null && user.Role == "student")
        {
            user.Role = "group_leader";
            user.UpdatedAt = DateTime.UtcNow;
        }

        // Update the request
        req.Status = "approved";
        req.DecisionNotes = decisionNotes;
        req.ReviewedByUserId = reviewedByUserId;
        req.ReviewedAt = DateTime.UtcNow;
        req.CreatedGroupId = group.Id;

        await db.SaveChangesAsync();
        return req;
    }

    public async Task<GroupRegistrationRequest?> RejectAsync(
        Guid requestId, Guid reviewedByUserId, string? decisionNotes)
    {
        var req = await db.GroupRegistrationRequests
            .FirstOrDefaultAsync(r => r.Id == requestId && r.Status == "pending");

        if (req is null) return null;

        req.Status = "rejected";
        req.DecisionNotes = decisionNotes;
        req.ReviewedByUserId = reviewedByUserId;
        req.ReviewedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return req;
    }

    public static GroupRegistrationRequestResponse ToResponse(GroupRegistrationRequest r) => new(
        r.Id,
        r.RequestedByUserId,
        r.RequestedBy?.DisplayName ?? r.RequestedBy?.Email,
        r.ProposedGroupName,
        r.ProposedDescription,
        r.ContactEmail,
        r.Status,
        r.DecisionNotes,
        r.CreatedAt,
        r.ReviewedAt
    );
}
