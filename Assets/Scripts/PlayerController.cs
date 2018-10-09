using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	private Rigidbody _rb;
    private float _speed = 25.0F / 4;
    private float _rotationSpeed = 75.0F;

    void Start(){
        _rb = this.GetComponent<Rigidbody>();
    }
	
	void FixedUpdate () {
	
        float translation = Input.GetAxis("Vertical") * _speed;
        float rotation = Input.GetAxis("Horizontal") * _rotationSpeed;
        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;
        Quaternion turn = Quaternion.Euler(0f,rotation,0f);
        _rb.MovePosition(_rb.position + this.transform.forward * translation);
        _rb.MoveRotation(_rb.rotation * turn);
	}
}
