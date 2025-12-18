using System.Collections.Generic;
using System;
//Chat panel ludo class
[System.Serializable]
public class RoomResponse
{
    public bool success;
    public RoomDataChat data;
    public long timestamp;
}

[System.Serializable]
public class RoomDataChat
{
    public int roomId;
    public string hostId;
    public string mode;
    public int? durationMinutes;    // Nullable int
    public int maxPlayers;
    public List<EligiblePlayer> eligiblePlayers;
}

[System.Serializable]
public class EligiblePlayer
{
    public string userId;
    public string username;
    public string profilePic;
    public string role;
    public bool isOnline;
}

[Serializable]
public class ChatRoomGameRosterUpdate
{
    public int roomId;
    public string hostId;
    public string mode;
    public string status;                 // pending | inviting | ready | starting | started | finished
    public int? durationMinutes;          // nullable int
    public int maxPlayers;

    public List<EligiblePlayer> eligiblePlayers;
    public List<InvitedPlayer> invitedPlayers;

    public List<string> acceptedPlayers;
    public List<string> declinedPlayers;
}


[Serializable]
public class InvitedPlayer
{
    public string playerId;
    public string status;                 // pending | accepted | declined
    public string username;
}

[Serializable]
public class InviteResponse
{
    public bool success;
    public InviteData data;
    public long timestamp;
}

[Serializable]
public class InviteData
{
    public int roomId;
    public List<InvitedPlayer> invitedPlayers;
    public List<string> acceptedPlayers;
}
[Serializable]
public class PlayerStatusResponse
{
    public bool success;
    public PlayerStatusData data;
    public long timestamp;
}

[Serializable]
public class PlayerStatusData
{
    public int roomId;
    public string playerId;
    public string status;                 // accepted | declined | pending
    public List<string> acceptedPlayers;
}
[Serializable]
public class TableCreateResponse
{
    public bool success;
    public TableCreateData data;
    public long timestamp;
}

[Serializable]
public class TableCreateData
{
    public int roomId;
    public string tableId;
    public string boardId;
    public string gameId;
    public string mode;
    public int? durationMinutes;  // nullable
    public bool isPrivate;
    public string tableName;

    public List<TablePlayer> players;
}

[Serializable]
public class TablePlayer
{
    public string userId;
    public string colorName;
    public int colorId;
}
[Serializable]
public class TableDatac
{
    public int roomId;
    public string tableId;
    public string boardId;
    public string gameId;
    public string mode;
    public int? durationMinutes;   // nullable
    public bool isPrivate;
    public string tableName;

    public List<TablePlayer> players;
}

[Serializable]
public class BoardTimerData
{
    public int roomId;
    public string boardId;
    public int remainingSeconds;
}
[Serializable]
public class GameResultData
{
    public int roomId;
    public string tableId;
    public string boardId;
    public string gameId;

    public string reason;    // "completed" | "timeout" | "cancelled"

    public List<GameWinner> winners;
}

[Serializable]
public class GameWinner
{
    public string playerId;
    public string playerName;
    public int position;
    public int winningAmount;
    public int score;
}

//*******************************************************************
[Serializable]
public class PokerEventResponse
{
    public string status;
    public string result;
    public string message;
    public string transactionId;
}

[Serializable]
public class PokerEventResponse<T> where T : class
{
    public string status;
    public T result;
    public string message;
    public string statusCode;
}

[Serializable]
public class PokerEventListResponse<T> where T : class
{
    public PokerEventListResponse()
    {
        result = new List<T>();
    }

    public string status;
    public List<T> result;
    public string message;
    public string statusCode;
}


[Serializable]
public class EventResponse<T> where T : class
{
    public string status;
    public string message;
    public T result;
}

[Serializable]
public class InAppResponse<T> where T : class
{
    public string status;
    public string message;
    public T result;
}


[Serializable]
public class EventListResponse<T> where T : class
{
    public string status;
    public string message;
    public List<T> result;
}

