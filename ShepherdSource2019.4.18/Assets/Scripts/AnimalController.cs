using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalController : MonoBehaviour
{
    public enum AnimalType { Sheep, Dog, Wolf};

    public AnimalType myAnimal;
    public GameObject GFX;
    private Animator animator;

    public float animalHealth;
    public float damageSpeed = 0.5f;
    public int price = 10;

    public float minSpeed, maxSpeed;
    public float minX, maxX, minY, maxY;
    public float reachDistance = .5f;
    public float radius = 3f;

    [Header("Sheep")]
    public float grassAmount = 4f;
    public float coinRate = 5f;

    [Header("Effects")]
    public ParticleSystem coinEffect;
    public GameObject dieEffect;


    private Vector2 nextPoint;
    private float movementSpeed;

    private GameObject animalTarget;

    private void Start()
    {
        animalTarget = null;
        animator = GetComponent<Animator>();
        UpdateNextPoint();

        if (myAnimal == AnimalType.Sheep)
        {
            GameManager.instance.sheepCount++;
            GameManager.instance.UpdateText();
            InvokeRepeating("AddCoin", coinRate, coinRate);
        }
    }

    private void Update()
    {

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);

        switch (myAnimal)
        {
            case AnimalType.Sheep:
                foreach (Collider2D item in colliders)
                {
                    if (item.CompareTag("Grass"))
                    {
                        if (animalTarget == null)
                        {
                            animalTarget = item.gameObject;
                        }
                        nextPoint = animalTarget.transform.position;
                    }
                }
                break;

            case AnimalType.Dog:
                foreach (Collider2D item in colliders)
                {
                    if (item.CompareTag("Wolf"))
                    {
                        if (animalTarget == null)
                        {
                            animalTarget = item.gameObject;
                        }
                        nextPoint = animalTarget.transform.position;
                    }
                }

                break;
            case AnimalType.Wolf:
                foreach (Collider2D item in colliders)
                {
                    if (item.CompareTag("Sheep"))
                    {
                        if (animalTarget == null)
                        {
                            animalTarget = item.gameObject;
                        }
                        nextPoint = animalTarget.transform.position;
                    }
                }
                break;
        }

        //Update animation
        animator.SetFloat("speed", Vector2.Distance(transform.position, nextPoint));

        if (transform.position.x - nextPoint.x > reachDistance)
            GFX.transform.localScale = new Vector3(-1f, 1f, 1f);
        else if(transform.position.x - nextPoint.x < reachDistance)
            GFX.transform.localScale = new Vector3(1f, 1f, 1f);



        if (Vector2.Distance(transform.position, nextPoint) < reachDistance)
            UpdateNextPoint();
        else
            transform.position = Vector2.MoveTowards(transform.position, nextPoint, movementSpeed * Time.deltaTime);




        if (myAnimal == AnimalType.Sheep)
        {
            if (grassAmount < 0f)
            {
                animalHealth -= damageSpeed * Time.deltaTime * 0.1f;
                transform.localScale = Vector3.one * animalHealth;
                return;
            }


            grassAmount -= damageSpeed * Time.deltaTime * 0.1f;

            if (animalHealth < 1f)
            {
                animalHealth += damageSpeed * Time.deltaTime * 0.1f;
                transform.localScale = Vector3.one * animalHealth;
            }
        }


    }

    private void UpdateNextPoint()
    {
        nextPoint = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
        movementSpeed = Random.Range(minSpeed, maxSpeed);
    }


    private void OnTriggerStay2D(Collider2D other)
    {


        if (other.CompareTag("Grass") && myAnimal == AnimalType.Sheep)
        {
            other.GetComponent<Grass>().grassAmount -= damageSpeed * Time.deltaTime;
            grassAmount += damageSpeed * Time.deltaTime;

        }else if (other.CompareTag("Wolf") && myAnimal == AnimalType.Dog)
        {
            other.GetComponent<AnimalController>().animalHealth -= damageSpeed * Time.deltaTime;

        }else if (other.CompareTag("Sheep") && myAnimal == AnimalType.Wolf)
        {
            other.GetComponent<AnimalController>().animalHealth -= damageSpeed * Time.deltaTime;
        }



        if (animalHealth <= 0.3f)
            Die();

        transform.localScale = Vector3.one * animalHealth;
    }

    

    private void Die()
    {
        if (myAnimal == AnimalType.Sheep)
            GameManager.instance.sheepCount--;
       
        GameObject dieGO = Instantiate(dieEffect, transform.position, Quaternion.identity);
        Destroy(dieGO, 2f);
        Destroy(gameObject);
    }

    private void AddCoin()
    {
        coinEffect.Play();
        GameManager.instance.coinCount += 1;
        GameManager.instance.UpdateText();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, radius);
    }



}
