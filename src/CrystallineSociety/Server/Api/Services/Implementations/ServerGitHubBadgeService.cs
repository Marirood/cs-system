﻿using CrystallineSociety.Shared.Dtos.BadgeSystem;
using Octokit;

namespace CrystallineSociety.Server.Api.Services.Implementations
{
    public class ServerGitHubBadgeService : IGitHubBadgeService
    {
        public Task<List<BadgeDto>> GetBadgesAsync(string url)
        {
            throw new NotImplementedException();
        }

        public async Task<BadgeDto> GetBadgeAsync(string url)
        {
            throw new NotImplementedException();
            //var client = new GitHubClient(new ProductHeaderValue("CS-System"));
            //var repos =  await client.Repository.GetAllForOrg("cs-internship");
            //var repo = repos.First(r => r.Name == "cs-system");

            //var refs= await client.Git.Reference.GetAll(repo.Id);

            //var main = refs.First(r => r.Ref.Contains("refs/heads/main"));
            ////var main = refs.First(r => r.Ref.Contains("refs/heads/main"));
            //var refx =
            //    "refs/heads/main/src/CrystallineSociety/Shared/CrystallineSociety.Shared.Test/BadgeSystem/SampleBadgeDocs/serialization-badge-sample";
            //var xx = await client.Git.Tree.GetRecursive(repo.Id, main.Ref);

            //return default;
        }
    }
}
