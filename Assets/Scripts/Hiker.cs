using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Hiker : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite walking_sprite;
    public Sprite sleeping_sprite;
    public Sprite abducted_sprite;
    
    public int hikingDirection;
    public bool hasDog = false;
    public NewMonoBehaviourScript ownedDog;
    public float hiking_speed = 5;

    public GameObject Manager;
    public Manager Manager_Script;

    public enum HIKER_STATES
    {
        HIKING,
        SLEEPING,
        TALKING,
        ABDUCTED

    }
    public HIKER_STATES State = HIKER_STATES.HIKING;

    private bool is_colliding = false;

    private GameObject collided_object;

    private float talking_timer = 0;
    public int talking_duration;

    public bool has_talked = false;


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
            case HIKER_STATES.HIKING:
                gameObject.GetComponent<Transform>().position += new Vector3(hiking_speed, 0, 0) * Time.deltaTime * hikingDirection;
                
                spriteRenderer.sprite = walking_sprite;
                if (hikingDirection == 1)
                {
                    spriteRenderer.flipX = false;
                }
                else
                {
                    spriteRenderer.flipX = true;
                }

                //transitions:
               

                talking_timer += Time.deltaTime;
                if (talking_timer >= talking_duration)
                {
                    has_talked = false;
                }
                
                if (is_colliding == true && has_talked == false &&
                collided_object.GetComponent<Prefab>().type == Prefab.TYPE.HIKER)
                {
                    if (collided_object.GetComponent<Hiker>().has_talked == false)
                    {
                        talking_timer = 0;
                        State = HIKER_STATES.TALKING;
                        //Debug.Log("Happened");
                    }
                }

                 if (Manager_Script.timeOfDay > 21)
                {
                    State = HIKER_STATES.SLEEPING;
                    Debug.Log("happened");
                }

                break;


            case HIKER_STATES.SLEEPING:
            
                spriteRenderer.sprite = sleeping_sprite;

                //transitions:
                if (Manager_Script.timeOfDay > 9 && Manager_Script.timeOfDay < 21)
                {
                    State = HIKER_STATES.HIKING;
                    //Debug.Log("happened 2");
                }

                break;

            case HIKER_STATES.TALKING:
            
            has_talked = true;

            //SET SPRITE/ANIMATION

            //transitions:
            talking_timer += Time.deltaTime;
            if (talking_timer > talking_duration)
            {
                talking_timer = 0;
                State = HIKER_STATES.HIKING;
            }

                break;

            case HIKER_STATES.ABDUCTED:
                spriteRenderer.sprite = abducted_sprite;
                break;

             
        }

        //handles destruction:
        if (hikingDirection == -1 && transform.position.x < -7)
        {
            Delete();
        }
        
        if (hikingDirection == 1 && transform.position.x > 7)
        {
            Delete();
        }
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        is_colliding = true;

        collided_object = collision.gameObject;

        //Debug.Log("happened");
    }


    void OnTriggerExit2D(Collider2D collision)
    {
        is_colliding = false;
        //Debug.Log("Happened");
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        is_colliding = false;
    }

    public void Delete()
    {
        Manager_Script.activeHikers.Remove(gameObject);

        if (hasDog == true)
        {
            ownedDog.Delete();
        }

        Destroy(gameObject);
    }


}
