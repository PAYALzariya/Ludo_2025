// DummySocketLogPlayer.cs
// Place in Assets/Scripts/ and attach to an active GameObject in scene.
// Requires the log file at: Assets/StreamingAssets/dummy_socket_log.txt

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using BestHTTP.JSON; // optional: if you use BestHTTP JSON; otherwise use Unity's JsonUtility or a JSON parser

[Serializable]
public class DummyResponseEvent : UnityEvent<string, string> { } // (eventName, jsonPayload)

public class DummySocketLogPlayer : MonoBehaviour
{
    [Header("Log file (StreamingAssets)")]
    public string logFileName = "dummy_socket_log.txt";

    [Header("Keyboard keys")]
    public KeyCode emitKey = KeyCode.E;
    public KeyCode broadcastKey = KeyCode.B;

    [Header("Auto-detect event lines")]
    public bool detectLowercaseVariants = true; // handle startTimer vs StartTimer

    [Header("Hooks")]
    public DummyResponseEvent onEmitReceived;      // Hook your Emit response handler here
    public DummyResponseEvent onBroadcastReceived; // Hook your Broadcast handler here

    // parsed lists
    private List<(string eventName, string json)> emitList = new List<(string, string)>();
    private List<(string eventName, string json)> broadcastList = new List<(string, string)>();

    private int emitIndex = 0;
    private int broadcastIndex = 0;

