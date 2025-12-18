using System.Collections.Generic;

public static class DummyEmitResponses
{
    private static Dictionary<string, string[]> data = new Dictionary<string, string[]>()
    {
        { "GetAllTableParameters", new string[] {
            "[{\"status\":\"succe0ss\",\"message\":\"All table record.\",\"result\":{\"tableData\":[{\"_id\":\"664f1e2727aa3d0040a5bfa3\",\"entryFee\":\"10\",\"totalPrice\":17,\"totalCom\":3,\"goti\":\"1\",\"onlineUser\":0},{\"_id\":\"66bb34b830bf5b04dc0e2929\",\"entryFee\":\"43\",\"totalPrice\":77.4,\"totalCom\":8.6,\"goti\":\"1\",\"onlineUser\":0},{\"_id\":\"66bb35aa30bf5b04dc0e294f\",\"entryFee\":\"45\",\"totalPrice\":81,\"totalCom\":9,\"goti\":\"1\",\"onlineUser\":0},{\"_id\":\"66bb37e730bf5b04dc0e2b23\",\"entryFee\":\"55\",\"totalPrice\":99,\"totalCom\":11,\"goti\":\"1\",\"onlineUser\":0},{\"_id\":\"6787f56cdfc308d76a9aa8b8\",\"entryFee\":\"555\",\"totalPrice\":777,\"totalCom\":333,\"goti\":\"1\",\"onlineUser\":0},{\"_id\":\"672e101053357d00f743bd32\",\"entryFee\":\"100000\",\"totalPrice\":180000,\"totalCom\":20000,\"goti\":\"1\",\"onlineUser\":0}]",
            "[{\"status\":\"success\",\"message\":\"All table record.\",\"result\":{\"tableData\":[{\"_id\":\"619c88d71d82ba7851dc9a2f\",\"entryFee\":\"15\",\"totalPrice\":27,\"totalCom\":3,\"goti\":\"2\",\"onlineUser\":0},{\"_id\":\"61c1cd087b1707218f0eb7db\",\"entryFee\":\"25\",\"totalPrice\":45,\"totalCom\":5,\"goti\":\"2\",\"onlineUser\":0},{\"_id\":\"61459657ce0b82452f50a53e\",\"entryFee\":\"40\",\"totalPrice\":72,\"totalCom\":8,\"goti\":\"2\",\"onlineUser\":0},{\"_id\":\"61458c7bce0b82452f50a025\",\"entryFee\":\"90\",\"totalPrice\":162,\"totalCom\":18,\"goti\":\"2\",\"onlineUser\":0},{\"_id\":\"61458c87ce0b82452f50a02e\",\"entryFee\":\"200\",\"totalPrice\":360,\"totalCom\":40,\"goti\":\"2\",\"onlineUser\":0},{\"_id\":\"61a9d34f86b31007177b427e\",\"entryFee\":\"400\",\"totalPrice\":720,\"totalCom\":80,\"goti\":\"2\",\"onlineUser\":0},{\"_id\":\"61458c93ce0b82452f50a04b\",\"entryFee\":\"600\",\"totalPrice\":1080,\"totalCom\":120,\"goti\":\"2\",\"onlineUser\":0},{\"_id\":\"61c1cd707b1707218f0eb88e\",\"entryFee\":\"1500\",\"totalPrice\":2700,\"totalCom\":300,\"goti\":\"2\",\"onlineUser\":0},{\"_id\":\"61c1cb817b1707218f0eb583\",\"entryFee\":\"3500\",\"totalPrice\":6300,\"totalCom\":700,\"goti\":\"2\",\"onlineUser\":0}]",
            "[{\"status\":\"success\",\"message\":\"All table record.\",\"result\":{\"tableData\":[{\"_id\":\"664f1e2727aa3d0040a5bfa3\",\"entryFee\":\"10\",\"totalPrice\":17,\"totalCom\":3,\"goti\":\"1\",\"onlineUser\":0},{\"_id\":\"66bb34b830bf5b04dc0e2929\",\"entryFee\":\"43\",\"totalPrice\":77.4,\"totalCom\":8.6,\"goti\":\"1\",\"onlineUser\":0},{\"_id\":\"66bb35aa30bf5b04dc0e294f\",\"entryFee\":\"45\",\"totalPrice\":81,\"totalCom\":9,\"goti\":\"1\",\"onlineUser\":0},{\"_id\":\"66bb37e730bf5b04dc0e2b23\",\"entryFee\":\"55\",\"totalPrice\":99,\"totalCom\":11,\"goti\":\"1\",\"onlineUser\":0},{\"_id\":\"6787f56cdfc308d76a9aa8b8\",\"entryFee\":\"555\",\"totalPrice\":777,\"totalCom\":333,\"goti\":\"1\",\"onlineUser\":0},{\"_id\":\"672e101053357d00f743bd32\",\"entryFee\":\"100000\",\"totalPrice\":180000,\"totalCom\":20000,\"goti\":\"1\",\"onlineUser\":0}]"
        } },
        { "JoinGame", new string[] {
            "[{\"status\":\"success\",\"message\":\"Player Joint Game successfully.\",\"result\":{\"boardId\":\"69185a4efd3aa3ab3b6b0cb0\",\"turnTime\":30}}]"
        } },
        { "RollDice", new string[] {
            "[{\"status\":\"success\",\"message\":\"Roll Dice successfully.\",\"result\":{\"message\":\"Roll Dice Successfully \",\"isTotalSixesZero\":false}}]"
        } },
        { "PlayerAction", new string[] {
            "[{\"status\":\"success\",\"result\":null,\"data\":{\"status\":\"success\",\"message\":\"Player Action Successfully \"},\"message\":\"Player action successful.\"}]",
            "[{\"status\":\"fail\",\"result\":null,\"data\":{},\"message\":\"Something went wrong.\"}]"
        } },
        { "LeaveRoom", new string[] {
            "[{\"status\":\"success\",\"message\":\"Player Leave successfully.\",\"result\":null}]"
        } }
    };

    public static string Get(string eventName)
    {
        if (data.ContainsKey(eventName))
            return data[eventName][0]; // return first variant for emits (callbacks usually single)
        return null;
    }
}