using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    static GameController m_GameController = null;
    MarioPlayerController m_Mario;
    List<IRestartGameElement> m_RestartGameElements = new List<IRestartGameElement>();

    private void Start()
    {
        
    }
    public static GameController GetGameController()
    {

    }
    //copiar el destroy on singleton de la practica 1, lo de arriba tambien y lo del get player

    public void AddRestartGameElement(IRestartGameElement RestartGameElement)
    {
        m_RestartGameElements.Add(RestartGameElement);
    }
    public void RestartGame()
    {
        foreach (IRestartGameElement l_RestartGameElement in m_RestartGameElements)
            l_RestartGameElement.RestartGame();
    }

    private void Update()
    {
        
    }
}
