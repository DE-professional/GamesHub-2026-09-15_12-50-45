using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace Util
{
    public class LoadImageFromUrl : MonoBehaviour
    {
        public void FetchSpriteFromUrl(string url, Action<Sprite> onComplete)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError("URL is null or empty.");
                onComplete?.Invoke(null);
                return;
            }

            StartCoroutine(DownloadImageAsSprite(url, onComplete));
        }

        private IEnumerator DownloadImageAsSprite(string url, Action<Sprite> onComplete)
        {
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError($"Error downloading image from {url}: {request.error}");
                    onComplete?.Invoke(null); 
                }
                else
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(request);
                
                    // Convert Texture2D into a Sprite
                    Sprite downloadedSprite = Sprite.Create(
                        texture, 
                        new Rect(0, 0, texture.width, texture.height), 
                        new Vector2(0.5f, 0.5f)
                    );

                    // Return the sprite via the callback
                    onComplete?.Invoke(downloadedSprite);
                }
            }
        }
    }
}
