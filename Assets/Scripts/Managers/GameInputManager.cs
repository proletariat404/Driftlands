// GameInputManager.cs
using UnityEngine;

public class GameInputManager : MonoBehaviour
{
	public static GameInputManager Instance { get; private set; }

	private bool inputEnabled = true;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public bool IsInputEnabled()
	{
		return inputEnabled;
	}

	public void EnableInput()
	{
		inputEnabled = true;
		Debug.Log("输入已启用");
	}

	public void DisableInput()
	{
		inputEnabled = false;
		Debug.Log("输入已禁用");
	}
}
