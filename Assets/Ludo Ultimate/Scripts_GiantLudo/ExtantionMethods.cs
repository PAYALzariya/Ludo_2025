using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BestHTTP.SocketIO;
using UnityEngine;
using UnityEngine.UI;

public static class ExtantionMethods
{

    /*public static void Open(this MonoBehaviour obj)
    {
        obj.gameObject.SetActive(true);

    }
    public static void Close(this MonoBehaviour obj)
    {
        obj.gameObject.SetActive(false);
    }*/
    public static T[] Subsequence<T>(this IEnumerable<T> arr, int startIndex, int length)
    {
        return arr.Skip(startIndex).Take(length).ToArray();
    }

    public static T[] SubsequenceReverse<T>(this T[] arr, int startIndex)
    {

        List<T> li = new List<T>();
        for (int i = startIndex - 1; i >= 0; i--)
        {
            li.Add(arr[i]);
        }

        return li.ToArray();
    }

    public static void DebugDisplay<T>(this T[] arr)
    {
        string s = "";
        for (int i = 0; i < arr.Length; i++)
        {
            s += $"{i} => {arr[i]} {Environment.NewLine}";
        }
        Debug.Log($"{s}");
    }

    public static string GetPacketString(this Packet p)
    {
        JSONArray jSONArray = new JSONArray(p.ToString());
        if (jSONArray.length() >= 2)
        {
            return jSONArray.getString(1);
        }
        return jSONArray.length() > 0 ? jSONArray.getString(0) : "";// (arr.length() - 1);
     //   return jSONArray.getString(jSONArray.length() - 1);
    }

    public static LudoGame_SocketManager GetSocketGameManager(this MonoBehaviour m)
    {
        return Ludo_UIManager.instance.socketManager;
    }
}

public static class Cust_Utility
{

    public static bool VerifyEmail(string email)
    {
        Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        Match match = regex.Match(email);

        return match.Success;
    }

    public static string GetColoredString(Color32 c, object message)
    {
        //Color.red, "Hello"
        //return Value = "<color=#ff0000>Hello</color>";
        return $"<color=#{c.r.ToString("X2")}{c.g.ToString("X2")}{c.b.ToString("X2")}> {message} </color>";
    }

    public static string GetDeviceType
    {
        get
        {
#if UNITY_EDITOR
            return "editor";
#elif UNITY_ANDROID && !UNITY_EDITOR
            return "android";
#elif UNITY_IOS && !UNITY_EDITOR
            return "ios";
#elif UNITY_WEBGL && !UNITY_EDITOR
            return "web";
#else
            return "editor";
#endif
        }
    }

    public static JSON_Object GetJsonObjects(Dictionary<string, object> data)
    {
        JSON_Object jsonObj = new JSON_Object();
        foreach (KeyValuePair<string, object> item in data)
        {
            jsonObj.put(item.Key, item.Value);
        }
        return jsonObj;
    }

