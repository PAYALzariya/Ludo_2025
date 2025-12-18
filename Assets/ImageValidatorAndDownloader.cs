using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks; // UniTask
using System;

public class ImageValidatorAndDownloader : MonoBehaviour
{
    
    public async UniTask<Sprite> GetSpriteWithValidation(string url)
    {
        Debug.Log($"Attempting to get sprite from URL: '{url}'" + Uri.IsWellFormedUriString(url, UriKind.Absolute));

        // --- 1. Basic URL Syntax Check (Fastest, no network call) ---
        if (string.IsNullOrEmpty(url) || !Uri.IsWellFormedUriString(url, UriKind.Absolute))
        {
            Debug.LogWarning($"Invalid URL format: '{url}'. Returning default sprite.");
            return null;
        }

        // --- 2. Content Type Validation (Efficient network call using HEAD) ---
        // This checks if the URL actually points to an image without downloading it.
       var (isImage, extension) = await VerifyAndGetImageExtension(url);
          Debug.Log($" GetSpriteWithValidation isurlimage::"+isImage+"::extension::"+extension
        );
        if (!isImage)
        {
            Debug.Log($"URL content validation failed. The content at '{url}' is not an image or the URL is broken. Returning default sprite.");
            return null;
        }

        // --- 3. Full Download and Sprite Creation (Slowest step) ---
        // Only proceed if all checks have passed.
        Debug.Log($"Validation successful for '{url}'. Proceeding to download.");
        try
        {
            return await GetSpriteFromUrl(url);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to download or create sprite from '{url}'. Exception: {ex.Message}. Returning default sprite.");
            return null;
        }
    }
private async UniTask<(bool isImage, string extension)> VerifyAndGetImageExtension(string url)
{
    try
    {
        using (var uwr = UnityWebRequestTexture.GetTexture(url))
        {
            uwr.timeout = 15;
            await uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                // Try decode
                Texture2D tex = DownloadHandlerTexture.GetContent(uwr);
                if (tex != null && tex.width > 0 && tex.height > 0)
                {
                    // Check header again for extension
                    string contentType = uwr.GetResponseHeader("Content-Type");
                    string extension = MapContentTypeToExtension(contentType);

                    return (true, extension);
                }
            }
            else
            {
                Debug.LogWarning($"Download failed for '{url}'. Error: {uwr.error}");
            }
        }
    }
    catch (Exception ex)
    {
        Debug.Log($"Exception verifying image '{url}': {ex.Message}");
    }

    return (false, string.Empty);
}

private string MapContentTypeToExtension(string contentType)
{
    switch (contentType.ToLower())
    {
        case "image/png": return ".png";
        case "image/jpeg": return ".jpg";
        case "image/jpg": return ".jpg";
        case "image/gif": return ".gif";
        case "image/webp": return ".webp";
        case "image/bmp": return ".bmp";
        default: return string.Empty; // unknown type
    }
}
    private async UniTask<bool> IsUrlAnImage(string url)
    {
        try
        {
            using (var uwr = UnityWebRequest.Head(url))
            {
               
                uwr.timeout = 10;

                await uwr.SendWebRequest();

                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    // Check the 'Content-Type' header returned by the server
                    string contentType = uwr.GetResponseHeader("Content-Type");
                    if (!string.IsNullOrEmpty(contentType) && contentType.ToLower().StartsWith("image/"))
                    {
                        Debug.Log($"URL '{url}' has a valid image Content-Type: {contentType}");
                        return true; // It's an image!
                    }
                    else
                    {
                        Debug.LogWarning($"URL '{url}' does not have an image Content-Type. It has: {contentType}");
                        return false;
                    }
                }
                else
                {
                    
                    Debug.LogWarning($"HEAD request failed for '{url}'. Error: {uwr.error}");
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Exception during HEAD request for '{url}': {ex.Message}");
            return false;
        }
    }

 
    private async UniTask<Sprite> GetSpriteFromUrl(string url)
    {
        using (var uwr = UnityWebRequestTexture.GetTexture(url))
        {
            await uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr);

                if (texture != null)
                {
                    // Create the Sprite from the downloaded texture
                    var rect = new Rect(0, 0, texture.width, texture.height);
                    var pivot = new Vector2(0.5f, 0.5f);
                    return Sprite.Create(texture, rect, pivot);
                }
                else
                {
                     return null;
                }
            }

           
            throw new Exception($"UnityWebRequest failed: {uwr.error}");
        }
    }
}