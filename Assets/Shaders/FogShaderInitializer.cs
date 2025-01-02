using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogShaderInitializer : MonoBehaviour
{
    public Material fogMaterial; // Assign your material with the shader here

    void Start()
    {
        // Generate a random seed between 0.0 and 1.0
        float randomSeed = Random.value;

        // Set the random seed in the shader
        fogMaterial.SetFloat("_RandomSeed", randomSeed);
    }
}


