using UnityEngine;

public class InputPosition : MonoBehaviour
{
    public static InputPosition Instance { get; private set; }

    private void Awake()
    {
        Instance = this; 
    }

    public Vector3 GetMouseScreenPostion()
    {
        //Debug.Log(Camera.main.ScreenToViewportPoint(Input.mousePosition));
        //Debug.Log(Input.mousePosition + " mouse position");
        return Camera.main.ScreenToViewportPoint(Input.mousePosition);
    }

    public Vector3 GetMousePlanePosition()
    {
        Ray mouseCameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        Plane plane = new Plane(Vector3.up, Vector3.zero);

        if(plane.Raycast(mouseCameraRay, out float distance))
        {
            //Debug.Log(mouseCameraRay.GetPoint(distance));
            return mouseCameraRay.GetPoint(distance);
        }
        else { return Vector3.zero; }
    }

    public Vector3 GetMousePhysicPosition()
    {
        Ray mouseCameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(mouseCameraRay, out RaycastHit hitInfo))
        {
            return hitInfo.point;
        }
        else { return Vector3.zero; }
    }
}
