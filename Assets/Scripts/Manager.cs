using System;
using System.Collections.Generic;
using UnityEditor.Experimental;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public GameObject SunAndMoon;

    public GameObject UFOPrefab;
    private int UFOSpawnTimer = 0;
    public int UFOFrequency;

    public GameObject hikerPrefab;

    private int hikerSpawnTimer = 0;
    public int hikerFrequency;
    public float hikerSpeed;
    public float dogSpeed;
    public float dogDistance;
    public float UFOSpeed;


    public GameObject dogPrefab;

    public List<GameObject> activeHikers = new List<GameObject>();


    public List<GameObject> activeDogs = new List<GameObject>();

    public List<GameObject> activeUFOs = new List<GameObject>();


    public double timeOfDay= 9;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timeOfDay > 19 && timeOfDay < 21)
        {
            this.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, .4f);
        }
        else if (timeOfDay > 21)
        {
            this.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, .7f);
        }
        else if (timeOfDay < 7)
        {
            this.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, .7f);
        }
        else if (timeOfDay < 9)
        {
            this.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, .4f);
        }
        else 
        {
            this.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
        }
    }

    void FixedUpdate()
    {
        timeOfDay += 0.008;
        //timeOfDay += .04;
        if (timeOfDay > 24)
        {
            timeOfDay = 0;
        }
        float Degrees = Convert.ToSingle(timeOfDay/24*360);
        SunAndMoon.GetComponent<Transform>().RotateAround(transform.position, Vector3.up, Degrees);

        hikerSpawnTimer += 1;
        if (hikerSpawnTimer + UnityEngine.Random.Range(0, 50) > hikerFrequency && timeOfDay > 9 && timeOfDay < 21)
        {
            Spawn_Hiker();

            hikerSpawnTimer = 0;
        }

        UFOSpawnTimer += 1;
        if (UFOSpawnTimer + UnityEngine.Random.Range(0, 50) > UFOFrequency)
        {
            if (activeUFOs.Count == 0)
            {
                SpawnUFO();
                
            }
            UFOSpawnTimer = 0;

        }

        

    }



    public void Spawn_Hiker()
    {
        GameObject new_hiker = Instantiate(hikerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        Hiker new_hiker_script = new_hiker.GetComponent<Hiker>();
        new_hiker_script.hiking_speed = hikerSpeed;
        activeHikers.Add(new_hiker);

        if (UnityEngine.Random.Range(0f,1f) > .5)
         {
             //positive direction

            new_hiker_script.transform.position = new Vector3(-7, 0, 0);
            new_hiker_script.hikingDirection = 1;
          }
          else
          {
             //negative direction

            new_hiker_script.transform.position = new Vector3(7, 0, 0);
            new_hiker_script.hikingDirection = -1;
          }

          //dog:

          if (UnityEngine.Random.Range(0f,1f) < 0.33)
          {
            GameObject new_dog = Instantiate(dogPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            NewMonoBehaviourScript new_dog_script = new_dog.GetComponent<NewMonoBehaviourScript>();

            new_hiker_script.hasDog = true;
            new_hiker_script.ownedDog = new_dog_script;
            new_dog_script.owner = new_hiker;
            new_dog_script.owner_script = new_hiker_script;
            new_dog_script.walking_direction = new_hiker_script.hikingDirection;
            new_dog.GetComponent<Transform>().position = new_hiker.GetComponent<Transform>().position;
            new_dog_script.runSpeed = dogSpeed;
            new_dog_script.runDistance = dogDistance;

            activeDogs.Add(new_dog);

          }


    }

    public void SpawnUFO()
    {
        GameObject new_UFO = Instantiate(UFOPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        new_UFO.GetComponent<UFO>().flyingSpeed = UFOSpeed;

        new_UFO.GetComponent<Transform>().position = new Vector3(0, 7, 0);

        activeUFOs.Add(new_UFO);
    }

}
