
using Core.UI;
using Core.Systems;
using UI.ExerPro.EnglishPro.CorrectionScene.Controls;
using System.Collections.Generic;
using UnityEngine;
using ExerPro.EnglishModule.Data;
using ExerPro.EnglishModule.Services;
using static ExerPro.EnglishModule.Data.CorrectionQuestion;

namespace UI.ExerPro.EnglishPro.CorrectionScene
{

    /// <summary>
    /// 场景
    /// </summary>
    public class CorrectionScene : BaseScene
	{

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public ArticleDisplay articleDisplay;

        /// <summary>
        /// 外部系统设置
        /// </summary>
        EnglishService engSer;
        //private string articleStr = "Bay is a phenomenon-the world's garage sale, online center, car dealer and auction site in 30 countries as of March 2005. You can find everything from encyclopedias to olives to snow boots to stereos to airplanes for sale. And if you stumble on it before the eBay overseers do, you might even find a human kidney or a virtual date.";
        //private string articleStr = "as of March 2005 .airplanes for sale.  a virtual date.";

        //public Font font;
        //private List<List<GameObject>> wordObjs;
        //private List<GameObject> sentenceObjs;
        //private GameObject articleObj;

        #region 初始化

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems()
        {
            base.initializeSystems();
            engSer = EnglishService.get();
        }

