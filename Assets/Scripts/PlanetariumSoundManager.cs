using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProVideo;
using FMOD.Studio;

public class PlanetariumSoundManager : MonoBehaviour
{
    [SerializeField]
    private FMODUnity.StudioEventEmitter[] planetariumEmitters;

    public void PlaySound(int videoNumber)
    {
        SetGlobalParameter("Planetarium_Video", (float)videoNumber);
        foreach (FMODUnity.StudioEventEmitter see in planetariumEmitters)
        {
            see.Stop();
            see.Play();
        }
    }

    void SetGlobalParameter(string name, float value)
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName(name, value);
    }
}
