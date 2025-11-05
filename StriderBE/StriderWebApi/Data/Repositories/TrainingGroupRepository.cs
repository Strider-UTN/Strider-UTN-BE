using Google;
using Microsoft.EntityFrameworkCore;
using StriderWebApi.Data.Repositories.Interfaces;
using StriderWebApi.Domain.DomainClasses;
using StriderWebApi.Domain.Enums;

namespace StriderWebApi.Data.Repositories
{
    /// <summary>
    /// Implementación del repositorio para TrainingGroup
    /// </summary>
    public class TrainingGroupRepository(StriderDbContext context) : ITrainingGroupRepository
    {
        public async Task<TrainingGroup?> GetByIdAsync(int id)
        {
            return await context.TrainingGroups
                .Include(tg => tg.TrainingPoints)
                .Include(tg => tg.Members)
                    .ThenInclude(m => m.User)
                .Include(tg => tg.CreatedBy)
                .Include(tg => tg.Notifications)
                .FirstOrDefaultAsync(tg => tg.Id == id);
        }

        public async Task<IEnumerable<TrainingGroup>> GetByCoachIdAsync(int coachId)
        {
            return await context.TrainingGroups
                .Include(tg => tg.TrainingPoints)
                .Include(tg => tg.Members)
                    .ThenInclude(m => m.User)
                .Include(tg => tg.CreatedBy)
                .Include(tg => tg.Notifications)
                .Where(tg => tg.CreatedByUserId == coachId)
                .OrderByDescending(tg => tg.CreatedDate)
                .ToListAsync();
        }

        public async Task<TrainingGroup> CreateAsync(TrainingGroup trainingGroup)
        {
            await context.TrainingGroups.AddAsync(trainingGroup);
            await context.SaveChangesAsync();
            return trainingGroup;
        }

        public async Task<TrainingGroup> UpdateAsync(TrainingGroup trainingGroup)
        {
            trainingGroup.UpdatedAt = DateTime.UtcNow;
            context.TrainingGroups.Update(trainingGroup);
            await context.SaveChangesAsync();
            return trainingGroup;
        }

        public async Task DeleteAsync(int id)
        {
            var trainingGroup = await context.TrainingGroups
                .Include(tg => tg.TrainingPoints)
                .Include(tg => tg.Members)
                .FirstOrDefaultAsync(tg => tg.Id == id);

            if (trainingGroup != null)
            {
                context.TrainingGroups.Remove(trainingGroup);
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await context.TrainingGroups.AnyAsync(tg => tg.Id == id);
        }

        public async Task<IEnumerable<TrainingGroupMember>> GetMembersByGroupIdAsync(int groupId)
        {
            return await context.TrainingGroupMembers
                .Include(m => m.User)
                .Include(m => m.TrainingGroup)
                .Where(m => m.TrainingGroupId == groupId)
                .OrderBy(m => m.JoinedDate)
                .ToListAsync();
        }

        public async Task<int> CountActiveMembersAsync(int groupId)
        {
            return await context.TrainingGroupMembers
                .CountAsync(m => m.TrainingGroupId == groupId &&
                                m.Status == TrainingGroupMemberStatus.Active);
        }

        public async Task<TrainingGroupMember?> CreateMemberInvitationAsync(TrainingGroupMember member)
        {
            context.TrainingGroupMembers.Add(member);
            await context.SaveChangesAsync();
            return await GetMemberInvitationByIdAsync(member.Id);
        }

        public async Task<TrainingGroupMember?> GetMemberInvitationByIdAsync(int invitationId)
        {
            return await context.TrainingGroupMembers
                .Include(m => m.TrainingGroup)
                    .ThenInclude(g => g.CreatedBy)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == invitationId);
        }

        public async Task<IEnumerable<TrainingGroupMember>> GetPendingInvitationsByAthleteIdAsync(int athleteId)
        {
            return await context.TrainingGroupMembers
                .Include(m => m.TrainingGroup)
                    .ThenInclude(g => g.CreatedBy)
                .Include(m => m.User)
                .Where(m => m.UserId == athleteId && m.Status == TrainingGroupMemberStatus.Pending)
                .OrderByDescending(m => m.JoinedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<TrainingGroupMember>> GetActiveGroupsByAthleteIdAsync(int athleteId)
        {
            return await context.TrainingGroupMembers
                .Include(m => m.TrainingGroup)
                    .ThenInclude(g => g.CreatedBy)
                .Include(m => m.TrainingGroup)
                    .ThenInclude(g => g.TrainingPoints)
                .Include(m => m.User)
                .Where(m => m.UserId == athleteId && m.Status == TrainingGroupMemberStatus.Active)
                .OrderByDescending(m => m.JoinedDate)
                .ToListAsync();
        }

        public async Task<bool> ExistsMemberRelationshipAsync(int trainingGroupId, int athleteId)
        {
            return await context.TrainingGroupMembers
                .AnyAsync(m => m.TrainingGroupId == trainingGroupId && m.UserId == athleteId);
        }

        public async Task<TrainingGroupMember?> UpdateMemberInvitationStatusAsync(int invitationId, TrainingGroupMemberStatus status)
        {
            var member = await context.TrainingGroupMembers
                .FirstOrDefaultAsync(m => m.Id == invitationId);

            if (member == null)
                return null;

            member.Status = status;
            await context.SaveChangesAsync();

            return await GetMemberInvitationByIdAsync(invitationId);
        }

        public async Task<bool> RemoveMemberAsync(int memberId)
        {
            var member = await context.TrainingGroupMembers
                .FirstOrDefaultAsync(m => m.Id == memberId);

            if (member == null)
                return false;

            context.TrainingGroupMembers.Remove(member);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<TrainingGroupMember?> GetMemberByGroupAndUserAsync(int trainingGroupId, int userId)
        {
            return await context.TrainingGroupMembers
                .Include(m => m.TrainingGroup)
                    .ThenInclude(g => g.CreatedBy)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.TrainingGroupId == trainingGroupId && m.UserId == userId);
        }
    }
}
