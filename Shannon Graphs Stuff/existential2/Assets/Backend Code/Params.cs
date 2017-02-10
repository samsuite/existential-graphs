using UnityEngine;
using System.Collections;

public class Params {

	// this is a container for a whole boatload of parameters that inference rules may need access to.
    // for example, when creating a variable, what's the name of the variable? that's a parameter.
    // we'll just add to this as we find things that inference rules need to know.

    // we could also probably just have this be a <string, string> dictionary, but a class seems more readable.


    // we won't instantiate these variables. we'll set up the values when we create the particular instances.
    public string variable_name;
    public float posX;
    public float posY;
}
