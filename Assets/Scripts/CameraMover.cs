using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// Zoom and translation of the camera  ? Correggere per inclinazioni asse y!
public class CameraMover : MonoBehaviour{

    /// <summary>Minimum field of view angle</summary>
    public float minCameraSize;

    /// <summary>Zoom sensitivity</summary>
    public float sensitivity;

    /// <summary>Camera translation speed</summary>
    public float speed;

    /// <summary>Camera angle woth respect the x axis</summary>
    public float xAngle;

    /// <summary>Battle field boundaries</summary>
    public Transform terrain;

    /// <summary>Auxiliary camera which render the areas on top of the other objects</summary>
    public Camera areasCamera;

    /// <summary>Camera direction of each player, alligned with the deployment area forwards</summary>
	List<Vector3> playerForwardsDir = new List<Vector3> ();

    /// <summary>Center of each deployment area</summary>
    List<Vector3> playerAreaPos = new List<Vector3>();

    /// <summary>Current camera field of view angle</summary>
    float cameraSize;

    /// <summary>Maximum camera field of view angle</summary>
    float maxCameraSize;

    /// <summary>Minimum and maximum value of camera translation along the x axis</summary>
    Vector2 xBound;

    /// <summary>Minimum and maximum value of camera translation along the z axis</summary>
    Vector2 zBound;

    /// <summary>Fixed camera y position</summary>
    float yCamera;

    /// <summary>Speed traslation correction due to the camera x-axis rotation</summary>
    float speedCorrection;

    float currentYRotationAngle;
    Vector3 fieldCenter;
    Vector2 fieldSize;



    void Awake()
    {
		GameController.ArmyTurn += ArmyAlignment;
		GameController.DeploymentTurn += PlayerAlignment;
    }


    void Start()
    {
        speedCorrection = 1 / Mathf.Cos(xAngle * Mathf.Deg2Rad);
        yCamera = transform.position.y;
        SetTerrainParams();
        transform.rotation = Quaternion.Euler(xAngle, 0, 0);
        SetMaxFieldOfViewAngle();
        cameraSize = (maxCameraSize + minCameraSize) * 0.6f;
        SetBoundaries();
        currentYRotationAngle = transform.rotation.eulerAngles.y;
    }

    void FixedUpdate ()
    {
        // Zoom
        cameraSize -= Input.GetAxis("Mouse ScrollWheel") * sensitivity;
        cameraSize = Mathf.Clamp(cameraSize, minCameraSize, maxCameraSize);
        GetComponent<Camera>().fieldOfView = cameraSize;
        areasCamera.fieldOfView = cameraSize;

        // If the y angle changes, the boundaries are recalculated
        if (currentYRotationAngle != transform.rotation.eulerAngles.y)
            SetBoundaries();
        currentYRotationAngle = transform.rotation.eulerAngles.y;

        // Translation
        float moveH = Input.GetAxis ("Horizontal");
        float moveV = Input.GetAxis ("Vertical");
        Vector3 direction = new Vector3 (moveH, 0.0f, moveV * speedCorrection);
        transform.Translate(direction * speed);
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, xBound[0], xBound[1]),
            yCamera,
            Mathf.Clamp(transform.position.z, zBound[0], zBound[1])
        );
    }


	public void PlayerAlignment(Player player)
    {
        transform.rotation = Quaternion.LookRotation(playerForwardsDir[player.index]);
        transform.rotation = Quaternion.Euler(xAngle, transform.rotation.eulerAngles.y, 0);
        transform.position = CameraPosPointingWorldPoint(playerAreaPos[player.index]);
    }


	public void ArmyAlignment (ArmyController armyCtrl) {
        transform.rotation = Quaternion.LookRotation(playerForwardsDir[armyCtrl.player.index]);
        transform.rotation = Quaternion.Euler(xAngle, transform.rotation.eulerAngles.y, 0);
        transform.position = CameraPosPointingWorldPoint(armyCtrl.transform.position, -0.3f);
    }

    /// <summary>Sets the field size, field center and the orientation of each player</summary>
    void SetTerrainParams()
    {
        Transform north = terrain.FindChild("Boundaries").FindChild("North");
        Transform south = terrain.FindChild("Boundaries").FindChild("South");
        Transform east = terrain.FindChild("Boundaries").FindChild("East");
        Transform west = terrain.FindChild("Boundaries").FindChild("West");
        fieldCenter = new Vector3(north.position.x, 0, east.position.z);

        float width = east.position.x - west.position.x - east.localScale.x / 2.0f - west.localScale.x / 2.0f;
        float length = north.position.z - south.position.z - north.localScale.z / 2.0f - south.localScale.z / 2.0f;
        fieldSize = new Vector2(width, length);

        Transform areas = terrain.FindChild("Deployment Areas");
        foreach (Transform area in areas)
        {
            playerForwardsDir.Add(area.forward);
            playerAreaPos.Add(area.position + area.forward * area.localScale.x);
        }
            
    }

    /// <summary>
    /// Gets the camera coordinates in a way that the worldPoint is along the vertical central axis of the screen.
    /// If fractionForward=0 the worldPointis at the exact center, otherwise is at an intermediate poistion
    /// along the vertical axis.
    /// (Funziona un po' a cazzo la posizione intermedia, dovrebbe essere tra -1 e 1 ma i punti estremi corrispondono a valori diversi)
    /// The y coordinate and the camera rotation are not changed
    /// </summary>
    Vector3 CameraPosPointingWorldPoint(Vector3 worldPoint, float fractionForward)
    {
        float a = yCamera / Mathf.Tan((xAngle - cameraSize * fractionForward / 2.0f) * Mathf.Deg2Rad);
        float ax = a * Mathf.Sin(transform.rotation.eulerAngles.y * Mathf.Deg2Rad);
        float az = a * Mathf.Cos(transform.rotation.eulerAngles.y * Mathf.Deg2Rad);
        return new Vector3(-ax, yCamera, -az) + worldPoint;
    }


    /// <summary>Gets the camer coordinates in a way that the worldPoint is at the screen center.
    /// The y coordinate and the camera rotation are not changed</summary>
    Vector3 CameraPosPointingWorldPoint(Vector3 worldPoint)
    {
        return CameraPosPointingWorldPoint(worldPoint, 0);
    }

    void SetMaxFieldOfViewAngle()
    {
        float maxWidth = Mathf.Max(fieldSize.x, fieldSize.y);
        Vector3 cameraPosPointingCenter = CameraPosPointingWorldPoint(fieldCenter);
        float a = Vector3.Distance(fieldCenter, new Vector3(cameraPosPointingCenter.x, 0, cameraPosPointingCenter.z)) - maxWidth / 2.0f;
        float alp = transform.rotation.eulerAngles.x;
        if (a > 0) maxCameraSize = 2 * (Mathf.Atan(yCamera / a) * Mathf.Rad2Deg - alp);
        else maxCameraSize = 2 * (180 - Mathf.Atan(-yCamera / a) * Mathf.Rad2Deg - alp);
        GetComponent<Camera>().fieldOfView = cameraSize;
    }


    void SetBoundaries()
    {
        Vector3 cameraPosPointingCenter = CameraPosPointingWorldPoint(fieldCenter);
        xBound = new Vector2(cameraPosPointingCenter.x - fieldSize.x / 2.0f, cameraPosPointingCenter.x + fieldSize.x / 2.0f);
        zBound = new Vector2(cameraPosPointingCenter.z - fieldSize.y / 2.0f, cameraPosPointingCenter.z + fieldSize.y / 2.0f);
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



}