        /// <summary>
        /// 场景名
        /// </summary>
        /// <returns>场景名</returns>
        public override string sceneName() {
            return SceneSystem.Scene.EnglishProCorrectionScene;
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected override void start()
        {
            base.start();
            articleDisplay.startView(CorrectionQuestion.sample());
            //articleDisplay.setItems(items2);
            //articleDisplay.test();
        }
        #endregion




        // Use this for initialization
        //void Start ()
        //{

        //articleObj = GameObject.Find("Article");
        ////articleObj.AddComponent<Article>();
        ////articleObj.AddComponent<VerticalLayoutGroup>();
        ////articleObj.AddComponent<Image>();
        ////articleObj.AddComponent<ContentSizeFitter>();
        ////GameObject canvasObj = GameObject.Find("Canvas");
        ////articleObj.transform.SetParent(canvasObj.transform);
        ////articleObj.transform.localScale = new Vector3(1, 1, 1);
        ////articleObj.transform.position = canvasObj.transform.position;
        ////SceneUtils.get<RectTransform>(articleObj).sizeDelta = new Vector2(800, 600);
        ////SceneUtils.get<Image>(articleObj).color = new Color(60f / 255, 80f / 255, 1, 0.2f);
        ////SceneUtils.get<VerticalLayoutGroup>(articleObj).spacing = 5f;
        //ContentSizeFitter czf = SceneUtils.get<ContentSizeFitter>(articleObj);
        ////czf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        ////czf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;



        //int sentenceIndex = 0;
        //int wordIndex = 0;
        //string[] sentences = articleStr.Split('.');
        //foreach (string sentence in sentences)
        //{
        //    if (sentence == "")
        //        continue;
        //    wordIndex = 0;
        //    string s = sentence.Trim();

        //    GameObject sentenceObj = (GameObject)Resources.Load("Assets/Prefabs/CorrectionScene/Sentence");
        //    sentenceObj = Instantiate(sentenceObj);

        //    //GameObject sentenceObj = new GameObject();
        //    sentenceObj.name = "Sentence" + sentenceIndex.ToString();
        //    //sentenceObj.AddComponent<Sentence>();
        //    //sentenceObj.AddComponent<ContentSizeFitter>();
        //    //sentenceObj.AddComponent<HorizontalLayoutGroup>();
        //    sentenceObj.transform.SetParent(articleObj.transform);
        //    //sentenceObj.transform.position = new Vector3(0, 0, 0);
        //    //sentenceObj.transform.localScale = new Vector3(1, 1, 1);
        //    string[] words = s.Split(' ');

        //    //czf = SceneUtils.get<ContentSizeFitter>(sentenceObj);
        //    //czf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        //    //HorizontalLayoutGroup hlg = SceneUtils.get<HorizontalLayoutGroup>(sentenceObj);
        //    //hlg.spacing = 10;
        //    //hlg.padding.top = 10;
        //    //hlg.padding.bottom = 10;
        //    //hlg.childScaleWidth = true;
        //    //hlg.childControlWidth = false;
        //    //hlg.childForceExpandWidth = false;

        //    //FontData data = Resources.Load<FontData>("");

        //    foreach (string word in words)
        //    {
        //        if (word == "")
        //            continue;
        //        string w = word;
        //        if (wordIndex == words.Length - 1)
        //            w += '.';

        //        GameObject wordObj = (GameObject)Resources.Load("Assets/Prefabs/CorrectionScene/Word");
        //        wordObj = Instantiate(wordObj);

        //        //GameObject wordObj = new GameObject("Word" + wordIndex.ToString());
        //        wordObj.name = "Sentence" + wordIndex.ToString();

        //        wordObj.transform.SetParent(sentenceObj.transform);
        //        //wordObj.AddComponent<Word>();
        //        //wordObj.AddComponent<ContentSizeFitter>();
        //        //wordObj.transform.position = new Vector3(0, 0, 0);
        //        //wordObj.transform.localScale = new Vector3(1, 1, 1);
        //        //wordObj.GetComponent<RectTransform>().sizeDelta = new Vector2(15 * w.Length, 18);
        //        //hlg = SceneUtils.get<HorizontalLayoutGroup>(wordObj);
        //        //hlg.childScaleHeight = true;
        //        //hlg.childScaleWidth = true

        //        //wordObj.AddComponent<ContentSizeFitter>();
        //        //SceneUtils.get<ContentSizeFitter>(wordObj).horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

        //        //GameObject textObj = new GameObject("Text");
        //        //textObj.transform.SetParent(wordObj.transform);
        //        //textObj.AddComponent<Text>();
        //        //textObj.AddComponent<ContentSizeFitter>();
        //        //SceneUtils.get<Text>(textObj).text = w;
        //        //SceneUtils.get<Text>(textObj).fontSize = 18;
        //        //SceneUtils.get<Text>(textObj).font = font;
        //        //SceneUtils.get<RectTransform>(textObj).sizeDelta = new Vector2(15 * w.Length, 18);

        //        //czf = SceneUtils.get<ContentSizeFitter>(textObj);
        //        //czf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        //        //czf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        //        //GameObject selectedFlag = new GameObject("SelectedFlag");
        //        //GameObject highlightFlag = new GameObject("HighLightFlag");

        //        //selectedFlag.transform.SetParent(wordObj.transform);
        //        //selectedFlag.AddComponent<Image>();
        //        //SceneUtils.get<Image>(selectedFlag).color = new Color(0, 0, 0, 100f / 255);

        //        //highlightFlag.transform.SetParent(wordObj.transform);
        //        //highlightFlag.AddComponent<Image>();
        //        //SceneUtils.get<Image>(highlightFlag).color = new Color(1, 1, 1, 100f / 255);

        //        //SceneUtils.get<Word>(wordObj).selectedFlag = selectedFlag;
        //        //SceneUtils.get<Word>(wordObj).highlightFlag = highlightFlag;

        //        GameObject textObj = wordObj.GetComponent<Transform>().GetChild(0).gameObject;

        //        textObj.GetComponent<ContentSizeFitter>().SetLayoutHorizontal();
        //        textObj.GetComponent<ContentSizeFitter>().SetLayoutVertical();

        //        Debug.Log(textObj.GetComponent<RectTransform>().sizeDelta);
        //        //selectedFlag.GetComponent<RectTransform>().sizeDelta = textObj.GetComponent<RectTransform>().sizeDelta;
        //        //highlightFlag.GetComponent<RectTransform>().sizeDelta = textObj.GetComponent<RectTransform>().sizeDelta;
        //        wordObj.GetComponent<RectTransform>().sizeDelta = textObj.GetComponent<RectTransform>().sizeDelta;

        //        //czf = SceneUtils.get<ContentSizeFitter>(wordObj);
        //        //czf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        //        //czf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        //        wordIndex++;
        //    }

        //    sentenceIndex++;
        //}

        //}

    }


}

