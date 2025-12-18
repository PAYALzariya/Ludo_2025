using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class DataManager : MonoBehaviour
{
    public static DataManager instance; 
    public FrameData frameScritableData; // Assign this in the inspector
    public CountryData countryScritableData; // Assign this in the inspector
    public Leveldata levelScritableData; // Assign this in the inspector
    public CurrencyData currencyScritableData; // Assign this in the inspector
    public SpriteMaganer spriteMaganer; // Assign this in the inspector
    public int coin, dimond;
    private void Start()
    {
        coin = PlayerPrefs.GetInt("Coin", 2000);
        dimond = PlayerPrefs.GetInt("Dimonad", 2000);
    }
    internal  Currency GetCurrency(string Currencyname)
    {
        Currency currency;

        if (currencyScritableData.LookupDictionary.ContainsKey(Currencyname))
        {
            currency = currencyScritableData.LookupDictionary[Currencyname];
            return currency;
        }
        else
        {
            currency = new Currency();
            currency.CurrencyType = "notfound";
            
            currency.CurrencySprite = spriteMaganer.default_currencySprite;
            return currency;
        }
       
    }
   internal  level GetLevel(int levelNo)
    {
        level level;

        if (levelScritableData.LookupDictionary.ContainsKey(levelNo))
        {
            level = levelScritableData.LookupDictionary[levelNo];
            return level;
        }
        else
        {
            level = new level();
            level.LevelNo = 000;
            level.Levelscore = "0";
            level.levelSprite = spriteMaganer.default_levelSprite;
            return level;
        }
       
    }
    internal Frame GetFrame(int frameIndex)
    {
        Frame frame;
        if (frameScritableData.LookupDictionary.ContainsKey(frameIndex))
        {
            frame = frameScritableData.LookupDictionary[frameIndex];
            return frame;
        }
        else
        {
            frame = new Frame();
            frame.frameSprite = spriteMaganer.default_frameSprite;
              return frame;
        }
        
      
    }
  internal Country GetCountry(string countryname)
    {
        Country country;
        if (countryScritableData.countryLookup.ContainsKey(countryname.ToLower()))
        {
            country = countryScritableData.countryLookup[countryname.ToLower()];
            return country;
        }
        else
        {

            country = new Country();
            country.countryname = "Unknown";
            country.flagSprite = spriteMaganer.default_countrySprite;
            return country;

        }
           
        }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
  
    }
    public string AccessToken { get;  set; }
    public UserProfileData MyProfile { get;  set; }

    public ChatParticipantData MyParticipantInfo { get;  set; }
   
    public string roomID
    {
        get
        {

            return PlayerPrefs.GetString("roomID");
        }
        set
        {


            PlayerPrefs.SetString("roomID", value);

        }
    }
