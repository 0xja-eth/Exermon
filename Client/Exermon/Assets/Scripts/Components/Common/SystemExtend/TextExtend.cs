using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
[AddComponentMenu("UI/Text Extend", 10)]
public class TextExtend : Text {
    const float FontSizeRate = 0.5f;

    public static readonly Vector2 ImgPadding = new Vector2(2, 0);
    public static readonly Vector2 ImgOffset = new Vector2(0, -2);

    public const string SpaceIdentifier = "&S&";
    public const string SpaceEncode = "\u00A0";

    public static Texture2D[] texturePool { get; set; } = new Texture2D[0];
    /*
    public static void setTexturePool(Texture2D[] textures) { texturePool = textures; }
    public static Texture2D[] getTexturePool() { return texturePool; }
    */
    [SerializeField]
    QuadImageHandler m_QuadImageHandler = new QuadImageHandler();

    public QuadImageHandler QuadImageHandler {
        get { return m_QuadImageHandler; }
    }
    [SerializeField]
    FontAdjustHandler m_FontAdjustHandler = new FontAdjustHandler();

    public FontAdjustHandler FontAdjustHandler {
        get { return m_FontAdjustHandler; }
    }

    [SerializeField]
    private string[] m_FilterLabels = new string[] {
        "br", "a", "div", "em", "label", "span", "!--"
    };
    public string[] FilterLabels {
        get { return m_FilterLabels; }
        set { m_FilterLabels = value; }
    }

    [SerializeField]
    private float m_ContentSizeRate = 1.0f;

    public float ContentSizeRate {
        get { return m_ContentSizeRate; }
        set { m_ContentSizeRate = value; }
    }


    private string[] m_MatchRegs = {QuadImageHandler.quadRegexStr,
        QuadImageHandler.imgRegexStr, FontAdjustHandler.adjRegexStr };
    private string m_SizeRegs = @"<size=(\d+?)>";

    private float m_SpaceWidth = -1;

    public float SpaceWidth {
        get {
            if (m_SpaceWidth < 0) m_SpaceWidth = meansure(SpaceEncode).x;
            return m_SpaceWidth;
        }
    }
    private float m_EnterHeight = -1;

    public float EnterHeight {
        get {
            if (m_EnterHeight < 0) m_EnterHeight = meansure("\n").y/2;
            return m_EnterHeight;
        }
    }

    private float m_BigCharWidth = -1;

    public float BigCharWidth {
        get {
            if (m_BigCharWidth < 0) m_BigCharWidth = meansure("口").x;
            return m_BigCharWidth;
        }
    }

    private readonly int[] visASCIIs = { 32, 126 };
    private float[] m_SmallCharWidth = { };

    public float[] SmallCharWidth {
        get {
            if (m_SmallCharWidth.Length <= 0) {
                int min = visASCIIs[0], max = visASCIIs[1];
                m_SmallCharWidth = new float[max - min + 1];
                for (int i = 0; i <= max - min; i++)
                    m_SmallCharWidth[i] = meansure(((char)(i + min)).ToString()).x;
            }
            return m_SmallCharWidth;
        }
    }

    private string m_OutputText;
    string m_LastText = "";

    Vector2 m_PerfectSize = new Vector2(0,0);

    public override float preferredHeight {
        get { return m_PerfectSize.y; }
    }

    public Vector2 meansure(string s, int fsize = -1) {
        Vector2 size = rectTransform.rect.size; size.y = 100;
        var mTextGenerator = cachedTextGeneratorForLayout;
        var m_TgSettings = GetGenerationSettings(size);
        if (fsize > 0) m_TgSettings.fontSize = fsize;

        float w = mTextGenerator.GetPreferredWidth(s, m_TgSettings);
        float h = mTextGenerator.GetPreferredHeight(s, m_TgSettings);

        return new Vector2(w, h);
    }
    public float meansureWidth(char c, int fsize = -1) {
        int ascii = c; float res = BigCharWidth;
        if (ascii >= visASCIIs[0] && ascii <= visASCIIs[1])
            res = SmallCharWidth[ascii - visASCIIs[0]];
        if (fsize > 0) res *= fsize*1.0f / fontSize;
        return res;
    }

    protected override void Start() {
        /*
        var mTextGenerator = cachedTextGeneratorForLayout;
        var mTgSettings = GetGenerationSettings(rectTransform.rect.size);
        var width = mTextGenerator.GetPreferredWidth(" ", mTgSettings);
        */
    }

