using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using Unity.WebRTC;
[System.Serializable] // Make it serializable if you want to see it in the inspector (optional)
public struct LoadedSpriteAsset
{
    
    public Sprite SpriteAssset;

   
    internal string _sourceUrl;

    
    internal bool _isLoaded;
    internal bool _isLoading;

    
}
#region Common Data Models

[System.Serializable]
public class UserProfileData
{

    public string id;
    public string email;
    public string username;
    public string firstName;
    public string lastName;
    public string role;
    public Sprite SpriteProfile;
}
[System.Serializable]
public class UserData
{
    public string id;
    public string firstName;
    public string lastName;
    public string email;
    public bool isEmailVerified;
    public string avatar;
    public string role;
    
}

[System.Serializable]
public class GameMoveData
{
    public string player; 
    public string move;   
    public string id;
}




#endregion


#region Auth & User Responses

[System.Serializable]
public class AuthSuccessResponse
{
   
    public string accessToken;
    public string refreshToken;
    public UserProfileData user;
  internal void UpdatData()
    {
        Debug.Log("AuthSuccessResponse constructor called::"+ accessToken);
        DataManager.instance.AccessToken  = accessToken ;
      Debug.Log("AuthSuccessResponse constructor called  after::"+ DataManager.instance.AccessToken);
        DataManager.instance.refreshToken = refreshToken ;
        DataManager.instance.MyProfile = user;
        DataManager.instance.userId = user.id;
       
       
    } 
}
[System.Serializable]
public class RefreshTokenResponse
{
    public string accessToken;
    public string refreshToken;

    internal void UpdatData()
    {
        DataManager.instance.AccessToken = accessToken;
        DataManager.instance.refreshToken = refreshToken;
    }
}
[System.Serializable]
public class SuccessMessageResponse
{
    public string message;
}

#endregion

#region Friends Section
[System.Serializable]
public class friendsearchuserResponse // Renamed for clarity to avoid confusion with singular "ParticipantResponse"
{
    public bool success;

    // This MUST be an array or List to match the JSON array `[...]`
    public ChatParticipantData data;
}
#endregion
#region Chat Room Responses


[System.Serializable]
public class ChatParticipantData
{
    public string id;
    public int roomId;
    // public string userId;
    public string uniqueId;
    public bool isMuted;
    public bool micLocked;
    public bool isHost;
    public bool isActive;
    public bool isOnline;
    public string username;
    public string profilePicture;
    public int level;
    public string country;
    public string seatNumber;
    public bool seatLocked;
    public bool isSeated;



  


 
     [NonSerialized]
     public int frame=1;
      public LoadedSpriteAsset profilePictureAsset;
    public Country countryData;
    public level levelData;
    public Frame frameData;
    public async UniTask LoadAllAssets()
    {
        if (!profilePictureAsset._isLoaded)
        {

            profilePictureAsset._sourceUrl = profilePicture; // Set the source URL for the asset
            Debug.Log("loadasst::" + profilePictureAsset._isLoaded + "_sourceUrl:" + profilePictureAsset._sourceUrl);

            profilePictureAsset.SpriteAssset = await DataManager.instance.LoadSprite(profilePictureAsset);
            if (profilePictureAsset.SpriteAssset == null)
            {
                profilePictureAsset.SpriteAssset = DataManager.instance.spriteMaganer.default_ProfileSprite;
            }
             Debug.Log("data get::" + country);
            Debug.Log("data get::" + DataManager.instance);
            if (!string.IsNullOrEmpty(country))
            {
                
            countryData = DataManager.instance.GetCountry(country.ToLower());
            }
            else
            {
                countryData = DataManager.instance.GetCountry(" ");
            }
            levelData = DataManager.instance.GetLevel(level);
            frameData = DataManager.instance.GetFrame(frame);
            profilePictureAsset._isLoaded = true;
        }

    }
}
[System.Serializable]
public class ChatParticipantsListResponse 
{
    public bool success;

    // This MUST be an array or List to match the JSON array `[...]`
    public ChatParticipantData[] data;
}
[System.Serializable]
public class ChatRoomData : IEquatable<ChatRoomData>
{
    public string id;
    public string roomCode;
    public string name;
    public string description;
    public string roomImage;
    public bool isPrivate;
    public int maxParticipants;
    public int currentParticipants;
    public bool isActive;
    public bool isLocked;
    public string announcement;
    public string roomType;
    public string micMode;
    public int maxSeats;
    public bool superMicEnabled;
    public bool publicScreenEnabled;
   public string themeUrl;
    public bool effectsEnabled;
    public bool isVoiceChatActive;
    public string hostId;
    public string createdAt;
    public string updatedAt;
    public int level;
      public level levelData;

