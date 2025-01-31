using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SingleStory : MonoBehaviour {
    [SerializeField] Image Background;
    [SerializeField] GameObject BasicIcon;
    [SerializeField] GameObject ARROWS;
    [SerializeField] GameObject BG;
    [SerializeField] GameObject CUBE;
    [SerializeField] GameObject FOOTPRINT;
    [SerializeField] GameObject PURPLEFLAG;
    [SerializeField] GameObject GRAYFLAG;
    [SerializeField] GameObject POSITION;
    [SerializeField] GameObject GoBackButton;

    public static int status;
    public static string clickedButtonName;
    private bool isShowText = true;
    private bool isClick = false;
    private bool isBigCubeClick = false;
    private int currentPos = 0;
    private string[] ColorPriority = { "Orange", "Green", "Blue", "Red" };
    private ApplicationHandler applicationHandler;

    void Awake()
    {
        applicationHandler = GameObject.Find("ApplicationHandler").GetComponent<ApplicationHandler>();
    }

    void Start()
    {
        status = 0;     //Set!!
        StartCoroutine(ShowIntro());
    }

    void Update()
    {
        StartCoroutine(UpdateIcon());
        StartCoroutine(UpdateButton());
        if (Mouse.current.leftButton.isPressed)
            if (isShowText)
                isClick = true;

        if (status == 2) {
            StartCoroutine(Clicked_SmallCube());
            status = 0;
            isShowText = true;
        }
        if (status == 3) {
            StartCoroutine(Clicked_Footprint());
            status = 0;
            isShowText = true;
        }
        if (status == 4) {
            StartCoroutine(Clicked_Skeleton());
            status = 0;
            isShowText = true;
        }
        if (status == 5) {
            StartCoroutine(ShowFinal());
            status = 0;
            isShowText = true;
        }
        if (status == 6) {
            if (0 <= currentPos && currentPos < 4 && applicationHandler.GameData.IsIntro_Single[currentPos] == true) {    //apply it while first click big cube
                StartCoroutine(ShowChapterContent());
                status = 0;
                isShowText = true;
                applicationHandler.GameData.IsIntro_Single[currentPos] = false;
                applicationHandler.GameData.SaveData();
            }
            else {
                StartCoroutine(Clicked_BigCube());
                status = 1;
            }
        }
    }

    public void CompleteSkeleton()          //call this while complete skeleton
    {
        status = 5;
    }

    private IEnumerator UpdateIcon()
    {
        if (status == 1 && 0 <= applicationHandler.GameData.Schedule_Single && applicationHandler.GameData.Schedule_Single < 1 << 17) {
            for (int i = 0;i < 4;i += 1)
                CUBE.transform.Find("cube" + ColorPriority[i]).gameObject.SetActive(true);
            CUBE.transform.Find("skeleton").gameObject.SetActive(true);

            int tempNum = 14;
            for (int i = 0;i < 4;i += 1) {
                if ((applicationHandler.GameData.Schedule_Single & tempNum) == tempNum) {
                    GRAYFLAG.transform.Find("grayFlag" + i.ToString()).gameObject.SetActive(true);
                    for (int j = 1;j <= 3;j += 1)
                        PURPLEFLAG.transform.Find("purpleFlag" + (i * 4 + j).ToString()).gameObject.SetActive(false);
                    currentPos = i * 4 + 4;
                    if ((applicationHandler.GameData.Schedule_Single & 1 << (i * 4 + 4)) > 0)
                        currentPos = i + 1;
                }
                else if ((applicationHandler.GameData.Schedule_Single & tempNum) > 0) {
                    for (int j = 1;j <= 3;j += 1)
                        if ((applicationHandler.GameData.Schedule_Single & 1 << (i * 4 + j)) > 0)
                            PURPLEFLAG.transform.Find("purpleFlag" + (i * 4 + j).ToString()).gameObject.SetActive(true);
                    currentPos = i;
                }
                tempNum <<= 4;
            }

            for (int i = 0;i <= 2;i += 1) {
                if ((applicationHandler.GameData.Schedule_Single & 1 << (i * 4 + 4)) > 0) {
                    FOOTPRINT.transform.Find("footprint" + (i * 4 + 4).ToString()).gameObject.SetActive(false);
                    FOOTPRINT.transform.Find("footcomplete" + (i * 4 + 4).ToString()).gameObject.SetActive(true);
                }
                else {
                    FOOTPRINT.transform.Find("footprint" + (i * 4 + 4).ToString()).gameObject.SetActive(true);
                    FOOTPRINT.transform.Find("footcomplete" + (i * 4 + 4).ToString()).gameObject.SetActive(false);
                }
            }

            for (int i = 0;i < POSITION.transform.childCount;i += 1)
                POSITION.transform.GetChild(i).gameObject.SetActive(false);
            POSITION.transform.Find("position" + currentPos.ToString()).gameObject.SetActive(true);

            BasicIcon.SetActive(true);
            for (int i = 0;i < ARROWS.transform.childCount;i += 1)
                ARROWS.transform.GetChild(i).gameObject.SetActive(true);
        }
        else {
            for (int i = 0;i < CUBE.transform.childCount;i += 1)
                CUBE.transform.GetChild(i).gameObject.SetActive(false);
            for (int i = 0;i < GRAYFLAG.transform.childCount;i += 1)
                GRAYFLAG.transform.GetChild(i).gameObject.SetActive(false);
            for (int i = 0;i < PURPLEFLAG.transform.childCount;i += 1)
                PURPLEFLAG.transform.GetChild(i).gameObject.SetActive(false);
            for (int i = 0;i < FOOTPRINT.transform.childCount;i += 1)
                FOOTPRINT.transform.GetChild(i).gameObject.SetActive(false);
            for (int i = 0;i < POSITION.transform.childCount;i += 1)
                POSITION.transform.GetChild(i).gameObject.SetActive(false);
            BasicIcon.SetActive(false);
            for (int i = 0;i < ARROWS.transform.childCount;i += 1)
                ARROWS.transform.GetChild(i).gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(1);
    }

    private IEnumerator UpdateButton()
    {
        if (status == 1) {
            for (int i = 0;i < 4;i += 1) {
                if (isBigCubeClick) {
                    for (int j = 1;j <= 3;j += 1) {
                        if ((applicationHandler.GameData.Schedule_Single & 1 << (i * 4 + j)) == 0 && currentPos == i)
                            CUBE.transform.Find("cube" + ColorPriority[i]).Find("cube" + (i * 4 + j).ToString()).GetComponent<Button>().enabled = true;
                        else
                            CUBE.transform.Find("cube" + ColorPriority[i]).Find("cube" + (i * 4 + j).ToString()).GetComponent<Button>().enabled = false;
                    }
                }
                else if (!isBigCubeClick || currentPos >= 4) {
                    GameObject tempObject = GameObject.Find("cube" + ColorPriority[i]);
                    tempObject.transform.GetChild(0).gameObject.SetActive(true);
                    if (currentPos < 4) {
                        if (currentPos == i)
                            tempObject.transform.GetChild(0).GetComponent<Button>().enabled = true;
                        else
                            tempObject.transform.GetChild(0).GetComponent<Button>().enabled = false;
                    }
                    for (int j = 2;j < 5;j += 1)
                        tempObject.transform.GetChild(j).gameObject.SetActive(false);
                }
                if ((i * 4 + 4) < 16) {
                    if ((applicationHandler.GameData.Schedule_Single & 1 << (i * 4 + 4)) == 0 && currentPos == (i * 4 + 4)) {
                        FOOTPRINT.transform.Find("footprint" + (i * 4 + 4).ToString()).GetComponent<Button>().enabled = true;
                        isBigCubeClick = false;
                    }
                    else {
                        FOOTPRINT.transform.Find("footprint" + (i * 4 + 4).ToString()).GetComponent<Button>().enabled = false;
                    }
                }
                else if ((i * 4 + 4) == 16) {
                    if ((applicationHandler.GameData.Schedule_Single & 1 << 16) == 0 && currentPos == 16) {
                        GameObject.Find("skeleton").GetComponent<Button>().enabled = true;
                        isBigCubeClick = false;
                    }
                    else {
                        GameObject.Find("skeleton").GetComponent<Button>().enabled = false;
                    }
                }
            }
        }
        yield return new WaitForSeconds(1);
    }

    private IEnumerator ShowIntro()
    {
        //GameObject.Find("mainBG").transform.GetComponent<Image>().color = new Color32(255, 255, 255, 80);
        Background.color = new Color32(255, 255, 255, 80);
        GameObject BGobject = BG.transform.Find("bgOrange").gameObject;
        BGobject.SetActive(true);
        BGobject.transform.GetChild(0).gameObject.SetActive(true);
        BGobject.transform.GetChild(1).gameObject.SetActive(false);
        BGobject.transform.GetChild(2).gameObject.SetActive(false);
        if (applicationHandler.GameData.FirstIntro_Single) {
            BGobject.transform.GetChild(0).gameObject.GetComponent<Text>().text = Resources.Load<TextAsset>("plot/intro").ToString();
            while (!isClick)
                yield return null;
            isClick = false;
            applicationHandler.GameData.FirstIntro_Single = false;
            applicationHandler.GameData.SaveData();
        }
        Background.color = new Color32(255, 255, 255, 110);
        BGobject.SetActive(false);

        status = 1;
        isShowText = false;
    }

    private IEnumerator ShowChapterContent()
    {
        Background.color = new Color32(255, 255, 255, 80);
        GameObject BGobject = BG.transform.Find("bg" + ColorPriority[currentPos]).gameObject;
        BGobject.SetActive(true);
        BGobject.transform.GetChild(0).gameObject.SetActive(true);
        BGobject.transform.GetChild(1).gameObject.SetActive(true);
        BGobject.transform.GetChild(2).gameObject.SetActive(false);
        BGobject.transform.GetChild(0).gameObject.GetComponent<Text>().text = Resources.Load<TextAsset>("plot/0" + currentPos.ToString() + "-11").ToString();
        BGobject.transform.GetChild(1).gameObject.GetComponent<Text>().text = Resources.Load<TextAsset>("plot/0" + currentPos.ToString() + "-2").ToString();

        while (!isClick)
            yield return null;
        isClick = false;

        BGobject.transform.GetChild(0).gameObject.GetComponent<Text>().text = Resources.Load<TextAsset>("plot/0" + currentPos.ToString() + "-12").ToString();

        while (!isClick)
            yield return null;
        isClick = false;

        BGobject.transform.GetChild(0).gameObject.GetComponent<Text>().text = Resources.Load<TextAsset>("plot/0" + currentPos.ToString() + "-13").ToString();

        while (!isClick)
            yield return null;
        isClick = false;

        Background.color = new Color32(255, 255, 255, 110);
        BGobject.SetActive(false);

        status = 6;
        isShowText = false;
    }

    private IEnumerator ShowFinal()
    {
        Background.color = new Color32(255, 255, 255, 80);
        GameObject BGobject = BG.transform.Find("bgFinal").gameObject;
        BGobject.SetActive(true);
        BGobject.transform.GetChild(0).gameObject.SetActive(true);
        BGobject.transform.GetChild(0).gameObject.GetComponent<Text>().text = Resources.Load<TextAsset>("plot/final-1").ToString();

        while (!isClick)
            yield return null;
        isClick = false;

        BGobject.transform.GetChild(0).gameObject.GetComponent<Text>().text = Resources.Load<TextAsset>("plot/final-2").ToString();

        while (!isClick)
            yield return null;
        isClick = false;

        Background.color = new Color32(255, 255, 255, 110);
        BGobject.SetActive(false);

        status = 1;
        isShowText = false;
    }

    private IEnumerator Clicked_SmallCube()
    {
        Background.color = new Color32(255, 255, 255, 80);
        GoBackButton.SetActive(true);
        GameObject BGobject = BG.transform.Find("bg" + ColorPriority[Convert.ToInt32(clickedButtonName) / 4]).gameObject;
        BGobject.SetActive(true);
        BGobject.transform.GetChild(0).gameObject.SetActive(true);
        BGobject.transform.GetChild(1).gameObject.SetActive(true);
        BGobject.transform.GetChild(2).gameObject.SetActive(false);
        BGobject.transform.GetChild(0).gameObject.GetComponent<Text>().text = Resources.Load<TextAsset>("plot/" + clickedButtonName + "-1").ToString();
        BGobject.transform.GetChild(1).gameObject.GetComponent<Text>().text = Resources.Load<TextAsset>("plot/" + clickedButtonName + "-2").ToString();

        while (!isClick)
            yield return null;
        isClick = false;

        BGobject.transform.GetChild(0).gameObject.SetActive(false);
        BGobject.transform.GetChild(1).gameObject.SetActive(false);
        BGobject.transform.GetChild(2).gameObject.SetActive(true);
        BGobject.transform.GetChild(2).gameObject.GetComponent<Text>().text = Resources.Load<TextAsset>("plot/" + clickedButtonName + "-3").ToString();

        while (!isClick)
            yield return null;
        isClick = false;

        Background.color = new Color32(255, 255, 255, 110);
        BGobject.SetActive(false);

        ChooseCharacter();
        isShowText = false;
    }

    private IEnumerator Clicked_Footprint()
    {
        Background.color = new Color32(255, 255, 255, 80);
        GameObject BGobject = BG.transform.Find("bgWarning").gameObject;
        BGobject.SetActive(true);
        BGobject.transform.GetChild(0).gameObject.SetActive(true);
        BGobject.transform.GetChild(1).gameObject.SetActive(true);
        BGobject.transform.GetChild(2).gameObject.SetActive(false);
        BGobject.transform.GetChild(0).gameObject.GetComponent<Text>().text = Resources.Load<TextAsset>("plot/" + clickedButtonName + "-11").ToString();
        BGobject.transform.GetChild(1).gameObject.GetComponent<Text>().text = Resources.Load<TextAsset>("plot/" + clickedButtonName + "-2").ToString();

        while (!isClick)
            yield return null;
        isClick = false;

        BGobject.transform.GetChild(0).gameObject.GetComponent<Text>().text = Resources.Load<TextAsset>("plot/" + clickedButtonName + "-12").ToString();

        while (!isClick)
            yield return null;
        isClick = false;

        BGobject.transform.GetChild(0).gameObject.GetComponent<Text>().text = Resources.Load<TextAsset>("plot/" + clickedButtonName + "-13").ToString();

        while (!isClick)
            yield return null;
        isClick = false;

        BGobject.transform.GetChild(0).gameObject.SetActive(false);
        BGobject.transform.GetChild(1).gameObject.SetActive(false);
        BGobject.transform.GetChild(2).gameObject.SetActive(true);
        BGobject.transform.GetChild(2).gameObject.GetComponent<Text>().text = Resources.Load<TextAsset>("plot/" + clickedButtonName + "-3").ToString();

        while (!isClick)
            yield return null;
        isClick = false;

        Background.color = new Color32(255, 255, 255, 110);
        BGobject.SetActive(false);

        ChooseCharacter();
        isShowText = false;
    }

    private IEnumerator Clicked_Skeleton()
    {
        Background.color = new Color32(255, 255, 255, 80);
        GameObject BGobject = BG.transform.Find("bgRed").gameObject;
        BGobject.SetActive(true);
        BGobject.transform.GetChild(0).gameObject.SetActive(true);
        BGobject.transform.GetChild(1).gameObject.SetActive(true);
        BGobject.transform.GetChild(2).gameObject.SetActive(false);
        BGobject.transform.GetChild(0).gameObject.GetComponent<Text>().text = Resources.Load<TextAsset>("plot/16-11").ToString();
        BGobject.transform.GetChild(1).gameObject.GetComponent<Text>().text = Resources.Load<TextAsset>("plot/16-2").ToString();

        while (!isClick)
            yield return null;
        isClick = false;

        BGobject.transform.GetChild(0).gameObject.GetComponent<Text>().text = Resources.Load<TextAsset>("plot/16-12").ToString();

        while (!isClick)
            yield return null;
        isClick = false;

        BGobject.transform.GetChild(0).gameObject.GetComponent<Text>().text = Resources.Load<TextAsset>("plot/16-13").ToString();

        while (!isClick)
            yield return null;
        isClick = false;

        BGobject.transform.GetChild(0).gameObject.SetActive(false);
        BGobject.transform.GetChild(1).gameObject.SetActive(false);
        BGobject.transform.GetChild(2).gameObject.SetActive(true);
        BGobject.transform.GetChild(2).gameObject.GetComponent<Text>().text = Resources.Load<TextAsset>("plot/16-3").ToString();

        while (!isClick)
            yield return null;
        isClick = false;

        Background.color = new Color32(255, 255, 255, 110);
        BGobject.SetActive(false);

        ChooseCharacter();
        isShowText = false;
    }

    private IEnumerator Clicked_BigCube()
    {
        GameObject tempObject = CUBE.transform.Find(clickedButtonName).gameObject;
        tempObject.transform.GetChild(0).gameObject.SetActive(false);
        for (int i = 2;i < 5;i += 1)
            tempObject.transform.GetChild(i).gameObject.SetActive(true);

        isBigCubeClick = true;
        yield return null;
    }

    private void ChooseCharacter()
    {
        for (int i = 0;i < GameObject.Find("CHARACTER").transform.childCount;i += 1)
            GameObject.Find("CHARACTER").transform.GetChild(i).gameObject.SetActive(true);
    }

    public void CancelChooseCharacter()
    {
        for (int i = 0;i < GameObject.Find("CHARACTER").transform.childCount;i += 1)
            GameObject.Find("CHARACTER").transform.GetChild(i).gameObject.SetActive(false);
        status = 1;
        GoBackButton.SetActive(false);
    }

    public static void HideCharacter()
    {
        for (int i = 0;i < GameObject.Find("CHARACTER").transform.childCount;i += 1)
            GameObject.Find("CHARACTER").transform.GetChild(i).gameObject.SetActive(false);
    }

    public void ToSingleMode()
    {
        SceneManager.LoadScene("SingleMode");
    }
}