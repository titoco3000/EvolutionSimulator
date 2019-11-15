using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneSwapper : MonoBehaviour
{
    Camera mainCamera;



    public void WarpTo(int i)
    {
        SceneManager.LoadScene(i);

        //StartCoroutine(WarpCamera(i));
    }

    IEnumerator WarpCamera(int buildIndex)
    {
        mainCamera = Camera.main;
        float speed = 100;
        while(mainCamera.fieldOfView < 170)
        {
            mainCamera.fieldOfView += speed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        SceneManager.LoadScene(buildIndex);
    }

    private void OnLevelWasLoaded(int level)
    {
        //StartCoroutine(DecreaseFOV());
    }

    IEnumerator DecreaseFOV()
    {
        mainCamera = Camera.main;

        float speed = 100;

        float originalFOV = mainCamera.fieldOfView;
        
        mainCamera.fieldOfView = 170;
        while (mainCamera.fieldOfView > originalFOV)
        {
            mainCamera.fieldOfView -= speed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

    }
}
