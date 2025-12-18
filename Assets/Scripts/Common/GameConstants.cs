
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;




public class GameConstants : MonoBehaviour
{
    //    Poker Time Server 3 (Demo)

    //IP: 3.137.10.123

    public static int TURN_TIME = 5; /*20;*/

    public static int timerStart = 0;
    public static string sideShowRequesterId = "";
    public static float maxChaal = 0;


    #region WEB
    public const float NETWORK_CHECK_DELAY = 2f;
    public const int API_RETRY_LIMIT = 5;
    public const int API_TIME_OUT_LIMIT = 50;

public class ProjectScene
{
        public const string LudoGame = "Ludo Ultimate";
}

    public const string BASE_URL = "https://ludobaar.com";//"https://ludo-xj4l.onrender.com/api";

    public const string API_URL = BASE_URL;

    public static string Socket_URL = "https://ludobaar.com/socket/";


    public static string FormatUrl(string template, params (string key, object value)[] parameters)
    {
        string url = template;
        foreach (var param in parameters)
        {
            url = url.Replace("{" + param.key + "}", param.value.ToString());
        }
        return url;
    }


    public static readonly Dictionary<int, string> GAME_URLS = new Dictionary<int, string>
    {
        { (int)RequestType.SendSignupOTP,  API_URL+"/api/auth/send-signup-otp" },// send otp for email verification during signup
        { (int)RequestType.VerifyOTP, API_URL+"/api/auth/verify-signup-otp" },
        { (int)RequestType.Login, API_URL +"/api/auth/login"}, //+"/auth/login"},
        { (int)RequestType.Refresh, API_URL +"/api/auth/refresh"},
        { (int)RequestType.UserProfile_GetProfileByUserID, API_URL +"/api/user/profile/{uid}"},
        { (int)RequestType.ChatRoom_CreatechatRoom, API_URL +"/api/chat-rooms"},
        { (int)RequestType.ChatRoom_PopularChatList, API_URL +"/api/chat-rooms/popular"},
        { (int)RequestType.ChatRoom_JoinchatRoombyCode, API_URL +"/api/chat-rooms/join-by-code"},
        { (int)RequestType.ChatRoom_LeaveRoomByID, API_URL +"/api/chat-rooms/{roomId}/leave"},
        { (int)RequestType.ChatRoom_SettingsByID, API_URL +"/api/chat-rooms/{roomId}/settings"},
        { (int)RequestType.ChatRoom_InviteByHostByRoomID, API_URL +"/api/chat-rooms/{roomId}/invites"},
        { (int)RequestType.ChatRoom_SendRequestToHost, API_URL +"/api/user/profile/{uid}"},
        { (int)RequestType.GiftsGetCategories, API_URL +"/api/gifts/categories"},
        { (int)RequestType.GiftsGetCategoriesByID, API_URL +"/api/gifts/{categoryId}"},
        { (int)RequestType.GiftsSend, API_URL +"/api/gifts/send}"},
        { (int)RequestType.GiftsGetSent, API_URL +"/api/gifts/sent}"},
        { (int)RequestType.GiftsReceived, API_URL +"/api/gifts/received}"},
        { (int)RequestType.Friend_search,API_URL +"/api/friend/search" },
        { (int)RequestType.Friend_request,API_URL +"/api/friend/request" },
        { (int)RequestType.Friend_requests,API_URL +"/api/friend/requests" },
        { (int)RequestType.Friend_request_respond,API_URL +"/api/friend/request/respond" },
        { (int)RequestType.Friend_request_delete,API_URL +"/api/friend/request/delete" },
        { (int)RequestType.Friend_list,API_URL +"/api/friend/list" },
        { (int)RequestType.Friend_unfriend,API_URL +"/api/friend/unfriend" },
        { (int)RequestType.ChatRoom_ParticipantsByID, API_URL +"/api/chat-rooms/{roomId}/participants"},
        { (int)RequestType.bcg_GameGetplayerProgress, API_URL +  "/api/mini-games/player/data" },
    { (int)RequestType.bcg_GameReset, API_URL +"/api/mini-games/player/reset" },
    { (int)RequestType.uploadImage, API_URL +"/api/upload/signed-url"},
      { (int)RequestType.socketdisconnection, API_URL +"/api/chat-rooms/socket-disconnection/{userId}" }
        // ... add all other URLs here
    };
    public class EmitEvents
    {
        //Friends emits

