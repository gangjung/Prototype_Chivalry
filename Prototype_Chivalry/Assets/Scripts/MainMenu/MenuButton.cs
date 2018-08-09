using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MenuButton : MonoBehaviour, IPointerClickHandler{
    


    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.pointerPress.tag == "Team")
        {
            SceneManager.UnloadSceneAsync("MainMenu");
            SceneManager.LoadScene("TeamMenu", LoadSceneMode.Additive);
        }

        

        throw new System.NotImplementedException();
    }
}
