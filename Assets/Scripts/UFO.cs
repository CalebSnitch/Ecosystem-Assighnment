using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Xml.Serialization;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class UFO : MonoBehaviour
{
     
      
    public float target_x;

    public float target_y;
    public Vector3 target_location;
    public GameObject abductee;
    public float flyingSpeed;
    public GameObject Manager;
    public Manager Manager_Script;
    public enum UFO_STATES
    {
        WANDERING_CHOOSING,
        WANDERING,
        INVISIBLE,
        MOVINGTOABDUCT,
        ABDUCTING,
        LEAVING

    }

    public UFO_STATES State = UFO_STATES.WANDERING_CHOOSING;


    void Start()
    {
        Manager = GameObject.Find("Manager");
        Manager_Script = Manager.GetComponent<Manager>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (State)
        {
            case UFO_STATES.WANDERING_CHOOSING:
                target_x = UnityEngine.Random.Range(-4f, 4f);
                target_y = UnityEngine.Random.Range(1f, 3f);
                target_location = new Vector3(target_x, target_y, 0);
                State = UFO_STATES.WANDERING;
                //Debug.Log("happened");
                break;

            case UFO_STATES.WANDERING:
                float x_distance = target_x - transform.position.x;
                float y_distance = target_y - transform.position.y;
                if (x_distance == 0 && y_distance == 0)
                    x_distance = .1f;
                float total_distance = Mathf.Abs(x_distance) + Mathf.Abs(y_distance);
                float x_movement = flyingSpeed * Time.deltaTime * x_distance/total_distance;
                float y_movement = flyingSpeed * Time.deltaTime * y_distance/total_distance;
                if (Mathf.Abs(x_movement) > Mathf.Abs(x_distance))
                {
                    x_movement = x_distance;
                }
                if (Mathf.Abs(y_movement) > Mathf.Abs(y_distance))
                {
                    y_movement = y_distance;
                }
                Vector3 movementVector = new Vector3(x_movement, y_movement, 0);
                transform.position += movementVector;

                //transition:
                if (transform.position == target_location)
                {
                    List<GameObject> hikersList = Manager_Script.activeHikers;
                    if (hikersList.Count > 0 && UnityEngine.Random.Range(0f, 1f) < .25)
                    {
                        //Debug.Log("happened 1");
                        if (hikersList.Count <= 3 && hikersList[hikersList.Count - 1].GetComponent<Hiker>().hasDog == false)
                        {
                            abductee = hikersList[hikersList.Count - 1];
                            State = UFO_STATES.MOVINGTOABDUCT;
                            //Debug.Log("happened 2");
                        }
                        else
                        {
                            State = UFO_STATES.WANDERING_CHOOSING;
                            //Debug.Log("Happened 3");
                        }
                    }
                    else
                    {
                        State = UFO_STATES.WANDERING_CHOOSING;
                        //Debug.Log("happened");
                    }
                }
                if (Manager_Script.timeOfDay > 7 && Manager_Script.timeOfDay < 21)
                {
                    State = UFO_STATES.INVISIBLE;
                }
                break;
            
            case UFO_STATES.INVISIBLE:
                this.GetComponent<SpriteRenderer>().color = new Color (1f, 1f, 1f, .25f);

                //transition:
                if (Manager_Script.timeOfDay > 21)
                {
                    this.GetComponent<SpriteRenderer>().color = new Color (1f, 1f, 1f, 1f);
                    State = UFO_STATES.WANDERING_CHOOSING;
                }
                break;

            case UFO_STATES.MOVINGTOABDUCT:
                Transform target_transform = abductee.GetComponent<Transform>();
                target_location = target_transform.position + new Vector3 (0, 1, 0);
                target_x = target_location.x;
                target_y = target_location.y;
                x_distance = target_x - transform.position.x;
                y_distance = target_y - transform.position.y;
                total_distance = Mathf.Abs(x_distance) + Mathf.Abs(y_distance);
                x_movement = flyingSpeed * (5/4) * Time.deltaTime * x_distance/total_distance;
                y_movement = flyingSpeed * (5/4) * Time.deltaTime * y_distance/total_distance;
                if (Mathf.Abs(x_movement) > Mathf.Abs(x_distance))
                {
                    x_movement = x_distance;
                }
                if (Mathf.Abs(y_movement) > Mathf.Abs(y_distance))
                {
                    y_movement = y_distance;
                }
                movementVector = new Vector3(x_movement, y_movement, 0);
                transform.position += movementVector;

                //transition:
                if (transform.position == target_location)
                {
                    abductee.GetComponent<Hiker>().State = Hiker.HIKER_STATES.ABDUCTED;
                    State = UFO_STATES.ABDUCTING;
                }

                if (Manager_Script.timeOfDay > 7 && Manager_Script.timeOfDay < 21)
                {
                    State = UFO_STATES.INVISIBLE;
                }
                break;

            case UFO_STATES.ABDUCTING:
                Hiker abductee_script = abductee.GetComponent<Hiker>();
                float lift = 1 * Time.deltaTime;
                abductee_script.transform.position += new Vector3(0, lift, 0);

                if (abductee_script.transform.position.y >= 1)
                {
                    abductee_script.Delete();
                    target_x = 0;
                    target_y = 10;
                    target_location = new Vector3(0, 10, 0);
                    State = UFO_STATES.LEAVING;
                }
                break;

            case UFO_STATES.LEAVING:
                x_distance = target_x - transform.position.x;
                y_distance = target_y - transform.position.y;
                if (x_distance == 0 && y_distance == 0)
                    x_distance = .1f;
                total_distance = Mathf.Abs(x_distance) + Mathf.Abs(y_distance);
                x_movement = flyingSpeed * (7/4) * Time.deltaTime * x_distance/total_distance;
                y_movement = flyingSpeed * (7/4) * Time.deltaTime * y_distance/total_distance;
                if (Mathf.Abs(x_movement) > Mathf.Abs(x_distance))
                {
                    x_movement = x_distance;
                }
                if (Mathf.Abs(y_movement) > Mathf.Abs(y_distance))
                {
                    y_movement = y_distance;
                }
                movementVector = new Vector3(x_movement, y_movement, 0);
                transform.position += movementVector;

                if (transform.position.y > 8)
                {
                    Delete();
                }
                break;
        }
    }

    public void Delete()
    {
        Manager_Script.activeUFOs.Remove(gameObject);
        Destroy(gameObject);
    }

}
