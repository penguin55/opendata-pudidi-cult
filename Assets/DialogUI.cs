﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TomWill;

public class DialogUI : MonoBehaviour
{
    [SerializeField] private GameObject dialogUI, selectBoss, selectItem;
    [SerializeField] [TextArea(0,30)] private string[] chat;
    [SerializeField] [TextArea(0, 30)] private string[] charname;
    [SerializeField] [TextArea(0, 30)] private string[] bossDesc;
    [SerializeField] [TextArea(0, 30)] private string[] itemDesc;
    [SerializeField] private Text dialog, chara, boss, item;
    [SerializeField] private int index = 0, bossIndex,itemIndex;
    [SerializeField] private GameObject x, commander, heli, mysterious;
    // Start is called before the first frame update
    void Start()
    {
        TWLoading.OnSuccessLoad(() => {
            TWTransition.FadeOut();
        });
        bossIndex = 4;
        itemIndex = 5;
    }

    // Update is called once per frame
    void Update()
    {
        Dialog();
        BossDesc();
        ItemDesc();
        ChangeImage();
    }

    public void ChangeImage()
    {
        if (charname[index].Contains("X "))
        {
            x.SetActive(true);
            commander.SetActive(false);
            heli.SetActive(false);
            mysterious.SetActive(false);
        }
        if (charname[index].Contains("Colonel"))
        {
            x.SetActive(false);
            commander.SetActive(true);
            heli.SetActive(false);
            mysterious.SetActive(false);
        }
    }

    public void button1()
    {
        if(index ==12) bossIndex = 0;
        if (index == 15) itemIndex = 0;
    }

    public void button2()
    {
        if (index == 12) bossIndex = 1;
        if (index == 15) itemIndex = 1;
    }
    public void button3()
    {
        if (index == 12) bossIndex = 2;
        if (index == 15) itemIndex = 2;
    }
    public void button4()
    {
        if (index == 12) bossIndex = 3;
        if (index == 15) itemIndex = 3;
    }

    public void button5()
    {
        if (index == 12) bossIndex = 4;
        if (index == 15) itemIndex = 4;
    }

    public void ItemDesc()
    {
        item.text = itemDesc[itemIndex];
    }
    public void BossDesc()
    {
        boss.text = bossDesc[bossIndex];
    }
    public void next()
    {
        index++;
    }

    public void SelectBoss()
    {
        index++;
        selectBoss.SetActive(false);
        dialogUI.SetActive(true);
    }

    public void SelectItem()
    {
        index++;
        selectItem.SetActive(false);
        dialogUI.SetActive(true);
    }

    public void Dialog()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (index < 12)
            {
                index = 12;
            }
            if (index >= 12 && index < 13)
            {
                dialogUI.SetActive(false);
                selectBoss.SetActive(true);
            }
            if(index == 13)
            {
                index = 15;
            }
            else if (index >= 15 && index < 16)
            {
                dialogUI.SetActive(false);
                selectItem.SetActive(true);
            }
        }
        if (index < chat.Length)
        {
            dialog.text = chat[index];
            chara.text = charname[index];
        }

        if (index >= 12 && index < 13)
        {
            dialogUI.SetActive(false);
            selectBoss.SetActive(true);
        }
        if (index >= 15 && index < 16)
        {
            dialogUI.SetActive(false);
            selectItem.SetActive(true);
        }
        if (index >= chat.Length)
        {
            index = 0;
            dialogUI.SetActive(false);
            commander.SetActive(false);
            x.SetActive(false);
            heli.SetActive(false);
            StartCoroutine(End());
        }
    }

    IEnumerator End()
    {
        TimelineManager.instance.Director.Play();
        Debug.Log((float)TimelineManager.instance.Director.duration);
        yield return new WaitForSeconds((float)TimelineManager.instance.Director.duration);
        Debug.Log((float)TimelineManager.instance.Director.duration);
        Debug.Log(TimelineManager.instance.Director.state);
        TWTransition.FadeIn(() => TWLoading.LoadScene("BossTest"));
    }

    public void EndTimeline()
    {
        if (TimelineManager.instance.Director.state != UnityEngine.Playables.PlayState.Playing)
        {
            Debug.Log("Hi");
            TWTransition.FadeIn(() => TWLoading.LoadScene("BossTest"));
        }
    }
}