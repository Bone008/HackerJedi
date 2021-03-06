﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyParty : MonoBehaviour {

    public LevelGenerator level;
    public AudioSource song;
    public float songBPM;

    private Material blockMaterial;
    private Color initialColor;

    void Start() {
        blockMaterial = level.world[0, 0].GetComponentInChildren<Renderer>().sharedMaterial;
        initialColor = blockMaterial.GetColor("_MKGlowColor");
	}

    private void OnDisable()
    {
        blockMaterial.SetColor("_MKGlowColor", initialColor);
    }

    void Update () {
        float t = Time.time * (songBPM / 60.0f);
        Color c = (((int)t % 2) == 0 ? new Color(0, 0.5f, 1, 1) : new Color(1, 0, 0, 1)); //new Vector4(0, 0.5f, 1, 1);
        blockMaterial.SetColor("_MKGlowColor", Color.Lerp(c, new Color(0.75f, 0f, 1f, 1f)/*Color.red*/, t % 1.0f));

        if (Time.timeScale > 0 && song != null)
            song.pitch = Time.timeScale;
    }
}
