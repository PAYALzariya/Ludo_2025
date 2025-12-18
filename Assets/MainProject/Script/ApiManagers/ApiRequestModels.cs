

using System; 

#region Auth Requests

[Serializable]
public class RegisterRequest
{
    public string username;
    public string firstName;
    public string lastName;
    public string email;
    public string password;
}

[Serializable]
public class LoginRequest
{
    public string email;
    public string password;
}

//[Serializable]
public class RefreshTokenRequest
{
    public string refreshToken;

}

[Serializable]
public class VerifyEmailRequest
{
    public string hash;
}

[Serializable]
public class ForgotPasswordRequest
{
    public string email;
}

[Serializable]
public class ResetPasswordRequest
{
    public string password;
    public string hash;
}
[Serializable]
public class EmailBindingRequest
{
    public string email;
}

#endregion


#region User Requests

[Serializable]
public class UpdateProfileRequest
{
    
    public string firstName;
    public string lastName;
    public string avatar;
}

#endregion


#region Chat Room Requests

[Serializable]
public class CreateRoomRequest
{
    public string name;
    public string description;
    public string roomImage;
    public bool isPrivate;
    public int maxParticipants;
}

[Serializable]
public class RequestDataByRoomCodeOnly
{
    public string roomCode;
}
[Serializable]
public class RequestDataByRoomIDOnly
{
    public string roomId;
}

[Serializable]
public class RequestDataByUserIDOnly
{
    public int uid;
}

#endregion


#region User Requests
[Serializable]
public class UserProfileRequest
{
    public string uid;
}

[Serializable]
public class CreateGameRequest
{
    public string gameType; 
    public int betAmount;
    public int maxPlayers; 
}

[Serializable]
public class MovePawnRequest
{
    public int pawnIndex;
    public int diceRoll;
}

[Serializable]
public class UserDataPlayer
{

    public string id;
    public int uniqueId;

    public string username;
    public string firstName;

    public string lastName;
    public object gender;
    public object birthday;
    public string level;
    public string country;
    public string profilePicture;
    public string coverPicture;
    public string[] purchasedFrames;
  
}
[Serializable]
public class UserEallet
{
    public string coins;
    public string diamonds;
 
}

#endregion