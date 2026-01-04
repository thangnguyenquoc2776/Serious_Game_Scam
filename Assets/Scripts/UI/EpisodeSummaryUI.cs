using System.Text;
using UnityEngine;
using TMPro;
using SeriousGame.Runtime;

// UI tổng kết episode, hiển thị các flag trong GameState
public class EpisodeSummaryUI : MonoBehaviour
{
    public static EpisodeSummaryUI Instance;

    [Header("UI Root")]
    public GameObject root; // panel tổng kết, có thể chính gameObject này

    [Header("Texts")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;

    [Header("Optional: Player để khoá điều khiển")]
    public PlayerController playerController;

    void Awake()
    {
        Instance = this;
        if (root == null) root = gameObject;
        root.SetActive(false);
    }

    public void ShowSummary()
    {
        // Khoá điều khiển player nếu có
        if (playerController != null)
        {
            playerController.SetLockState(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (titleText != null)
        {
            titleText.text = "Tổng kết tập";
        }

        if (bodyText != null)
        {
            bodyText.text = BuildSummaryText();
        }

        root.SetActive(true);
    }

    string BuildSummaryText()
    {
        var gsm = GameStateManager.Instance;
        if (gsm == null)
            return "Không có dữ liệu trạng thái.";

        var snapshot = gsm.GetSnapshot();
        if (snapshot == null || snapshot.Count == 0)
            return "Bạn chưa tạo ra trạng thái quan trọng nào trong tập này.";

        var sb = new StringBuilder();
        sb.AppendLine("Các trạng thái đã đạt:");

        foreach (var kv in snapshot)
        {
            if (!kv.Value) continue;
            sb.AppendLine("- " + kv.Key);
        }

        return sb.ToString();
    }
}