[Serializable]
public class HomeDataItem
{
    public int playerOnline2;
    public int playerOnline3;
    public int playerOnline4;
    public int totalPlayers;
}
[Serializable]
public class TableDataItem
{
    public string nameSpace = "";
    public int tableType = 0;
    public int selectedGoti = 0;
    public List<GetAllTableData> tableData = new List<GetAllTableData>();

    public int goti1OnlinePlayer = 0;
    public int goti2OnlinePlayer = 0;
    public int goti3OnlinePlayer = 0;
    public int goti4OnlinePlayer = 0;
}
[Serializable]
public class GetAllTableData
{
    public string _id;
    public int onlineUser;
    public double entryFee;
    public double totalPrice;
    public double totalCom;
    public string goti;
}

[Serializable]
public class GmailUserData
{
    public string name;
    public string email;
    public string token;
    public string profilePic;
    public string phoneno;
}

[Serializable]
public class SearchingJoinGameResponse : GetAllTableData
{
    public string status;
    public GameStartDataResp runningGameData;

}
public static class EventResponse
{
    public const string STATUS_SUCCESS = "success";
    public const string STATUS_FAIL = "fail";
    public const string STATUS_SOCIALFAIL = "socialFail";

    public const string STATUS_MAINTENANCE = "maintenance";
}


public static class InAppResponse
{
    public const string STATUS_SUCCESS = "success";
    public const string STATUS_FAIL = "fail";
}

[Serializable]
public class InAppResult
{
    public string status;
    public string result;
    public string message;
}


[Serializable]
public class PlayerDetails
{
    public string username; //"superadmin",
    public string playerId; //hjkl7490378fgjed74jf7845ujt85u57594jkrtui5854j
    public string userId; //"12323",
    public string name; //"Admin",
    public string email; //"admin@gmail.com",
    public string password; //"e10adc3949ba59abbe56e057f20f883e",
    public string phnoCode; //"+91",
    public string phno; //"1234567890",
    public string profilePic; //"202107141043455516.jpg",
    public int avatar; //"0",
    public string role; //"admin",
    public string loginToken; //"",
    public string socketId; //"",
    public string country; //"IN",
    public string reference_refferal_id; //null,
    public string reference_refferal_code; //"",
    public string refferal_code; //"",
    public string referral_code_amount; //0,
    public string referral_code_join_user; //0,
    public double totalBalance; //"0",
    public string deviceToken; //"",
    public string deviceType; //"web",
    public string createdAt; //"2021-07-02T08:06:06.835Z",
    public string updatedAt; //"2021-07-02T08:06:06.835Z",
    public string _id; //"60dec906da3cd603a3a74559"
    public bool isActive; //true,
    public bool isDeleted; //false,
    public bool isLogin; //true,
    public bool isOnline; //false,
    public bool isVerified; //false,
    public bool IscheatEnable = false; //false,
    public bool isGmailLogin = false;
    public int refer;
    public bool noticeStatus = false;
    public string fcmRegistrationToken;
    public double defualtCashForNewPlayer;
    public bool isEmailVerified;
    public string otpSenderId;

    public bool isFirstDeposit;

    public bool isRestricted = false;

    // public bool countdown;

    public string noticeText = "";
    public List<string> promoImages = new List<string>();

    public bool isWalletEnable = true;

    public string metaUrl = "";
    public string twitterUrl = "";
    public string instaUrl = "";
    public string pinterestUrl = "";

    public string whatsappUrl = "https://api.whatsapp.com/send?phone=+916351050358&text=hello";//6351050358//"https://api.whatsapp.com/send?phone=+916239089406&text=hello";
    public string telegramUrl = string.Empty;
    public string youtubeUrl = "https://youtube.com/channel/UCJdfXCZu2jT6Yv0PHo2Ug2w";
    public string customerCareNumber = "+916239089406";

