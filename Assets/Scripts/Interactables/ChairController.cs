using SeriousGame.Runtime;
using UnityEngine;

public class ChairController : MonoBehaviour
{
    public Transform sitPoint;

    public void Sit(PlayerController player)
    {
        player.SetLockState(true);
        player.isSitting = true;

        player.TeleportTo(sitPoint);

        player.cameraHolder.localPosition += new Vector3(0, 0, 2f);
    }

    public void Stand(PlayerController player)
    {
        player.isSitting = false;
        player.cameraHolder.localPosition -= new Vector3(0, 0, 2f);
        player.SetLockState(false);
    }
}

