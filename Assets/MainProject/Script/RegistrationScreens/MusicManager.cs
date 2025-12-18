using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO; // Needed for Editor file operations
using UnityEngine.UI; // Needed for UI elements like Text
using TMPro; // Needed for TextMeshPro elements
using System;
using UnityEngine.Networking;
using UnityEngine.Audio;
using UnityEngine.Experimental.GlobalIllumination;

using Cysharp.Threading.Tasks;
using LitJson;
using System.Linq;
using System.Threading.Tasks;










// These are needed for the Android-specific code
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

/// <summary>
/// This is a simple data container to hold information about one song.
/// It is defined inside the MusicManager so it's always available.
/// </summary>
/// 
[System.Serializable]
public class MusicTrack
{
    public string Title;
    public string Artist;
    public string FilePath;
    public bool ISMyMusic;

    // --- NEW ADDITIONS ---
    // This will hold the actual audio data when it's loaded.
    // It will be 'null' if it's not in memory.
    [NonSerialized]

    public AudioClip LoadedClip;

    // A handy way to check if the audio is ready to play instantly.

    public bool IsLoaded
    {
        get { return LoadedClip != null; }
    }
    // --- END OF NEW ADDITIONS ---

    public MusicTrack(string title, string artist, string filePath)
    {
        this.Title = title;
        this.Artist = artist;
        this.FilePath = filePath;
        this.LoadedClip = null; // Always start as unloaded
    }
}

[System.Serializable]
public class MyMusicList
{
    public List<MusicTrack> MyMusic = new List<MusicTrack>();
}
    /// <summary>
    /// This single script manages finding and listing all music.
    /// It works in both the Unity Editor and on Android devices.
    /// </summary>
    public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance; // Singleton instance

   
    public List<MusicTrack> AllFoundMusic = new List<MusicTrack>();
  // public List<MusicTrack> MyMusic = new List<MusicTrack>();
    public MyMusicList MyMusicListRepsonse;
    public static bool IsInitialized { get; private set; } = false;
    public string MyMusicJsonName = "MyMusic_cache.json";
    private  void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Define the path for our cache file in a writable location
            cacheFilePath = Path.Combine(Application.persistentDataPath, MyMusicJsonName);
        }
        else
        {
            Destroy(gameObject);
        }
       
    }
    private async void  OnEnable()
    {
        AllFoundMusic = await NewScanDeviceForMusicAsync();
        print(AllFoundMusic.Count);
    }

    private async UniTask<List<MusicTrack>> ScanDeviceForMusicAsync()
    {
        var foundTracks = new List<MusicTrack>();

#if UNITY_EDITOR
        // In the editor, we scan the user's "My Music" folder.
        await UniTask.RunOnThreadPool(() => {
            string musicPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyMusic);
            if (Directory.Exists(musicPath))
            {
                var filePaths = Directory.GetFiles(musicPath, "*.*", SearchOption.AllDirectories)
                    .Where(f => f.EndsWith(".mp3", System.StringComparison.OrdinalIgnoreCase) || f.EndsWith(".wav", System.StringComparison.OrdinalIgnoreCase));

                foreach (var filePath in filePaths)
                {
                    foundTracks.Add(new MusicTrack(Path.GetFileNameWithoutExtension(filePath), "PC Scan", filePath));
                }
            }
        });
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
        // On Android, we query the MediaStore.
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
            await UniTask.Delay(1500); // Give user time to respond
        }

        if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        {
            await UniTask.RunOnThreadPool(() => {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject>("getContentResolver");
                AndroidJavaClass mediaStoreAudio = new AndroidJavaClass("android.provider.MediaStore$Audio$Media");
                AndroidJavaObject externalContentUri = mediaStoreAudio.GetStatic<AndroidJavaObject>("EXTERNAL_CONTENT_URI");
                string[] projection = {
                    mediaStoreAudio.GetStatic<string>("TITLE"),
                    mediaStoreAudio.GetStatic<string>("ARTIST"),
                    mediaStoreAudio.GetStatic<string>("_DATA")
                };
                AndroidJavaObject cursor = contentResolver.Call<AndroidJavaObject>("query", externalContentUri, projection, null, null, null);
                if (cursor != null)
                {
                    while (cursor.Call<bool>("moveToNext"))
                    {
                        foundTracks.Add(new MusicTrack(
                            cursor.Call<string>("getString", 0),
                            cursor.Call<string>("getString", 1),
                            cursor.Call<string>("getString", 2)
                        ));
                    }
                    cursor.Call("close");
                }
            });
        }
