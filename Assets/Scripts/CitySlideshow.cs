using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitySlideshow : MonoBehaviour
{
    public List<GameObject> cities;
    private Queue<GameObject> qcities;

    public List<GameObject> lands;
    private Queue<GameObject> qlands;

    public float duration = 5f;
    public bool cityBool = true;

    // Fades the sprite out
    private Color fadeTargetColor;
    SpriteRenderer spriteToFade;

    private Color enterTargetColor;
    SpriteRenderer spritetoEnter;

    void Start()
    {
        //create city queue
        qcities = new Queue<GameObject>();
        foreach (GameObject go in cities)
        {
            qcities.Enqueue(go);
        }

        //create lands queue
        qlands = new Queue<GameObject>();
        foreach (GameObject go in lands)
        {
            qlands.Enqueue(go);
        }

        fadeTargetColor = new Color(1, 1, 1, 0);
        enterTargetColor = new Color(1, 1, 1, 1);
    }

    public void flipCityBool()
    {
        cityBool = !cityBool;
    }

    public void NextUp()
    {
        if (cityBool)
        {
            GameObject stadt1 = qcities.Dequeue();
            GameObject stadt2 = qcities.Dequeue();
            StartCoroutine(LerpFunction(fadeTargetColor));
            spriteToFade = stadt1.GetComponent<SpriteRenderer>();
            qcities.Enqueue(stadt1);
            StartCoroutine(LerpFunction(enterTargetColor));
            spritetoEnter = stadt2.GetComponent<SpriteRenderer>();
            qcities.Enqueue(stadt2);
        } else
        {
            GameObject stadt1 = qlands.Dequeue();
            GameObject stadt2 = qlands.Dequeue();
            StartCoroutine(LerpFunction(fadeTargetColor));
            spriteToFade = stadt1.GetComponent<SpriteRenderer>();
            qlands.Enqueue(stadt1);
            StartCoroutine(LerpFunction(enterTargetColor));
            spritetoEnter = stadt2.GetComponent<SpriteRenderer>();
            qlands.Enqueue(stadt2);
        }
    }

    IEnumerator LerpFunction(Color endValue)
    {
        float time = 0;
        Color startValue = spriteToFade.color;

        while (time < duration)
        {
            spriteToFade.color = Color.Lerp(startValue, endValue, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        spriteToFade.color = endValue;
        WaitFiveSeconds();
        NextUp();
    }

    IEnumerator WaitFiveSeconds()
    {
        yield return new WaitForSeconds(5f);
    }
}
