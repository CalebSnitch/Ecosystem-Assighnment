using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    
    public SpriteRenderer spriteRenderer;

    public Sprite walking_sprite;
    public Sprite sitting_sprite;
    public Sprite sleeping_sprite;
    
    public int walking_direction;

    public GameObject owner;
    public Hiker owner_script;
    public Transform owner_transform;
    public Transform this_transform;
    public GameObject Manager;
    public Manager Manager_Script;
    public enum DOG_STATES
    {
        RUNNING_AHEAD,
        RUNNING_BACK,
        SITTING,
        SLEEPING

    }

    public float runDistance;
    public float runSpeed;
    private float target_x;
    private Vector3 target_location;
    private float distance_to_owner;
    private DOG_STATES last_state = DOG_STATES.RUNNING_AHEAD;

    public DOG_STATES State = DOG_STATES.RUNNING_AHEAD;

    void Start()
    {
        owner_transform = owner.GetComponent<Transform>();
        target_x = owner_transform.position.x + runDistance * walking_direction;
        target_location = new Vector3(target_x, 0, 0);

        this_transform = gameObject.GetComponent<Transform>();

        Manager = GameObject.Find("Manager");
        Manager_Script = Manager.GetComponent<Manager>();
    }

    // Update is called once per frame
    void Update()
    {
        distance_to_owner = Mathf.Abs(gameObject.GetComponent<Transform>().position.x - owner_transform.position.x);
        
        switch (State)
        {
            case DOG_STATES.RUNNING_AHEAD:
                gameObject.GetComponent<Transform>().position += new Vector3 (runSpeed, 0, 0) * walking_direction * Time.deltaTime;
                last_state = State;

                spriteRenderer.sprite = walking_sprite;
                if (walking_direction == 1)
                {
                    spriteRenderer.flipX = false;
                }
                else
                {
                    spriteRenderer.flipX = true;
                }

                //transitions:
                if (walking_direction == 1 && gameObject.GetComponent<Transform>().position.x > target_x)
                {
                    target_x = (owner_transform.position.x + gameObject.GetComponent<Transform>().position.x)/2 + UnityEngine.Random.Range(-1f, 1f);
                    target_location = new Vector3(target_x, 0, 0);
                    State = DOG_STATES.RUNNING_BACK;

                    //Debug.Log("happened");
                }

                if (walking_direction == -1 && gameObject.GetComponent<Transform>().position.x < target_x)
                {
                    target_x = (owner_transform.position.x + gameObject.GetComponent<Transform>().position.x)/2 + UnityEngine.Random.Range(-1f, 1f);
                    target_location = new Vector3(target_x, 0, 0);
                    State = DOG_STATES.RUNNING_BACK;
                }

                if (Manager_Script.timeOfDay > 21)
                {
                    State = DOG_STATES.SLEEPING;
                }

                break;

            case DOG_STATES.RUNNING_BACK:
                this_transform.position += new Vector3 (runSpeed, 0, 0) * -walking_direction * Time.deltaTime;
                last_state = State;
                
                spriteRenderer.sprite = walking_sprite;
                if (walking_direction == 1)
                {
                    spriteRenderer.flipX = true;
                }
                else
                {
                    spriteRenderer.flipX = false;
                }

                //transitions:
                if (walking_direction == 1 && gameObject.GetComponent<Transform>().position.x < target_x)
                {
                    State = DOG_STATES.SITTING;
                }

                if (walking_direction == -1 && gameObject.GetComponent<Transform>().position.x > target_x)
                {
                    State = DOG_STATES.SITTING;
                }

                if (Manager_Script.timeOfDay > 21)
                {
                    State = DOG_STATES.SLEEPING;
                }
                break;

                case DOG_STATES.SITTING:
                    last_state = State;
                    
                    spriteRenderer.sprite = sitting_sprite;

                    //transitions:
                    if (distance_to_owner < .25)
                    {
                        target_x = owner_transform.position.x + runDistance * walking_direction + UnityEngine.Random.Range(-1f, 1f);
                        target_location = new Vector3(target_x, 0, 0);
                        State = DOG_STATES.RUNNING_AHEAD;
                    }

                    if (Manager_Script.timeOfDay > 21)
                    {
                        State = DOG_STATES.SLEEPING;
                    }

                    break;

                case DOG_STATES.SLEEPING:
                    
                    spriteRenderer.sprite = sleeping_sprite;

                    //transitions:

                if (Manager_Script.timeOfDay > 9 && Manager_Script.timeOfDay < 21)
                {
                    State = last_state;
                }

                    break;
        }
    }

    public void Delete()
    {
        Manager_Script.activeDogs.Remove(gameObject);
        Destroy(gameObject);
    }
}
