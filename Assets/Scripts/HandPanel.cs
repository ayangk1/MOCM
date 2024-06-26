using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPanel : MonoBehaviour
{
    public GameObject cam;
    public float matchPosOffset;
    public Transform target;
    public float lerpSpeed;

    public bool isFollow;

    public bool isLookAt;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isLookAt)
        {
            transform.LookAt(cam.transform);
        }
        
        if (isFollow)
        {
            Follow();
        }
    }

    void Follow()
    {
        transform.LookAt(cam.transform);
        //transform.localRotation = Quaternion.Euler(cam.transform.rotation.x,cam.transform.rotation.y,cam.transform.rotation.z);
        transform.position = cam.transform.position + cam.transform.forward * matchPosOffset;
        
        
        // Vector3 matchPos = new Vector3(target.localPosition.x + matchPosOffset.x, target.localPosition.y+ matchPosOffset.y
        //     ,target.localPosition.z+ matchPosOffset.z);
        // transform.position = new Vector3(Mathf.Lerp(transform.position.x, matchPos.x, lerpSpeed),
        //     Mathf.Lerp(transform.position.y, matchPos.y, lerpSpeed), Mathf.Lerp(transform.position.z, matchPos.z, lerpSpeed));
    }
}