   [NonSerialized]
    public LoadedSpriteAsset roomImageAsset;
   
    public async UniTask LoadAllAssets()
    {
        if (!roomImageAsset._isLoaded)
        {

            Debug.Log("roomimage url::" + roomImage);
            roomImageAsset._sourceUrl = roomImage; // Set the source URL for the asset
            roomImageAsset.SpriteAssset = await DataManager.instance.LoadSprite(roomImageAsset);
            if (roomImageAsset.SpriteAssset == null)
            {
                roomImageAsset.SpriteAssset = DataManager.instance.spriteMaganer.default_RoomProfileSprite;
            }
            Debug.Log("roomimage::" + roomImageAsset.SpriteAssset);
            levelData = DataManager.instance.GetLevel(level);
            roomImageAsset._isLoaded = true;
        }

    }
    public bool Equals(ChatRoomData other)
    {
        if (other == null) return false;

        // The two objects are considered "equal" (i.e., unchanged) if their unique IDs
        // and their "updatedAt" timestamps are the same.
        // We still check the ID to ensure we are comparing the same logical room.
        return this.id == other.id && this.updatedAt == other.updatedAt;
    }
}


[System.Serializable]
public class JoinChatRoomResponse
{

    public bool success;
    public string timestamp;
    public ResponseData data;
}

[System.Serializable]
public class ResponseData
{
    public ChatRoomData room;
    public ChatParticipantData host;
    public ChatParticipantData[] participants;
    public bool isNewRoom;
     public string message;
}
[System.Serializable]
public class ON_JoinRoomResponse
{
    public bool success;
    public ON_JoinRoomData data;
  //  public string timestamp;
}
[System.Serializable]
public class ON_JoinRoomData
{

    public ChatParticipantData[] participants;
    public string message;
   
}
[System.Serializable]
public class ON_LeavRoomResponse
{
    public bool success;
    public string timestamp;
    public ON_LeavRoomData data;
}
[System.Serializable]
public class ON_LeavRoomData
{
    public ON_JoinRoomData_leftuser leftUser;
    public ChatParticipantData[] participants;
    public string message;
}
[System.Serializable]
public class ON_JoinRoomData_leftuser
{
    public string id;
    public string uniqueId;
    public string customId;
    public string username;
    public string profilePicture;

}



#endregion
#region List Responses (Paginated)


[System.Serializable]
public class PaginationData
{
    public int page;
    public int limit;
    public int total;
    public int totalPages;
}


[System.Serializable]
public class PopularChatListResponse
{
    public bool success;
    public ChatRoomData[] data;
    public PaginationData pagination;
}

[System.Serializable]
public class ChatRoomSettingResponse
{

   
        public bool success;
        public ChatRoomData data;
    
}
#endregion




[System.Serializable]
public class updloadimageResponse
{
    public bool success;

    public updloadimageData data; 
    
}
[System.Serializable]
public class updloadimageData
{
    public string signedUrl;
    public string publicUrl;
    public string imageType;
 public string maxFileSize;
 }
//======================================================================SocktReposne======================================================
[System.Serializable]
public class user_apply_seatSuccessResponse
{
    public bool success;

  //  public string timestamp;
    public userseatData data;
}
[System.Serializable]
public class userseatData
{
    public string roomId;
    public string seatNumber;
    public string userId;
    public string status;
    public string applicationId;
    public string action;
    public string timestamp;
}

[System.Serializable]
public class joinvoicesessionRepsonse
{
    public bool success;

    public joinvoicesessionData data;
}
[System.Serializable]
public class joinvoicesessionData
{

    public string roomId;

    public string newParticipant;
    public ChatParticipantData[] participants;
    public string massage;

}
[System.Serializable]
public class VoiceParticipantData
{

    
}
[System.Serializable]
public class OnVoiceParticipantJoinedReceived
{

    public bool success;
    public string timestamp;
    public OnVoiceParticipantJoinedReceivedData data;
   

}
[System.Serializable]
public class OnVoiceParticipantJoinedReceivedData
{

    public string roomId;
    public string newParticipant;
}
//==================================================================Room host Funcation Sockt======================================================
[System.Serializable]
public class LockedSeat
{
    public bool success;
   
    public string timestamp;
    public LockedSeatData data;
   
}
[System.Serializable]
public class LockedSeatData
{ 
public string roomId;
    public string seatNumber;
    public string action;
    public string lockedBy;
    public string timestamp;
}
//======================================================================GroupTextMassage======================================================

