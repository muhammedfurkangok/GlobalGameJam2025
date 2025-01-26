using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public CanvasGroup ImageCanvasGroup;
    public Animator transitionBubbleAnimator;

    public void Start()
    {
        SceneStart();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneRestart();
        }
        
        if( Input.GetKeyDown(KeyCode.N))
        {
            SceneEnd();
        }
    }

    public void SceneStart()
    {
        ImageCanvasGroup.DOFade(0, 1f).OnComplete(() =>
        {
            ImageCanvasGroup.blocksRaycasts = false;
            ImageCanvasGroup.interactable = false;
        });
        SoundManager.Instance.PlayOneShotSound(SoundType.BubbleTransition);
        transitionBubbleAnimator.SetTrigger("Start");
    }

    public void SceneEnd()
    {
        ImageCanvasGroup.DOFade(1, 2f).OnComplete(() =>
        {
            ImageCanvasGroup.blocksRaycasts = true;
            ImageCanvasGroup.interactable = true;
            LoadNextScene();
        });
        SoundManager.Instance.PlayOneShotSound(SoundType.BubbleTransition);
        transitionBubbleAnimator.SetTrigger("End");
    }

    public void SceneRestart()
    {
        ImageCanvasGroup.DOFade(1, 2f).OnComplete(() =>
        {
            ImageCanvasGroup.blocksRaycasts = true;
            ImageCanvasGroup.interactable = true;
            RestartCurrentScene();
        });
        SoundManager.Instance.PlayOneShotSound(SoundType.BubbleTransition);
        transitionBubbleAnimator.SetTrigger("End");
    }

    private void RestartCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}