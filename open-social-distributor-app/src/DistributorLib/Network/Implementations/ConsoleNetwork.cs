using DistributorLib.Post;
using DistributorLib.Post.Formatters;

namespace DistributorLib.Network.Implementations;

public class ConsoleNetwork : AbstractNetwork
{
    public ConsoleNetwork() : base(NetworkType.Console, "Console", "Console.Network", PostFormatVariantFactory.Console)
    {
    }

    protected override async Task DisposeClientAsync()
    {
        Console.WriteLine($"{ShortCode} disposed.");
    }

    protected override async Task InitClientAsync()
    {
        Console.WriteLine($"{ShortCode} ready.");
    }

    protected override async Task<ConnectionTestResult> TestConnectionImplementationAsync()
    {
        return new ConnectionTestResult(this, true);
    }

    protected override async Task<PostResult> PostImplementationAsync(ISocialMessage message)
    {
        var text = Formatter.FormatText(message);
        Console.WriteLine(text);
        foreach (var image in message.Images ?? Array.Empty<ISocialImage>())
        {
            Console.WriteLine($" - Image Uri:   {image.Uri}");
            Console.WriteLine($" - Description: {image.Description}");
        }
        return new PostResult(this, message, true);
    }
}