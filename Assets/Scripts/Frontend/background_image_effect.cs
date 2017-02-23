using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class background_image_effect : MonoBehaviour {

    int res_w;
    int res_h;

	void Start () {
		
        RenderTexture prepass = new RenderTexture(Screen.width,Screen.height,24);

        Camera camera = GetComponent<Camera>();
        Shader fill_shader = Shader.Find("Hidden/Fill Replace");

        camera.targetTexture = prepass;
        camera.SetReplacementShader(fill_shader, "RenderType");
        Shader.SetGlobalTexture("_FillPrepassTex", prepass);

        res_w = Screen.width;
        res_h = Screen.height;
	}


    void Update () {

        if (res_w != Screen.width || res_h != Screen.height) {
            print ("screen size changed");
            res_w = Screen.width;
            res_h = Screen.height;

            // clear the rendertexture
            Camera camera = GetComponent<Camera>();
            if (camera.targetTexture != null) {
                camera.targetTexture.Release();
            }

            // make a new one
            RenderTexture prepass = new RenderTexture(Screen.width,Screen.height,24);
            Shader fill_shader = Shader.Find("Hidden/Fill Replace");

            camera.targetTexture = prepass;
            camera.SetReplacementShader(fill_shader, "RenderType");
            Shader.SetGlobalTexture("_FillPrepassTex", prepass);
             
        }
    }


}
