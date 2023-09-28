using UnityEngine;

public class Route: MonoBehaviour {
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private Transform pointC;
    [SerializeField] private Transform pointD;
    // [SerializeField] private Transform pointAB;
    // [SerializeField] private Transform pointBC;
    // [SerializeField] private Transform pointCD;
    // [SerializeField] private Transform pointAB_BC;
    // [SerializeField] private Transform pointBC_CD;
    [SerializeField] private Transform pointABCD;

    private float interpolateAmount;
    private void Update() {
        pointABCD.position = CubicLerp(pointA.position, pointB.position, pointC.position, pointD.position, interpolateAmount);
        interpolateAmount = (interpolateAmount + Time.deltaTime) % 1f;
        // pointAB.position = Vector3.Lerp(pointA.position, pointB.position, interpolateAmount);
        // pointBC.position = Vector3.Lerp(pointB.position, pointC.position, interpolateAmount);
        // pointCD.position = Vector3.Lerp(pointC.position, pointD.position, interpolateAmount);
        // pointABCD.position = Vector3.Lerp(pointC.position, pointD.position, interpolateAmount);
        
        // pointAB_BC.position = Vector3.Lerp(pointAB.position, pointBC.position, interpolateAmount);
        // pointBC_CD.position = Vector3.Lerp(pointBC.position, pointCD.position, interpolateAmount);
    
        // pointABCD.position = Vector3.Lerp(pointAB_BC.position, pointBC_CD.position, interpolateAmount);
    }

private Vector3 QuadraticLerp(Vector3 a, Vector3 b, Vector3 c, float t) {
    Vector3 ab = Vector3.Lerp(a, b, t);
    Vector3 bc = Vector3.Lerp(b, c, t);

    return Vector3.Lerp(ab, bc, interpolateAmount);
}

private Vector3 CubicLerp(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t) {
    Vector3 ab_bc = QuadraticLerp(a, b, c, t);
    Vector3 bc_cd = QuadraticLerp(b, c, d, t);

    return Vector3.Lerp(ab_bc, bc_cd, interpolateAmount);
}

    // [ExecuteInEditMode]
    // [SerializeField]
    // private Transform[] controlPoints;

    // private Vector3 gizmosPosition;
    // private void OnDragGizmos() {
    //     for (float t = 0; t <= 1; t += 0.05f) {
    //         gizmosPosition = Mathf.Pow(1 - t, 3) * controlPoints[0].position +
    //             3 * Mathf.Pow(1 - t, 2) * t * controlPoints[1].position +
    //             3 * (1 - t) * Mathf.Pow(t, 2) * controlPoints[2].position +
    //             Mathf.Pow(t, 3) * controlPoints[3].position;

    //         Gizmos.DrawSphere(gizmosPosition, 0.25f);
    //     }

    //     Gizmos.DrawLine(new Vector3(controlPoints[0].position.x, controlPoints[0].position.y),
    //         new Vector3(controlPoints[1].position.x, controlPoints[1].position.y));

    //     Gizmos.DrawLine(new Vector3(controlPoints[2].position.x, controlPoints[2].position.y),
    //         new Vector3(controlPoints[3].position.x, controlPoints[3].position.y));
    // }
}