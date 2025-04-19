namespace Game.Features.ChatGptImageGeneration;

public static class ImagePromptTemplates
{
    public const string StyleVersion = "v1.0";
    
    public static string GenerateDefault(string itemDescription) =>
        $"{itemDescription}, centered and isolated on a transparent background, in dark fantasy RPG style, highly detailed, soft directional lighting, painterly texture, consistent item icon framing, slight 3D tilt, game inventory artwork style, no props or scenery, style version {StyleVersion}";
    
    public static string GenerateEpic(string itemDescription) =>
        $"{itemDescription}, glowing with magical aura, featuring ornate design and rare materials, centered and isolated on a transparent background, in dark fantasy RPG style, extremely detailed, cinematic lighting, painterly texture, rich contrast, consistent item icon framing, slight 3D tilt, high-fantasy inventory artwork style, no props or scenery, style version {StyleVersion}";
    
    public static string GenerateCursed(string itemDescription) =>
        $"{itemDescription}, emitting a faint dark mist or eerie glow, corrupted appearance, centered and isolated on a transparent background, in dark fantasy RPG style, gritty textures, shadowed lighting, painterly strokes, slightly twisted form, consistent item icon framing, slight 3D tilt, game inventory artwork style, no props or scenery, style version {StyleVersion}";
    
    public static string GeneratePotion(string itemDescription) =>
        $"{itemDescription}, stylized glass or natural material, centered and isolated on a transparent background, in dark fantasy RPG style, vibrant colors, soft lighting, painterly texture, consistent icon framing, slight 3D tilt, fantasy inventory artwork style, no labels or text, style version {StyleVersion}";
    
    public static string GenerateScroll(string itemDescription) =>
        $"{itemDescription}, aged or enchanted appearance, centered and isolated on a transparent background, in dark fantasy RPG style, mystical aura, soft parchment textures, gold or magical symbols, soft directional lighting, painterly texture, consistent icon framing, slight 3D tilt, game inventory artwork style, no props or scenery, style version {StyleVersion}";
}
