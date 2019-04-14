using UnityEngine;

public class ScoreManager : MonoBehaviour {

    private GameObject flagGameObject;

    private void Start() {
        flagGameObject = transform.GetChild(1).gameObject;
        flagGameObject.GetComponent<MeshRenderer>().materials[2].color = Constants.colors[PersistanceDataManager.instance.currentColor];
    }

    private void OnTriggerEnter(Collider other) {
        
        if (other.tag == "ball") {

            AudioManager.instance.playHoleSound();
            //flagGameObject.GetComponents<TweenTransforms>()[0].Begin();
            transform.GetChild(0).GetComponent<ParticleSystem>().Play();
            //https://insights.nimblechapps.com/unity/implementing-confetti-coming-out-of-an-object-in-unity

            other.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            other.GetComponent<Rigidbody>().useGravity = false;

            Destroy(other.gameObject);

            GameManager.instance.PlayGameOverAnimation();

        }

    }

}