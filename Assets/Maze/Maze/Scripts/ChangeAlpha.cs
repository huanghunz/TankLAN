using UnityEngine;
using System.Collections;

public class ChangeAlpha : MonoBehaviour {

    public float Alpha = 0.5f;

    private Material _mat;
    private float _prevAlpha;

	// Use this for initialization
	void Start () {
        //var renderer = this.gameObject.GetComponent<MeshRenderer>();

        //var color = renderer.material.color;
        
        //color = new Color(color.r, color.g, color.b,0.1f);

        //renderer.material.color = color;

        _mat = this.gameObject.GetComponent<MeshRenderer>().material;
    }
	
	// Update is called once per frame
	void Update () {
        if (Alpha == _prevAlpha)
        {
            return;
        }

        _prevAlpha = Alpha;

        Color oldColor = _mat.color;
        Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, Alpha);
        _mat.SetColor("_Color", newColor);
    }
}
