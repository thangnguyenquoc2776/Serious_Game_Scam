using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class InteractorLogger : MonoBehaviour
{
    public XRRayInteractor rayInteractor;

    void OnEnable()
    {
        rayInteractor.hoverEntered.AddListener(args => Debug.Log("[Interactor] HOVER ENTER " + args.interactableObject));
        rayInteractor.hoverExited.AddListener(args => Debug.Log("[Interactor] HOVER EXIT " + args.interactableObject));
        rayInteractor.selectEntered.AddListener(args => Debug.Log("[Interactor] SELECT ENTER " + args.interactableObject));
        rayInteractor.selectExited.AddListener(args => Debug.Log("[Interactor] SELECT EXIT " + args.interactableObject));
    }

    void OnDisable()
    {
        rayInteractor.hoverEntered.RemoveAllListeners();
        rayInteractor.hoverExited.RemoveAllListeners();
        rayInteractor.selectEntered.RemoveAllListeners();
        rayInteractor.selectExited.RemoveAllListeners();
    }
}
