﻿using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using CrystallineSociety.Shared.Dtos.BadgeSystem;
using Octokit;

namespace CrystallineSociety.Server.Api.Services.Implementations
{
    public partial class ServerGitHubBadgeService : IGitHubBadgeService
    {
        [AutoInject] public IBadgeUtilService BadgeUtilService { get; set; }

        public async Task<List<BadgeDto>> GetBadgesAsync(string url)
        {
            var client = new GitHubClient(new ProductHeaderValue("CS-System"));
            var repos = await client.Repository.GetAllForOrg("cs-internship");
            var repo = repos.First(r => r.Name == "cs-system");
            var lastSegment = GetLastSegmentFromUrl(url, out var parentFolderPath);
            var folderContents = await client.Repository.Content.GetAllContents(repo.Id, parentFolderPath);
            var destinationFolderSha = folderContents?.First(f => f.Name == lastSegment).Sha;
            var destinationFolderContents = await client.Git.Tree.GetRecursive(repo.Id, destinationFolderSha);

            var badges = new List<BadgeDto>();

            foreach (var item in destinationFolderContents.Tree)
            {
                if (Path.GetExtension(item.Path) != ".json")
                    continue;

                var badgeBlob = await client.Git.Blob.Get(repo.Id, item.Sha);

                if (badgeBlob.Encoding != EncodingType.Base64)
                    continue;

                var bytes = Convert.FromBase64String(badgeBlob.Content);
                var badgeContent = Encoding.UTF8.GetString(bytes);
                badges.Add(BadgeUtilService.ParseBadge(badgeContent));
            }

            return badges;
        }

        public async Task<BadgeDto> GetBadgeAsync(string url)
        {
            var client = new GitHubClient(new ProductHeaderValue("CS-System"));

            var (orgName, repoName) = GetRepoAndOrgNameFromUrl(url);

            var repo = await client.Repository.Get(orgName, repoName);

            var refs = await client.Git.Reference.GetAll(repo.Id);
            var branchName = GetBranchNameFromUrl(url, refs) ??
                             throw new ResourceNotFoundException($"Unable to locate branchName: {url}");
            var branchRef = refs.First(r => r.Ref.Contains($"refs/heads/{branchName}"));
            var folderPath = GetRelativeFolderPath(url);
            var folderContents =
                await client.Repository.Content.GetAllContentsByRef(repo.Id, folderPath, branchRef.Ref);

            var badgeFilePath =
                folderContents.FirstOrDefault(x => x.Name.EndsWith("-badge.json"))?.Path
                ?? throw new FileNotFoundException($"Badge file not found in: {url}");
            var contents = await client.Repository.Content.GetAllContentsByRef(repo.Id, badgeFilePath, branchRef.Ref);
            var badgeFile = contents!.First();
            
            var badgeFileContent = badgeFile.Content;
            
            try
            {
                var badge = BadgeUtilService.ParseBadge(badgeFileContent);
                return badge;
            }
            catch (Exception exception)
            {
                throw new FormatException($"Can not parse badge with url: '{url}'", exception);
            }

        }

        private static string GetRelativeFolderPath(string url)
        {
            var urlSrcIndex = url.IndexOf("src", StringComparison.Ordinal);
            var folderPath = url[urlSrcIndex..];
            return folderPath;
        }

        private static string? GetBranchNameFromUrl(string url, IReadOnlyList<Reference> refs)
        {
            var uri = new Uri(url);
            var afterTreeSegments = String.Join("", uri.Segments[4..]);
            foreach (var reference in refs)
            {

                var branchInRefWithEndingSlash = $"{Regex.Replace(reference.Ref, @"^[^/]+/[^/]+/", "")}/";
                if (afterTreeSegments.StartsWith(branchInRefWithEndingSlash))
                {
                    return branchInRefWithEndingSlash.TrimEnd('/');
                }
            }

            return null;
        }

        private static string GetLastSegmentFromUrl(string url, out string parentFolderPath)
        {
            var uri = new Uri(url);
            var lastSegment = uri.Segments.Last().TrimEnd('/');
            var parentFolderUrl = uri.GetLeftPart(UriPartial.Authority) +
                                  string.Join("", uri.Segments.Take(uri.Segments.Length - 1));
            var urlSrcIndex = parentFolderUrl.IndexOf("src", StringComparison.Ordinal);
            parentFolderPath = parentFolderUrl[urlSrcIndex..];

            return lastSegment;
        }

        private static (string org, string repo) GetRepoAndOrgNameFromUrl(string url)
        {
            var uri = new Uri(url);
            var segments = uri.Segments;
            var org = segments[1].TrimEnd('/');
            var repo = segments[2].TrimEnd('/');

            return (org, repo);
        }
    }
}