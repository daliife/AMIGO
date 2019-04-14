using UnityEngine;

public class DeathManager : MonoBehaviour {

    private void OnTriggerEnter(Collider other) {
        if(other.tag.Equals("ball")) {
            Debug.Log("RESPAWN");
            BallController.instance.RespawnBall();
        }

    }

}