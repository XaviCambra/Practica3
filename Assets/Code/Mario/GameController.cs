using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    static GameController m_GameController = null;
    MarioPlayerController m_Mario;

    float m_PlayerLife = 100.0f;
    List<IRestartGameElement> m_RestartGameElements = new List<IRestartGameElement>();

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    public static GameController GetGameController()
    {
        if (m_GameController == null)
        {
            m_GameController = new GameObject("GameController").AddComponent<GameController>();
            //GameControllerData l_GameControllerData = Resources.Load<GameControllerData>("GameControllerData");
            //m_GameController.m_PlayerLife = l_GameControllerData.m_Life;
        }

        return m_GameController;
    }
    public static void DestroySingleton()
    {
        if (m_GameController != null)
        {
            GameObject.Destroy(m_GameController.gameObject);
        }
        m_GameController = null;
    }
    public void SetPlayerLife(float PlayerLife)
    {
        m_PlayerLife = PlayerLife;

    }

    public float GetPlayerLife()
    {
        return m_PlayerLife;
    }

    public MarioPlayerController GetPlayer()
    {
        return m_Mario;
    }

    public void SetPlayer(MarioPlayerController Player)
    {
        m_Mario = Player;
    }

    public void AddRestartGameElement(IRestartGameElement RestartGameElement)
    {
        m_RestartGameElements.Add(RestartGameElement);
    }
    public void RestartGame()
    {
        foreach (IRestartGameElement l_RestartGameElement in m_RestartGameElements)
            l_RestartGameElement.RestartGame();
    }
}
