using UnityEngine;

public class PlanetConstraint : MonoBehaviour
{
    public Transform TargetPlanet;

    private void FixedUpdate()
    {
        Quaternion rotation = Quaternion.FromToRotation(-transform.up, TargetPlanet.position - transform.position);
        transform.rotation = rotation * transform.rotation;
    }
}
