using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class active_highlight : MonoBehaviour {

    Button my_button;

    static Color normal_color;
    static Color highlight_color;
	
    void Awake()
    {
        my_button = GetComponent<Button>();
        if(my_button == null)
        {
            print("Um, there is no button attached to this component");
        }
        else
        {
            normal_color = my_button.colors.normalColor;
            highlight_color = my_button.colors.disabledColor;
        }
    }


    public void EnableHighlight()
    {
        ColorBlock cb = my_button.colors;
        cb.normalColor = highlight_color;
        my_button.colors = cb;
    }

    public void DisableHighlight()
    {
        ColorBlock cb = my_button.colors;
        cb.normalColor = normal_color;
        my_button.colors = cb;
    }
}
