using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class GameResponse
{
    public string slug;
    public string name;
}

[Serializable]
public class ApiError
{
    public string code;
    public string message;
}

[Serializable]
public class ApiErrorResponse
{
    public ApiError error;
}

public class GameInfoClient : MonoBehaviour
{
    [SerializeField] private string baseUrl = "http://localhost:3900";
    [SerializeField] private string gameSlug = "minesweeper";

    private IEnumerator Start()
    {
        using var request = UnityWebRequest.Get($"{baseUrl}/games/{gameSlug}");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError($"Couldn't reach the server at {baseUrl}: {request.error}");
            yield break;
        }

        string body = request.downloadHandler.text;

        if (request.result == UnityWebRequest.Result.ProtocolError)
        {
            var err = JsonUtility.FromJson<ApiErrorResponse>(body);
            Debug.LogError($"{request.responseCode} {err.error.code}: {err.error.message}");
            yield break;
        }

        var game = JsonUtility.FromJson<GameResponse>(body);
        Debug.Log($"Found game: {game.name} ({game.slug})");
    }
}