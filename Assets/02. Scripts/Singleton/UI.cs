using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using System;

[System.Serializable]
public struct DialogData
{
    public string name;
    [TextArea(3, 10)] public string script;
}

public delegate void DialogDelegate(object sender);

public class UI : Singleton<UI>
{
    [SerializeField] private Image fadeImage;
    private Coroutine fadeCor;


    private GameObject dialog;
    private TextMeshProUGUI dialogName;
    private TextMeshProUGUI dialogScript;
    private Button nextDialog;
    private Queue<DialogData> dialogQueue = new Queue<DialogData>();

    protected override void Awake()
    {
        base.Awake();
        dialog = transform.Find("Dialog").gameObject;
        dialogName = dialog.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        dialogScript = dialog.transform.Find("Script").GetComponent<TextMeshProUGUI>();
        nextDialog = dialog.transform.Find("Next").GetComponent<Button>();
        dialog.SetActive(false);
    }

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

    public void SetDialog(List<DialogData> datas, Action exit, bool startImmediately = true)
    {
        nextDialog.onClick.RemoveAllListeners();
        nextDialog.onClick.AddListener(() =>
        {
            NextDialog(exit);
        });
        foreach (var data in datas)
        {
            dialogQueue.Enqueue(data);
        }
        if (startImmediately)
            NextDialog(exit);
    }

    private void NextDialog(Action action)
    {
        if(dialogQueue.TryDequeue(out DialogData data))
        {
            StartCoroutine(PrintDialogScript(data, 0.1f));
        }
        else
        {
            action?.Invoke();
            dialog.SetActive(false);
        }
    }

    private IEnumerator PrintDialogScript(DialogData data, float textTime)
    {
        dialog.SetActive(true);
        dialogName.text = data.name;
        dialogScript.text = "";
        WaitForSecondsRealtime waitTime = new WaitForSecondsRealtime(textTime);
        for (int i = 0; i < data.script.Length; i++)
        {
            dialogScript.text += data.script[i];
            yield return waitTime;
        }
    }
}