    public string updatedAPKLink = "https://play.google.com/store/apps/details?id=com.wonga.ludo3";
    public double win = 0;
    public bool isRegistered = false;
    public string registerAndGetRewardText = string.Empty;
    public string privacy_policy = string.Empty;
    public string terms_conditions = string.Empty;
    public List<string> states;
    public float bonusAmount;
    public bool isUnreadTicket;
    public int maxBlockPlayersCount = 5;
}

[Serializable]
public class TournamentListItemData
{
    public string _id; //"60eee3e60b1d0a0fdc6594d5",
    public string name; //"1v1",
    public int entry_fee; //50,
    public int price_pool; //98,
    public int firstRank; //0,
    public int secondRank; //0,
    public int playerOnline; //0
    public int startTime;
    public bool joinTable; //false,
    public string createdAt; //"2021-07-14T13:14:57.599Z",
    public int time; //1

    public DateTime GetTimeOfCreating
    {
        get
        {
            return Convert.ToDateTime(createdAt);
        }
    }
}

[Serializable]
public class UpdateTableListingData
{
    public string tableId = "";
    public int onlinePlayer = 0;

    public int goti1OnlinePlayer = 0;
    public int goti2OnlinePlayer = 0;
    public int goti3OnlinePlayer = 0;
    public int goti4OnlinePlayer = 0;
}

[Serializable]
public class UserSelectGoti
{
    public string TableId; //"61090aed73c6f3231e632bdd",
    public string uniqueId; //"FN2DSARFGX22EHB8",
    public UserSelectGotiPlayers[] players;
}

[Serializable]
public class UserSelectGotiPlayers

{
    public int timer; //1000,
    public string selectedGoti; //-1
    public string socketId; //"BOiqhdPpye6OhLscAAAB",
    public string playerId; //"610a4f357839213047a6308e",
    public string userSelectTableId; //"61090aed73c6f3231e632bdd",
}

[Serializable]
public class PlayersItem
{
    public int timer;
    public string socketId;
    public string playerId;
    public string userSelectTableId;
    public string selectedGoti;
}
[Serializable]
public class backToLobbyResult
{
    public string TableId;
    public List<PlayersItem> players;
}

[Serializable]
public class TransactionData
{
    public string message;
    public string time;
    public string transactionId;
    public double amount;
    public string status;
    public bool isFirstDeposit;
}


[Serializable]
public class StartTimerData
{
    public string tableId; //610a340f773f4f2bd74a6d92
    public int timer; //1
    public List<Joinersusers> joiners;
}
[Serializable]
public class Joinersusers
{

    public string playerName;
    public string id;
    public int avatar;
    public string profilePic;
}

[Serializable]
public class GameStartDataResp
{
    public bool GameStart = true;
    public string selectedGoti = "";
    public int playerCount = 0;
    public List<string> players;
    public string tableId = "";
    public string boardId = "";
    public string tournamentId = "";
    public string uniqueId = "";
    public string nameSpace = "";
    public double entryFee = 0;
    public double priceAmount = 0;
}

[Serializable]
public class GameStartData
{
    public string roomName;
    public string gameDetail;
    public HistoryDetail[] historyDetail;
}

[Serializable]
public class HistoryDetail
{
    public PlayerDetails userdetail;
    public UserJoinHistoryDetail userJoinHistoryTabelDetail;
}

[Serializable]
public class UserJoinHistoryDetail
{
    public int bet_price; //500
    public int table_type; //2
    public string user_goti; //yellow
    public string table_id; //610a340f773f4f2bd74a6d92
    public string table_code; //1358097
    public string user_id; //610a4f357839213047a6308e
    public string game_id; //610e1e08dce55354e0ca7e2
}

[Serializable]
public class UserSelectGotiRemove
{
    public string uniqueId; //1Y98DBQT6GTD2UMF
    public string removeGotiColor; //green
}

