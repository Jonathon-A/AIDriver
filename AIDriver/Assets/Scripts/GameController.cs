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
    private bool Change = true;

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
      
       
    }

}
