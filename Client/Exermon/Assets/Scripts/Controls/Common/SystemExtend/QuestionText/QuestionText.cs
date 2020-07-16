using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using Core.Systems;
using Core.UI.Utils;

using UI.Common.Controls.ItemDisplays;

/// <summary>
/// 系统拓展控件
/// </summary>
namespace UI.Common.Controls.SystemExtend { }

/// <summary>
/// 题目显示文本控件
/// </summary>
namespace UI.Common.Controls.SystemExtend.QuestionText {

    /// <summary>
    /// 题目文本控件
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("UI/Question Text", 10)]
    public class QuestionText : Text {

        /// <summary>
        /// 测试用
        /// </summary>
        /// <param name="str"></param>
        public static void TestLog(string str) {
            //Debug.Log(str);
        }

        /// <summary>
        /// 资源池管理器
        /// </summary>
        public static class TexturePool {

            /// <summary>
            /// 纹理池
            /// </summary>
            static Texture2D[] textures = new Texture2D[0];

            /// <summary>
            /// 设置纹理池
            /// </summary>
            /// <param name="textures"></param>
            public static void setTextures(Texture2D[] textures) {
                TexturePool.textures = textures;
            }

            /// <summary>
            /// 获取纹理
            /// </summary>
            /// <returns>纹理数组</returns>
            public static Texture2D[] getTextures() {
                return textures;
            }

            /// <summary>
            /// 获取纹理
            /// </summary>
            /// <returns>纹理数组</returns>
            public static void clearTextures() {
                textures = new Texture2D[0];
            }

            /// <summary>
            /// 获取纹理
            /// </summary>
            /// <param name="index">索引</param>
            /// <returns>纹理</returns>
            public static Texture2D getTexture(int index) {
                if (index < textures.Length) return textures[index];
                return null;
            }
        }

        /// <summary>
        /// 常量定义
        /// </summary>
        public const string SpaceIdentifier = "&S&"; // 普通空格识别符
        public const string SpaceEncode = "\u00A0"; // 不换行空格代码

        public const float FontSizeRate = 0.5f; // 小字体尺寸比率

        /// <summary>
        /// 处理器列表
        /// </summary>
        private List<TextHandler> handlers;

        /// <summary>
        /// 嵌入图片
        /// </summary>
        [SerializeField]
        private bool _embedImage;
        public bool embedImage {
            get { return _embedImage; }
            set { _embedImage = value; }
        }

        /// <summary>
        /// 纹理
        /// </summary>
        [SerializeField]
        private Texture2D[] _textures = new Texture2D[0];
        public Texture2D[] textures {
            get { return _textures; }
            set { _textures = value; }
        }

        /// <summary>
        /// 图片对象预制件
        /// </summary>
        [SerializeField]
        private GameObject _imagePrefab;
        public GameObject imagePrefab {
            get { return _imagePrefab; }
            set { _imagePrefab = value; }
        }

        /// <summary>
        /// 图片链接点击回调
        /// </summary>
        [SerializeField]
        private UnityAction<int> _onImageLinkClick = null;
        public UnityAction<int> onImageLinkClick {
            get { return _onImageLinkClick; }
            set { _onImageLinkClick = value; }
        }

        /// <summary>
        /// 图片容器控件
        /// </summary>
        [SerializeField]
        private ISelectableContainerDisplay _imageContainer = null;
        public ISelectableContainerDisplay imageContainer {
            get { return _imageContainer; }
            set { _imageContainer = value; }
        }
        
        /// <summary>
        /// 空格宽度
        /// </summary>
        private float _spaceWidth = -1;
        public float spaceWidth {
            get {
                if (_spaceWidth < 0) _spaceWidth = meansure(SpaceEncode).x;
                return _spaceWidth;
            }
        }

        /// <summary>
        /// 回车高度
        /// </summary>
        private float _enterHeight = -1;
        public float enterHeight {
            get {
                if (_enterHeight < 0) _enterHeight = meansure("\n").y / 2;
                return _enterHeight;
            }
        }

        /// <summary>
        /// 中文字符宽度
        /// </summary>
        private float _bigCharWidth = -1;
        public float bigCharWidth {
            get {
                if (_bigCharWidth < 0) _bigCharWidth = meansure("口").x;
                return _bigCharWidth;
            }
        }

        /// <summary>
        /// 可视ASCII范围
        /// </summary>
        private readonly int[] visASCIIs = { 32, 126 };

        /// <summary>
        /// 小字符宽度
        /// </summary>
        private float[] _smallCharWidth = { };
        public float[] smallCharWidth {
            get {
                if (_smallCharWidth.Length <= 0) {
                    int min = visASCIIs[0], max = visASCIIs[1];
                    _smallCharWidth = new float[max - min + 1];
                    for (int i = 0; i <= max - min; i++)
                        _smallCharWidth[i] = meansure(((char)(i + min)).ToString()).x;
                }
                return _smallCharWidth;
            }
        }

        /// <summary>
        /// 测量字符串尺寸
        /// </summary>
        /// <param name="s">字符串</param>
        /// <param name="fsize">字体</param>
        /// <returns>尺寸向量</returns>
        public Vector2 meansure(string s, int fsize = -1) {
            Vector2 size = rectTransform.rect.size; size.y = 9999;
            var textGenerator = cachedTextGenerator;
            var tgSettings = GetGenerationSettings(size);
            if (fsize > 0) tgSettings.fontSize = fsize;

            float w = textGenerator.GetPreferredWidth(s, tgSettings);
            float h = textGenerator.GetPreferredHeight(s, tgSettings);

            float unitsPerPixel = 1 / pixelsPerUnit;

            return new Vector2(w, h) * unitsPerPixel;
        }

        /// <summary>
        /// 测量宽度
        /// </summary>
        /// <param name="c">字符串</param>
        /// <param name="fsize">字体大小</param>
        /// <returns>宽度</returns>
        float meansureWidth(char c, int fsize = -1) {
            int ascii = c; float res = bigCharWidth;
            if (ascii >= visASCIIs[0] && ascii <= visASCIIs[1])
                res = smallCharWidth[ascii - visASCIIs[0]];
            if (fsize > 0) res *= fsize * 1.0f / fontSize;
            return res;
        }

        /// <summary>
        /// 内部变量声明
        /// </summary>
        private FontData fontData = FontData.defaultFontData;

        string lastText;

        public Vector2 perfectSize { get; set; } = new Vector2(0, 0);

        QuestionTextParser parser = null;

        /// <summary>
        /// 重载 PreferredHegiht 和 MinHeight
        /// </summary>
        public override float preferredHeight {
            get {
                if (perfectSize == new Vector2(0, 0))
                    return parser.getPreferredSize().y;
                return perfectSize.y;
            }
        }
        public override float minHeight {
            get {
                if (perfectSize == new Vector2(0, 0))
                    return parser.getPreferredSize().y;
                return perfectSize.y;
                /*
                float res;
                if (perfectSize == new Vector2(0, 0))
                    res = parser.getPreferredSize().y;
                res = perfectSize.y;
                Debug.LogError("get minHeight:" + res);
                return res;
                */
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        protected QuestionText() {
            // 反射获取基类的 fontData
            fontData = typeof(Text).GetField("m_FontData",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance).GetValue(this) as FontData;

            //createParser();
        }

        /// <summary>
        /// 创建分析器
        /// </summary>
        QuestionTextParser createParser() {
            if (parser != null) return parser;
            parser = new QuestionTextParser(this);
            parser.registerHandler(new QuadImageHandler(this));
            parser.registerHandler(new SubTextHandler(this));
            parser.registerHandler(new SupTextHandler(this));
            return parser;
        }

        /// <summary>
        /// 重载基类 SetVerticesDirty 函数
        /// </summary>
        public override void SetVerticesDirty() {
            TestLog("SetVerticesDirty");

            base.SetVerticesDirty();
            //if (lastText != m_Text) {
            //    lastText = m_Text;
            perfectSize = new Vector2(0, 0);

            TestSystem.startTimer("Parse Text Start");

            createParser().parseText(m_Text);

            TestSystem.endTimer("Parse Text End");

            //}
        }

        /// <summary>
        /// 覆盖基类 OnPopulateMesh 函数
        /// </summary>
        readonly UIVertex[] m_TempVerts = new UIVertex[4];
        protected override void OnPopulateMesh(VertexHelper toFill) {
            TestSystem.startTimer("OnPopulateMesh Start");

            if (font == null) return;
            if (parser == null) return;
            //QuestionText.TestLog("OnPopulateMesh: parser: " + parser);
            //createParser().parse(m_Text);

            // We don't care if we the font Texture changes while we are doing our Update.
            // The end result of cachedTextGenerator will be valid for this instance.
            // Otherwise we can get issues like Case 619238.
            m_DisableFontTextureRebuiltCallback = true;

            Vector2 extents = rectTransform.rect.size; extents.y = 99999;
            var settings = GetGenerationSettings(extents);

            var resText = parser.processedText();

            TestSystem.catchTimer("Populate Start");

            cachedTextGenerator.Populate(resText, settings);

            TestSystem.catchTimer("Parse End");

            IList<UIVertex> verts = cachedTextGenerator.verts;

            TestSystem.startTimer("Parse Verts Start");

            verts = parser.parseVertices(enterHeight, verts);

            TestSystem.catchTimer("Parse Verts End");

            float unitsPerPixel = 1 / pixelsPerUnit;
            //Last 4 verts are always a new line... (\n)
            int vertCount = verts.Count;// - 4;

            // We have no verts to process just return (case 1037923)
            if (vertCount <= 0) {
                toFill.Clear();
                return;
            }

            Vector2 roundingOffset = new Vector2(verts[0].position.x, verts[0].position.y) * unitsPerPixel;
            roundingOffset = PixelAdjustPoint(roundingOffset) - roundingOffset;
            toFill.Clear();
            if (roundingOffset != Vector2.zero) {
                for (int i = 0; i < vertCount; ++i) {
                    int tempVertsIndex = i & 3;
                    m_TempVerts[tempVertsIndex] = verts[i];
                    m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                    m_TempVerts[tempVertsIndex].position.x += roundingOffset.x;
                    m_TempVerts[tempVertsIndex].position.y += roundingOffset.y;
                    if (tempVertsIndex == 3)
                        toFill.AddUIVertexQuad(m_TempVerts);
                }
            } else {
                for (int i = 0; i < vertCount; ++i) {
                    int tempVertsIndex = i & 3;
                    m_TempVerts[tempVertsIndex] = verts[i];
                    m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                    if (tempVertsIndex == 3)
                        toFill.AddUIVertexQuad(m_TempVerts);
                }
            }
            /*
            List<UIVertex> vertList = new List<UIVertex>();
            toFill.GetUIVertexStream(vertList);

            var vcnt = vertList.Count;
            QuestionText.TestLog("vcnt: " + vcnt);

            for (int i = 0; i < vcnt / 6; ++i) {
                //var c = meshText[i];
                //var cd = charData[i];
                var v1 = vertList[i * 6];
                var v2 = vertList[i * 6 + 1];
                var v3 = vertList[i * 6 + 2];
                var v4 = vertList[i * 6 + 3];
                var v5 = vertList[i * 6 + 4];
                var v6 = vertList[i * 6 + 5];
                //cd.setVertices(v1, v2, v3, v4);
                QuestionText.TestLog("GetUIVertexStream: " + i
                    + "\nverts: " + v1.position + "," + v2.position
                    + "," + v3.position + "," + v4.position
                    + "," + v5.position + "," + v6.position);
            }
            */
            m_DisableFontTextureRebuiltCallback = false;

            parser.clear();

            TestSystem.endTimer("OnPopulateMesh End");
        }

        /// <summary>
        /// 实例化一个图片预制件
        /// </summary>
        public GameObject createImagePrefab() {
            return Instantiate(imagePrefab, rectTransform);
        }

        /// <summary>
        /// 附加一个图片对象
        /// </summary>
        /// <param name="go"></param>
        public void attachImage(GameObject go) {
            go.layer = gameObject.layer;

            var rt = go.transform as RectTransform;
            rt.SetParent(rectTransform);
            rt.localPosition = Vector3.zero;
            rt.localRotation = Quaternion.identity;
            rt.localScale = Vector3.one;
        }
    }

    /// <summary>
    /// 处理器类
    /// </summary>
    public class TextHandler {

        /// <summary>
        /// 正则表达式格式
        /// </summary>
        const string RegexpFormat = @"<{0}>(?<{1}>((?<n><{0}>)|</{0}>(?<-n>)|.*?)*)</{0}>";

        /// <summary>
        /// 文本对象
        /// </summary>
        protected QuestionText textObj;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="textObj">文本对象</param>
        public TextHandler(QuestionText textObj) {
            this.textObj = textObj;
        }

        /// <summary>
        /// 次数统计（处理完后清除）
        /// </summary>
        protected int configCnt = 0;
        protected int adjustCnt = 0;
        protected int handleCnt = 0;

        /// <summary>
        /// 是否作为区块处理
        /// </summary>
        public virtual bool block() { return false; }

        /// <summary>
        /// 处理器标签
        /// </summary>
        public virtual string tag() { return ""; }

        /// <summary>
        /// 替换分组组名
        /// </summary>
        /// <returns></returns>
        public virtual string groupName() { return "str"; }

        /// <summary>
        /// 正则表达式文本
        /// </summary>
        /// <returns></returns>
        protected virtual string regexStr() {
            string t = tag(), g = groupName();
            return string.Format(RegexpFormat, t, g);
        }

        /// <summary>
        /// 正则表达式
        /// </summary>
        /// <returns></returns>
        Regex _regex = null;
        public Regex regex() {
            if (_regex == null)
                _regex = new Regex(regexStr(), RegexOptions.Singleline);
            return _regex;
        }

        /// <summary>
        /// 处理单个匹配项
        /// </summary>
        /// <param name="match">匹配项</param>
        /// <param name="oriText">原始文本</param>
        /// <param name="start">起始位置</param>
        /// <param name="builder">实际字符串</param>
        /// <returns>结束位置</returns>
        public int processMatch(Match match, string oriText, int start, ref StringBuilder builder) {
            var index = match.Index;
            var len = match.Length;

            builder.Append(oriText, start, index - start);
            builder.Append(replace(match));

            return index + len;
        }

        /// <summary>
        /// 匹配
        /// </summary>
        /// <param name="text">匹配文本</param>
        /// <returns>匹配结果</returns>
        public MatchCollection matches(string text) {
            return regex().Matches(text);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void initialize() { }

        /// <summary>
        /// 清除缓存信息
        /// </summary>
        public virtual void clear() {
            configCnt = adjustCnt = handleCnt = 0;
        }

        /// <summary>
        /// 替换（去除标签文本）
        /// </summary>
        /// <param name="match">匹配结果</param>
        /// <returns>替换后字符串</returns>
        public virtual string replace(Match match) {
            return match.Groups[2].Value;
            //return regex().Replace(match.Value, "${" + groupName() + "}");
        }

        /// <summary>
        /// 替换（去除标签文本）
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns>替换后字符串</returns>
        public virtual string replace(string text) {
            return regex().Replace(text, "${" + groupName() + "}");
        }

        /// <summary>
        /// 配置（OnPopulateMesh 前执行）
        /// </summary>
        /// <param name="cd">字符数据</param>
        /// <param name="frag">片段数据</param>
        public virtual void config(QuestionTextParser.CharData cd,
            FragmentData frag) { configCnt++; }

        /// <summary>
        /// 调整（调整顶点坐标）
        /// </summary>
        /// <param name="cd">字符数据</param>
        /// <param name="frag">片段数据</param>
        public virtual void adjust(QuestionTextParser.CharData cd,
            FragmentData frag) { adjustCnt++; }

        /// <summary>
        /// 处理（进行实际处理）
        /// </summary>
        /// <param name="cd">字符数据</param>
        /// <param name="frag">片段数据</param>
        public virtual void handle(QuestionTextParser.CharData cd,
            FragmentData frag) { handleCnt++; }
    }

    /// <summary>
    /// 内嵌图片处理器
    /// </summary>
    public class QuadImageHandler : TextHandler {

        /// <summary>
        /// 是否作为区块处理
        /// </summary>
        public override bool block() { return true; }

        /// <summary>
        /// Quad标签格式
        /// </summary>
        public const string QuadTagFormat = "<quad size={0} width={1}/>";

        /// <summary>
        /// 图片文本
        /// </summary>
        public const string ImageFormat = "<color=blue>[图片{0}]</color>";

        /// <summary>
        /// 未知图片文本
        /// </summary>
        public const string UnknownImageText = "<color=grey>[未知图片]</color>";

        /// <summary>
        /// 图片前后各拓展的宽度（像素）
        /// </summary>
        public const float widthDelta = 2;

        /// <summary>
        /// 图像对象池
        /// </summary>
        public List<Image> imageObjs = new List<Image>();

        /// <summary>
        /// 图片数量
        /// </summary>
        public int imageCnt = 0;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="textObj">文本对象</param>
        /// <param name="embedImage">是否嵌入显示图片</param>
        public QuadImageHandler(QuestionText textObj) : base(textObj) {}

        /// <summary>
        /// 处理器标签
        /// </summary>
        public override string tag() { return "img"; }

        /// <summary>
        /// 正则表达式文本
        /// </summary>
        /// <returns></returns>
        protected override string regexStr() {
            return @"<img id=(\d+?) size=(\d*\.?\d+%?)(.+?)/>";
        }

        /// <summary>
        /// 是否嵌入图片
        /// </summary>
        /// <returns>返回是否嵌入图片</returns>
        bool embedImage() {
            return textObj.embedImage;
        }

        /// <summary>
        /// 替换（去除标签文本）
        /// </summary>
        /// <param name="match">匹配结果</param>
        /// <returns>替换后字符串</returns>
        public override string replace(Match match) {
            var mw = textObj.rectTransform.rect.width;
            return replace(match, mw);
        }
        /// <param name="maxWidth">最大宽度</param>
        public string replace(Match match, float maxWidth) {
            if (embedImage()) {
                Texture2D texture; float width, height;
                getMatchInfo(match, maxWidth, out texture, out width, out height);

                if (texture == null) return UnknownImageText;

                var eWidth = width + widthDelta * 2;
                var widthRate = eWidth / height;

                return string.Format(QuadTagFormat, height, widthRate);
            } else
                return replace(getTextureIndex(match));
        }
        /// <param name="index">图片索引</param>
        public string replace(int index, bool mensure=false) {
            var res = string.Format(ImageFormat, index+1);
            return mensure ? res : " " + res + " ";
        }

        /// <summary>
        /// 获取图片索引
        /// </summary>
        /// <param name="frag">片段</param>
        /// <returns>返回索引</returns>
        int getTextureIndex(FragmentData frag) {
            var info = (FragmentData.MatchInfo)frag.info;
            return getTextureIndex(info.match);
        }
        /// <param name="match">匹配信息</param>
        int getTextureIndex(Match match) {
            return int.Parse(match.Groups[1].Value);
        }

        /// <summary>
        /// 获取图片尺寸
        /// </summary>
        /// <param name="frag">片段</param>
        /// <returns>返回图片尺寸（宽度）</returns>
        int getTextureSize(FragmentData frag) {
            var info = (FragmentData.MatchInfo)frag.info;
            return getTextureIndex(info.match);
        }
        /// <param name="match">匹配信息</param>
        float getTextureSize(Match match) {
            return float.Parse(match.Groups[2].Value);
        }

        /// <summary>
        /// 获取纹理
        /// </summary>
        /// <param name="frag">片段</param>
        /// <returns>纹理</returns>
        Texture2D getTexture(FragmentData frag) {
            return getTexture(getTextureIndex(frag));
        }
        /// <param name="match">匹配信息</param>
        Texture2D getTexture(Match match) {
            return getTexture(getTextureIndex(match));
        }
        /// <param name="index">索引</param>
        Texture2D getTexture(int index) {
            var textures = textObj.textures;
            if (index < textures.Length) return textures[index];
            return QuestionText.TexturePool.getTexture(index);
        }

        /// <summary>
        /// 获取匹配的具体信息
        /// </summary>
        /// <param name="frag">片段数据</param>
        /// <param name="maxWidth">最大宽度</param>
        /// <param name="texture">纹理</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        void getMatchInfo(FragmentData frag, float maxWidth, 
            out Texture2D texture, out float width, out float height) {
            var info = (FragmentData.MatchInfo)frag.info;
            getMatchInfo(info.match, maxWidth, out texture, out width, out height);
        }
        /// <param name="match">匹配数据</param>
        void getMatchInfo(Match match, float maxWidth,
            out Texture2D texture, out float width, out float height) {
            var size = getTextureSize(match);

            texture = getTexture(match);
            width = Math.Min(maxWidth, size);
            var scale = width / texture.width; // 缩放比率
            height = texture.height * scale; // 根据缩放比率求出实际高度
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public override void initialize() {
            base.initialize();
            initializeImageObjs();
            // initializeImageContainer();
        }

        /// <summary>
        /// 初始化图片对象
        /// </summary>
        void initializeImageObjs() {
            imageCnt = 0;
            imageObjs.RemoveAll(image => image == null);
            // 自动读取
            if (imageObjs.Count == 0)
                textObj.GetComponentsInChildren(imageObjs);

            QuestionText.TestLog("imageObjs.Count = " + imageObjs.Count);
        }

        /*
        /// <summary>
        /// 初始化图片容器
        /// </summary>
        void initializeImageContainer() {
            if (textObj.imageContainer == null) return;
            textObj.imageContainer.setItems(
                QuestionText.TexturePool.getTextures());
        }
        */

        /// <summary>
        /// 清除缓存
        /// </summary>
        public override void clear() {
            base.clear();
            clearUnusedImages();
        }

        /// <summary>
        /// 禁用未使用的图片
        /// </summary>
        void clearUnusedImages() {
            for (int i = imageCnt; i < imageObjs.Count; i++)
                imageObjs[i].gameObject.SetActive(false);
        }

        /// <summary>
        /// 创建默认图片对象
        /// </summary>
        /// <returns></returns>
        GameObject createDefaultImageObject() {
            var resources = new DefaultControls.Resources();
            var obj = DefaultControls.CreateImage(resources);
            textObj.attachImage(obj);
            return obj;
        }
        
        /// <summary>
        /// 创建图片对象
        /// </summary>
        /// <returns>图片对象</returns>
        Image createImageObject() {
            var obj = (textObj.imagePrefab == null ?
                createDefaultImageObject() : 
                textObj.createImagePrefab());
            var img = SceneUtils.image(obj);
            var btn = obj.AddComponent<Button>();
            imageObjs.Add(img);

            return img;
        }

        /// <summary>
        /// 获取指定索引的图片对象
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>图片对象</returns>
        Image getImageObject(int index) {
            QuestionText.TestLog("index = " + index + ", imageObjs.Count = " + imageObjs.Count);
            if (index >= imageObjs.Count) // 如果没有足够的图片对象
                return createImageObject();
            return imageObjs[index];
        }

        /// <summary>
        /// 设置图片纹理
        /// </summary>
        /// <param name="image">图片对象</param>
        /// <param name="texture">纹理</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        void setImageTexture(Image image, Texture2D texture, float width, float height) {
            var rect = new Rect(0, 0, texture.width, texture.height);
            image.color = new Color(1, 1, 1, 1);
            image.overrideSprite = Sprite.Create(
                texture, rect, new Vector2(0.5f, 0.5f));
            image.overrideSprite.name = texture.name;
        }

        /// <summary>
        /// 设置图片链接
        /// </summary>
        /// <param name="image">图片</param>
        /// <param name="index">索引</param>
        Vector2 setImageLink(Image image, int index) {
            var obj = image.gameObject;
            var btn = SceneUtils.button(obj);
            // 隐藏
            image.color = new Color(0.75f, 0.75f, 1, 0.5f);
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => {
                Debug.Log("setImageLink: " + index);
                if (textObj.onImageLinkClick != null)
                    textObj.onImageLinkClick.Invoke(index);
                else if (textObj.imageContainer != null)
                    textObj.imageContainer.select(index);
            }
            );
            return textObj.meansure(replace(index, true));
        }

        /// <summary>
        /// 设置图片RectTransform
        /// </summary>
        /// <param name="rt">变换对象</param>
        void setRectTransform(RectTransform rt, float width, float height) {
            rt.pivot = new Vector2(0, 1); // 左上角
            SceneUtils.setRectWidth(rt, width);
            SceneUtils.setRectHeight(rt, height);
            rt.anchorMin = textObj.rectTransform.pivot;
            rt.anchorMax = textObj.rectTransform.pivot;
        }

        /// <summary>
        /// 设置图片位置
        /// </summary>
        /// <param name="image">图片对象</param>
        /// <param name="cd">字符数据</param>
        void setImagePosition(Image image, UIVertex[] verts) {
            float x = verts[0].position.x, y = verts[0].position.y;
            if (embedImage()) x += widthDelta;

            image.rectTransform.anchoredPosition = new Vector2(x, y);
            image.gameObject.SetActive(true);
        }

        /// <summary>
        /// 预处理
        /// </summary>
        /// <param name="cd">字符数据</param>
        /// <param name="frag">片段数据</param>
        public override void config(QuestionTextParser.CharData cd, FragmentData frag) {
            var image = getImageObject(imageCnt++);
            if (embedImage()) {
                Texture2D texture; float width, height;
                var mw = textObj.rectTransform.rect.width;
                getMatchInfo(frag, mw, out texture, out width, out height);
                setImageTexture(image, texture, width, height);
                setRectTransform(image.rectTransform, width, height);
            } else {
                var index = getTextureIndex(frag);
                var size = setImageLink(image, index);
                setRectTransform(image.rectTransform, size.x, size.y);
            }
            base.config(cd, frag);
        }

        /// <summary>
        /// 处理（进行实际处理）
        /// </summary>
        /// <param name="cd">字符数据</param>
        /// <param name="frag">片段数据</param>
        public override void handle(QuestionTextParser.CharData cd, FragmentData frag) {
            QuestionText.TestLog("handle: "); cd.display();
            var image = getImageObject(handleCnt);
            setImagePosition(image, cd.verts);
            base.handle(cd, frag);
        }
    }

    /// <summary>
    /// 尺寸处理器
    /// </summary>
    public class SizeTextHandler : TextHandler {

        /// <summary>
        /// Size标签格式
        /// </summary>
        public const string SizeTagFormat = "<size={0}>{1}</size>";

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="textObj">文本对象</param>
        public SizeTextHandler(QuestionText textObj) : base(textObj) { }

        /// <summary>
        /// 替换（去除标签文本）
        /// </summary>
        /// <param name="match">匹配结果</param>
        /// <returns>替换后字符串</returns>
        public override string replace(Match match) {
            var fs = textObj.fontSize * QuestionText.FontSizeRate;
            return string.Format(SizeTagFormat, fs, base.replace(match));
            // return regex().Replace(match.Value, "<size=" + fs + ">${" + groupName() + "}</size>");
        }
    }

    /// <summary>
    /// 上标处理器
    /// </summary>
    public class SupTextHandler : SizeTextHandler {

        /// <summary>
        /// 偏移率（对于行高）
        /// </summary>
        const float OffsetRate = 0.4f;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="textObj">文本对象</param>
        public SupTextHandler(QuestionText textObj) : base(textObj) { }

        /// <summary>
        /// 处理器标签
        /// </summary>
        public override string tag() { return "sup"; }

        /// <summary>
        /// 调整（调整顶点坐标）
        /// </summary>
        /// <param name="cd">字符数据</param>
        /// <param name="frag">片段数据</param>
        public override void adjust(QuestionTextParser.CharData cd, FragmentData frag) {
            var offset = textObj.enterHeight * OffsetRate;
            cd.offset(0, offset);
            base.adjust(cd, frag);
        }
    }

    /// <summary>
    /// 下标处理器
    /// </summary>
    public class SubTextHandler : SizeTextHandler {

        /// <summary>
        /// 偏移率（对于行高）
        /// </summary>
        const float OffsetRate = 0.2f;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="textObj">文本对象</param>
        public SubTextHandler(QuestionText textObj) : base(textObj) { }

        /// <summary>
        /// 处理器标签
        /// </summary>
        public override string tag() { return "sub"; }

        /// <summary>
        /// 调整（调整顶点坐标）
        /// </summary>
        /// <param name="cd">字符数据</param>
        /// <param name="frag">片段数据</param>
        public override void adjust(QuestionTextParser.CharData cd, FragmentData frag) {
            var offset = textObj.enterHeight * OffsetRate;
            cd.offset(0, -offset);
            base.adjust(cd, frag);
        }
    }
}