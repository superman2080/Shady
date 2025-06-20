using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;




public delegate void DialogDelegate(object sender);

public class UI : Singleton<UI>
{
    [SerializeField] private Image fadeImage;
    private Coroutine fadeCor;
    public DialogSystem dialogSystem;



    public void Fade(bool fadeOut, Color color, float fadeTime, float maxFadeAlpha)
    {
        if (fadeCor != null)
        {
            StopCoroutine(fadeCor);
            fadeCor = null;
        }
        fadeCor = StartCoroutine(FadeCor(fadeOut, color, fadeTime, maxFadeAlpha));
    }

    private IEnumerator FadeCor(bool fadeIn, Color color, float fadeTime, float maxFadeAlpha)
    {
        float a = fadeIn ? maxFadeAlpha : 0;
        Color c = new Color(color.r, color.g, color.b, a);
        fadeImage.color = c;

        for (float eT = 0; eT <= fadeTime; eT += Time.deltaTime)
        {
            fadeImage.color = c;
            c.a = Mathf.Lerp(fadeIn ? maxFadeAlpha : 0, fadeIn ? 0 : maxFadeAlpha,  eT / fadeTime);
            yield return null;
        }
        a = fadeIn ? 0 : maxFadeAlpha;
        c.a = a;
        fadeImage.color = c;
        fadeCor = null;
    }


}