    public override void SetVerticesDirty() {
        base.SetVerticesDirty();
        /*
        m_QuadImageHandler.setTextObj(this);
        m_FontAdjustHandler.setTextObj(this);
        m_OutputText = GetOutputTextBefore(text);
        m_OutputText = m_FontAdjustHandler.update(m_OutputText);
        m_OutputText = m_QuadImageHandler.update(m_OutputText);
        m_OutputText = GetOutputTextAfter(m_OutputText);
        */
        if(m_LastText != text) {
            m_LastText = text;
            m_QuadImageHandler.setTextObj(this);
            m_FontAdjustHandler.setTextObj(this);
            m_OutputText = GetOutputTextBefore(text);
            m_OutputText = parseExtends(m_OutputText);
            m_OutputText = GetOutputTextAfter(m_OutputText);
        }
    }

    #region Utils
    public struct Range {
        public int l, r;
        public string tag;
        public Range(int l, int r, string tag = "") {
            this.l = l; this.r = r; this.tag = tag;
        }
        public int[] toArray() {
            return new int[] { l, r };
        }
    }
    void beforeParse() {
        m_PerfectSize = new Vector2(0, 0);
        m_QuadImageHandler.beforeParse();
        m_FontAdjustHandler.beforeParse();
    }
    void afterParse() {
        m_QuadImageHandler.afterParse();
        m_FontAdjustHandler.afterParse();
    }
    void processReplaceAtTail(string rpl, int pos, ref int i, ref StringBuilder oriText) {
        int len = i - pos + 1;
        //Debug.Log(i+": pos:" + pos + " , len:" + len + " , rpl: " + rpl);
        oriText.Remove(pos, len);
        oriText.Insert(pos, rpl);
        i += rpl.Length - len;
    }
    void processInsert(string value, int pos, ref StringBuilder oriText,
        ref int i, ref List<Range> ranges, ref List<int> lines) {
        int cnt = value.Length;
        //Debug.Log("Pos : "+pos+" value.Length : " + cnt);
        oriText.Insert(pos, value);
        if (pos <= i) i += cnt;
        for (int j = 0; j < ranges.Count; j++) {
            Range r = ranges[j];
            if (pos <= r.l) r.l += cnt;
            if (pos <= r.r) r.r += cnt;
            ranges[j] = r;
        }
        for (int j = 0; j < lines.Count; j++)
            if (pos <= lines[j]) lines[j] += cnt;
    }
    void endLine(int i, ref float x, ref float y, ref int l, ref List<int> lines,
        ref List<int> enters, ref List<Range> ranges) {
        x = 0; l++;
        y += EnterHeight * lineSpacing;
        lines.Add(i); enters.Add(0);
        // 把本行的 ranges 缓冲加入
        clearRanges(ref ranges);
    }

    // 读入并清除 ranges 缓冲
    void clearRanges(ref List<Range> ranges) {
        // 把本行的 ranges 缓冲加入
        int j = 0;
        foreach (Range r in ranges) 
            switch (r.tag) {
                case "</sub>":
                    m_FontAdjustHandler.subIndexs.Add(r.toArray());
                    break;
                case "</sup>":
                    m_FontAdjustHandler.supIndexs.Add(r.toArray());
                    break;
                case "quad":
                    m_QuadImageHandler.imgIndexs.Add(r.l);
                    break;
            }
        ranges.Clear();
    }

    void processQuad(Vector2 size, float mw, int len, ref StringBuilder text, 
        ref int i, ref float x, ref float y, ref int l, ref List<int> lines,
        ref List<int> enters, ref List<Range> ranges) {
        string rpl = "", ent = "";
        // 空格数，回车数
        //float sw = meansure(GameUtils.spaceEncode).x;
        //Debug.Log("size = " + size);
        float w = size.x + ImgPadding.x * 2;
        float h = size.y + ImgPadding.y + ImgOffset.y;
        int spcnt = Mathf.CeilToInt(w / SpaceWidth);
        int etcnt = Mathf.CeilToInt(h / EnterHeight);
        //Debug.Log("Size = " + size.x + " Cnt = " + spcnt);

        int pos = i - len + 1;

        // 若填充空格后宽度超出范围
        if (x + spcnt * SpaceWidth >= mw) {
            //endLine(pos, ref x, ref y, ref l, ref lines, ref enters, ref ranges);
            // 回车占位 & 手动换行
            for (int j = 0; j < etcnt; j++) ent += "\n";
            y += (etcnt) * EnterHeight * lineSpacing;
            //Debug.Log("etcnt = " + etcnt);
            //Debug.Log("processInsert for img next line");
            processInsert(ent, pos, ref text, ref i, ref ranges, ref lines);
            // 如果在本行，回车数量大于当行最大数量
        } else if (etcnt > enters[l]) {
            int delta = etcnt - enters[l];
            int lid = lines[l]; // 本行行首 id
            enters[l] = etcnt;
            for (int j = 0; j < delta; j++) ent += "\n";
            y += delta * EnterHeight * lineSpacing;
            //Debug.Log("delta = " + delta);
            //Debug.Log("processInsert for this line");
            processInsert(ent, lid, ref text, ref i, ref ranges, ref lines);
        }
        // 空格占位
        x += spcnt * SpaceWidth;
        for (int j = 0; j < spcnt; j++) rpl += SpaceEncode;

        processReplaceAtTail(rpl, i - len + 1, ref i, ref text);
    }
    #endregion

