using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUp : MonoBehaviour
{
    //VARIABLES
    public int level;
    private float experience;
    private float experienceRequired;

    public float hp;            


    //METHODS
    void Start()
    {
        level =1;
        hp = 100;
        experience = 0;
        experienceRequired = 100;

    } 

    void Update()
    {
        Exp();

        if (Input.GetKeyDown(KeyCode.E))
        {
            experience += 100;
        }
    }

    void RankUp()
    {
        level +=1;
        experience = 0;

        switch (level)
        {
            case 2:
                hp = 200;
                experienceRequired = 200;
                break;
            case 3:
                hp = 300;
                experienceRequired = 300;
                print("Congratulations! You have Level 3 on your character");
                break;
        }
    }
    void Exp()
    {
        if (experience >= experienceRequired)
            RankUp();
    }
}
