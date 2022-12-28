using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
            await _restClient.GetAsync(request);
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
        VerifyRestClientSpecified();
        if (string.IsNullOrEmpty(accountName))
            throw new ApplicationException("Please specify the account name");

        var request = new RestRequest($"accounts/lookup?acct={accountName}");
        var response = await _restClient.GetAsync(request);
        CheckForNullContent(response.Content, "Lookup account");

        Debug.Assert(response.Content != null, "response.Content != null");
        var account = JsonConvert.DeserializeObject<MastodonId>(response.Content);
        return account?.Id ??
               throw new ApplicationException(
                   "Couldn't get the account details. Are you sure you entered the account name correctly?");
    }

    private void CheckForNullContent(string? content, string apiMethodName)
    {
        if (content == null)
            throw new InvalidOperationException($"{apiMethodName} API method returned nothing");
    }

    private void VerifyRestClientSpecified()
    {
        if (_restClient == null)
            throw new InvalidOperationException(
                $"You must call {nameof(VerifyCredentials)} before calling this method");
    }

    public async Task<List<MastodonId>> GetFollowerIdsForAccountId(string accountId)
    {
        VerifyRestClientSpecified();

        var request = new RestRequest($"accounts/{accountId}/followers?limit=80");
        var response = await _restClient.GetAsync(request);
        CheckForNullContent(response.Content, "Lookup followers");

        Debug.Assert(response.Content != null, "response.Content != null");
        var followerIds = JsonConvert.DeserializeObject<List<MastodonId>>(response.Content);
        return followerIds ?? throw new ApplicationException(
            "Couldn't get the list of followers. Are you sure you entered the account has followers?");
    }

    public async Task<List<MastodonStatus>> GetStatusesForFollowerId(string accountId)
    {
        VerifyRestClientSpecified();
        
        var request = new RestRequest($"accounts/{accountId}/statuses?limit=40");
        var response = await _restClient.GetAsync(request);
        CheckForNullContent(response.Content, $"Get statuses for follower {accountId}");

        Debug.Assert(response.Content != null, "response.Content != null");
        var statuses = JsonConvert.DeserializeObject<List<MastodonStatus>>(response.Content);
        return statuses ?? new List<MastodonStatus>();
    }
}