    string parseExtends(string outputText) {
        //Regex areg = new Regex(getAllRegs(), RegexOptions.Singleline | RegexOptions.IgnoreCase);
        Regex sreg = new Regex(m_SizeRegs, RegexOptions.Singleline | RegexOptions.IgnoreCase);
        Regex qreg = QuadImageHandler.quadRegex;
        Regex ireg = QuadImageHandler.imgRegex;

        string special = ""; // 特殊代码储存
        StringBuilder text = new StringBuilder(outputText);
        List<int> lines = new List<int>();  // 行起始 id
        List<int> enters = new List<int>(); // 行回车数
        List<Range> ranges = new List<Range>(); // 范围

        int l = -1; float x = 0, y = 0;
        float mw = rectTransform.rect.width;
        float fsize = fontSize, lsize = fontSize;
        string type = ""; bool spec = false;

        //Debug.Log("SpaceWidth = " + SpaceWidth);
        //Debug.Log("EnterHeight = " + EnterHeight);

        int temp = 0;

        beforeParse();

        endLine(0, ref x, ref y, ref l, ref lines, ref enters, ref ranges);

        for (int i = 0; i < text.Length; i++) {
            char c = text[i];
            // 处理主动换行
            if (c == '\n') endLine(i, ref x, ref y, ref l, ref lines, ref enters, ref ranges);
            if (c == '<' || spec) { // 开始特殊代码
                spec = true; special += c;
                // 特殊码结束
                if (c == '>') {
                    spec = false;
                    string rpl = "";
                    int len = special.Length;
                    // 判断特殊码
                    switch (special) {
                        case "<sub>":
                        case "<sup>":
                            fsize *= FontSizeRate;
                            type = "sub"; temp = i - len + 1;
                            //Debug.Log("temp = " + temp + " , len = " + len);
                            rpl = "<size=" + fsize + ">";
                            processReplaceAtTail(rpl, temp, ref i, ref text);
                            /*
                            text.Remove(temp, len);
                            text.Insert(temp, rpl);
                            i += rpl.Length - len;
                            */
                            break;
                        case "</sub>":
                        case "</sup>":
                            fsize /= FontSizeRate;
                            type = ""; rpl = "</size>";
                            ranges.Add(new Range(temp, i, special));
                            processReplaceAtTail(rpl, i-len+1, ref i, ref text);
                            /*
                            text.Remove(i - len, len);
                            text.Insert(i - len, rpl);
                            i += rpl.Length - len;
                            */
                            break;
                        case "</size>":
                            fsize = lsize;
                            type = "";
                            break;
                        default:
                            foreach (Match match in sreg.Matches(special)) {
                                lsize = fsize; type = "size";
                                fsize = float.Parse(match.Groups[1].Value);
                            }
                            foreach (Match match in qreg.Matches(special)) {
                                //Debug.Log("qreg Matched!");

                                int pos = i - len + 1;
                                ranges.Add(new Range(pos,pos,"quad"));

                                m_QuadImageHandler.addImage();
                                m_QuadImageHandler.createImageObjectIfNeeded();

                                var name = match.Groups[1].Value;
                                var size = Math.Min(mw, float.Parse(match.Groups[2].Value));
                                //size *= m_ContentSizeRate;
                                var img = m_QuadImageHandler.getCurrentImage();

                                m_QuadImageHandler.setupImageSprite(img, name);
                                var rsize = m_QuadImageHandler.setupImageTransform(img, size);

                                processQuad(rsize, mw, len, ref text, ref i,
                                    ref x, ref y, ref l, ref lines, ref enters, ref ranges);
                            }
                            foreach (Match match in ireg.Matches(special)) {
                                //Debug.Log("ireg Matched!");

                                int pos = i - len + 1;
                                ranges.Add(new Range(pos, pos, "quad"));

                                m_QuadImageHandler.addImage();
                                m_QuadImageHandler.createImageObjectIfNeeded();

                                var id = int.Parse(match.Groups[1].Value);
                                var size = Math.Min(mw, float.Parse(match.Groups[2].Value));
                                size *= m_ContentSizeRate;
                                var img = m_QuadImageHandler.getCurrentImage();

                                m_QuadImageHandler.setupImageSprite(img,
                                    m_QuadImageHandler.Textures[id]);
                                var rsize = m_QuadImageHandler.setupImageTransform(img, size);

                                processQuad(rsize, mw, len, ref text, ref i,
                                    ref x, ref y, ref l, ref lines, ref enters, ref ranges);
                            }
                            break;
                    }
                    special = "";
                }
            } else { // 处理普通字符
                //float w = meansureWidth(c, (int)fsize);
                float w = meansure(c.ToString(), (int)fsize).x;
                //Debug.Log("meansure = " + w);
                // 如果超出范围
                if (x + w >= mw) endLine(i, ref x, ref y, ref l, ref lines, ref enters, ref ranges);
                x += w;
            }
            //Debug.Log(i + "/" + text.Length + " , " + c + " \n\t\t(" + x + "/" + mw + "," + y + ")");

        }
        float tmpy = 0;
        endLine(text.Length, ref x, ref tmpy, ref l, ref lines, ref enters, ref ranges);

        m_PerfectSize = new Vector2(mw, y);
        //Debug.Log("Finally: "+text);
        #region OldCode
        /*
        m_QuadImageHandler.beforeParse();
        m_FontAdjustHandler.beforeParse();

        int pointerIndex = 0;
        string outText = "";
        float spacew = SpaceWidth;
        foreach (Match match in reg.Matches(outputText)) {
            string _content = match.Value;
            // 若匹配的是 QuadImageHandler.quadRegex
            if (QuadImageHandler.quadRegex.IsMatch(match.Value)) {
                string name = match.Groups[2].Value;
                float size = float.Parse(match.Groups[3].Value);
                int spaceCnt = Mathf.CeilToInt(size / spacew);
                string content = "";
                for (int i = 0; i < spaceCnt; i++) content += " ";
            }
            // 若匹配的是 QuadImageHandler.imgRegex
            if (QuadImageHandler.imgRegex.IsMatch(match.Value)) {
                int id = int.Parse(match.Groups[2].Value);
                float size = float.Parse(match.Groups[3].Value);
                int spaceCnt = Mathf.CeilToInt(size / spacew);
                string content = "";
                for (int i = 0; i < spaceCnt; i++) content += " ";
            }
            // 若匹配的是 FontAdjustHandler.adjRegex
            if (FontAdjustHandler.adjRegex.IsMatch(match.Value)) {
                int startIndex = match.Index, endIndex;
                string tagH = match.Groups[8].Value;
                string tagT = match.Groups[10].Value;
                string content = match.Groups[11].Value;

                Debug.Log("tagH = " + tagH + " tagT = " + tagT);
                Capture tail = match.Groups[9];
                content = "<size=" + (fsize / 1.4f) + ">" + content + "</size>";
                outText += outputText.Substring(pointerIndex, startIndex - pointerIndex);
                startIndex = outText.Length;
                endIndex = startIndex + content.Length;
                outText += content;

                pointerIndex = tail.Index + tail.Length;

                if (tagH != tagT) continue;
                if (tagH == "sub") m_FontAdjustHandler.subIndexs.
                        Add(new int[] { startIndex, endIndex });
                if (tagH == "sup") m_FontAdjustHandler.supIndexs.
                        Add(new int[] { startIndex, endIndex });
            }
            
        }


        foreach (Match match in reg.Matches(outputText)) {
            if (QuadImageHandler.quadRegex.IsMatch(match.Value))
                outputText = m_QuadImageHandler.parseQuadMatch(match, outputText);
            if (QuadImageHandler.imgRegex.IsMatch(match.Value))
                outputText = m_QuadImageHandler.parseTextureMatch(match, outputText);
            if (FontAdjustHandler.adjRegex.IsMatch(match.Value))
                outputText = m_FontAdjustHandler.parseAdjust(match, outputText);
        }



        outText += outputText.Substring(pointerIndex);
        Debug.Log("After updateSupAdjust: " + outText);
        return outText;
        */
        #endregion
        afterParse();

        return text.ToString();
    }

/*
using System;
using System.Text.RegularExpressions;

public class Example
{
public const string quadRegexStr = @"\[quad name=(.+?) size=(\d*\.?\d+%?) width=(\d*\.?\d+%?) /\]";
public const string imgRegexStr = @"\[quad id=(\d+?) size=(\d*\.?\d+%?) width=(\d*\.?\d+%?) /\]";

public static readonly Regex quadRegex = new Regex(quadRegexStr, RegexOptions.Singleline);
public static readonly Regex imgRegex = new Regex(imgRegexStr, RegexOptions.Singleline);

public const string adjRegexStr = @"\[(sup|sub)\](?<str>.*?)(\[/(sup|sub)\])";

public static readonly Regex adjRegex = new Regex(adjRegexStr, RegexOptions.Singleline);

private static string[] m_MatchRegs = {quadRegexStr,
    imgRegexStr, adjRegexStr };

static string getAllRegs() {
    return "(" + string.Join("|", m_MatchRegs) + ")";
}

public static void Main()
{        
   Regex reg = new Regex(Example.getAllRegs(), RegexOptions.Singleline | RegexOptions.IgnoreCase);

   string text = "[[97745[sup]XXASDWE[/sup]";

    // quad: 2,3,4
    // img : 5,6,7
    // sup : 8,10,11
    // sub : 8,10,11
    foreach (Match match in reg.Matches(text)) {

        string out_ = match.Value.Replace("<","[");
        out_ = out_.Replace(">","]");

        int index = 0;
        Console.WriteLine("Match: "+out_);
        foreach(Capture c in match.Groups)
            Console.WriteLine((index++) + ": " + c.Value);
        // 若匹配的是 QuadImageHandler.quadRegex
        if (quadRegex.IsMatch(match.Value)) {
            Console.WriteLine("quadRegex");
        }
        // 若匹配的是 QuadImageHandler.imgRegex
        if (imgRegex.IsMatch(match.Value)) {
            Console.WriteLine("imgRegex");
        }
        // 若匹配的是 FontAdjustHandler.adjRegex
        if (adjRegex.IsMatch(match.Value)) {
            Console.WriteLine("adjRegex");
        }
    }
}
*/