        public const string f_search_user = "friend_search_user";
        public const string f_send_request = "friend_send_request";
        public const string f_list_requests = "friend_list_requests";
        public const string f_respond_request = "friend_respond_request";
        public const string f_delete_request = "friend_delete_request";
        public const string f_list_friends = "friend_list_friends";
        public const string f_unfriend = "friend_unfriend";



        public const string SendGift = "send_gift";
        public const string WinnerPlayers = "WinnerPlayers";
        public const string joinroom = "join-room-realtime";
        public const string SendGroupTextmessage = "send-message-realtime";
        public const string userapplyseat = "user-apply-seat";
        public const string userleaveseat = "user-leave-seat";
        public const string seatlockmic = "seat-lock-mic";
        public const string seatunlockmic = "seat-unlock-mic";
        public const string leaveroom = "leave-room";
        public const string voicequalitychange = "voice-quality-change";
        public const string joinvoicesession = "join-voice-session";
        public const string leavevoicesession = "leave-voice-session";
        public const string startvoicechat = "start-voice-chat";
        public const string endvoicechat = "end-voice-chat";
        public const string voicespeakingstart = "voice-speaking-start";
        public const string voicespeakingstop = "voice-speaking-stop";
        public const string seatinvite = "seat-invite";
        public const string getseatwaitinglist = "get-seat-waiting-list";

        public const string userRespondSeatInvitation = "user-respond-seat-invitation";

    }
    public class ONEvents
    {

        public const string f_search_result = "friend_search_result";
        public const string f_search_error = "friend_search_error";
        public const string f_request_sent = "friend_request_sent";
        public const string f_request_received = "friend_request_received";
        public const string f_request_error = "friend_request_error";
        public const string f_requests_list = "friend_requests_list";
        public const string f_requests_error = "friend_requests_error";
        public const string f_request_responded = "friend_request_responded";
        public const string f_request_accepted = "friend_request_accepted";
        public const string f_respond_error = "friend_respond_error";
        public const string f_request_deleted = "friend_request_deleted";
        public const string f_delete_error = "friend_delete_error";
        public const string f_list = "friends_list";
        public const string f_list_error = "friends_list_error";
        public const string f_unfriended = "friend_unfriended";
        public const string f_unfriended_by = "friend_unfriended_by";
        public const string f_unfriend_error = "friend_unfriend_error";


        public const string gift_received = "gift_received";
        public const string gift_sent_in_room = "gift-sent-in-room";

        public const string received_GroupTextmessage = "message_sent_success";
        public const string userapplyseatsuccess = "user_apply_seat_success";
        public const string OnlyforHost_seatapplicationreceived = "seat-application-received";
        public const string userleaveseatsuccess = "user_leave_seat_success";
        public const string seatlockmicsuccess = "seat_lock_mic_success";
        public const string voicechatstarted = "voice_chat_started";
        public const string voicechatended = "voice_chat_ended";
        public const string voicesessionjoined = "voice_session_joined";
        public const string voicesessionleft = "voice_session_left";
        public const string voiceparticipantjoined = "voice-participant-joined";
        public const string voiceparticipantleft = "voice-participant-left";
        public const string voiceuserspeakingstart = "voice-user-speaking-start";
        public const string voiceuserspeakingstop = "voice-user-speaking-stop";
        public const string voicequalitychanged = "voice_quality_changed";
        public const string voicequalitychangedbyuser = "voice-quality-changed-by-user";

