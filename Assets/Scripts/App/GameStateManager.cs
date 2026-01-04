using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour {
    public static GameStateManager Instance;
    public Dictionary<string, bool> flags = new Dictionary<string, bool>();

    void Awake() { Instance = this; DontDestroyOnLoad(gameObject); }

    public void SetFlag(string key, bool value) {
        flags[key] = value;
        Debug.Log($"State updated: {key} = {value}");
    }

    public bool CheckFlag(string key) {
        return flags.ContainsKey(key) && flags[key];
    }

    // Đếm bao nhiêu flag trong danh sách keys đang true
    public int CountTrue(params string[] keys) {
        int count = 0;
        if (keys == null) return 0;
        foreach (var k in keys) {
            if (CheckFlag(k)) count++;
        }
        return count;
    }

    // Đếm số flag true theo prefix (vd: "CHOICE_") để chấm điểm theo nhóm
    public int CountTrueByPrefix(string prefix) {
        if (string.IsNullOrEmpty(prefix)) return 0;
        int count = 0;
        foreach (var kv in flags) {
            if (kv.Key.StartsWith(prefix) && kv.Value)
                count++;
        }
        return count;
    }

    // Lấy snapshot readonly để script khác có thể tự tính toán thêm nếu cần
    public IReadOnlyDictionary<string, bool> GetSnapshot() {
        return flags;
    }
}