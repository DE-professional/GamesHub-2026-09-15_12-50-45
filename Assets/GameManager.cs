using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

[Serializable]
public class SteamAppListResponse
{
    [SerializeField] private AppListContainer response;
    public AppListContainer responseData => response;
}

[Serializable]
public class AppListContainer
{
    [SerializeField] private List<SteamAppEntry> apps = new();
    public List<SteamAppEntry> appList
    {
        get => apps;
        set => apps = value;
    }
}

[Serializable]
public class SteamAppEntry
{
    [SerializeField] private int appid;
    [SerializeField] private string name;
    public int appId => appid;
    public string gameName => name;
}

[Serializable]
public class StoreDetailsResponseWrapper
{
    [SerializeField] private StoreDetailsData data;
    public StoreDetailsData storeData => data;
}
[Serializable]
public class Screenshot
{
    public int id;
    public string path_full;
}

[Serializable]
public class StoreDetailsData
{
    public string name;
    public string short_description;
    public string required_age;
    public string header_image;
    
    public List<Screenshot> screenshots = new();
    public ContentDescriptors content_descriptors;
    public List<GenreEntry> genres = new();
    public PriceOverview price_overview;

    public string Name => name;
    public string ShortDescription => short_description;
    public string RequiredAge => required_age;
    public ContentDescriptors ContentDescriptors => content_descriptors;
    public List<GenreEntry> Genres => genres;
    public PriceOverview PriceOverview => price_overview;
    public List<Screenshot> Screenshots => screenshots;
}

[Serializable]
public class ContentDescriptors
{
    [SerializeField] private List<int> ids = new();
    public List<int> Ids => ids;
}

[Serializable]
public class GenreEntry
{
    [SerializeField] private string id;
    [SerializeField] private string description;
    public string Id => id;
    public string Description => description;
}

[Serializable]
public class PriceOverview
{
    [SerializeField] private string final_formatted;
    public string FinalFormatted => final_formatted;
}

public class GameManager : MonoBehaviour
{
    [Header("Steam Config")] [SerializeField]
    private string apiKey = "65E6A4ADD9A187AEFE1F687424CB7A3E";

    private readonly HashSet<int> _blockedDescriptorIds = new() { 1, 3, 5 };

    private readonly HashSet<string> _blockedGenres = new(StringComparer.OrdinalIgnoreCase)
    {
        "Nudity", "Sexual Content", "Erotic"
    };

    private AppListContainer _cachedAppList;
    private string _appListPath;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private async void Start()
    {
        _cachedAppList = new AppListContainer();
        _appListPath =  Application.persistentDataPath + "/applist.json";
        if (File.Exists(_appListPath))
        {
            try
            {
                _cachedAppList = ReadAppList();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        var randomGame = await GetSafeRandomSteamGameAsync();
        
        
    }

    public void DisplayGameInfo(StoreDetailsData game)
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void WriteAppList(AppListContainer appList)
    {
        var toWrite = JsonUtility.ToJson(appList, true);
        File.WriteAllText(_appListPath, toWrite);
    }

    public AppListContainer ReadAppList()
    {
        AppListContainer localAppList = JsonUtility.FromJson<AppListContainer>(File.ReadAllText(_appListPath));
        return localAppList;
    }

    private async Task<List<SteamAppEntry>> FetchAppListAsync()
    {
        var url =
            $"https://api.steampowered.com/IStoreService/GetAppList/v1/?key={apiKey}&include_games=true&include_dlc=false&include_software=false&max_results=50000";
        using (var request = UnityWebRequest.Get(url))
        {
            var operation = request.SendWebRequest();
            while (!operation.isDone) await Task.Yield();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(request.error);
                return null;
            }
            var response = JsonUtility.FromJson<SteamAppListResponse>(request.downloadHandler.text);
            
            return response?.responseData?.appList;
        }
    }

    public async Task<StoreDetailsData> GetSafeRandomSteamGameAsync()
    {
        
        if (_cachedAppList == null || _cachedAppList.appList.Count == 0)
        {
            _cachedAppList.appList = await FetchAppListAsync();
            
            if (_cachedAppList == null || _cachedAppList.appList.Count == 0)
            {
                Debug.Log("No Steam App List");
                return null;
            }
        }

        while (true)
        {
            var randomIndex = Random.Range(0, _cachedAppList.appList.Count);
            var chosenApp = _cachedAppList.appList[randomIndex];
            var gameData = await FetchAppDetailsAsync(chosenApp.appId);
            if (gameData == null)
                // Unlisted app or failed response; try another
                continue;
            if (IsNSFW(gameData))
            {
                Debug.Log($"[Steam Store] Skipped NSFW App: {gameData.Name} (AppID: {chosenApp.appId})");
                continue;
            }
            return gameData;
        }
    }

    private async Task<StoreDetailsData> FetchAppDetailsAsync(int appId)
    {
        var url = $"https://store.steampowered.com/api/appdetails?appids={appId}";

        using (var request = UnityWebRequest.Get(url))
        {
            var operation = request.SendWebRequest();
            while (!operation.isDone) await Task.Yield();

            if (request.result != UnityWebRequest.Result.Success)
                return null;

            var jsonText = request.downloadHandler.text;
            
            int dataIndex = jsonText.IndexOf("\"data\":");
            if (dataIndex == -1) return null;

            int startBrace = jsonText.IndexOf('{', dataIndex);
            if (startBrace == -1) return null;

            // 2. Count matching braces to extract the EXACT inner object payload
            int openBraces = 0;
            int endBrace = -1;

            for (int i = startBrace; i < jsonText.Length; i++)
            {
                if (jsonText[i] == '{')
                {
                    openBraces++;
                }
                else if (jsonText[i] == '}')
                {
                    openBraces--;
                    if (openBraces == 0)
                    {
                        endBrace = i;
                        break;
                    }
                }
            }

            if (endBrace == -1) return null;

            // 3. Slice precisely from '{' to '}'
            string dataJson = jsonText.Substring(startBrace, (endBrace - startBrace) + 1);
            // Deserialize directly into StoreDetailsData
            StoreDetailsData data = JsonUtility.FromJson<StoreDetailsData>(dataJson);
            return data;
            
        }
    }

    private bool IsNSFW(StoreDetailsData data)
    {
        // 1. Check Content Descriptors
        if (data.ContentDescriptors?.Ids != null)
            foreach (var id in data.ContentDescriptors.Ids)
                if (_blockedDescriptorIds.Contains(id))
                    return true;

        // 2. Check Required Age
        if (data.RequiredAge == "18" || data.RequiredAge == "17+") return true;

        // 3. Check Genres
        if (data.Genres == null) return false;
        foreach (var genre in data.Genres)
            if (_blockedGenres.Contains(genre.Description))
                return true;

        return false;
    }
}