        public const string seatwaitinglistupdated = "seat-waiting-list-updated";
        public const string seatInvitationReceived = "seat-invitation-received";
        public const string user_respond_seat_invitation_success = "user_respond_seat_invitation_success";

    }
    public class BroadcastEvents
    {
        public const string joinroom = "join-room-realtime";
        public const string userleftseat = "user-left-seat";
        public const string seatlocked = "seat-locked";
        public const string userleftrealtime = "user-left-realtime";
        public const string newmessagerealtime = "new-message-realtime";
        public const string useracceptedseatinvitation = "user-accepted-seat-invitation";
        public const string seatapplicationresponsebroadcast = "seat-application-response-broadcast";
        public const string giftsentinroom = "gift-sent-in-room";
        public const string seatunlocked = "seat-unlocked";

    }
    public class WebRTC_ONEvents
    {

        public const string webrtcofferreceived = "webrtc-offer-received";
        public const string webrtcanswerreceived = "webrtc-answer-received";
        public const string webrtcicecandidatereceived = "webrtc-ice-candidate-received";
    }
    public class WebRTC_EmitEvents
    {

        public const string webrtcoffer = "webrtc-offer";
        public const string webrtcanswer = "webrtc-answer";
        public const string webrtcicecandidate = "webrtc-ice-candidate";
    }
    public const string KeyResult = "result";
    public const string KeyMessage = "message";
    public const string KeyssStatus = "status";
    public const string KeyStatusSuccess = "success";
    #region LudoGame
  
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


    /// <summary>
    /// Ludo events
    /// </summary>
    public class LudoEvents
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
        public const string getAllTable = "getAllTable";
        public const string LeaveTableListing = "LeaveTableListing";
        public const string userSelectGoti = "userSelectGoti";
        public const string JoinRoom = "JoinRoom";
        public const string CheckGameStartTime = "CheckGameStartTime";
        public const string userSelectGotiRemove = "userSelectGotiRemove";
        public const string SelectedToken = "SelectedToken";
        public const string enterRoom = "enterRoom";
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
        public const string UpdateTableListingData = "UpdateTableListingData";
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

    }

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

    public const int CHAT_BUBBLE_DISPLAY_TIMER = 4; //5
    #endregion
}
#endregion
public static class GameStaticData
{
    public static int playerCount = 0;
    public static GamesType gamesType = GamesType.Player2;
    public static int numberGoties = 1;
    public static int vsComputerColorIndex = 0;
    public static bool commingBackFromGame = false;
    public static List<string> playersName;

    // Stephen update
    public static bool isRegisteredPlayer = false;
}

public enum GamesType
{
    Tournament = 0,
    Player2 = 1,
    Player3 = 2,
    Player4 = 3,
    QuickPlay = 4,
    LocalMultiplayer = 5,
    WithComputer = 6,
    Private = 7,
}
[System.Serializable]
public enum RequestType
{
    SendSignupOTP,
    VerifyOTP,
    Login,
    Refresh,
    GetUserDetails,
    UserProfile_GetProfileByUserID,
    ChatRoom_CreatechatRoom,
    ChatRoom_PopularChatList,
    ChatRoom_JoinchatRoombyCode,
    ChatRoom_LeaveRoomByID,
    ChatRoom_ParticipantsByID,
    ChatRoom_SettingsByID,
    ChatRoom_InviteByHostByRoomID,
    ChatRoom_SendRequestToHost,
    GiftsGetCategories,
    GiftsSend,
    GiftsGetSent,
    GiftsReceived,
    Friend_search,
    Friend_request,
    Friend_requests,
    Friend_request_respond,
    Friend_request_delete,
    Friend_list,
    Friend_unfriend,
    bcg_GameGetplayerProgress,
    bcg_amePostplayerProgress,
    bcg_GameReset,
    uploadImage,
    socketdisconnection,
    GiftsGetCategoriesByID
}