    string getAllRegs() {
        return "(" + string.Join("|", m_MatchRegs) + ")";
    }

    protected virtual string GetOutputTextBefore(string oriText) {
        // 过滤非法标签
        string regText = "</?(" + string.Join("|", FilterLabels) + ").*?/?>";
        oriText = Regex.Replace(oriText, regText, string.Empty, RegexOptions.IgnoreCase);
        return oriText;
    }
    protected virtual string GetOutputTextAfter(string oriText) {
        return oriText;
    }

    protected override void OnPopulateMesh(VertexHelper toFill) {
        var orignText = m_Text;
        m_Text = m_OutputText;
        base.OnPopulateMesh(toFill);
        m_Text = orignText;
        m_FontAdjustHandler.onMesh(toFill);
        m_QuadImageHandler.onMesh(toFill);
    }
}

[System.Serializable]
public class QuadImageHandler {

    public const string quadRegexStr = @"<quad name=(.+?) size=(\d*\.?\d+%?) width=(\d*\.?\d+%?) />";
    public const string imgRegexStr = @"<quad id=(\d+?) size=(\d*\.?\d+%?) width=(\d*\.?\d+%?) />";

    public static readonly Regex quadRegex = new Regex(quadRegexStr, RegexOptions.Singleline);
    public static readonly Regex imgRegex = new Regex(imgRegexStr, RegexOptions.Singleline);

