using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	private static GameManager instance = null;
	public static GameManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new GameObject("GameManager").AddComponent<GameManager>();
			}

			return instance;
		}
	}

	public bool isStarted, isFinished;
	public PlayerControl playerControl;
	public RopeManager ropeManager;

	private void OnEnable()
	{
		instance = this;
	}

    public void StartGame()
    {
		isStarted = true;
    }

	public GameObject TapToTryAgainButton;
	public void Restart()
    {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