[Serializable]
public class EnterRoomData
{
    public string uniqueId; //9KCQOZ8F2QM5TIER
    public string[] selectedColor; //["red"]
    public List<string> colorName;
    public List<int> colorId;
}

[Serializable]
public class SelectedToken
{
    public List<string> colorName;
    public List<int> colorId;
}

[Serializable]
public class TokensItem
{
    public int id;
    public int startPos;
    public int currentPos;
    public int distance;
    public int inAtTurn;
}
[Serializable]
public class PlayerInfoItem
{
    public string username;
    public string mobile;
    public string id;
    public int seatIndex;
    public int avatar;
    public string profilePic;
    public string tokenColor;
    public int colorIndex;
    public List<TokensItem> tokens;
}
[Serializable]
public class PlayerInfoList
{
    public string boardId;
    public List<PlayerInfoItem> playerInfo;
    public string gamePlayercounts;
    public string gamePlaySelectedGoti;
    public List<WinnerPrize> winnerPrizeData;
}

[Serializable]
public class WinnerPrize
{
    public int Rank;
    public double amount;
}

[Serializable]
public class gameStartTimerData
{
    public int timer;
    public string message;
    public string playerId;
    public string boardId;
    public string boardNumber;
}

[Serializable]
public class TurnTimerData
{
    public string playerId; //612f241abc0cdd168fbb58b6
    public string boardId; //612f3b63bc0cdd168fbb5bbe
    public float timer; //29
    public float maxTimer; //30
    public bool diceRolled;
    public TurnPlayerData turnPlayerData;
}

[Serializable]
public class TurnPlayerData
{
    public int diceValue = 0;
    public List<TokensItem> tokens = new List<TokensItem>();
}

[Serializable]
public class RollDiceDetails
{
    public PawnData[] readyToMove; //[],
    public string boardId; //board.id
    public string playerId; //board.id
    public int diceValue; //diceValue
    public bool isRoll = true;
}

[Serializable]
public class PawnData
{
    public int id; //8,
    public int startPos; //8,
    public int currentPos; //8,
    public int distance; //0,
    public int inAtTurn; //-1,
    public string color; //"blue"
}
[Serializable]
public class PlayerActionDetails
{
    public int tokenId;
    public string playerId;
    public int currentDistance;
    public int stepsToMove;
    public int fromPosition;
    public int toPosition;
    public string boardId;
    public List<PlayerScore> scores;
}
[Serializable]
public class PlayerScore
{
    public string playerId;
    public int score;
}
[Serializable]
public class Attacker
{
    public string playerId;
    public int tokenId;
    public int currentDistance;
}
[Serializable]
public class KilledPlayer
{
    public string playerId;
    public int tokenId;
    public int currentDistance;
}

[Serializable]
public class PlayerKillDetails
{
    public Attacker attacker;
    public KilledPlayer killedPlayer;
    public string boardId;
}
[Serializable]
public class Winners
{
    public string playerId;
    public string playerName;
    public string mobile;
    public double amount;
    public int rank;
    public int winnerSeatIndex;
    public int commission;
}
[Serializable]
public class PlayerGameWin
{
    public Winners winners;
    public string boardId;
    public string shareMessage;
}
[Serializable]
public class PlayerGameWinBots
{
    public List<Winners> winners;
    public string boardId;
    public bool isTournament = false;
}
[Serializable]
public class EndOfJourneyData
{

    public string playerId;
    public string boardId;

    public string diceKey = "";
}
[Serializable]
public class PlayAgainData
{
    public string playerId;
    public string tableId;
    public string userSelectGoti;
    public double entryFee = 0;
    public double priceAmount = 0;
}
[Serializable]
public class QuickGameTimer
{
    public string boardId;//  "boardId": "board-uuid",
    public string gameId; // "gameId": "game-uuid",
    public int remainingSeconds;     // "remainingSeconds": 480,
    public int remainingMinutes;       // "remainingMinutes": 8,
    public int totalSeconds;        //"totalSeconds": 480
}
[Serializable]
public class Profile
{
    public string playerId;
    public string email;
    public string username;
    public int avatar;
    public double chips;
    public long mobile;
}
[Serializable]
public class ShopResultData
{
    public string url = "";
    public string upiUrl = "";
    public string action_url = "";

