using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

namespace MastodonFollowerTimes
{
    internal class GitHubApiClient
    {
        public async Task<bool> IsNewVersionAvailable(string currentVersion)
        {
            var shortVersion = Convert.ToInt16(currentVersion.Replace(".", ""));
            var client = new RestClient("https://api.github.com/");
            var request = new RestRequest("repos/GrahamDo/MastodonFollowerTimes/releases?per_page=5");
            // Get top 5, in case there are drafts or pre-releases, which we need to exclude

            var response = await client.GetAsync(request);
            var releases = JsonConvert.DeserializeObject<List<GitHubRelease>>(response.Content);
            if (releases == null || !releases.Any())
                return false;

            var latestActiveRelease = releases.FirstOrDefault(x => x is { IsDraft: false, IsPreRelease: false });
            if (latestActiveRelease == null)
                return false;

            var releaseVersion = Convert.ToInt16(latestActiveRelease.TagName.Replace(".", ""));
            return releaseVersion > shortVersion;
        }
    }
}
