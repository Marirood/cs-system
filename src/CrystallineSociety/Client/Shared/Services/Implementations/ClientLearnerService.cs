﻿using CrystallineSociety.Shared.Dtos.BadgeSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystallineSociety.Client.Shared.Services.Implementations
{
    public class ClientLearnerService : ILearnerService
    {
        public Task<List<BadgeDto>> GetLearnerBadgesAsync(LearnerDto learner)
        {
            throw new NotImplementedException();
        }

        public Task<LearnerDto> GetLearnerByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<LearnerDto> GetLearnerByUsernameAsync(string username)
        {
            throw new NotImplementedException();
        }

        public IQueryable<LearnerDto> GetLearners()
        {
            throw new NotImplementedException();
        }

        public Task<List<LearnerDto>> GetLearnersHavingBadgeAsync(params BadgeCountDto[] requiredEarnedBadges)
        {
            throw new NotImplementedException();
        }

        public IQueryable<BadgeDto> GetLearnerBadges(LearnerDto learner)
        {
            throw new NotImplementedException();
        }
    }
}
