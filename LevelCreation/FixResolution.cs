using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixResolution : MonoBehaviour
{
    [System.Serializable]
    public class Resolution
    {
        public float aspect;
        public float orthographicSize;
    }

    [SerializeField] Resolution[] resolutions;
    void Start()
    {
        Fix();
    }

    void Fix()
    {
        float currentAspect = float.MaxValue;
        foreach (var item in resolutions)
        {
            if (currentAspect > item.aspect)
                if (Camera.main.aspect < item.aspect)
                {
                    currentAspect = item.aspect;
                    Camera.main.orthographicSize = item.orthographicSize;
                }
        }
    }


}
