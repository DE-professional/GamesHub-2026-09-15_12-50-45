using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Downloads images from a list of URLs and converts them into Sprites,
/// ready to feed into ImageCarousel.SetSprites(...).
///
/// Usage:
///   StartCoroutine(SpriteUrlLoader.LoadSprites(urls, sprites => {
///       carousel.SetSprites(sprites);
///   }));
/// </summary>
public static class SpriteUrlLoader
{
    /// <summary>
    /// Downloads each URL in order and returns a list of Sprites via callback.
    /// Failed downloads are skipped (logged as warnings) rather than aborting the whole batch.
    /// Must be run as a Coroutine (it needs a MonoBehaviour to run on - see the overload below
    /// if you don't already have one handy).
    /// </summary>
    public static IEnumerator LoadSprites(List<string> urls, Action<List<Sprite>> onComplete)
    {
        List<Sprite> results = new List<Sprite>();

        foreach (string url in urls)
        {
            yield return LoadSingleSprite(url, sprite =>
            {
                if (sprite != null)
                    results.Add(sprite);
            });
        }

        onComplete?.Invoke(results);
    }

    /// <summary>
    /// Same as above, but downloads all images in parallel instead of one-by-one.
    /// Faster for large lists, but be mindful of hammering a server with many
    /// simultaneous requests if the list is very large.
    /// Preserves the original URL order in the resulting list.
    /// </summary>
    public static IEnumerator LoadSpritesParallel(List<string> urls, Action<List<Sprite>> onComplete)
    {
        Sprite[] results = new Sprite[urls.Count];
        int remaining = urls.Count;

        if (remaining == 0)
        {
            onComplete?.Invoke(new List<Sprite>());
            yield break;
        }

        // We need a runner to actually kick off each coroutine - grab a hidden helper object.
        MonoBehaviour runner = SpriteUrlLoaderRunner.Instance;

        for (int i = 0; i < urls.Count; i++)
        {
            int index = i; // local copy to avoid closure capture bug
            runner.StartCoroutine(LoadSingleSprite(urls[index], sprite =>
            {
                results[index] = sprite; // stays null on failure, filtered out below
                remaining--;
            }));
        }

        while (remaining > 0)
            yield return null;

        List<Sprite> ordered = new List<Sprite>();
        foreach (Sprite s in results)
        {
            if (s != null)
                ordered.Add(s);
        }

        onComplete?.Invoke(ordered);
    }

    private static IEnumerator LoadSingleSprite(string url, Action<Sprite> onDone)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            bool failed = request.result != UnityWebRequest.Result.Success;
#else
            bool failed = request.isNetworkError || request.isHttpError;
#endif
            if (failed)
            {
                Debug.LogWarning($"Failed to load image from {url}: {request.error}");
                onDone?.Invoke(null);
                yield break;
            }

            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            Sprite sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f) // pivot at center
            );

            onDone?.Invoke(sprite);
        }
    }
}

/// <summary>
/// Tiny auto-created MonoBehaviour used purely as a coroutine host for
/// LoadSpritesParallel, so you don't need to pass in your own MonoBehaviour reference.
/// You won't need to touch this directly.
/// </summary>
public class SpriteUrlLoaderRunner : MonoBehaviour
{
    private static SpriteUrlLoaderRunner _instance;

    public static SpriteUrlLoaderRunner Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("SpriteUrlLoaderRunner");
                _instance = obj.AddComponent<SpriteUrlLoaderRunner>();
                UnityEngine.Object.DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }
}