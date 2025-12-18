
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Ludo_Constants
{
    /// <summary>
    /// LudoConstants.
    /// </summary>
    public class LudoConstants
    {

        public static string BaseUrl = "";
#if UNITY_EDITOR
        public static string _screenShotPath = Path.Combine(Application.dataPath, "ScreenShot.png");
#else
        public static string _screenShotPath = Path.Combine(Application.persistentDataPath, "ScreenShot.png");
#endif

    }
        public class Messages
    {

        public const string RoomToCloseOneHour = "Room will be close in next one hour.";
        public const string RoomToCloseHalfHour = "Room will be close in next half hour";
        public const string Update_Profile_Error = "Profile not updated. Please try again.";
        public const string NoInternetConnection = "Internet connection not available.";
        public const string OtherPlayerDisconnect = "The other player disconnected. Your winstreak will not be reset.";

        public const string RegistredTournamentmessage = "Running Game! Click here.";
        public const string LoggingIn = "Logging In..";
        public const string PleaseWait = "Please wait..";
        public const string SomethingWentWrong = "Something went wrong.";

        public const string Disconnected = "Disconnected from the server.";
        public const string DisconnectedServer = "Internet Connection is Low Or Server TimerOut";

        public const string UpdateChipsCredit = "<color>" + KeyChips + "</color> Chips credited into your account";
        public const string UpdateChipsDebit = "<color>" + KeyChips + "</color> Chips debited from your account";

        //		public const string RequestForChips = "Your request for <color>" + KeyChips + "</color> chips will be sent. Are you sure?";
        public const string KeyChips = "[CHIPS]";

        /// <summary>
        /// Login panel messages
        /// </summary>
        public class Login
        {
            public const int PasswordLength = 6;

            public const string UsernameEmpty = "Username is empty.";
            public const string PasswordEmpty = "Password is empty.";

            public const string MinPasswordLength = "Password must be at least 6 characters.";
        }

        /// <summary>
        /// Register panel messages
        /// </summary>
        public class Register
        {
            public const string UsernameEmpty = "Username is empty.";
            public const string FirstNameEmpty = "Name is empty.";
            public const string LastNameEmpty = "Last name is empty.";
            public const string EmailEmpty = "Email is empty.";
            public const string EmailInvalid = "Email is invalid.";
            public const string PasswordEmpty = "Password is empty.";
            public const string AccountNumberEmpty = "ID is empty.";
            public const string NewPasswordEmpty = "New password is empty.";
            public const string ConfirmPasswordEmpty = "Confirm password is empty.";
            public const string PasswordNotMatched = "Password not matched.";
            public const string MobileEmpty = "Mobile is empty.";
            public const string MobileInvalid = "Mobile is invalid.";
            public const string paypal = "Paypal/Venmo Id is empty";

            public const string MinPasswordLength = "Password must be at least 6 characters.";
        }

        public class TransferChips
        {
            public const string ChipsAmountEmpty = "Chips amount is empty.";
            public const string ChipsAmountGreaterThan = "Chips amount should greater than";
        }

        /// <summary>
        /// Lobby panel messages.
        /// </summary>
        public class Lobby
        {
            public const string LogoutConfirmation = "You will be logout. Are you sure?";
            public const string ExitGameConfirmation = "Game will be closed. Are you sure?";
        }

        /// <summary>
        /// Create table panel messages
        /// </summary>
        public class CreateTable
        {
            public const string TableNameEmpty = "Table name is empty.";
        }

        /// <summary>
        /// Game panel messages.
        /// </summary>
        public class Game
        {

            public const string NotEnoughChipsToPlayThisTable = "You do not have enough chips to play in this table.";
            public const string RedirectToLobbyConfirmation = "Are you sure you want to quit? Quitting may reset your win streak.";
            public const string BeforeGamePlayText = "Make sure to be at the table before the tournament begins! If you are not present at the table, you will not be permitted into the tournament and lose your TC entry.";
            public const string AfterGamePlayText = "If you leave the tournament, you will not be allowed back in.";
            public const string LeaveGamePlayText = "Are you sure you want to quit?\u0003\u0003\u0003\u0003\u0003 Quitting may reset your win streak.\u0003";
            public const string NotEnoughChips = "You do not have enough chips.";

        }



        /// <summary>
        /// In app panel messages.
        /// </summary>
        public class InApp
        {
            public const string RequestForChips = "Your request for <color>" + KeyChips + "</color> chips will be sent. Are you sure?";
            public const string KeyChips = "[CHIPS]";
        }
    }

    /// <summary>
    /// Poker API.
    /// </summary>
    public class LudoAPI
    {
            
       

        public const string KeyResult = "result";
        public const string KeyMessage = "message";
        public const string KeyssStatus = "status";
        public const string KeyStatusSuccess = "success";
      
        public const string KeyPlayerIdUrl = "[PLAYER_ID]";
        public static string UpdateProfileUrl = LudoConstants.BaseUrl + "/rest-api/v1/player/updateUser";
        public const string FieldAuthorization = "Authorization";
        public const string PlayerProfileAvatar = "home/developer/poker-sharks/public/backend/img/users/";

        public const string KeyBody = "body";

        public const string KeyStatusCode = "statusCode";
        public const int StatusCodeSuccessValue = 200;

        public const string StatusAuthorized = "authorized";
        public const string StatusUnauthorized = "unauthorized";
        public const string StatusFailed = "failed";

    }

    /// <summary>
    /// Poker.
    /// </summary>
    public class Ludo
    {
        public static string roomId = "";
        public static string boardId = "";
        public static string TournamentId = "";
        public static string GameId = "";
        public static string TableType = "";

        public static string TableNumber = "";
        public static string GameNumber = "";
        public static string FCMTokenId = "";
        public const float OnOffTurnAnimTime = .25f;

        public static Vector2 OffTurnScale = Vector2.one;
        public static Vector2 OnTurnScale = Vector2.one * 1.15f;

        public const float OwnPlayerPostionMargin = 15f;

        public const string CardAnimationTrigger = "card-flip";
        public const string SpinHandleTrigger = "spin";

        public static double MinBuyinAmount;
        public static double MaxBuyinAmount;

        public static double SmallBlindAmount;
        public static double BigBlindAmount;

        public const float FoldCanvasAlpha = .4f;

        public const float HandMinRank = 0f;
        public const float HandMaxRank = 303f;

        public const float MatchedCardAlpha = 1f;
        public const float NotMatchedCardAlpha = .25f;

        /// <summary>
        /// The card distribution speed.
        /// </summary>
        public const float CardDistributionSpeed = 5f;

        /// <summary>
        /// The player chat display time in seconds.
        /// </summary>
        public const float PlayerChatDisplayTime = 3f;

        /// <summary>
        /// The player Action display time in seconds.
        /// </summary>
        public const float PlayeractionDisplayTime = 1.5f;

        /// <summary>
        /// The dealer message display time.
        /// </summary>
        public const float DealerMessageDisplayTime = 2f;

        /// <summary>
        /// The tip amount.
        /// </summary>
        public static long TipAmount = 10;

        /// <summary>
        /// The refresh table interval in seconds.
        /// </summary>
        public const int RefreshTableInterval = 15;
        /// <summary>
        /// The refresh home online players interval in seconds.
        /// </summary>
        public const int RefreshHomeOnlineInterval = 5;
        /// <summary>
        /// The refresh tournamentRegistretedtable interval in seconds.
        /// </summary>
        public const int RefreshRegistredTableInterval = 2;

        public const string PokerPlayerStatusLeft = "Left";
    }

    public class StoreLinks
    {
        public const string playStoreAppLink = "https://play.google.com/store/apps/details?id=com.wonga.ludo3";
        public const string appleStoreAppLink = "";
    }
    public class ChatRoomLudo
    {   //---------------------------------------------------------------------------------------------------
        //-- Emits Events
        //---------------------------------------------------------------------------------------------------

        public const string ChatRoomGameInit = "ChatRoomGameInit";
        public const string ChatRoomGameInvite = "ChatRoomGameInvite";

        public const string ChatRoomGameInviteResponse = "ChatRoomGameInviteResponse";
        public const string ChatRoomGameReady = "ChatRoomGameReady";

        public const string ChatRoomGameStart = "ChatRoomGameStart";
        //---------------------------------------------------------------------------------------------------
        //-- Broadcast Events
        //---------------------------------------------------------------------------------------------------
        public const string ChatRoomGameRosterUpdate = "ChatRoomGameRosterUpdate";

        public const string ChatRoomQuickTimerTick = "ChatRoomQuickTimerTick";

        public const string ChatRoomQuickTimerExpired = "ChatRoomQuickTimerExpired";
        public const string ChatRoomGameFinished = "ChatRoomGameFinished";
        public const string ChatRoomTableClosed = "ChatRoomTableClosed";

    }
    /// <summary>
    /// Ludo events
    /// </summary>
    public class LudoEvents
    {
        // User Management & Authentication
        public const string OnlineUserList = "OnlineUserList";
        public const string CheckRunningGame = "CheckRunningGame";
        public const string UpdateUserSocketId = "UpdateUserSocketId";
        

        // Game Room & Table Management
        public const string LeaveRunningGame = "LeaveRunningGame";
        public const string JoinTableListing = "JoinTableListing";

        public const string GetAllTableParameters = "GetAllTableParameters";
        public const string EnterRoom = "EnterRoomParameters";
        public const string JoinRoom = "JoinRoom";

        public const string LeaveTableListing = "LeaveTableListing";
        public const string UserSelectGoti = "UserSelectGoti";
        public const string CheckGameStartTime = "CheckGameStartTime";
        public const string UserSelectGotiRemove = "UserSelectGotiRemove";
        public const string SelectedToken = "SelectedToken";
        public const string ExitRoom = "Exitroom";
        public const string LeaveRoom = "LeaveRoom";

        // Game Actions
        public const string JoinGame = "JoinGame";
        public const string RollDice = "RollDice";
        public const string PlayerAction = "PlayerAction";
        public const string KillPlayer = "KillPlayer";
        public const string ReconnectPlayer = "ReconnectPlayer";
        public const string ReconnectGame = "ReconnectGame";
        public const string PlayAgain = "PlayAgain";
        public const string StartGame = "StartGame";
        public const string WinnerPlayers = "WinnerPlayers";
        public const string GetGameDetails = "GetGameDetails";

        // Tournaments
        public const string GetTournamentsData = "GetTournamentsData";
        public const string JoinTournament = "JoinTournament";

        // Player Profile
        public const string Playerprofile = "Playerprofile";
        public const string PlayerProfileInfo = "PlayerProfileInfo";
        public const string UpdateProfile = "UpdateProfile";

        // Payment & Shop
        public const string RechargeRequest = "RechargeRequest";
        public const string GetShopDetails = "GetShopDetails";
        public const string GetWithdrawData = "GetWithdrawData";
        public const string RequestTransfer = "RequestTransfer";
        public const string GetUserWithdrawList = "GetUserWithdrawList";
        public const string GetUserPurchaseHistory = "GetUserPurchaseHistory";
        public const string PurchaseCoinsFromPlaystore = "PurchaseCoinsFromPlaystore";

        // Private Rooms & Friends
        public const string PlayerWithFriend = "PlayerWithFriend";
        public const string SubscribePrivateRoom = "SubscribePrivateRoom";

        //---------------------------------------------------------------------------------------------------
        //-- Broadcast Events
        //---------------------------------------------------------------------------------------------------

        // Table & Room Updates
        public const string UpdateTableListingData = "UpdateTableListingData";
        public const string GetUserSelectGoti = "GetUserSelectGoti";
        public const string BackToLobby = "BackToLobby";

        // Game Timing
        public const string StartTimer = "StartTimer"; // for player wainting timer 30 sedconds
        public const string GameStartTimer = "GameStartTimer";  //4,3,2,
        public const string TurnTimer = "OnTurnTimer";
        public const string TourStartTimer = "TourStartTimer";

        // Game State
        public const string GameStart = "GameStart";
        public const string PlayerInfoList = "PlayerInfoList";
        public const string GameStarted = "GameStarted";
        public const string KeepCurrentPlayer = "KeepCurrentPlayer";

        public const string QuickGameTimerTick = "QuickGameTimerTick";
        // Dice & Actions
        public const string RollDiceDetails = "RollDiceDetails";
        public const string RollingDice = "RollingDice";
        public const string PlayerActionDetails = "PlayerActionDetails";
        public const string SkipMove = "SkipMove";

        // Game Events
        public const string PlayerKill = "PlayerKill";
        public const string PlayerLeft = "PlayerLeft";
        public const string PlayerGameWin = "PlayerGameWin";
        public const string GameFinished = "GameFinished";
        public const string GameFinishedBots = "GameFinishedBots";
        public const string EndOfJourney = "EndOfJourney";
        public const string ResetGame = "ResetGame";
        public const string MaxPlayerReached = "MaxPlayerReached";

        // Game State & Turn Management (Backend Custom Events)
        public const string TurnTimeout = "TurnTimeout";
        public const string GameStatusUpdated = "GameStatusUpdated";
        public const string TurnSwitched = "TurnSwitched";
        public const string TokenKilled = "TokenKilled";
        public const string PlayerDisconnected = "PlayerDisconnected";

        // Messages & Communication
        public const string DisplayMessage = "DisplayMessage";
        public const string SendMessage = "SendMessage";
        public const string MessageList = "MessageList";
        public const string SendEmoji = "SendEmoji";

        // Private Room Events
        public const string PrivateRoomDetails = "PrivateRoomDetails";
        public const string LeavePrivateRoom = "LeavePrivateRoom";
        public const string ReconnectPrivateDashboard = "ReconnectPrivateDashboard";

        // Leaderboard & Stats
        public const string GetPlayerDefaultTurnCount = "GetPlayerDefaultTurnCount";
        public const string LeaderBoard = "LeaderBoard";
        public const string PlayerProfileInfo_Broadcast = "PlayerProfileInfo";

        // App Content & Info
        public const string TermsDetails = "TermsDetails";
        public const string RulesDetails = "RulesDetails";
        public const string PrivacyPolicyDetails = "PrivacyPolicyDetails";
        public const string NoticeDetails = "NoticeDetails";
        public const string GetServerMaintenanceStatus = "GetServerMaintenanceStatus";
        public const string BannerList = "BannerList";
        public const string InviteAndEarnShareText = "InviteAndEarnShareText";
    }
    /*  public class LudoEvents
      {
          public const string onlineUserList = "onlineUserList";
          public const string checkRunningGame = "checkRunningGame";
          public const string WinnerPlayers = "WinnerPlayers";
          public const string GetBlockedPlayers = "GetBlockedPlayers";
          public const string FindPlayerToBlock = "FindPlayerToBlock";
          public const string BlockPlayer = "BlockPlayer";
          public const string UnBlockPlayer = "UnBlockPlayer";


          public const string CreateTicket = "CreateTicket";
          public const string UpdateTicketReadStatus = "UpdateTicketReadStatus";
          public const string GetSupportData = "GetSupportData";

          public const string leaveRunningGame = "leaveRunningGame";
          public const string JoinTableListing = "JoinTableListing";
          public const string getAllTable = "getAllTableParameters";
          public const string LeaveTableListing = "LeaveTableListing";
          public const string userSelectGoti = "userSelectGoti";
          public const string JoinRoom = "JoinRoom";
          public const string CheckGameStartTime = "CheckGameStartTime";
          public const string userSelectGotiRemove = "userSelectGotiRemove";
          public const string SelectedToken = "SelectedToken";
          public const string enterRoom = "enterRoomParameters";
          public const string exitRoom = "exitroom";
          public const string JoinGame = "JoinGame";
          public const string RollDice = "RollDice";
          public const string PlayerAction = "PlayerAction";
          public const string getTournamentsData = "getTournamentsData";
          public const string updateUserSocketId = "updateUserSocketId";
          public const string verifyOTP = "verifyOTP";
          public const string ResendOTP = "ResendOTP";
          public const string registerDataSocket = "registerDataSocket";
          public const string checkMobileNumberSocket = "checkMobileNumberSocket";
          public const string LeaveRoom = "LeaveRoom";
          public const string ReconnectPlayer = "ReconnectPlayer";
          public const string ReconnectGame = "ReconnectGame";
          public const string PlayAgain = "PlayAgain";
          public const string Playerprofile = "Playerprofile";
          public const string RechargeRequest = "RechargeRequest";
          public const string GetShopDetails = "GetShopDetails";
          public const string JoinTournament = "JoinTournament";
          public const string GetWithdrawData = "GetWithdrawData";
          public const string RequestTransfer = "RequestTransfer";
          public const string getUserWithdrawList = "getUserWithdrawList";
          public const string getUserPurchaseHistory = "getUserPurchaseHistory";
          public const string PlayerProfileInfo = "PlayerProfileInfo";
          public const string UpdateProfile = "UpdateProfile";
          public const string GetGameDetails = "GetGameDetails";
          public const string PlayerWithFriend = "PlayerWithFriend";
          public const string SubscribePrivateRoom = "SubscribePrivateRoom";
          public const string StartGame = "StartGame";
          public const string GetOTPForMobileVerification = "getOtp";


          //---------------------------------------------------------------------------------------------------
          //--Broadcasts
          //public const string UpdateTableListingData = "UpdateTableListingData";
          public const string UpdateTableListingData = "OnUpdateTableListingData";
          public const string getUserSelectGoti = "getUserSelectGoti";
          public const string startTimer = "startTimer";
          public const string backToLobby = "backToLobby";
          public const string gameStart = "gameStart";
          public const string PlayerInfoList = "playerInfoList";
          public const string gameStartTimer = "gameStartTimer";
          public const string GameStarted = "GameStarted";
          public const string TurnTimer = "OnTurnTimer";
          public const string KeepCurrentPlayer = "KeepCurrentPlayer";
          public const string RollDiceDetails = "rollDiceDetails";
          public const string rollingDice = "rollingDice";
          public const string PlayerActionDetails = "playerActionDetails";
          public const string PlayerKill = "PlayerKill";
          public const string PlayerLeft = "PlayerLeft";
          public const string PlayerGameWin = "PlayerGameWin";
          public const string GameFinished = "GameFinished";
          public const string GameFinishedBots = "GameFinishedBots";
          public const string EndOfJourney = "EndOfJourney";
          public const string ResetGame = "ResetGame";
          public const string displayMessage = "displayMessage";
          public const string PrivateRoomDetails = "PrivateRoomDetails";
          public const string LeavePrivateRoom = "LeavePrivateRoom";
          public const string ReconnectPrivateDashboard = "ReconnectPrivateDashboard";
          public const string GetPlayerDefaultTurnCount = "GetPlayerDefaultTurnCount";
          public const string LeaderBoard = "LeaderBoard";
          public const string sendMessage = "sendMessage";
          public const string messageList = "messageList";
          public const string playerProfileInfo = "PlayerProfileInfo";
          public const string sendEmoji = "sendEmoji";
          public const string skipMove = "skipMove";
          public const string TermsDetails = "TermsDetails";
          public const string RulesDetails = "RulesDetails";
          public const string PrivacyPolicyDetails = "PrivacyPolicyDetails";
          public const string NoticeDetails = "NoticeDetails";
          public const string GetServerMaintenanceStatus = "GetServerMaintenanceStatus";

          public const string tourStartTimer = "tourStartTimer";
          public const string maxPlayerReached = "maxPlayerReached";

          // Stephen's Added Event
          public const string BannerList = "BannerList";
          public const string NewPlayerRegistration = "NewPlayerRegistration";
          public const string InviteAndEarnShareText = "InviteAndEarnShareText";
          public const string PurchaseCoinsFromPlaystore = "PurchaseCoinsFromPlaystore";

          public const string GuestLogin = "GuestLogin";

      }*/

    public static class Animation
    {
        public const string PAWN_HIGHLIGHT_ANIMATION = "pawnHighlighter";
        public const string BOARD_HIGHLIGHT_ANIMATION = "BoardHightlight";
    }

    public class Tag
    {
        public const string AI_target = "AI_target";
        public const string TagSlice = "Slice";
        public const string TagPlayer = "Player";
        public const string TagAI = "AI";
        public const int StopReelAfterTimeIfError = 10;
        public const string LuckyDrawWinTextReset = "LuckyDrawHeaderResetter";
        public const string LuckyDrawReelResetter = "LuckyDrawReelResetter";
    }

    public class ConstantURLs
    {
        public static string registerAndGetRewardText = string.Empty;
        public static string privacy_policy = string.Empty;
        public static string terms_conditions = string.Empty;
    }
}

public enum SERVER
{
    Production,
    info,
    Staging,
    Developer,
    Custom
}


