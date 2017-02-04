using UnityEngine;
using System.Collections;

public class DeploymentArea : MonoBehaviour {

    public Player player;
	public Transform ground;


    void Awake () {
		//SetShape(ground.localScale.x, ground.localScale.z, player.playerIndex);        
        transform.name = "Deployment Area Player " + player.index;

		GameController.StartGame += SelfDestruction;
    }


//	void SetShape(float groundWidth, float groundHeight, int playerIndex) // For a rectangular field, 2 players
//    {
//        float deplAreaRatio = 1.0f / 3.0f;
//        Vector3 size = new Vector3(groundWidth, 1.0f, groundHeight * deplAreaRatio);
//        transform.localScale = size;
//        int playerSign = -1;
//		if (playerIndex == 1)
//        {
//            playerSign = 1;
//            transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
//        }
//        transform.position = new Vector3(0.0f, 0.01f, playerSign * (groundHeight - size.z) * 5.0f);
//    }


	public void SetVisible(int playerIndex)
    {
		if (playerIndex == player.index) {
			GetComponent<MeshRenderer>().enabled = true;
			GetComponent<MeshRenderer> ().material.renderQueue = 3090;
		}
        else GetComponent<MeshRenderer>().enabled = false;
    }

    void SelfDestruction() {Destroy(gameObject); }
}
