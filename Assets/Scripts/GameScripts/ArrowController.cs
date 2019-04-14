using UnityEngine;
using UnityEngine.UI;

public class ArrowController : MonoBehaviour {

    public static ArrowController instance;
    private Transform arrowBase;
    private Transform arrowPower;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != null) {
            Destroy(gameObject);
        }
    }

    private void Start() {
        arrowBase = transform.GetChild(0).transform;
        arrowPower = transform.GetChild(1).transform;
        arrowPower.gameObject.GetComponent<Image>().color = Constants.colors[PersistanceDataManager.instance.currentColor];
        ToggleArrowsVisibililty(false);
    }

    public void UpdateArrowProgress(float percentage) {
        arrowPower.GetComponent<Image>().fillAmount = percentage;
    }

    public void ToggleArrowsVisibililty(bool state) {
        if (!state) {
            arrowBase.GetComponents<TweenTransforms>()[0].Begin();
            arrowPower.GetComponents<TweenTransforms>()[0].Begin();
        } else {
            arrowBase.GetComponents<TweenTransforms>()[1].Begin();
            arrowPower.GetComponents<TweenTransforms>()[1].Begin();
        }

    }

    public void ResetDirectionArrows() {
        arrowBase.eulerAngles = new Vector3(90, 0, 0);
        arrowPower.eulerAngles = new Vector3(90, 0, 0);
    }

    public void UpdateRotationArrows(Quaternion rotation) {
        arrowBase.transform.rotation = rotation;
        arrowPower.transform.rotation = rotation;
    }

}