public bool ISmusicOn
    {
        get
        {

            return PlayerPrefs.GetInt("MusicOn", 1) == 1;
        }
        set
        {

            int intValue = value ? 1 : 0;


            PlayerPrefs.SetInt("MusicOn", intValue);
        }
    }
    public bool ISSoundOn
    {
         get
        {
            
            return PlayerPrefs.GetInt("ISSoundOn", 1) == 1;
        }
        set
        {
            
            int intValue = value ? 1 : 0;

         
            PlayerPrefs.SetInt("ISSoundOn", intValue);
        }
    }
 public bool ISVibration
    {
        get
        {

            return PlayerPrefs.GetInt("ISVibration", 1) == 1;
        }
        set
        {

            int intValue = value ? 1 : 0;


            PlayerPrefs.SetInt("ISVibration", intValue);
        }
    }
    public bool IsRoomCreated
    {
         get
        {
            
            return PlayerPrefs.GetInt("IsRoomCreated", 1) == 1;
        }
        set
        {
            
            int intValue = value ? 1 : 0;

         
            PlayerPrefs.SetInt("IsRoomCreated", intValue);
        }
    }

   

    public  string IsLoggedIn
    {
        get
        {
            
            return PlayerPrefs.GetString ("IsLoggedIn");
        }
        set
        {
           
           
            PlayerPrefs.SetString("IsLoggedIn", value);
            
        }
    }
    public string refreshToken
    {
        get
        {

            return PlayerPrefs.GetString("refreshToken");
        }
        set
        {


            PlayerPrefs.SetString("refreshToken", value);

        }
    }
    public string userId
    {
        get
        {

            return PlayerPrefs.GetString("userId");
        }
        set
        {


            PlayerPrefs.SetString("userId", value);

        }
    }
    public string uniqueId
    {
        get
        {

            return PlayerPrefs.GetString("uniqueId");
        }
        set
        {


            PlayerPrefs.SetString("uniqueId", value);

        }
    }










    public void StoreLoginData(AuthSuccessResponse loginResponse)
    {
        Debug.Log("Storing Login Data...");
       
        MyProfile = loginResponse.user;
    }

    

    public void StoreUpdatedProfile(UserProfileData newProfileData)
    {
        Debug.Log("Updating stored profile...");
        MyProfile = newProfileData;
    }

    public void ClearAllData()
    {
        Debug.Log("Clearing all session data...");
        AccessToken = null;
        MyProfile = null;
        
        MyParticipantInfo = null;
    }


    private async UniTask<bool> IsUrlAnImage(string url)
    {
        try
        {
                Debug.Log("Called IsUrlAnImage::" + url);
            using (var request = UnityWebRequest.Head(url))
            {
                

                await request.SendWebRequest().ToUniTask();
                Debug.Log("Called  UnityWebRequest IsUrlAnImage::" + request);
                if (request.result == UnityWebRequest.Result.Success)
                {
                    // Success! Now check the content type.
                    string contentType = request.GetResponseHeader("Content-Type");
                    if (contentType != null && contentType.StartsWith("image/"))
                    {
                        Debug.Log($"URL '{url}' is a valid image. Content-Type: {contentType}");
                        return true;
                    }
                    else
                    {
                        Debug.LogWarning($"URL '{url}' is valid, but not an image. Content-Type: {contentType}");
                        return false;
                    }
                }
                else
                {
                    // The URL is broken (e.g., 404 Not Found) or the server is down.
                    Debug.LogError($"HEAD request failed for URL '{url}'. Error: {request.error}");
                    return false;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Exception during URL validation: {e.Message}");
            return false;
        }
    }





    public async UniTask<Sprite> LoadSprite(LoadedSpriteAsset asset)
    {
        if (string.IsNullOrEmpty(asset._sourceUrl))
        {

            // asset.SpriteAssset = spriteMaganer.default_Sprite;
            asset.SpriteAssset = null;
            return asset.SpriteAssset;
        }
        else
        {


            if (asset.SpriteAssset != null)
            {
                return asset.SpriteAssset;
            }


            if (asset._isLoading)
            {
                return null;
            }


            asset._isLoading = true;
            try
            {

                Sprite loadedSprite = await spriteMaganer.imageValidatorAndDownloader.GetSpriteWithValidation(asset._sourceUrl);


                asset.SpriteAssset = loadedSprite;


                if (asset.SpriteAssset != null)
                {
                    //Debug.Log($"Load complete for URL '{asset._sourceUrl}'. Assigned sprite: '{asset.SpriteAssset.name}'");
                }
                else
                {
                    asset.SpriteAssset = null;
                }

                return asset.SpriteAssset;
            }
            catch (Exception ex)
            {

                Debug.LogError($"An unexpected error occurred while loading sprite for '{asset._sourceUrl}': {ex.Message}");

                //    asset.SpriteAssset = spriteMaganer.default_Sprite;
                asset.SpriteAssset = null;
                return asset.SpriteAssset;
            }
            finally
            {

                asset._isLoading = false;
            }
        }
    }
    
}

