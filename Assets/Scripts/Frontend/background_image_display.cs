using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class background_image_display : MonoBehaviour {

    public Material effect_mat;

	void OnRenderImage (RenderTexture src, RenderTexture dst) {
        Graphics.Blit(src, dst, effect_mat, 0);
	}
}