    public static IEnumerator MoveFromPositionToPositionXY(bool isHome, float time, Transform from, Transform to, Action callback)
    {
        var wait = new WaitForEndOfFrame();

        Vector3 startPosition = from.position;
        Vector3 lastPosition = to.position;

        float t = 0;

        while (t <= 1)
        {
            from.position = Vector3.Lerp(startPosition, lastPosition, t);
            if (isHome)
            {
                from.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.5f, Get0to0From0to1(t));
            }
            else
            {
                from.localScale = Vector3.Lerp(Vector3.one, Vector3.one /** 1.5f*/, Get0to0From0to1(t));
            }

            // Debug.LogError("Moving Goti ");
            t += Time.deltaTime / time;
            yield return wait;
        }
        from.position = lastPosition;
        from.localScale = Vector3.one;
        callback();
    }

    public static IEnumerator MoveFromPositionToHome(float time, Transform from, Transform to, Action callback)
    {
        var wait = new WaitForEndOfFrame();

        Vector3 startPosition = from.position;
        Vector3 lastPosition = to.position;

        float t = 0;

        while (t <= 1)
        {
            from.position = Vector3.Lerp(startPosition, lastPosition, t);
            t += Time.deltaTime / time;

            yield return wait;
        }
        from.position = lastPosition;
        from.localScale = Vector3.one;
        callback();
    }

    public static IEnumerator MoveFromPositionToPositionXY(float time, Transform from, Vector3 to, Action callback)
    {
        var wait = new WaitForEndOfFrame();

        Vector3 startPosition = from.position;
        Vector3 lastPosition = to;

        float t = 0;

        while (t <= 1)
        {
            from.position = Vector3.Lerp(startPosition, lastPosition, t);
            from.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.5f, Get0to0From0to1(t));
            t += Time.deltaTime / time;

            yield return wait;
        }
        from.position = lastPosition;
        from.localScale = Vector3.one;
        callback();
        yield return wait;
    }

    public static IEnumerator MoveToPath(Transform target, PathElement[] path, float time, Action stepcallback, Action callback)
    {
        // Debug.LogError("MoveToPath..." + time);
        var wait = new WaitForEndOfFrame();

        //  for (int i = 0; i < path.Length - 1; i++)
        for (int i = 0; i < path.Length; i++)
        {
            Vector3 startPosition;
            Vector3 lastPosition;
            try
            {
                startPosition = path[i].transform.position;

                //   lastPosition = path[i + 1].transform.position;
                lastPosition = path[i].transform.position;
            }
            catch (System.IndexOutOfRangeException e)
            {
                //When Flow comes here it means the pawn is already on it's final position and there are no more place for the pawn to go.
                //And this is the reason that there is nothing other than a simple break and debug.
                //The debug below was annowing for me so I commented it.

                Debug.Log($"{e.Message}");

                break;
            }

            float t = 0;
            stepcallback();

            // Moving player from here once it is out of home - Original
            while (t <= 1)
            {
                // Debug.LogError("Mooving player..");
                target.position = Vector3.Lerp(startPosition, lastPosition, t);
                target.localScale = Vector3.Lerp(Vector3.one, Vector3.one * (1.25f), Get0to0From0to1(t));
                t += Time.deltaTime / time;
                yield return wait;
            }

            target.position = lastPosition;
            target.localScale = Vector3.one;
        }
        if (path.Length == 1)
        {
            target.GetComponent<MonoBehaviour>().StartCoroutine(MoveFromPositionToPositionXY(false, time, target, path[0].transform, callback));
            //    stepcallback();
        }
        else
        {
            callback();
        }
        yield return wait;
    }

    /// <summary>
    ///Will fill the Image in intended time.
    /// </summary>
    /// <returns></returns>
    public static IEnumerator FillWithTime(Image img, float time, Action callback)
    {
        float f = 1;

        while (f >= 0)
        {
            img.fillAmount = f;
            f -= Time.deltaTime / time;
            yield return 0;
        }
        img.fillAmount = 0;
        callback();
    }

    public static IEnumerator FillWithTime(Image img, float currentTime, float totalTime, Action callback)
    {
        float f = currentTime / totalTime;

        while (f >= 0)
        {
            img.fillAmount = f;
            f -= Time.deltaTime / totalTime;
            yield return 0;
        }
        img.fillAmount = 0;
        callback();
    }

    public static string GetBase64String(Texture2D texture)
    {
        byte[] bytes = texture.EncodeToPNG();
        return Convert.ToBase64String(bytes);
    }

    public static float Get0to0From0to1(float f)
    {
        f = Mathf.Clamp(f, 0, 1);
        return Mathf.Sin(f * Mathf.PI);
    }

    public static string GetColorName(int i)
    {
        string color = "";
        switch (i)
        {
            case 0:
                color = "yellow";
                break;

            case 1:
                color = "blue";
                break;

            case 2:
                color = "red";
                break;

            case 3:
                color = "green";
                break;

            default:

                Debug.LogError($"color Index onError: {i}");
                color = "";
                break;
        }
        return color;
    }

    public static int GetColorIndexFromName(string color)
    {
        int num = -1;
        switch (color)
        {
            case "yellow":
                num = 0;
                break;

            case "blue":
                num = 1;
                break;

            case "red":
                num = 2;
                break;

            case "green":
                num = 3;
                break;

            default:
                num = -1;
                break;
        }
        return num;
    }

    public static void DrawPathElements(PathElement[] path)
    {
        if (path.Length <= 0)
            return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(path[0].transform.position, 0.06f);

        for (int i = 1; i < path.Length; i++)
        {

            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(path[i - 1].transform.position, path[i].transform.position);

            Gizmos.color = (path[i].type == ElementType.immortalLand) ? Color.cyan : Color.black;
            Gizmos.DrawSphere(path[i].transform.position, 0.06f);
        }
    }

}