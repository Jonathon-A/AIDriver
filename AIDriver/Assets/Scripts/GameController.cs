using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class GameController : MonoBehaviour
{
    public bool AllowCol;
    public static bool ColAllowed;
    // Start is called before the first frame update
    void Awake()
    {
        ColAllowed = AllowCol;
        Physics.IgnoreLayerCollision(3, 3, !AllowCol);
    }

    public bool Render;
    public bool RenderWaypoints;
    public bool RenderBarriers;
    private bool Change = true;
    private bool Change2 = true;
    private bool Change3 = true;
    public Transform WaypointTransform;
    public Transform BarrierTransform;
    private void Update()
    {
        if (Render != Change)
        {
            Change = Render;
            List<GameObject> rootObjects = new List<GameObject>();
            Scene scene = SceneManager.GetActiveScene();
            scene.GetRootGameObjects(rootObjects);
            if (Render)
            {
                foreach (GameObject GO in rootObjects)
                {
                    Renderer Rndr = GO.GetComponent<Renderer>();
                    if (Rndr)
                    {
                        Rndr.enabled = true;
                    }



                    Renderer[] renderChildren = GO.GetComponentsInChildren<Renderer>();

                   
                    for (int i = 0; i < renderChildren.Length; ++i)
                    {
                        renderChildren[i].enabled = true;
                    }

                }
            }
            else {
                foreach (GameObject GO in rootObjects)
                {
                    Renderer Rndr = GO.GetComponent<Renderer>();
                    if (Rndr)
                    {
                        Rndr.enabled = false;
                    }
                    Renderer[] renderChildren = GO.GetComponentsInChildren<Renderer>();

                
                    for (int i = 0; i < renderChildren.Length; ++i)
                    {
                        renderChildren[i].enabled = false;
                    }
                }
            }
        }

        if (RenderWaypoints != Change2)
        {
            Change2 = RenderWaypoints;
            if (RenderWaypoints)
            {
                foreach (Transform Waypoint in WaypointTransform)
                {

                    Waypoint.GetComponent<Renderer>().enabled = true;
                }
            }
            else
            {
                foreach (Transform Waypoint in WaypointTransform)
                {

                    Waypoint.GetComponent<Renderer>().enabled = false;
                }
            }
 
        }

        if (RenderBarriers != Change3)
        {
            Change3 = RenderBarriers;
            if (RenderBarriers)
            {
                foreach (Transform Barrier in BarrierTransform)
                {

                    Barrier.GetComponent<Renderer>().enabled = true;
                }
            }
            else
            {
                foreach (Transform Barrier in BarrierTransform)
                {

                    Barrier.GetComponent<Renderer>().enabled = false;
                }
            }

        }


    }

}
