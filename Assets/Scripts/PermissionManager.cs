using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class PermissionManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))  //Check for Camera permission
        {
            Permission.RequestUserPermission(Permission.Camera);
        }
    }

}
