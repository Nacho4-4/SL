using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyRotation : MonoBehaviour
{
    [SerializeField][Range(0, 5)] private float RotationPerSecond = 2;

    protected void Update() => RotationSky();

    private void RotationSky()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * RotationPerSecond);
    }
}