using UnityEngine;

public static class GeomF {


	// Projection of a point to an y-plane (coordinate y) along the ray between the point and the camera
	public static Vector3 ToYPlane(Vector3 point, Vector3 cameraCoord, float y)
	{
		float t = (cameraCoord.y - y) / (point.y - cameraCoord.y);
		return new Vector3(cameraCoord.x - (point.x - cameraCoord.x) * t, y, cameraCoord.z - (point.z - cameraCoord.z) * t);
	}


	/// <summary> Angle between two directions in the y plane between 0 and 360. </summary>
	/// <returns> The angle in degrees between 0 and 360 </returns>
	public static float YAngleWithSign(Vector3 dir1, Vector3 dir2){
		Vector3 normDir1 = dir1.normalized;
		Vector3 normDir2 = dir2.normalized;
		float angleY1 =  Mathf.Acos(normDir1.z) * Mathf.Rad2Deg;
		float angleY2 =  Mathf.Acos(normDir2.z) * Mathf.Rad2Deg;
		if (normDir1.x < 0) angleY1 = - angleY1;
		if (normDir2.x < 0) angleY2 = - angleY2;
		if (angleY2 - angleY1 < 0) return 360f + angleY2 - angleY1;
		return angleY2 - angleY1;
	}


	// direction parallel to the y plane given the angle with the z axis
	public static Vector3 DirFromAngleY(float degAngle){
		return new Vector3 (Mathf.Sin(degAngle*Mathf.Deg2Rad), 0, Mathf.Cos(degAngle*Mathf.Deg2Rad)).normalized;
	}


	// opposite angle for a deg angle in [0, 360]
	public static float OppositeAngle(float degAngle) {
		float newAngle = degAngle + 180;
		if (newAngle > 360) newAngle -= 360;
		return newAngle;
	}
}