    public string status = "";
    public string orderToken = "";
    public string paymentLink = "";
    public string upiPayload = "";
    public string message = "";
}
[Serializable]
public class ShopDetailsData
{
    public string _id;
    public int coin;
    public int amount;
    public bool isExtraBonus = false;
}
[Serializable]
public class GetWithdrawData
{
    public double depositAmount;
    public double winAmount;
    public double processingFee;

    public bool upiOption = true;
    public bool bankAccountOption = true;
    public bool paytmOption = false;

    // stephen Update
    public string upi;
    public string accountNumber;
    public string ifscCode;

    public string defaultOption = ""; // "upi", "paytm", "bank"

    public bool isChargeable;
    public double minAmount;
    public double maxAmount;
    public WDupiCharges upiCharges;
    public WDbankCharges bankCharges;

    [Serializable]
    public class WDupiCharges
    {
        public double OneToThousand = 1;
        public double aboveThousand = 1;
    }

    [Serializable]
    public class WDbankCharges
    {
        public double OneToThousand = 1;
        public double aboveThousand = 1;
    }
}
[Serializable]
public class getUserWithdrawListData
{
    public string _id;
    public string transactionId;
    public string createdAt;
    public string adminstatus;
    public string bankstatus;
    public double amount;
}
[Serializable]
public class getPurchaseListData
{
    public string _id;
    public string orderId;
    public string createdAt;
    public string status;
    public string bankstatus;
    public double amount;
}
[Serializable]
public class PlayerProfileInfo
{
    public string _id;
    public string refferal_code;
    public int totalWon;
    public int twoPlayerWin;
    public int threePlayerWin;
    public int fourPlayerWin;
    public string winRate;
    public string tournamentWin;
    public double totalEarn;
    public string referEarn;
    public string username;
    public int privateWin;
}
[Serializable]
public class PlayWithFriendResult
{
    public string nameSpace;
}
[Serializable]
public class JoinPrivateRoomResult
{
    public string uniqueId;
    public string boardId;
    public int tableId;
    public string nameSpace;
    public int turnTime;
}
[Serializable]
public class OnPrivateRoomDetailsData
{
    public List<JoinersItem> joiners;
    public string createrId;
    public string passcode;
    public string boardId;
}
[Serializable]
public class JoinersItem
{
    public string id;
    public string playerName;
    public int avatar;
    public string profilePic;
}
[Serializable]
public class gameHistoryData
{
    public string date;
    public double amount;
    public string win;
    public double coins;
    public string name;
}
[Serializable]
public class referResultItemData
{
    public string _id;
    public int totalPlayed;
    public double totalEarn;
    public string createdAt;
    public string username;
}

[Serializable]
public class LeaderboardResultData
{
    public string winnerOfMonth;
    public List<LeaderboardResult> list;
    public string banner;
}
[Serializable]
public class LeaderboardResult
{
    public string _id;
    public int rank;
    public string name;
    public string mobile;
    public double amount;
}
[Serializable]
public class LuckyDrawResponse
{
    public string method;
    public int prize;
    public double amount;
    public string status;
    public string message;
}
[Serializable]
public class skipstepsresp
{
    public int turnCount;
}
[Serializable]
public class checkRunningGameData
{
    public GameStartDataResp gamestart;
    public string gameType;
}

[Serializable]
public class updateProfileResult
{
    public string playerId;
    public string profilePic;
    public int avatar;

}

[Serializable]
public class MessageListItem
{
    public string message;
}

