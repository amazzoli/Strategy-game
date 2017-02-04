using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// Zoom and translation of the camera  ? Correggere per inclinazioni asse y!
public class CameraMover : MonoBehaviour{

    public float minCameraSize, sensitivity, speed, xAngle;
    private float cameraSize, maxCameraSize, xBound, zBound, y0, speedCorrection;
    private Vector3 position0;
    public Transform ground;
	public DeploymentArea[] deploymentAreas;
	private List<Vector3> areaForward = new List<Vector3> ();



    void Awake()
    {
		GameController.ArmyTurn += ArmyAlignment;
		GameController.DeploymentTurn += PlayerAlignment;
        // Some initializations
        if (transform.rotation.eulerAngles.x == 90) transform.Rotate(new Vector3(-1, 0, 0));
        speedCorrection = 1 / Mathf.Cos(xAngle * Mathf.Deg2Rad);
        y0 = transform.position.y;

        // Setting position0, where the camera points to the center of the field (which must be the origin)
        SetPosition0();

        // Setting the maximum field of view, i.e. cameraSize, and the initial field of view
		float maxWidth = Mathf.Sqrt(ground.localScale.x * ground.localScale.x + ground.localScale.z * ground.localScale.z) * 10;
        float a = Vector3.Distance(Vector3.zero, new Vector3(transform.position.x, 0, transform.position.z)) - maxWidth / 2.0f;
        float alp = transform.rotation.eulerAngles.x;
        if (a > 0) maxCameraSize = 2 * (Mathf.Atan(transform.position.y / a) * Mathf.Rad2Deg - alp);
        else maxCameraSize = 2 * (180 - Mathf.Atan(-transform.position.y / a) * Mathf.Rad2Deg - alp);
        cameraSize = maxCameraSize - 5;
        GetComponent<Camera>().fieldOfView = cameraSize;       
            
        // Setting the boundaries of the camera translation, which depends on the field of view
        SetBoundaries();
    }


    void Start()
    {
		for (int i = 0; i < deploymentAreas.Length; i++) areaForward.Add (deploymentAreas [i].transform.forward);
    }

    void FixedUpdate ()
    {
        // Zoom
        float cameraSize0 = cameraSize;
        cameraSize -= Input.GetAxis("Mouse ScrollWheel") * sensitivity;
        cameraSize = Mathf.Clamp(cameraSize, minCameraSize, maxCameraSize);
        GetComponent<Camera>().fieldOfView = cameraSize;

        // If the field of view changes, the boundaries are recalculated
        if (cameraSize0 != cameraSize) SetBoundaries();

        // Translation
        float moveH = Input.GetAxis ("Horizontal");
        float moveV = Input.GetAxis ("Vertical");
        Vector3 direction = new Vector3 (moveH, 0.0f, moveV * speedCorrection);
        transform.Translate(direction * speed);
        transform.position = new Vector3 (
            Mathf.Clamp (transform.position.x, position0.x - xBound, position0.x + xBound),
            y0,
            Mathf.Clamp (transform.position.z, position0.z - zBound, position0.z + zBound)
        );
    }


	public void PlayerAlignment(Player player)
    {
		Quaternion rotation = Quaternion.FromToRotation(new Vector3(0, 0, 1), areaForward[player.index]);
        transform.rotation = Quaternion.Euler(0, 0, 0)*rotation* Quaternion.Euler(xAngle, 0, 0);
        SetPosition0();
        transform.position += GetCorrection((maxCameraSize + minCameraSize) / 2.0f);
		transform.rotation = Quaternion.Euler (xAngle, transform.rotation.eulerAngles.y, 0);
    }


	void ArmyAlignment (ArmyController armyCtrl) {
		PlayerAlignment (armyCtrl.player);
	}


    void SetPosition0()
    {
        float ax = transform.position.y / Mathf.Tan(xAngle * Mathf.Deg2Rad) * Mathf.Sin(transform.rotation.eulerAngles.y * Mathf.Deg2Rad);
        float az = transform.position.y / Mathf.Tan(xAngle * Mathf.Deg2Rad) * Mathf.Cos(transform.rotation.eulerAngles.y * Mathf.Deg2Rad);
        position0 = new Vector3(-ax, transform.position.y, -az);
        transform.position = position0;
    }

    // The camera view is asymmetric if the center of the screen coincides to the center of the field (because of the camera inclination). Below this asymmetry is 
    // approximately corrected. A better correction should be recomputed at each change of field of view
    Vector3 GetCorrection(float camSize)
    {
        float beta = (xAngle + camSize / 2.0f) * Mathf.Deg2Rad;
        float gam = (xAngle - camSize / 2.0f) * Mathf.Deg2Rad;
        float gap = transform.position.y * (1/Mathf.Tan(beta) + 1 / Mathf.Tan(gam) - 2 / Mathf.Tan(xAngle * Mathf.Deg2Rad)) / 2.0f;
        return - new Vector3(gap * Mathf.Sin(transform.rotation.eulerAngles.y * Mathf.Deg2Rad), 0, gap * Mathf.Cos(transform.rotation.eulerAngles.y * Mathf.Deg2Rad));
    }


    void SetBoundaries()
    {
		xBound = ground.localScale.x / 2.0f / (minCameraSize - maxCameraSize) * (cameraSize - maxCameraSize) * 10;
		zBound = ground.localScale.z / 2.0f / (minCameraSize - maxCameraSize) * (cameraSize - maxCameraSize) * 10;
    }
}
