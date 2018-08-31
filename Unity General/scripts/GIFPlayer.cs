using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GIFPlayer : MonoBehaviour {
    public Texture2D[] frames;
    public float framesPerSecond = 1.0f;

    void FixedUpdate()
    {
        int index = (int)System.Math.Round(Time.time * framesPerSecond, 0);
        index = index % frames.Length;
        GetComponent<Renderer>().material.mainTexture = frames[index];
        GetComponent<Renderer>().material.SetTexture("_EMISSION", frames[index]);
    }
}