#endif
       
        // --- KEY CHANGE: Move the heavy lifting to a background thread ---
        if (foundTracks.Count > 0)
        {
            var reconciledList = new List<MusicTrack>(foundTracks);
            var playlistPaths = new HashSet<string>();
           
            MyMusicListRepsonse= await LoadMyMusicListFromJson();
            if (MyMusicListRepsonse.MyMusic != null && MyMusicListRepsonse.MyMusic.Count > 0)
            {
                playlistPaths = new HashSet<string>(MyMusicListRepsonse.MyMusic.Select(t => t.FilePath));
            } 

            await UniTask.RunOnThreadPool(() =>
        {
            // This code block runs in the background.
            // It's safe to do heavy processing here.

            // Loop through our copy of the list
            foreach (var track in reconciledList)
            {
                // The check is the same, but it's happening off the main thread.
                if (playlistPaths.Contains(track.FilePath))
                {
                    track.ISMyMusic = true;
                }
                else
                {
                    track.ISMyMusic = false;
                }
            }
        });
        }
        Debug.Log($"Scan complete. Found {foundTracks.Count} tracks on device.");
        return foundTracks;
    }



    public async UniTask<List<MusicTrack>> NewScanDeviceForMusicAsync()
    {
        var foundTracks = new List<MusicTrack>();

#if UNITY_EDITOR
        // 🔹 Simulate scanning "My Music" folder in the Editor
        await UniTask.RunOnThreadPool(() =>
        {
            string musicPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyMusic);
            if (Directory.Exists(musicPath))
            {
                var filePaths = Directory.GetFiles(musicPath, "*.*", SearchOption.AllDirectories)
                    .Where(f => f.EndsWith(".mp3", System.StringComparison.OrdinalIgnoreCase)
                             || f.EndsWith(".wav", System.StringComparison.OrdinalIgnoreCase));

                foreach (var filePath in filePaths)
                {
                    foundTracks.Add(new MusicTrack(Path.GetFileNameWithoutExtension(filePath), "PC", filePath));
                }
            }
        });
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
        // 🔹 Request permissions dynamically
        if (!Permission.HasUserAuthorizedPermission("android.permission.READ_MEDIA_AUDIO") &&
            !Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        {
            Permission.RequestUserPermission("android.permission.READ_MEDIA_AUDIO");
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
            await UniTask.Delay(2000); // Give user time to respond
        }

        // 🔹 Only proceed if permission granted
        if (Permission.HasUserAuthorizedPermission("android.permission.READ_MEDIA_AUDIO") ||
            Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        {
            // Must run AndroidJava calls on main thread
            await UniTask.SwitchToMainThread();

            try
            {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject>("getContentResolver");

                AndroidJavaClass mediaStoreAudio = new AndroidJavaClass("android.provider.MediaStore$Audio$Media");
                AndroidJavaObject externalContentUri = mediaStoreAudio.GetStatic<AndroidJavaObject>("EXTERNAL_CONTENT_URI");

                string[] projection = { "_id", "title", "artist" };

                AndroidJavaObject cursor = contentResolver.Call<AndroidJavaObject>(
                    "query",
                    externalContentUri,
                    projection,
                    null,
                    null,
                    null
                );

                if (cursor != null)
                {
                    int idColumn = cursor.Call<int>("getColumnIndexOrThrow", "_id");
                    int titleColumn = cursor.Call<int>("getColumnIndexOrThrow", "title");
                    int artistColumn = cursor.Call<int>("getColumnIndexOrThrow", "artist");

                    AndroidJavaClass contentUris = new AndroidJavaClass("android.content.ContentUris");

                    int count = cursor.Call<int>("getCount");
                    Debug.Log($"🎵 Found {count} tracks in MediaStore");

                    while (cursor.Call<bool>("moveToNext"))
                    {
                        long id = cursor.Call<long>("getLong", idColumn);
                        string title = cursor.Call<string>("getString", titleColumn);
                        string artist = cursor.Call<string>("getString", artistColumn);

                        AndroidJavaObject uri = contentUris.CallStatic<AndroidJavaObject>(
                            "withAppendedId",
                            externalContentUri,
                            id
                        );

                        foundTracks.Add(new MusicTrack(title, artist, uri.Call<string>("toString")));
                    }

                    cursor.Call("close");
                }
                else
                {
                    Debug.LogWarning("⚠️ MediaStore query returned null cursor!");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Error scanning MediaStore: {e}");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ User denied audio/media permission.");
        }
#endif

        Debug.Log($"🎧 Total tracks found: {foundTracks.Count}");
        return foundTracks;
    }


    // Replace your old LoadAudio coroutine with this new, improved version.
    internal async UniTask<AudioClip> LoadAndGetAudioClipAsync(MusicTrack track)
    {
        // If the track is already loaded, just return the clip immediately.
        if (track.IsLoaded)
        {
            return track.LoadedClip;
        }



        Debug.Log($"Async load started for '{track.Title}'");

        try
        {
            string path = "file://" + track.FilePath;
            using (var www = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.UNKNOWN))
            {
                // Asynchronously wait for the web request to complete
                var asyncOperation = www.SendWebRequest();
                while (!asyncOperation.isDone)
                {
                    await UniTask.Yield(); // Await a frame, allowing Unity to continue processing
                }


                if (www.result == UnityWebRequest.Result.Success)
                {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                    // IMPORTANT: Assign the loaded clip back to our track object for caching.
                    track.LoadedClip = clip;
                    Debug.Log($"<color=green>Successfully loaded '{track.Title}'</color>");

                    // --- KEY CHANGE ---
                    // Return the clip that was just loaded.
                    return track.LoadedClip;
                }
                else
                {
                    Debug.LogError($"Error loading '{track.Title}': {www.error}");
                    // Return null to signal that loading failed.
                    return null;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"An exception occurred while loading '{track.Title}': {ex.Message}");
            return null; // Also return null on exceptions
        }

    }

    public AudioSource audioSource; // Connect this in the Inspector to your AudioSource component

    //save mymusic list to cache
    private string cacheFilePath;
    internal void SaveMyMusicListToJson(MusicTrack track)
    {
        MyMusicListRepsonse=new MyMusicList();
        MyMusicListRepsonse.MyMusic = new List<MusicTrack>();
        MyMusicListRepsonse.MyMusic.Add(track);
        
        Debug.Log("Saving MyMusic list to JSON..." + MyMusicListRepsonse.MyMusic.Count);
        string json = JsonUtility.ToJson(MyMusicListRepsonse);
        Debug.Log($"Saving {json} tracks to cache at: {cacheFilePath}");
        File.WriteAllText(cacheFilePath, json);
    }
    internal async UniTask<MyMusicList> LoadMyMusicListFromJson()
    {
        // 1. Check if the file exists. This is a very fast, synchronous check.
        if (!File.Exists(cacheFilePath))
        {
            Debug.LogWarning($"Cache file not found at: {cacheFilePath}. Returning an empty list.");
            return new MyMusicList(); // Return a completed task with an empty list.
        }

        try
        {

            // 2. Asynchronously read the entire file content.
            // The 'await' keyword pauses this method's execution here without freezing the game.
            // The method will resume once the file has been fully read in the background.
            string json = await File.ReadAllTextAsync(cacheFilePath);
            Debug.Log($"Starting async load from: {cacheFilePath}"+json);

            // 3. Once we have the json string, deserialize it.
            // Deserialization is usually fast enough to do on the main thread.
            if (!string.IsNullOrEmpty(json))
            {
             MyMusicList   tempMyMusicListRepsonse = JsonUtility.FromJson<MyMusicList>(json);

                if (tempMyMusicListRepsonse.MyMusic != null)
                {
                    Debug.Log($"<color=green>Async load complete. Loaded {tempMyMusicListRepsonse.MyMusic.Count} tracks.</color>");
                    foreach (var track in tempMyMusicListRepsonse.MyMusic)
                    {
                        // Ensure each track has its clip loaded
                        if (!track.IsLoaded)
                        {
                            await LoadAndGetAudioClipAsync(track);
                        }
                    }

                    return tempMyMusicListRepsonse;
                }
            }

            // If json is empty or deserialization results in null
            Debug.LogWarning("JSON file was empty or invalid. Returning empty list.");
             return new MyMusicList();
        }
       
        catch (Exception ex)
        {
            // Catch any other errors (e.g., file permissions)
            Debug.LogError($"An error occurred during async file read: {ex.Message}");
            return new MyMusicList();  // Return empty list on failure
        }
    }
}