using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPanning : MonoBehaviour
{

    public int CurrentCity;
    public int NumCities;
    public List<GameObject> CityLocations;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnRightButtonPress()
    {
        CurrentCity = (CurrentCity + 1) % NumCities;
        LerpCamera();
    }

    public void OnLeftButtonPress()
    {
        CurrentCity = (CurrentCity + NumCities - 1) % NumCities;
        LerpCamera();
    }

    void LerpCamera()
    {
        GameObject currentCityPos = CityLocations[CurrentCity];
        StartCoroutine(LerpFromTo(transform.position, currentCityPos.transform.position, 1.0f));
    }

    //https://answers.unity.com/questions/1495016/lerp-camera-position-smoothly-from-pos-a-to-pos-b.html
    IEnumerator LerpFromTo(Vector3 pos1, Vector3 pos2, float duration)
    {
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(pos1, pos2, t / duration);
            yield return 0;
        }
        transform.position = pos2;
    }
}
