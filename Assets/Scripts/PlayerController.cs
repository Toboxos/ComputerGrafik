using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Transform PlayerPos;
    Rigidbody rigidbody;
    Animator animator;

    public float Speed = 1;
    //Game Idea Player Speeds Up over Time and Must Avoid Obstacles
    // Or gets Followed by Homeing Missles
    float Acceleration;

    public bool isUboat = true;

    public float PitchSpeed = 15;

    //Uboat
    public float ResetTurningRate = 50;
    public float UboatYawSpeed = 15;

    //Plane
    public float PlaneRollSpeed = 10;

    // Start is called before the first frame update
    void Start()
    {
        PlayerPos = GetComponent<Transform>();
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    //Make Explosion on Collision
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Hit Something");
        Debug.Log("EXPLOSION BRIADSASDASDASFDASGF BUUUUUM");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rigidbody.velocity = transform.forward * Speed;
        animator.SetBool("isUboat", isUboat);

        if (isUboat)
        {
            Vector3 TargetRotation = transform.rotation.eulerAngles;

            //Angle Move Back to Z 90°
            TargetRotation.z = 90;

            rigidbody.MoveRotation(Quaternion.Lerp(transform.rotation, Quaternion.Euler(TargetRotation), Time.deltaTime * ResetTurningRate));
        }

        //Move Forward
        if (Input.GetKey(KeyCode.W))
        {
            Quaternion deltaRotation = Quaternion.Euler(Vector3.up * -PitchSpeed * Time.deltaTime);
            rigidbody.MoveRotation(rigidbody.rotation * deltaRotation);
        }
        if (Input.GetKey(KeyCode.S))
        {
            Quaternion deltaRotation = Quaternion.Euler(Vector3.up * PitchSpeed * Time.deltaTime);
            rigidbody.MoveRotation(rigidbody.rotation * deltaRotation);
        }

        if (Input.GetKey(KeyCode.A))
        {
            if (isUboat)
            {
                Quaternion deltaRotation = Quaternion.Euler(Vector3.up * -UboatYawSpeed * Time.deltaTime);
                rigidbody.MoveRotation(deltaRotation * rigidbody.rotation);
            }
            else
            {
                Quaternion deltaRotation = Quaternion.Euler(Vector3.back * -PlaneRollSpeed * Time.deltaTime);
                rigidbody.MoveRotation(rigidbody.rotation * deltaRotation);
            }
        }
        if (Input.GetKey(KeyCode.D))
        {
            if (isUboat)
            {
                //TODO: Rotate to World Space instead of Local Space
                Quaternion deltaRotation = Quaternion.Euler(Vector3.up * UboatYawSpeed * Time.deltaTime);
                rigidbody.MoveRotation(deltaRotation * rigidbody.rotation);
            }
            else
            {
                Quaternion deltaRotation = Quaternion.Euler(Vector3.back * PlaneRollSpeed * Time.deltaTime);
                rigidbody.MoveRotation(rigidbody.rotation * deltaRotation);
            }
        }
    }
}
