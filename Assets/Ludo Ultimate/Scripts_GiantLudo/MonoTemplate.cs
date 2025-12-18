using UnityEngine;
public class MonoTemplate : MonoBehaviour
{

     protected Loader LoaderScreen => GetUIManager.loader;
    protected Settings SettingsScreen => GetUIManager.settings;
    protected JoinRoom JointRoomScreen => GetUIManager.joinRoom;
    protected GameRules GameRulesScreen => GetUIManager.gameRules;
    protected PrivacyPolicy GetPrivacyPolicyScreen => GetUIManager.policy;
    protected LudoHomeScreen GetHomeScreen => GetUIManager.homeScreen;
    protected PlayOnline GetPlayOnlineScreen => GetUIManager.playOnline;
    protected CreateRoom GetCreateRoomScreen => GetUIManager.createRoom;
    protected ShopScreen GetShopScreen => GetUIManager.shopScreen;
    protected TermsAndConditions TermsAndConditionsScreen => GetUIManager.tnc;
    protected MessagePanel GetMessagePanel => GetUIManager.messagePanel;
    protected GameController GetGameScreen => GetUIManager.LocalGameScreen;
     protected PrizeDistribution GetDistribution => GetUIManager.distribution;
    protected LudoGame_SocketManager GetSocketManager => GetUIManager.socketManager;
    protected LocalGamePlayPanel GetLocalGamePlay => GetUIManager.localGamePlay;
   protected PlayWithComputer GetPlayWithComputer => GetUIManager.playWithComputer;
   protected StartGameTimer GetWaitForGameScreen => GetUIManager.WaitForGameScreen;
 
    protected AssetOfGame AssetOfGame => GetUIManager.assetOfGame;

    protected Ludo_UIManager GetUIManager => Ludo_UIManager.instance;
}