    [SerializeField]
    private Texture2D[] m_Textures = new Texture2D[0];
    public Texture2D[] Textures {
        get { return m_Textures; }
        set { m_Textures = value; }
    }


    public static Func<string, Sprite> funLoadSprite;
    /// <summary>
    /// 图片池
    /// </summary>
    public List<Image> images = new List<Image>();

    /// <summary>
    /// 图片的最后一个顶点的索引
    /// </summary>
    //private readonly List<int> quadIndexs = new List<int>();
    public List<int> imgIndexs = new List<int>();

    int imagesCnt = 0;

    TextExtend textObj;

    public void setTextObj(TextExtend textObj) {
        this.textObj = textObj;
    }
    /*
    public string update(string outputText) {
        beforeParse();
        outputText = updateQuadImage(outputText);
        outputText = updateTextureImage(outputText);
        afterParse();
        return outputText;
    }
    string updateQuadImage(string outputText) {
        foreach (Match match in quadRegex.Matches(outputText))
            outputText = parseQuadMatch(match, outputText);
        return outputText;
    }
    string updateTextureImage(string outputText) {
        foreach (Match match in imgRegex.Matches(outputText))
            outputText = parseTextureMatch(match, outputText);
        return outputText;
    }
    */
    public void beforeParse() {
        imgIndexs.Clear(); imagesCnt = 0;
        m_Textures = TextExtend.texturePool;// GameUtils.getTexturePool();
        images.RemoveAll(image => image == null);
        if (images.Count == 0)
            textObj.GetComponentsInChildren(images);
    }
    public void afterParse() {
        for (var i = imagesCnt = imgIndexs.Count; i < images.Count; i++)
            if (images[i]) images[i].enabled = false;
    }
    /*
    public string parseQuadMatch(Match match, string outputText) {
        int index = match.Index;
        imgIndexs.Add(index);

        if (imgIndexs.Count > images.Count) createImageObject();

        var name = match.Groups[1].Value;
        var size = float.Parse(match.Groups[2].Value);
        var img = images[imgIndexs.Count - 1];
        setupImageSprite(img, name);
        setupImageTransform(img, size);
        return outputText;
    }
    public string parseTextureMatch(Match match, string outputText) {
        int index = match.Index;
        imgIndexs.Add(index);

        if (imgIndexs.Count > images.Count) createImageObject();

        var id = int.Parse(match.Groups[1].Value);
        var size = float.Parse(match.Groups[2].Value);
        var img = images[imgIndexs.Count - 1];
        setupImageSprite(img, m_Textures[id]);
        setupImageTransform(img, size);
        return outputText;
    }*/
    #region Utils
    public void addImage() {
        imagesCnt++;
    }
    public Image getCurrentImage() {
        return images[imagesCnt - 1];
    }
    public void createImageObjectIfNeeded() {
        if (imagesCnt > images.Count) createImageObject();
    }
    public void createImageObject() {
        var resources = new DefaultControls.Resources();
        var go = DefaultControls.CreateImage(resources);
        go.layer = textObj.gameObject.layer;
        var rt = go.transform as RectTransform;
        if (rt) {
            rt.SetParent(textObj.rectTransform);
            rt.localPosition = Vector3.zero;
            rt.localRotation = Quaternion.identity;
            rt.localScale = Vector3.one;
        }
        images.Add(go.GetComponent<Image>());
    }
    public void setupImageSprite(Image img, string name) {
        if (img.overrideSprite == null || img.overrideSprite.name != name) {
            img.overrideSprite = funLoadSprite != null ? funLoadSprite(name) :
                Resources.Load<Sprite>(name);
        }
    }
    public void setupImageSprite(Image img, Texture2D texture) {
        if (texture == null) { img.overrideSprite = null; return; }
        if (img.overrideSprite == null || img.overrideSprite.name != texture.name) {
            Rect r = new Rect(0, 0, texture.width, texture.height);
            img.overrideSprite = Sprite.Create(texture, r, new Vector2(0.5f, 0.5f));
            img.overrideSprite.name = texture.name;
        }
    }
    public Vector2 setupImageTransform(Image img, float size) {
        if (img.overrideSprite != null) {
            float ow = img.overrideSprite.texture.width;
            float oh = img.overrideSprite.texture.height;
            float height = size * oh / ow;
            
            img.rectTransform.sizeDelta = new Vector2(size, height);
            /*
            img.rectTransform.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Horizontal, size);
            img.rectTransform.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Vertical, height);*/
            //) = new Vector2(size, size);
            img.rectTransform.anchorMax = textObj.rectTransform.pivot;
            img.rectTransform.anchorMin = textObj.rectTransform.pivot;
            img.rectTransform.pivot = new Vector2(0, 0);
            img.enabled = true;
        } else {
            img.rectTransform.sizeDelta = Vector2.zero;
            img.enabled = false;
        }
        return img.rectTransform.sizeDelta;
    }
    #endregion

