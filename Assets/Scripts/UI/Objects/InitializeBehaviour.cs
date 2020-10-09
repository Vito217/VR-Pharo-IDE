﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SaveAndLoad;
using PharoModule;
using LoggingModule;
using TMPro;

public class InitializeBehaviour : MonoBehaviour
{
    public TextMeshProUGUI code;
    public TMP_InputField logText;
    public TMP_InputField field;
    public TMP_InputField keyboardTarget;
    public VRIDEController player;
    public GameObject loadingWheel;
    public List<GameObject> Keyboards;
    public Image panel;
    public int lastCaretPosition = 0;
    public int lastAnchorPosition = 0;

    Vector3 new_pos;
    Vector3 rel_pos;
    Vector3 rel_fwd;
    float dist = 5f;
    float speed = 8f;
    int keyboardsIndex = 0;

    StringBuilder sb = new StringBuilder();
    List<char> notAN = new List<char> { ' ', '\n', '\t', '\r' };

    void Start()
    {
        StartCoroutine(Coroutine());
    }

    public IEnumerator Coroutine()
    {
        if(panel != null) panel.color = UnityEngine.Random.ColorHSV();
        yield return innerStart();
    }

    void Update()
    {
        transform.forward = new Vector3(
            transform.forward.x, 0f, transform.forward.z);
        innerBehaviour();
    }

    public async void onChangeInput()
    {
        try
        {
            /**
            string text = field.text;
            text = Regex.Replace(text, @"<color=#b32d00>|<color=#00ffffff>|</color>|<b>|</b>", "");
            text = Regex.Replace(text, @"\t", "".PadRight(4));
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                int i;
                for (i = field.caretPosition - 1; i >= 0 && !notAN.Contains(text[i]); i--)
                    sb.Insert(0, text[i].ToString(), 1);
                string previous_word = sb.ToString();
                int pw_len = previous_word.Length;
                for (i = i; i >= 0 && notAN.Contains(text[i]); i--) { }
                if ((i == -1) || (i >= 0 && text[i] == '.') || (pw_len > 0 && previous_word[0] == '#'))
                    field.caretPosition += 1;
                sb.Clear();
            }
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                field.caretPosition += 4;
                if (text[field.caretPosition - 1] != ' ')
                    field.caretPosition -= 1;
            }
            text = Regex.Replace(text, @"(\A|\.\s*\n*\s*)([a-zA-Z0-9]+)(\s|\n)", "$1<b>$2</b>$3");
            text = Regex.Replace(text, @"(\n?\s*)(#[a-zA-Z0-9]+)(\n?\s*)", "$1<color=#00ffffff>$2</color>$3");
            field.text = text;
            **/
        }
        catch
        {
            //await SaveAndLoadModule.Save(player);
            //await Pharo.Execute("SmalltalkImage current snapshot: true andQuit: true.");
            //InteractionLogger.SessionEnd();
            //Application.Quit();
        }
    }

    public string cleanCode(string code)
    {
        return Regex.Replace(code, @"<color=#b32d00>|<color=#00ffffff>|</color>|<b>|</b>", "");
    }

    public string getSelectedCode(string clean_code, bool includesEmpty)
    {
        int start = field.selectionAnchorPosition;
        int end = field.caretPosition;
        if (end < start) start = Interlocked.Exchange(ref end, start);
        string selection = clean_code.Substring(start, end - start);
        return !includesEmpty && selection == "" ? clean_code : selection;
    }

    public string getLastLineOfCode(string clean_code)
    {
        string[] lines = clean_code.Split('.');
        int len = lines.Length;
        if (len == 1)
            return lines[0];
        else
        {
            string last = lines[len - 1];
            string penultimate = lines[len - 2];
            return String.IsNullOrWhiteSpace(last) || String.IsNullOrEmpty(last) ?
                penultimate : last;
        }
    }

    public void OnDrag(BaseEventData data)
    {
        Camera theCamera = ((PointerEventData)data).enterEventCamera;
        VRIDEController thePlayer = theCamera.transform.root.gameObject.GetComponent<VRIDEController>();
        transform.SetParent(thePlayer.dragPivot);
        InteractionLogger.StartTimerFor("WindowDragging");
    }

    public void OnEndDrag(BaseEventData data)
    {
        new_pos = transform.parent.TransformPoint(transform.localPosition);
        transform.SetParent(null);
        transform.position = new_pos;
        InteractionLogger.EndTimerFor("WindowDragging");
    }

    public void DeactivateTemporarily()
    {
        loadingWheel.SetActive(true);
    }

    public void Reactivate()
    {
        loadingWheel.SetActive(false);
    }

    public void ChangeKeyboard()
    {
        Keyboards[keyboardsIndex].SetActive(false);
        keyboardsIndex = (keyboardsIndex + 1) % Keyboards.Count;
        Keyboards[keyboardsIndex].SetActive(true);
    }

    public void KeepActiveOnSlide()
    {
        keyboardTarget.ActivateInputField();
        keyboardTarget.caretPosition = lastCaretPosition;
    }

    public virtual void Initialize(Vector3 final_pos, Vector3 forward)
    {
        transform.position = final_pos;
        transform.forward = forward;
    }

    public virtual void OnSelect(BaseEventData data)
    {
        try
        {
            GetComponent<Canvas>().worldCamera =
                ((PointerEventData)data).enterEventCamera;
        }
        catch { }
        player = data.currentInputModule.transform.parent
            .gameObject.GetComponent<VRIDEController>();
        player.can_move = false;
    }

    public virtual void OnDeselect(BaseEventData data)
    {
        player = data.currentInputModule.transform.parent
            .gameObject.GetComponent<VRIDEController>();
        player.can_move = true;
    }

    public virtual void onClose() { }

    public virtual void innerBehaviour() { }

    public virtual IEnumerator innerStart()
    {
        yield return null;
    }
}