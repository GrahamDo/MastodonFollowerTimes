using System.Collections.Generic;
using System.Threading.Tasks;

namespace MastodonFollowerTimes;

internal class ApiClient
{
    public async Task VerifyCredentials(string instanceUrl, string token)
    {
        throw new System.NotImplementedException();
    }

    public async Task<string> GetIdForAccountName(string accountName)
    {
        throw new System.NotImplementedException();
    }

    public async Task<List<string>> GetFollowerIdsForAccountId(string accountId)
    {
        throw new System.NotImplementedException();
    }

    public async Task<List<MastodonStatus>> GetStatusesForFollowerId(string followerId)
    {
        throw new System.NotImplementedException();
    }
}