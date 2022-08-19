using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{
    public float grassAmount = 2f;
    public int price = 10;

    private void Update()
    {
        if (grassAmount <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
