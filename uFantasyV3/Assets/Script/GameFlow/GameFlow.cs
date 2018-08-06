using System.Collections.Generic;
using System.Collections;
using UnityEngine;


public partial class GameFlow : MonoBehaviour
{

	public Animator Animator;
	public GameObject LoadingMask;
	public static Animator Controller;
	internal string CurrentLoadingScene;
	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
	}
	private void Start()
	{
		Animator.enabled = true;
		Controller = Animator;
	}

	public void Loading(string sceneName)
	{
		CurrentLoadingScene = sceneName;
		Animator.Play("Empty");
	}





}