    public void onMesh(VertexHelper toFill) {
        UIVertex vert = new UIVertex();
        for (var i = 0; i < imgIndexs.Count; i++) {
            //Debug.Log("imgIndexs " + i+" : "+imgIndexs[i]);
            var endIndex = (imgIndexs[i]) * 4+3;
            var rt = images[i].rectTransform;
            var size = rt.sizeDelta;
            
            if (endIndex < toFill.currentVertCount) {
                toFill.PopulateUIVertex(ref vert, endIndex);
                //Debug.Log(vert.position);
                rt.anchoredPosition = vert.position;
                rt.anchoredPosition += new Vector2(
                    TextExtend.ImgPadding.x + TextExtend.ImgOffset.x,
                    TextExtend.ImgOffset.y);/* new Vector2(vert.position.x + size.x / 2,
                    vert.position.y + size.y / 2 - 2f);*/

                // 抹掉左下角的小黑点
                toFill.PopulateUIVertex(ref vert, endIndex - 3);
                var pos = vert.position;
                for (int j = endIndex, m = endIndex - 3; j > m; j--) {
                    toFill.PopulateUIVertex(ref vert, endIndex);
                    vert.position = pos;
                    toFill.SetUIVertex(vert, j);
                }
            }
        }
        if (imgIndexs.Count != 0) imgIndexs.Clear();
    }
}
public class FontAdjustHandler {

