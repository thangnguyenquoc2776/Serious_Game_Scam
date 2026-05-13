using UnityEngine;
using SeriousGame.App; // Import namespace chứa GameEventBus

public class UITester : MonoBehaviour
{
    void Update()
    {
        // Nhấn phím P để giả lập hành động người chơi mở tin nhắn của Guard_Main
        if (Input.GetKeyDown(KeyCode.P))
        {
            // Lệnh này sẽ tự động bật UI Điện thoại và kích hoạt Node "Guard_Main"
            GameEventBus.RaiseSwitchUIRequested("phone");
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            // Lệnh này sẽ tự động bật UI Normal và kích hoạt Node "Guard_Main"
            GameEventBus.RaiseSwitchUIRequested("normal");
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            // Lệnh này sẽ tự động bật UI PC và kích hoạt Node "Guard_Main"
            GameEventBus.RaiseSwitchUIRequested("pc");
        }
    }
}