[Serializable]
public class MessageListResult
{
    public List<MessageListItem> messageList;
}
[Serializable]
public class MessageDisplayData
{
    public string playerId;
    public string message;
}

[Serializable]
public class SendEmoji
{
    public string senderId;
    public string receiverId;
    public int emojiId;
}

[Serializable]
public class ReportListData
{
    public string status;
    public List<Result> result;
    public Total total;
    public string message;

    [Serializable]
    public class Total
    {
        public int totalPurchase;
        public int totalReffer;
        public int totalBonus;
        public double totalWin;
        public double totalBet;
    }
    [Serializable]
    public class Result
    {
        public string _id;
        public string date;
        public int purchase;
        public double bet;
        public double win;
        public int reffer;
        // public int other;
        public int bonus;
    }
}

[Serializable]
public class CmsResult
{
    public string status;
    public string result;
    public string message;
}

[Serializable]
public class MaintenaceResp
{
    public string status;
    public string message;
    public float minutes = 0;
}

[Serializable]
public class AppVersionData
{
    public PlatformVersionData playStore;
    public PlatformVersionData appStore;
}

[Serializable]
public class PlatformVersionData
{
    public float newVersion = 0;
    public string link = "";
    public bool request = true;
    public string title = "New version is available";
    public string message = "Update app for clean game experience. Visit store link to update app.";
}

[Serializable]
public class LudoPhotonChatMessageDataDice
{
    public string playerId = "";
    public string sendTime = "";
    public bool isDiceRollEvent = false;
    public string diceKey = "";
}

[Serializable]
public class LudoPhotonChatMessageDataToken
{
    public string playerId = "";
    public string sendTime = "";
    public bool isPawnMovementEvent = false;
    public int tokenId = 0;
    public int currentDistance = 0;
    public int stepsToMove = 0;
}

[Serializable]
public class LudoPhotonChatMessageData
{
    public string playerId = "";
    public string sendTime = "";

    public bool isDiceRollEvent = false;
    public string diceKey = "";

    public bool isPawnMovementEvent = false;
    public int tokenId = 0;
    public int currentDistance = 0;
    public int stepsToMove = 0;
}

[Serializable]
public class ServerMaintenanceData
{
    public bool maintenanceModeActivated = false;
}
[Serializable]
public class BannerDatas
{
    public string image;
    public string url;
}

[Serializable]
public class BannerDataResult
{
    public List<BannerDatas> bannerData;
    public int SliderTime;
}

[Serializable]
public class FoundPlayerdata
{
    public string playerId;
    public string mobile;
    public string profilePic;
    public string name;
}

[Serializable]
public class WinnerPlayersData
{
    public string playerId = null;
    public string playerName = null;
    public double amount = 0;
}

[Serializable]
public class GetSupportData
{
    public List<PendingTicket> pendingTickets;
    public List<ResolveDeclineTicket> resolveDeclineTickets;
    public List<SupportDatum> supportData;
}

[Serializable]
public class PendingTicket
{
    public string ticketId;
    public string playerId;
    public string status;
    public string category;
    public string question;
    public string description;
    public string image;
    public string remarkByAdmin;
    public string imageByAdmin;
    public bool isRead;
    public string _id;
    public string createdAt;
    public string updatedAt;
}

[Serializable]
public class ResolveDeclineTicket
{
    public string ticketId;
    public string playerId;
    public string status;
    public string category;
    public string question;
    public string description;
    public string image;
    public string remarkByAdmin;
    public string imageByAdmin;
    public bool isRead;
    public string _id;
    public string createdAt;
    public string updatedAt;
}

[Serializable]
public class SupportDatum
{
    public string category = null;
    public string question = null;
    public string answer = null;
}

[Serializable]
public class TicketCreated
{
    public string title = null;
    public string message = null;
    public string ticketId = null;
}

[Serializable]
public class ShareAndRefferal
{
    public string shareMessage = null;
    public string referralCode = null;
}
