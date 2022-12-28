using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators.OAuth2;

namespace MastodonFollowerTimes;

internal class ApiClient
{
    private RestClient _restClient;

    public async Task VerifyCredentials(string instanceUrl, string token)
    {
        if (string.IsNullOrEmpty(instanceUrl))
            throw new ApplicationException("Please specify the instance URL");
        if (string.IsNullOrEmpty(token))
            throw new ApplicationException("Please specify the token");

        var baseUrl = BuildBaseUrl(instanceUrl);
        _restClient = new RestClient(baseUrl)
        {
            Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(token, "Bearer")
        };
        
        var request = new RestRequest($"apps/verify_credentials");
        try
        {
            var response = await _restClient.GetAsync(request);
        }
        catch (HttpRequestException ex)
        {
            if (ex.Message.Contains("Unauthorized"))
                throw new ApplicationException("The token you entered was not valid");

            throw;
        }
    }

    private string BuildBaseUrl(string instanceUrl)
    {
        var baseUrlSb = new StringBuilder();
        if (!instanceUrl.StartsWith("https://"))
            baseUrlSb.Append("https://");
        baseUrlSb.Append(instanceUrl);
        if (!instanceUrl.EndsWith("/"))
            baseUrlSb.Append("/");
        baseUrlSb.Append("api/v1/");
        return baseUrlSb.ToString();
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