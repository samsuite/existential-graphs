using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class background_image_effect : MonoBehaviour {

	void Start () {
		
        RenderTexture prepass = new RenderTexture(Screen.width,Screen.height,24);

        Camera camera = GetComponent<Camera>();
        Shader fill_shader = Shader.Find("Hidden/Fill Replace");

        camera.targetTexture = prepass;
        camera.SetReplacementShader(fill_shader, "RenderType");
        Shader.SetGlobalTexture("_FillPrepassTex", prepass);
	}

}