[System.Serializable]
public class GrouptextchatResponse
{

    public bool success;
    public GrouptextchatData data;

}
[System.Serializable]
public class GrouptextchatData
{   
    public GrouptxtmassageData message;
    public ChatParticipantData sender;
}
[System.Serializable]
public class GrouptxtmassageData
{
    public string id;
    public string roomId;
    public string senderId;
    public string senderName;
    public string message;
    public string time;
    
}
[System.Serializable]
public class GrouptextchatsenderData
{
    public string id;
    public string username;
    public string email;
    public string phone;
    public string firstName;
    public string lastName;
    public string gender;
    public string bio;
    public string birthday;
    public string profilePicture;
    public string coverPicture;
    public bool isActive;
    public string role;
    public string provider;
    public int uniqueId;
    public ChatParticipantData sender;

}
[System.Serializable]
public class SeatInvitationReceived
{
    public bool success;
    public SeatInviteData data;
    public long timestamp;
}

[System.Serializable]
public class SeatInviteData
{
    public int roomId;
    public string seatNumber;
    public string invitationId;
    public string userId;
    public string invitedBy;


}
[System.Serializable]
public class Rootuseracceptedseatinvitation
{
    public bool success;
    public RoomDatauseracceptedseatinvitation data;
    public long timestamp;
}

[System.Serializable]
public class RoomDatauseracceptedseatinvitation
{
    public int roomId;
    public string seatNumber;
    public string userId;
}
[System.Serializable]
public class SeatwaitinglistUpdatedResponse
{
    public bool success;
    public long timestamp;
    public SeatwaitinglistData data;
}

[System.Serializable]
public class SeatwaitinglistData
{
    public string roomId;
    public string seatNumber;
    public string type;
    public string userId;
    public string applicationId;
}
[System.Serializable]
public class GetSeatwaitinglistResponse
{
    public bool success;
    public long timestamp;
    public GetSeatwaitinglistData data;
}

[System.Serializable]
public class GetSeatwaitingAlllistData
{
   
    public GetSeatwaitinglistAplicationData[] applications;
    public GetSeatwaitinglistAplicationData[] invitations;
}
[System.Serializable]
public class GetSeatwaitinglistData
{
    public GetSeatwaitingAlllistData waitingList;
}

     [System.Serializable]
public class GetSeatwaitinglistAplicationData
{
    public string id;
    public string userId;
    public string username;
    public string profilePicture;
    public string uniqueId;
}


[System.Serializable]
public class CreateAndSendOfferReceivedResponse
{
    public bool success;

    public CreateAndSendOfferData data;
}
[System.Serializable]
public class CreateAndSendOfferData
{
    public string roomId;
    public string fromUserId;
    public RTCSessionDescription offer;
    public string audioCodec;
    public string sampleRate;
    public string timestamp;
    

}
[System.Serializable]
public class WebRTCIceCandidateReceivedResponse
    {
        public bool success;

        public WebRTCIceCandidateData data;
    }
    [System.Serializable]
    public class WebRTCIceCandidateData
    {
        public string roomId;
        public string fromUserId;
        public RTCIceCandidate candidate;
         public string timestamp;
    }
[System.Serializable]
public class RTCIceCandidateData
{
    public string candidate;
    public string sdpMid;
    public int sdpMLineIndex;
}
[System.Serializable]
public class OnWebRTCAnswerReceivedResponse
    {
        public bool success;

        public OnWebRTCAnswerData data;
    }
    [System.Serializable]
    public class OnWebRTCAnswerData
    {
        public string roomId;
        public string fromUserId;
        public RTCSessionDescription answer;
         public string timestamp;
    }
[System.Serializable]
public class OnUserStartedSpeakingResponse
{
    public bool success;

    public OnUserStartedSpeakingReceivedData data;
}
[System.Serializable]
public class OnUserStartedSpeakingReceivedData
{
    public string roomId;
    public string userId;
    public string startedSpeaking;
}
[System.Serializable]
public class SendIceCandidateReposne
{
    public bool success;
    public string timestamp;
   public SendIceCandidateData data;
}
[System.Serializable]
public class SendIceCandidateData
{
    public string roomId;
    public string targetUserId ;
   public string candidateSent;
}


[System.Serializable]
public class FailedEmitResponse
{
    public bool success;
    public string timestamp;
    public string error;
}
[System.Serializable]
public class FailedApiResponse
{
    public string error;
}



