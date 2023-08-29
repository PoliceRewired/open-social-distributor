using DistributorLib.Post;
using DistributorLib.Post.Formatters;
using RestSharp;

namespace DistributorLib.Network.Implementations;

// Get a page access token: https://developers.facebook.com/docs/pages/access-tokens#get-a-page-access-token
// Publish a page post: https://developers.facebook.com/docs/pages/publishing#publish-a-page-post

public class FacebookNetwork : AbstractNetwork
{
    private static string FB_URL = "https://graph.facebook.com";

    public enum Mode { Page, User }

    public string? pageId, pageToken;
    public string? userToken;
    public Mode mode;

    private RestClient? graphClient;
    
    public FacebookNetwork(string code, Mode mode, 
        string? userToken = null, 
        string? pageId = null, string? pageToken = null) 
            : base(NetworkType.Facebook, code, "facebook", PostFormatVariantFactory.Facebook)
    {
        this.mode = mode;
        this.pageToken = pageToken;
        this.pageId = pageId;
        this.userToken = userToken;
    }

    protected override async Task InitClientAsync()
    {
        graphClient = new RestClient(FB_URL);

        if (userToken == null && pageToken == null)
        {
            throw new ArgumentException("Cannot initialise FacebookNetwork without a user or page access token");
        }

        if (mode == Mode.Page && pageToken == null) 
        {
            // TODO: get the page access token using the user access token
            // See: https://developers.facebook.com/docs/pages/access-tokens#get-a-page-access-token
            throw new NotImplementedException("TODO: get page access token from user access token");
        }
    }

    protected override async Task DisposeClientAsync()
    {
        graphClient?.Dispose();
        graphClient = null;
    }

    protected override async Task<PostResult> PostImplementationAsync(ISocialMessage message)
    {
        var texts = Formatter.FormatText(message);
        var responses = new List<RestResponse>();
        foreach (var text in texts)
        {
            // TODO: ensure that each post is a response to the previous
            switch (mode)
            {
                case Mode.Page:
                    var request = new RestRequest($"/{pageId}/feed", Method.Post);
                    request.AddParameter("message", text);
                    request.AddParameter("access_token", pageToken);
                    responses.Add(await graphClient!.ExecuteAsync(request));
                    break;
                case Mode.User:
                    throw new NotImplementedException("TODO: user posts not implemented");
                default:
                    throw new NotImplementedException($"{mode} not supported");
            }
        }
        var aok = responses.All(r => r.IsSuccessful);
        var errors = responses.Where(r => !r.IsSuccessful).Select(r => r.ErrorMessage);
        return new PostResult(this, message, aok, string.Join('\n', errors));
    }

    protected override async Task<ConnectionTestResult> TestConnectionImplementationAsync()
    {
        switch (mode)
        {
            case Mode.Page:
                var request = new RestRequest($"/me?fields=id,name");
                request.AddParameter("access_token", pageToken);
                var response = await graphClient!.ExecuteAsync(request);
                if (response.IsSuccessful)
                {
                    return new ConnectionTestResult(this, true, "Connection successful");
                }
                else
                {
                    return new ConnectionTestResult(this, false, response.ErrorMessage ?? response.Content);
                }
            case Mode.User:
                throw new NotImplementedException("TODO: user posts not implemented");
            default:
                throw new NotImplementedException($"{mode} not supported");
        }
    }
}