    public const string adjRegexStr = @"<(sup|sub)>(?<str>.*?)(</(sup|sub)>)";

    public static readonly Regex adjRegex = new Regex(adjRegexStr, RegexOptions.Singleline);

    public List<int[]> supIndexs = new List<int[]>();
    public List<int[]> subIndexs = new List<int[]>();

    TextExtend textObj;

    public void setTextObj(TextExtend textObj) {
        this.textObj = textObj;
    }
    /*
    public string update(string outputText) {
        subIndexs.Clear();
        supIndexs.Clear();
        int pointerIndex = 0;
        string outText = "";
        float size = textObj.fontSize;
        foreach (Match match in adjRegex.Matches(outputText)) {
            int startIndex = match.Index, endIndex;
            string tagH = match.Groups[1].Value;
            string tagT = match.Groups[3].Value;
            string content = match.Groups[4].Value;
            Debug.Log("tagH = " + tagH + " tagT = " + tagT);
            Capture tail = match.Groups[2];
            content = "<size=" + (size / 1.4f) + ">" + content + "</size>";
            outText += outputText.Substring(pointerIndex, startIndex - pointerIndex);
            startIndex = outText.Length; endIndex = startIndex + content.Length;
            outText += content;

            pointerIndex = tail.Index + tail.Length;

            if (tagH != tagT) continue;
            if (tagH == "sub") subIndexs.Add(new int[] { startIndex, endIndex });
            if (tagH == "sup") supIndexs.Add(new int[] { startIndex, endIndex });
        }
        outText += outputText.Substring(pointerIndex);
        Debug.Log("After updateSupAdjust: " + outText);
        return outText;
    }*/
    public void beforeParse() {
        subIndexs.Clear();
        supIndexs.Clear();
    }
    public void afterParse() {
    }
    /*
    public string parseAdjust(Match match, string outputText) {
        float size = textObj.fontSize;
        int startIndex = match.Index, endIndex;
        string tagH = match.Groups[1].Value;
        string tagT = match.Groups[3].Value;
        string content = match.Groups[4].Value;
        Debug.Log("tagH = " + tagH + " tagT = " + tagT);
        Capture tail = match.Groups[2];
        content = "<size=" + (size / 1.4f) + ">" + content + "</size>";
        outText += outputText.Substring(pointerIndex, startIndex - pointerIndex);
        startIndex = outText.Length; endIndex = startIndex + content.Length;
        outText += content;

        pointerIndex = tail.Index + tail.Length;

        if (tagH == tagT) {
            if (tagH == "sub") subIndexs.Add(new int[] { startIndex, endIndex });
            if (tagH == "sup") supIndexs.Add(new int[] { startIndex, endIndex });
        }
        outText += outputText.Substring(pointerIndex);

        return outText;
    }*/
    /*
    string updateSupAdjust(string outputText) {
        Debug.Log("Before updateSupAdjust: " + outputText);
        supIndexs.Clear();
        string outText = "";
        float size = textObj.fontSize;
        int pointerIndex = 0;
        foreach (Match match in supRegex.Matches(outputText)) {
            int startIndex = match.Index, endIndex;
            string tagH = match.Groups[1].Value;
            string tagT = match.Groups[3].Value;
            string content = match.Groups[4].Value;
            Capture tail = match.Groups[2];
            content = "<size=" + (size / 1.4f) + ">" + content + "</size>";
            outText += outputText.Substring(pointerIndex, startIndex - pointerIndex);
            startIndex = outText.Length; endIndex = startIndex + content.Length;
            outText += content;

            pointerIndex = tail.Index + tail.Length;

            if (tagH != tagT) continue;
            if (tagH == "sub") subIndexs.Add(new int[] { startIndex, endIndex });
            if (tagH == "sup") supIndexs.Add(new int[] { startIndex, endIndex });
        }
        outText += outputText.Substring(pointerIndex);
        Debug.Log("After updateSupAdjust: " + outText);
        foreach (int[] index in supIndexs)
            Debug.Log(index[0] + "," + index[1]);
        return outText;
    }
    string updateSubAdjust(string outputText) {
        subIndexs.Clear();
        Debug.Log("Before updateSubAdjust: " + outputText);
        string outText = "";
        float size = textObj.fontSize;
        int pointerIndex = 0;
        foreach (Match match in subRegex.Matches(outputText)) {
            int startIndex = match.Index, endIndex;
            string content = match.Groups[2].Value;
            Capture tail = match.Groups[1];
            content = "<size=" + (size / 1.4f) + ">" + content + "</size>";
            outText += outputText.Substring(pointerIndex, startIndex - pointerIndex);
            startIndex = outText.Length; endIndex = startIndex + content.Length;
            outText += content;

            pointerIndex = tail.Index + tail.Length;

            subIndexs.Add(new int[] { startIndex, endIndex });
        }
        outText += outputText.Substring(pointerIndex);
        Debug.Log("After updateSubAdjust: " + outText);
        foreach (int[] index in subIndexs)
            Debug.Log(index[0] + "," + index[1]);
        return outText;
    }
    */

