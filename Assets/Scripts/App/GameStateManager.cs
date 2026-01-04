using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour {
    public static GameStateManager Instance;
    private Dictionary<string, bool> flags = new Dictionary<string, bool>();

    void Awake() { Instance = this; DontDestroyOnLoad(gameObject); }

    public void SetFlag(string key, bool value) {
        flags[key] = value;
        Debug.Log($"State updated: {key} = {value}");
    }

    public bool CheckFlag(string key) {
        return flags.ContainsKey(key) && flags[key];
    }
}