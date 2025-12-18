using System.Collections;
using System.Globalization;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }
 //   public Transform spawnParent;
 //   public GameObject floatingBTC_Prefab;
    public double Bitcoins { get; private set; } = 0;
    public double BitcoinsPerClick = 1;
    public double BitcoinsPerSecond => _bps;

    // Passive income internal variable
    private double _bps = 0;

    public bool isREset;
    public AudioSource bitcoinSound;
    public BCG_DatatUpdateRoot bCGData;
    void Awake()
    {
        //if (Instance == null)
        {
            Instance = this;
         //   DontDestroyOnLoad(gameObject);
        }
        /* else
         {
             Destroy(gameObject);
         }*/
        if (isREset)
        {
            ResetGame();
        }

    }

    void Start()
    {
        StartCoroutine(IncomeTick());
        StartCoroutine(random1CoinAdd());
        // Start repeating updates every 5 minutes
        InvokeRepeating(nameof(LoadADVideo), 300f, 600f);
       // InvokeRepeating(nameof(SendDataToserver), 10f, 60f);
    }

    IEnumerator IncomeTick()
    {
        var wait = new WaitForSeconds(1f);
        while (true)
        {
            Add(BitcoinsPerSecond);
            yield return wait;
        }
    }
    public void SendDataToserver()
    {
        bCGData.gameId = CommonDataFatch.instance.bcg_GameGetData.data.games[0].gameId;
        bCGData.totalClicks = Bitcoins;
        bCGData.clickPower = GetBps();
        bCGData.sessionData.clicks = BitcoinsPerClick;
        //
        //  EggCrackerGameManager.instance.EggGamePostplayerProgress(eggDatatUpdateRoot);


    }
    public void LoadADVideo()
    {
        AdsManager._adsManager.LoadRewardedAd();
    }
    IEnumerator WatchAD()
    {
        var wait = new WaitForSeconds(3f);
        while (true)
        {
            AdsManager._adsManager.LoadRewardedAd();
            yield return wait;
        }
    }
    public void OnClickMine()
    {
        int multiplier = BTCMultiplierManager.Instance?.GetCurrentMultiplier() ?? 1;
        double gain = BitcoinsPerClick * multiplier;
        Add(gain);
        bitcoinSound.Play();
        // Spawn floating BTC at click/tap position
        FloatingBTCSpawner.Instance?.SpawnAtPointer(gain);
    }
    IEnumerator random1CoinAdd()
    {
        var wait = new WaitForSeconds(300f);
        while (true)
        {
            FloatingBTCSpawner.Instance?.SpawnAtRandomPointer(1);
            _bps += 5;
            BCGUIManager.Instance?.UpdateBPS(_bps);
            yield return wait;
        }
    }

    public void Add(double amount)
    {
        if (amount == 0) return;
  /*      // Apply multiplier
        double finalAmount = BTCMultiplierManager.Instance != null
            ? BTCMultiplierManager.Instance.ApplyMultiplier(amount)
            : amount;*/

        Bitcoins += amount;
        BCGUIManager.Instance?.UpdateCurrency(Bitcoins);

    }
    
    public void AddAdmobBitCoin()
    {
        double gain = BitcoinsPerClick * 5;
        Add(gain);

    }
   

    public bool TrySpend(double amount)
    {
        if (Bitcoins >= amount)
        {
            Bitcoins -= amount;
            BCGUIManager.Instance?.UpdateCurrency(Bitcoins);
            return true;
        }
        return false;
    }

    public void AddIncomePerSecond(double amount)
    {
        _bps += amount;
        BCGUIManager.Instance?.UpdateBPS(_bps);
    }

    public void MultiplyClick(double multiplier)
    {
        BitcoinsPerClick *= multiplier;
        BCGUIManager.Instance?.UpdateClickValue(BitcoinsPerClick);
    }

    // --- Save/Load helpers ---
    public void SetBitcoins(double value)
    {
        Bitcoins = value;
        BCGUIManager.Instance?.UpdateCurrency(Bitcoins);
    }

    public void SetBps(double value)
    {
        _bps = value;
        BCGUIManager.Instance?.UpdateBPS(_bps);
    }

    public void SetClickValue(double value)
    {
        BitcoinsPerClick = value;
        BCGUIManager.Instance?.UpdateClickValue(BitcoinsPerClick);
    }

    public double GetBps() => _bps;

    // Reset everything
    public void ResetGame()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        SetBitcoins(0);
        SetBps(0);
        SetClickValue(1);

        var allItems = Resources.LoadAll<ShopItem>("ShopItems");
        foreach (var item in allItems)
            item.owned = 0;

        BCGUIManager.Instance?.UpdateCurrency(0);
        BCGUIManager.Instance?.UpdateBPS(0);
        BCGUIManager.Instance?.UpdateClickValue(1);

        Debug.Log("Game data cleared!");
    }
}