    public void onMesh(VertexHelper toFill) {
        List<UIVertex> vertexs = new List<UIVertex>();
        bool lastSup = false, lastSub = false;
        int supIndex = 0, subIndex = 0;
        int[] indexs;
        toFill.GetUIVertexStream(vertexs);
        for (int i = 0; i < vertexs.Count; i++) {
            UIVertex vt = vertexs[i];
            if (supIndexs.Count > supIndex) {
                indexs = supIndexs[supIndex];
                if (i >= indexs[0] * 6 && i < indexs[1] * 6) {
                    vt.position += new Vector3(0, textObj.fontSize / 2, 0);
                    lastSup = true;
                } else if (lastSup) {
                    supIndex++; lastSup = false;
                }
            }
            /*
            if (subIndexs.Count > subIndex) {
                indexs = subIndexs[subIndex];
                if (i >= indexs[0] * 6 && i < indexs[1] * 6) {
                    //Debug.Log("Sub:True:" + i);
                    vt.position -= new Vector3(0, textObj.fontSize / 2, 0);
                    lastSub = true;
                } else if (lastSub) {
                    subIndex++; lastSub = false;
                }
            }
            */
            if (lastSub || lastSup) {
                vertexs[i] = vt;
                if (i % 6 <= 2) toFill.SetUIVertex(vt, (i / 6) * 4 + i % 6);
                if (i % 6 == 4) toFill.SetUIVertex(vt, (i / 6) * 4 + i % 6 - 1);
            }
        }
    }
    /*
    void performSupMesh(VertexHelper toFill) {
        List<UIVertex> vertexs = new List<UIVertex>();
        int indexCount = toFill.currentIndexCount;
        toFill.GetUIVertexStream(vertexs);
        UIVertex vt;
        foreach (int[] indexs in supIndexs) {
            Debug.Log("sup.pos: " + indexs[0] + "," + indexs[1]);
            for (int i = indexs[0] * 6; i < indexs[1] * 6; i++) {
                if (i >= indexCount) continue;
                vt = vertexs[i];
                vt.position += new Vector3(0, textObj.fontSize / 2, 0);
                vertexs[i] = vt;
                //以下注意点与索引的对应关系
                if (i % 6 <= 2) toFill.SetUIVertex(vt, (i / 6) * 4 + i % 6);
                if (i % 6 == 4) toFill.SetUIVertex(vt, (i / 6) * 4 + i % 6 - 1);
            }
        }
    }
    void performSubMesh(VertexHelper toFill) {
        List<UIVertex> vertexs = new List<UIVertex>();
        int indexCount = toFill.currentIndexCount;
        toFill.GetUIVertexStream(vertexs);
        UIVertex vt;
        foreach (int[] indexs in subIndexs) {
            Debug.Log("sub.pos: " + indexs[0] + "," + indexs[1]);
            for (int i = indexs[0] * 6; i < indexs[1] * 6; i++) {
                if (i >= indexCount) continue;
                vt = vertexs[i];
                vt.position -= new Vector3(0, textObj.fontSize / 4, 0);
                vertexs[i] = vt;
                //以下注意点与索引的对应关系
                if (i % 6 <= 2) toFill.SetUIVertex(vt, (i / 6) * 4 + i % 6);
                if (i % 6 == 4) toFill.SetUIVertex(vt, (i / 6) * 4 + i % 6 - 1);
            }
        }
    }
    */
}
