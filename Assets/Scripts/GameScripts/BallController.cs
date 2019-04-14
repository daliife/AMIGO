using UnityEngine;
using UnityEngine.UI;

public class BallController : MonoBehaviour {

    public static BallController instance;

    public Transform arrowObj;
    public GameObject fixedCircle;
    public GameObject movingCircle;
    public GameObject lineAsset;
    public GameObject cancelButton;

    private Rigidbody rb;
    private float angleInput = 0.0f;
    private float percentage = 0.0f;
    private float minForce = 100.0f;
    private float maxForce = 750.0f;
    private Vector3 initialClick;
    private Vector3 finalClick;

    private bool ballEnabled = true;
    private bool isHittingBall = false;
    private bool cancelButtonDetected = false;
    private Vector3 respawnPosition;
    
    private void Awake() {

        if (instance == null) {
            instance = this;
        } else if (instance != null) {
            Destroy(gameObject);
        }

    }

    void Start() {

        //Rigidbody reference
        rb = GetComponent<Rigidbody>();

        //Hide 2d assets
        toggleDirections(false);

        //Set initial respawn location
        respawnPosition = transform.position;

        //Update color 
        GetComponent<MeshRenderer>().material.color = Constants.colors[PersistanceDataManager.instance.currentColor];
        movingCircle.GetComponent<Image>().color = Constants.colors[PersistanceDataManager.instance.currentColor];

    }

    void Update() {

        //Update arrow sprite position
        arrowObj.transform.position = transform.position + new Vector3(0, 0, 1.25f);

        if (isHittingBall && rb.velocity.sqrMagnitude < 0.5f) {
            StopBall();
        }

        if (ballEnabled) {
            ClickEvent();
            DragEvent();
            CancelClickEvent();
            ReleaseEvent();
        }


    }

    public void ClickEvent() {

        if (Input.GetMouseButtonDown(0)) {

            StopBall();

            //Initial click
            RaytracePoint(true);

            //Update hud positions
            fixedCircle.transform.position = Input.mousePosition;
            movingCircle.transform.position = Input.mousePosition;

            //Show hud assets
            ArrowController.instance.ToggleArrowsVisibililty(true);
            toggleDirections(true);

            //Cancel button show/anim
            cancelButton.GetComponents<TweenTransforms>()[1].Begin();

            AudioManager.instance.playToggleShotSound(true);

        }

    }

    public void DragEvent() {

        if (Input.GetMouseButton(0)) {

            //Final click
            RaytracePoint(false);

            //Hud assets update
            movingCircle.transform.position = Input.mousePosition;
            lineAsset.GetComponent<LineRenderer>().SetPosition(0, fixedCircle.transform.localPosition);
            lineAsset.GetComponent<LineRenderer>().SetPosition(1, movingCircle.transform.localPosition);

            //Rotation arrow
            Vector3 target = initialClick - finalClick;
            angleInput = Vector3.SignedAngle(target.normalized, transform.forward, Vector3.up);
            Quaternion rotation = Quaternion.AngleAxis(90, Vector3.right) * Quaternion.AngleAxis(angleInput, Vector3.forward);
            ArrowController.instance.UpdateRotationArrows(rotation);

            //Power value
            percentage = Mathf.Clamp(target.magnitude / 5.0f, 0, 1);
            ArrowController.instance.UpdateArrowProgress(percentage);

        }
    }

    public void CancelClickEvent() {

        if (Input.GetMouseButton(1) || 
            (cancelButtonDetected && Input.GetMouseButton(0))) {

            ArrowController.instance.ToggleArrowsVisibililty(false);
            toggleDirections(false);

            cancelButtonDetected = false;
            cancelButton.GetComponents<TweenTransforms>()[0].Begin();
            AudioManager.instance.playToggleShotSound(false);

            Input.ResetInputAxes();
            return;

        }
    }

    public void ReleaseEvent() {

        if (Input.GetMouseButtonUp(0)) {

            ArrowController.instance.ToggleArrowsVisibililty(false);
            toggleDirections(false);
            cancelButton.GetComponents<TweenTransforms>()[0].Begin();

            GameManager.instance.RegisterShot();
            AudioManager.instance.playShotSound();

            Vector3 forwardVector = arrowObj.GetChild(0).transform.rotation * -arrowObj.GetChild(0).transform.forward;
            float ammountForce = (minForce + (percentage * Mathf.Abs(maxForce - minForce)));
            rb.AddRelativeForce(ammountForce * forwardVector);
            isHittingBall = true;

        }

    }

    public void toggleCancelButton() {
        cancelButtonDetected = true;
    }

    public void SetBallEnabled(bool state) {
        ballEnabled = state;
    }

    private void StopBall() {
        rb.velocity = new Vector3(0, 0, 0);
        rb.angularVelocity = new Vector3(0, 0, 0);
        transform.eulerAngles = new Vector3(0, 0, 0);
        isHittingBall = false;
        respawnPosition = transform.position;
        ArrowController.instance.ResetDirectionArrows();
    }

    public void RespawnBall() {     
        transform.position = respawnPosition;
        StopBall();
    }

    private void toggleDirections(bool state) {

        if (!state) {
            fixedCircle.GetComponents<TweenTransforms>()[0].Begin();
            movingCircle.GetComponents<TweenTransforms>()[0].Begin();
            lineAsset.GetComponents<TweenAlpha>()[0].Begin();
        } else {
            fixedCircle.GetComponents<TweenTransforms>()[1].Begin();
            movingCircle.GetComponents<TweenTransforms>()[1].Begin();
            lineAsset.GetComponents<TweenAlpha>()[1].Begin();
        }

    }

    private void RaytracePoint(bool isInitial) {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) {
            if (!isInitial)
                finalClick = hit.point;
            else
                initialClick = hit.point;
        } else {
            Debug.Log("NO HIT FOUND ON RAYTRACE");
        }
    }

}