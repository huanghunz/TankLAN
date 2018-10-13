using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow360 : MonoBehaviour {

	static public Transform player;

    public float distance = 10;
	public float height = 5;
	public Vector3 lookOffset = new Vector3(0,1,0);
	public float cameraSpeed = 10;
	public float rotSpeed = 10;

    private float _distance = 0;

    private float _originalDistance = 0;


    private Vector3 _lookOffset;
    private Vector3 _originalLookOffset;
    private void Awake()
    {
        _originalDistance = this.distance;

        _distance = this.distance;

        _lookOffset = this.lookOffset;
        _originalLookOffset = new Vector3(_lookOffset.x, _lookOffset.y, _lookOffset.z);
    }

    void FixedUpdate()
    {
        if (player == null)
        {
            return;
        }

        Vector3 lookPosition = player.position + lookOffset;
        Vector3 relativePos = lookPosition - transform.position;
        Quaternion rot = Quaternion.LookRotation(relativePos);

        transform.rotation = Quaternion.Slerp(this.transform.rotation, rot, Time.deltaTime * rotSpeed * 0.1f);

       
        RaycastHit hit;
        Vector3 targetPos = player.transform.position + player.transform.up * this.height - player.transform.forward * _distance;

        this.transform.position = Vector3.Lerp(this.transform.position, targetPos, Time.deltaTime * cameraSpeed * 0.1f);
     
        if (Physics.Raycast(lookPosition, this.transform.position, out hit, (this.transform.position - lookPosition).magnitude + 1))
        {
            if (hit.transform.name == "Wall")
            {
                _distance = Mathf.Max(_distance - 0.025f, 1);
            }
        }
        else
        {
            _distance = Mathf.Min(_distance + 0.025f, _originalDistance);
        }

        this.distance = _distance;
    }
}
