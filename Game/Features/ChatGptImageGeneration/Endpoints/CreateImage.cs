using FastEndpoints;
using OpenAI.Images;

namespace Game.Features.ChatGptImageGeneration.Endpoints;

public class CreateImage : Endpoint<CreateImageRequest>
{
    public override void Configure()
    {
        Post("/images");
    }

    public override async Task HandleAsync(CreateImageRequest req, CancellationToken ct)
    {
        ImageClient client = new("dall-e-3", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        ImageGenerationOptions options = new()
        {
            Quality = GeneratedImageQuality.High,
            Size = GeneratedImageSize.W1024xH1024,
            Style = GeneratedImageStyle.Vivid,
            ResponseFormat = GeneratedImageFormat.Bytes
        };
        
       // var fullPrompt = ImagePromptTemplates.GeneratePrompt(req.Prompt);
        
        GeneratedImage image = await client.GenerateImageAsync(req.Prompt, options, ct);
        
        BinaryData bytes = image.ImageBytes;

        var safePrompt = string.Join("_", req.Prompt.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
        await using FileStream stream = File.OpenWrite($"{Guid.NewGuid()}.png");
        await bytes.ToStream().CopyToAsync(stream, ct);
        
        
        await SendOkAsync(ct);
    }
}