    // regex patterns (robust to spacing)
    private Regex emitLineRegex = new Regex(@"Emit\s+name.*?:\s*(\{[\s\S]*?\}|\[\"".*?\""\s*,\s*\{[\s\S]*?\]|\[.*\])",
                                           RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private Regex broadcastRegex = new Regex(@"(Broadcast\s+response\s*:)\s*(\[\""?([A-Za-z0-9_]+)\""?\s*,\s*(\{[\s\S]*?\})\]|\[([^\]]+)\])",
                                             RegexOptions.Compiled | RegexOptions.IgnoreCase);

    // fallback: find specific event patterns like ["StartTimer", {...}]
    private Regex bracketEventRegex = new Regex(@"\[\s*""?([A-Za-z0-9_]+)""?\s*,\s*(\{[\s\S]*?\})\s*\]",
                                               RegexOptions.Compiled | RegexOptions.IgnoreCase);

    void Start()
    {
        string path = Path.Combine(Application.streamingAssetsPath, logFileName);
        if (!File.Exists(path))
        {
            Debug.LogError("[DummySocketLogPlayer] Log file not found at: " + path);
            return;
        }

        string text = File.ReadAllText(path);
        ParseLogText(text);

        Debug.LogFormat("[DummySocketLogPlayer] Parsed {0} emits and {1} broadcasts.",
                        emitList.Count, broadcastList.Count);
    }

    void Update()
    {
        if (Input.GetKeyDown(emitKey))
        {
            TriggerNextEmit();
        }

        if (Input.GetKeyDown(broadcastKey))
        {
            TriggerNextBroadcast();
        }
    }

    private void ParseLogText(string text)
    {
        emitList.Clear();
        broadcastList.Clear();

        // 1) find bracketed events like ["Name", { ... }]
        foreach (Match m in bracketEventRegex.Matches(text))
        {
            string eventName = m.Groups[1].Value;
            string json = m.Groups[2].Value.Trim();

            // Decide if it's broadcast or emit by looking at surrounding text near the match
            int idx = m.Index;
            int contextStart = Math.Max(0, idx - 80);
            int len = Math.Min(80 + m.Length, text.Length - contextStart);
            string context = text.Substring(contextStart, len);

            if (context.ToLower().Contains("broadcast"))
            {
                broadcastList.Add((eventName, json));
            }
            else
            {
                // treat as broadcast by default if event name is in broadcast list (common ones)
                broadcastList.Add((eventName, json));
            }
        }

        // 2) find explicit "Broadcast response  :[" lines (covers variants)
        // capture event name and JSON object
        foreach (Match m in Regex.Matches(text, @"([A-Za-z0-9_]+)\s+Broadcast\s+response\s*:\s*(\[[\s\S]*?\])", RegexOptions.IgnoreCase))
        {
            // m.Groups[1] is the human label (may be lowercase), m.Groups[2] full bracketed content
            string label = m.Groups[1].Value;
            string bracket = m.Groups[2].Value;
            // attempt to extract ["EventName", {...}] style
            Match inner = bracketEventRegex.Match(bracket);
            if (inner.Success)
            {
                string eventName = inner.Groups[1].Value;
                string json = inner.Groups[2].Value;
                broadcastList.Add((eventName, json));
            }
            else
            {
                // fallback: use label and entire bracket as payload
                broadcastList.Add((label, bracket));
            }
        }

        // 3) find "StartTimer Broadcast response  :["startTimer",{"..."}]" style that sometimes uses startTimer lowercase
        foreach (Match m in Regex.Matches(text, @"startTimer\s+Broadcast\s+response\s*:\s*(\[[\s\S]*?\])", RegexOptions.IgnoreCase))
        {
            Match inner = bracketEventRegex.Match(m.Groups[1].Value);
            if (inner.Success)
            {
                string eventName = inner.Groups[1].Value;
                string json = inner.Groups[2].Value;
                broadcastList.Add((eventName, json));
            }
        }

        // 4) find Emit lines where the pattern has "Emit name" followed by JSON parameters
        foreach (Match m in Regex.Matches(text, @"Emit\s+name.*?:\s*(\{[\s\S]*?\})", RegexOptions.IgnoreCase))
        {
            string json = m.Groups[1].Value.Trim();
            // try to extract event name from preceding text (look back 120 chars)
            int idx = m.Index;
            int start = Math.Max(0, idx - 120);
            int length = idx - start;
            string context = text.Substring(start, length);
            // find last "Emit name" label word (naive grab)
            Match label = Regex.Match(context, @"Emit\s+name\s*(?:request)?\s*[:\s]*([A-Za-z0-9_]+)?", RegexOptions.IgnoreCase);
            string eventName = label.Success && !string.IsNullOrEmpty(label.Groups[1].Value) ? label.Groups[1].Value : "Emit";
            emitList.Add((eventName, json));
        }

        // 5) final dedupe: remove empty or malformed entries
        emitList.RemoveAll(e => string.IsNullOrWhiteSpace(e.json));
        broadcastList.RemoveAll(e => string.IsNullOrWhiteSpace(e.json));
    }

    private void TriggerNextEmit()
    {
        if (emitList.Count == 0)
        {
            Debug.LogWarning("[DummySocketLogPlayer] No emits parsed from log.");
            return;
        }

        var item = emitList[emitIndex];
        emitIndex = (emitIndex + 1) % emitList.Count;

        Debug.LogFormat("[DummySocketLogPlayer] EMIT -> {0}\n{1}", item.eventName, item.json);

        // invoke UnityEvent for hooking
        onEmitReceived?.Invoke(item.eventName, item.json);

        // If you want automatic parsing with BestHTTP JSON:
        // var parsed = BestHTTP.JSON.Json.Decode(item.json);
        // then call your existing callback/handler passing `parsed`
    }

    private void TriggerNextBroadcast()
    {
        if (broadcastList.Count == 0)
        {
            Debug.LogWarning("[DummySocketLogPlayer] No broadcasts parsed from log.");
            return;
        }

        var item = broadcastList[broadcastIndex];
        broadcastIndex = (broadcastIndex + 1) % broadcastList.Count;

        Debug.LogFormat("[DummySocketLogPlayer] BROADCAST -> {0}\n{1}", item.eventName, item.json);

        // invoke UnityEvent for hooking
        onBroadcastReceived?.Invoke(item.eventName, item.json);

        // Optionally parse and dispatch to your socket handlers:
        // var parsed = BestHTTP.JSON.Json.Decode("[" + "\"" + item.eventName + "\"," + item.json + "]");
        // SocketBroadcastHandler.Dispatch(item.eventName, parsed);
    }
}
