using UnityEngine;
using System.Collections.Generic;
using SeriousGame.App;
using SeriousGame.State;

public class PlayerStateDebugger : MonoBehaviour
{
    [Header("Chỉ dùng để xem (View Only)")]
    public List<PlayerStateEntry> currentStates = new List<PlayerStateEntry>();
    public List<PlayerFlagEntry> currentFlags = new List<PlayerFlagEntry>();

    // Cập nhật liên tục mỗi giây trên Inspector
    void Update()
    {
        var ctx = GameBootstrap.Context;
        if (ctx != null && ctx.PlayerState != null)
        {
            var snapshot = ctx.PlayerState.GetSnapshot();
            currentStates = snapshot.entries;
            currentFlags = snapshot.flags;
        }
    }
}