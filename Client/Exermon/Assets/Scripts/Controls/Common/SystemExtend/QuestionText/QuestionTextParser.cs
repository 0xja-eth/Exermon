using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

using Core.Systems;

namespace UI.Common.Controls.SystemExtend.QuestionText {

    /// <summary>
    /// 题目文本组件分析器
    /// </summary>
    public class QuestionTextParser {

        /// <summary>
        /// 过滤的标签
        /// </summary>
        public static string[] filterLabels = new string[] {
            "br", "a", "div", "em", "label", "span", "!--"
        };

        /// <summary>
        /// Unity 内置标签
        /// </summary>
        public static string[] buildInLabels = new string[] {
            "size", "quad", "color"
        };

        /// <summary>
        /// 单个字符数据结构
        /// </summary>
        public class CharData {
            public int index, vIndex;
            public UIVertex[] verts;
            public bool hidden;

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="index">字符索引</param>
            public CharData(int index, bool hidden = false) {
                this.index = index; vIndex = -1;
                this.hidden = hidden;
                verts = new UIVertex[0];
            }

            /// <summary>
            /// 测试
            /// </summary>
            public void display() {
                QuestionText.TestLog("CharData(index: " + index + ", " +
                    ", m: " + midPoint() + ", left: " + left() + ", top: " + top() +
                    ", right: " + right() + ", bottom: " + bottom() + ")");
            }
            public void display(string text) {
                QuestionText.TestLog("CharData(index: " + index + ", char: " + text[index] +
                    ", m: " + midPoint() + ", left: " + left() + ", top: " + top() +
                    ", right: " + right() + ", bottom: " + bottom() + ")");
            }

            /// <summary>
            /// 设置顶点
            /// </summary>
            /// <param name="vIndex">实际顶点索引</param>
            /// <param name="v1">顶点1</param>
            /// <param name="v2">顶点2</param>
            /// <param name="v3">顶点3</param>
            /// <param name="v4">顶点4</param>
            public bool setVertices(int vIndex, UIVertex v1, UIVertex v2,
                UIVertex v3, UIVertex v4, bool force = false) {
                if (!force && v1.position.y == v3.position.y) return false;
                verts = new UIVertex[4] { v1, v2, v3, v4 };
                this.vIndex = vIndex; return true;
            }

            /// <summary>
            /// 左部坐标
            /// </summary>
            /// <returns></returns>
            public float left() { return verts[0].position.x; }

            /// <summary>
            /// 顶部坐标
            /// </summary>
            /// <returns></returns>
            public float top() { return verts[0].position.y; }

            /// <summary>
            /// 右部坐标
            /// </summary>
            /// <returns></returns>
            public float right() { return verts[2].position.x; }

            /// <summary>
            /// 底部坐标
            /// </summary>
            /// <returns></returns>
            public float bottom() { return verts[2].position.y; }

            /// <summary>
            /// 字符尺寸
            /// </summary>
            /// <returns></returns>
            public Vector2 size() { return verts[1].position - verts[3].position; }

            /// <summary>
            /// 中点
            /// </summary>
            /// <returns>中点坐标</returns>
            public Vector3 midPoint() {
                return (verts[0].position + verts[1].position +
                    verts[2].position + verts[3].position) / 4;
            }

            /// <summary>
            /// 偏移
            /// </summary>
            /// <param name="xVal">X偏移量</param>
            /// <param name="yVal">Y偏移量</param>
            public void offset(float xVal, float yVal) {
                verts[0].position += new Vector3(xVal, yVal, 0);
                verts[1].position += new Vector3(xVal, yVal, 0);
                verts[2].position += new Vector3(xVal, yVal, 0);
                verts[3].position += new Vector3(xVal, yVal, 0);
            }
        }

        /// <summary>
        /// 行数据
        /// </summary>
        class LineData {
            public int s, e;
            public float minY, maxY;
            public float offset; // 行高度，偏移量

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="s">开始位置</param>
            /// <param name="e">结束位置</param>
            /// <param name="lineHeight">行高</param>
            public LineData(int s, int e, float minY, float maxY) {
                this.s = s; this.e = e;
                this.minY = minY; this.maxY = maxY;
                offset = 0;
            }

            /// <summary>
            /// 测试
            /// </summary>
            public void display() {
                QuestionText.TestLog("LineData(s: " + s + ", e: " + e +
                    ", minY: " + minY + ", maxY: " + maxY +
                    ", height: " + lineHeight() + ", offset: " + offset + ")");
            }

            /// <summary>
            /// 行高
            /// </summary>
            /// <returns></returns>
            public float lineHeight() { return maxY - minY; }
        }

        /// <summary>
        /// 内部变量
        /// </summary>
        string oriText, resText, meshText;
        float unitsPerPixel;

        /// <summary>
        /// 文本对象
        /// </summary>
        QuestionText textObj;

        List<TextHandler> handlers; // 处理器

        FragmentData fragData; // 片段数据
        List<CharData> charData; // 字符数据
        List<LineData> lineData; // 行数据

        //CharDataPool charData; // 字符数据

        /// <summary>
        /// 构造函数
        /// </summary>
        public QuestionTextParser(QuestionText textObj) {
            this.textObj = textObj;
            unitsPerPixel = textObj.pixelsPerUnit;
            handlers = new List<TextHandler>();
            charData = new List<CharData>();
            lineData = new List<LineData>();
        }

        /// <summary>
        /// 注册处理器
        /// </summary>
        /// <param name="handler">处理器</param>
        public void registerHandler(TextHandler handler) {
            handlers.Add(handler);
        }

        /// <summary>
        /// 初始化（每次分析之前执行）
        /// </summary>
        public void initialize() {
            charData.Clear();
            lineData.Clear();
            initializeHandler();
        }

        /// <summary>
        /// 初始化处理器
        /// </summary>
        void initializeHandler() {
            foreach (var handler in handlers)
                handler.initialize();
        }

        #region 生成顶点前处理

        /// <summary>
        /// 分析文本
        /// </summary>
        /// <param name="text">文本</param>
        public void parseText(string text) {
            initialize();

            oriText = generateOriText(text);
            resText = generateResText(oriText);
            meshText = generateCharData(resText);

            configChars();

            QuestionText.TestLog("oriText: " + oriText);
            QuestionText.TestLog("resText: " + resText);
            QuestionText.TestLog("meshText: " + meshText);
        }

        /// <summary>
        /// 预处理
        /// </summary>
        /// <param name="text">原文本</param>
        /// <returns>处理后文本</returns>
        string generateOriText(string text) {
            // 过滤非法标签
            string regText = "</?(" + string.Join("|", filterLabels) + ").*?/?>";
            text = Regex.Replace(text, regText, string.Empty, RegexOptions.IgnoreCase);
            text = text.Replace("\n\n", "\n" + QuestionText.SpaceEncode + "\n");
            return text;
        }

        /// <summary>
        /// 处理（生成片段数据）
        /// </summary>
        /// <param name="text">原文本</param>
        string generateResText(string text) {
            fragData = new FragmentData(text, 0, oriText.Length);
            string resText = "";

            build(fragData); // 递归构建片段树
            fragData.traverse(ref resText);

            QuestionText.TestLog("====== Processed ======");
            fragData.display();

            return resText;
        }

        /// <summary>
        /// 递归构建片段树
        /// </summary>
        /// <param name="parent">父片段</param>
        void build(FragmentData parent) {
            var matchInfo = new List<FragmentData.MatchInfo>();
            var text = parent.text;

            // 进行正则表达式匹配，找到每个标签项的范围
            for (int i = 0; i < handlers.Count; ++i) {
                var handler = handlers[i];
                MatchCollection tmpMatches = handler.matches(text);
                foreach (Match match in tmpMatches)
                    matchInfo.Add(new FragmentData.MatchInfo(match, handler));
            }
            if (matchInfo.Count <= 0) return; // 如果没有特殊标签，直接返回

            QuestionText.TestLog("====== Matched ======");
            foreach (var m in matchInfo) m.display();

            matchInfo.Sort();

            int ls = -1, le = -1; // 上一个（有记录的） s 和 e
            int ms, me; // 每个匹配项的 s 和 e
            var lastInfo = matchInfo[0]; // 上一个匹配项
            string subText;

            // 遍历每个匹配项
            //for (int i = mi; i < mj; ++i) {
            foreach (var info in matchInfo) {
                ms = info.start(); me = info.end();
                if (ls == -1 && le == -1) { // 如果是第一个匹配项（0...ms[.....）

                    // 该匹配项之前的文本均为普通文本，建立叶子结点
                    subText = text.Substring(0, ms);
                    var child = new FragmentData(subText, 0, ms);
                    parent.children.Add(child);

                    lastInfo = info;
                    ls = ms; le = me; // 记录当前匹配项数据
                /* } else if (ms < le && me > le) { // 当两个匹配项重叠的时候（ls[..ms[..]le..]me..）
                    // 这时候只有当两个标签相同才处理（因为正则表达式无法处理内嵌的情况，在这里进行处理）
                    if (info.handler.tag() == lastInfo.handler.tag()) {
                        // 只生成一段：ls - me
                        subText = text.Substring(ls, me - ls);
                        QuestionText.TestLog("SubText = " + subText);
                        var child = new FragmentData(subText, ls, me, lastInfo);
                        parent.children.Add(child); // 添加子树
                        build(child); // 递归建树

                        le = me;
                    } */
                }
                if (ms >= le) { // 当当前匹配项起始位置大于已有匹配项的末位置（ls[...]le..ms[...）

                    // 建立上一个匹配项的子节点
                    var child1 = new FragmentData(ls, le, lastInfo);
                    parent.children.Add(child1); // 添加子树
                    build(child1); // 递归建树

                    // 建立两个匹配项之间的子节点
                    subText = text.Substring(le, ms - le);
                    var child2 = new FragmentData(subText, le, ms);
                    parent.children.Add(child2); // 添加子树

                    lastInfo = info;
                    ls = ms; le = me; // 记录当前匹配项数据
                }
            }

            // 处理最后一个匹配项（ls[....]le....text.Length）
            // 建立最后一个匹配项的子节点
            var lastChild1 = new FragmentData(ls, le, lastInfo);
            parent.children.Add(lastChild1); // 添加子树
            build(lastChild1); // 递归建树

            // 建立到文本末尾的子节点
            subText = text.Substring(le, text.Length - le);
            var lastChild2 = new FragmentData(subText, le, text.Length);
            parent.children.Add(lastChild2); // 添加子树
        }

        /// <summary>
        /// 再处理（生成 charData ）
        /// </summary>
        /// <param name="text">待处理文本</param>
        /// <returns>结果文本</returns>
        string generateCharData(string text) {
            // 过滤内置标签
            int start = 0;

            string regText = @"(</?(" + string.Join("|", buildInLabels) + ").*?/?>| |\n)";

            foreach (Match match in Regex.Matches(text, regText, RegexOptions.Singleline)) {
                for (int i = start; i < match.Index; ++i)
                    charData.Add(new CharData(i));

                // 如果是quad，需要新增一个占位符（隐藏）
                if (match.Groups[2].Value == "quad")
                    charData.Add(new CharData(match.Index, true));

                start = match.Index + match.Length;
            }
            for (int i = start; i < text.Length; ++i)
                charData.Add(new CharData(i));

            foreach (var cd in charData) fragData.addCharData(cd);

            text = Regex.Replace(text, regText, string.Empty, RegexOptions.IgnoreCase);

            return text;
        }

        /// <summary>
        /// 获取分析处理后的文本
        /// </summary>
        /// <returns></returns>
        public string processedText() { return resText; }

        /// <summary>
        /// 获取最佳高度
        /// </summary>
        /// <returns></returns>
        public Vector2 getPreferredSize() {
            return textObj.meansure(resText);
        }

        #endregion

        /// <summary>
        /// 绑定顶点
        /// </summary>
        /// <returns>生成是否成功</returns>
        public List<UIVertex> parseVertices(float lineHeight, IList<UIVertex> verts) {
            generateCharData(verts); generateLineData();

            adjustChars(); adjustLines(lineHeight); handleChars();

            return generateResult();
        }

        /// <summary>
        /// 生成字符数据
        /// </summary>
        void generateCharData(IList<UIVertex> verts) {
            var vcnt = verts.Count;

            QuestionText.TestLog("resText.Length: " + resText.Length);
            QuestionText.TestLog("meshText.Length: " + meshText.Length);
            QuestionText.TestLog("charData.Count: " + charData.Count);
            QuestionText.TestLog("vcnt: " + vcnt);

            var cdIndex = 0;

            for (int i = 0; i < vcnt / 4 &&
                cdIndex < charData.Count; ++i) {
                var cd = charData[cdIndex];
                var v1 = verts[i * 4];
                var v2 = verts[i * 4 + 1];
                var v3 = verts[i * 4 + 2];
                var v4 = verts[i * 4 + 3];

                QuestionText.TestLog("generateCharData: " + i + ", cdIndex: " + cdIndex
                    + "\nverts: " + v1.position + "," + v2.position
                    + "," + v3.position + "," + v4.position);

                if (cd.setVertices(i, v1, v2, v3, v4)) cdIndex++;
            }

            // 补全剩余数据
            for (int i = cdIndex; i < charData.Count; i++) {
                var cd = charData[i];
                var vIndex = vcnt / 4 - 1;
                var vert = verts[vcnt - 2];
                cd.setVertices(vIndex, vert, vert, vert, vert, true);
            }

            QuestionText.TestLog("====== GeneratedChars: " + charData.Count + " ======");
            foreach (var c in charData) c.display(resText);
        }

        /// <summary>
        /// 生成行数据
        /// </summary>
        /// <returns>生成是否成功</returns>
        void generateLineData() {
            if (charData.Count <= 0) return;
            var lcd = charData[0]; // 上一个数据
            int ls = 0; // 记录本行起始位置
            float minY = lcd.bottom(), maxY = lcd.top(); // 每一行最低点和最高点

            for (int i = 1; i < charData.Count; ++i) {
                var cd = charData[i];
                // 如果产生换行
                if (lcd.left() - cd.left() > -lcd.size().x / 2 &&
                    (lcd.top() > cd.top() || lcd.bottom() > cd.bottom())) {
                    lineData.Add(new LineData(ls, i, minY, maxY));
                    ls = i; // 设置下一行起始位置，重置最高点最低点
                    minY = cd.bottom(); maxY = cd.top();
                } else {
                    minY = Math.Min(minY, cd.bottom());
                    maxY = Math.Max(maxY, cd.top());
                }
                lcd = cd;
            }
            lineData.Add(new LineData(ls, charData.Count, minY, maxY));

            QuestionText.TestLog("====== GeneratedLines ======");
            foreach (var line in lineData) line.display();
        }

        /// <summary>
        /// 预处理每个字符数据（OnPopulateMesh 前执行）
        /// </summary>
        void configChars() {
            fragData.configChars();
        }

        /// <summary>
        /// 调整每个字符数据（可能需要修改顶点坐标）
        /// </summary>
        void adjustChars() {
            fragData.adjustChars();

            QuestionText.TestLog("====== AdjustedChars ======");
            foreach (var c in charData) c.display(resText);
        }

        /// <summary>
        /// 调整每一行
        /// </summary>
        void adjustLines(float lineHeight) {
            float resY = 0; // 总偏移量，实际每行的Y值（最高点）
            for (int i = 0; i < lineData.Count; i++) {
                var line = lineData[i];
                //offset += line.maxY - resY;
                processOffset(line, line.maxY - resY);
                resY -= Mathf.Max(lineHeight, line.lineHeight());
            }

            QuestionText.TestLog("====== AdjustedLines ======");
            foreach (var line in lineData) line.display();
        }

        /// <summary>
        /// 处理偏移
        /// </summary>
        /// <param name="line">行</param>
        void processOffset(LineData line, float offset) {
            line.offset = offset;
            line.maxY -= offset; line.minY -= offset;
            for (int i = line.s; i < line.e; ++i) {
                var cd = charData[i];
                cd.offset(0, -offset);
            }
        }

        /// <summary>
        /// 处理每个字符数据（需要特殊处理）
        /// </summary>
        void handleChars() {
            fragData.handleChars();
        }

        /// <summary>
        /// 生成结果
        /// </summary>
        /// <returns></returns>
        List<UIVertex> generateResult() {
            int vi = 0, lvi = 0;
            List<UIVertex> res = new List<UIVertex>();

            Vector2 min = new Vector2(9999, 9999);
            Vector2 max = new Vector2(-9999, -9999);

            foreach (var cd in charData) {
                vi = cd.vIndex;
                // 填补多余的空节点
                for (int i = 0; i < vi - lvi - 1; ++i)
                    addEmptyVertices(res, cd);

                // 如果需要隐藏，置为空节点
                if (cd.hidden) addEmptyVertices(res, cd);
                else res.AddRange(cd.verts);
                lvi = vi;

                if (cd.left() < min.x) min.x = cd.left();
                if (cd.right() > max.x) max.x = cd.right();
                if (cd.bottom() < min.y) min.y = cd.bottom();
                if (cd.top() > max.y) max.y = cd.top();
            }

            textObj.perfectSize = new Vector2(max.x - min.x, max.y - min.y);

            QuestionText.TestLog("====== GeneratedRes: " + res.Count + " ======");
            QuestionText.TestLog("min: " + min + ", max: " + max +
                ", size: " + textObj.perfectSize);

            var vcnt = res.Count;

            for (int i = 0; i < vcnt / 4; ++i) {
                //var c = meshText[i];
                //var cd = charData[i];
                var v1 = res[i * 4];
                var v2 = res[i * 4 + 1];
                var v3 = res[i * 4 + 2];
                var v4 = res[i * 4 + 3];
                //cd.setVertices(v1, v2, v3, v4);
                QuestionText.TestLog("res: " + i
                    + "\nverts: " + v1.position + "," + v2.position
                    + "," + v3.position + "," + v4.position);
            }

            return res;
        }

        /// <summary>
        /// 增加空节点（4个）
        /// </summary>
        /// <param name="res">节点数组</param>
        /// <param name="cd">字符数据</param>
        static void addEmptyVertices(List<UIVertex> res, CharData cd) {
            res.Add(cd.verts[2]); res.Add(cd.verts[2]);
            res.Add(cd.verts[2]); res.Add(cd.verts[2]);
        }

        /// <summary>
        /// 清除缓存
        /// </summary>
        public void clear() {
            clearHandlers();
        }

        /// <summary>
        /// 清空所有处理器缓存
        /// </summary>
        void clearHandlers() {
            foreach (var handler in handlers) handler.clear();
        }
    }

    /// <summary>
    /// 片段数据（片段树的结点）
    /// </summary>
    public class FragmentData {

        /// <summary>
        /// 匹配信息
        /// </summary>
        public struct MatchInfo : IComparable<MatchInfo> {
            public Match match;
            public TextHandler handler;

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="match">匹配项</param>
            /// <param name="handler">处理器</param>
            public MatchInfo(Match match, TextHandler handler) {
                this.match = match; this.handler = handler;
            }

            /// <summary>
            /// 测试用
            /// </summary>
            public void display() {
                QuestionText.TestLog("MatchInfo(index: " + match.Index + ", match:" + match.Value +
                    ", replaced:" + handler.replace(match) + ", handler:" + handler.tag() + ")");
            }

            /// <summary>
            /// 比较函数
            /// </summary>
            /// <param name="b">另一实例</param>
            /// <returns>大小关系</returns>
            public int CompareTo(MatchInfo b) {
                return start().CompareTo(b.start());
            }

            /// <summary>
            /// 开始位置
            /// </summary>
            /// <returns></returns>
            public int start() { return match.Index; }

            /// <summary>
            /// 长度
            /// </summary>
            /// <returns></returns>
            public int len() { return match.Length; }

            /// <summary>
            /// 结束位置
            /// </summary>
            /// <returns></returns>
            public int end() { return start() + len(); }
        }

        /// <summary>
        /// 数据
        /// </summary>
        public MatchInfo? info; // 匹配信息
        public string text;
        public int s, e; // 起点和终点（对于父节点的 text）
        public int rs, re; // 起点和终点（对于最终生成的 resText）
        public List<FragmentData> children; // 子树
        public List<QuestionTextParser.CharData> charData; // 字符数据

        /// <summary>
        /// 构造函数
        /// </summary>
        public FragmentData(string text, int s, int e) {
            this.text = text;
            this.s = s; this.e = e;
            children = new List<FragmentData>();
            charData = new List<QuestionTextParser.CharData>();
            info = null; rs = re = -1;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public FragmentData(int s, int e, MatchInfo info) {
            var handler = info.handler;
            text = handler.replace(info.match);
            this.s = s; this.e = e; this.info = info;
            children = new List<FragmentData>();
            charData = new List<QuestionTextParser.CharData>();
            rs = re = -1;
        }
        /*
        /// <summary>
        /// 构造函数
        /// </summary>
        public FragmentData(string text, int s, int e, MatchInfo info) {
            var handler = info.handler;
            this.text = handler.maxReplace(text);
            this.s = s; this.e = e; this.info = info;
            children = new List<FragmentData>();
            charData = new List<QuestionTextParser.CharData>();
            rs = re = -1;
        }
        */
        /// <summary>
        /// 测试用
        /// </summary>
        public void display() {
            QuestionText.TestLog("FragmentData(text: " + text + ", s: " + s + ", e: " + e +
                ", rs: " + rs + ", re: " + re + ")");
            info?.display();
            for (int i = 0; i < children.Count; ++i)
                children[i].display();
        }

        /// <summary>
        /// 遍历
        /// </summary>
        public void traverse(ref string resText) {
            rs = resText.Length;
            if (isLeaf()) // 如果是子节点
                resText += text;
            else for (int i = 0; i < children.Count; ++i)
                    children[i].traverse(ref resText);
            re = resText.Length;
        }

        /// <summary>
        /// 是否叶子结点
        /// </summary>
        /// <returns></returns>
        public bool isLeaf() {
            return children.Count <= 0;
        }

        /// <summary>
        /// 指定坐标是否在段内
        /// </summary>
        /// <param name="index">坐标</param>
        /// <returns></returns>
        public bool inside(int index) {
            return rs <= index && index < re;
        }

        /// <summary>
        /// 获取片段的尺寸
        /// </summary>
        /// <returns></returns>
        public Vector2 size(QuestionText text) {
            return text.meansure(this.text);
        }

        /// <summary>
        /// 增加一个字符数据
        /// </summary>
        /// <param name="cd">字符数据</param>
        public void addCharData(QuestionTextParser.CharData cd) {
            charData.Add(cd);
            foreach (var child in children)
                if (child.inside(cd.index)) child.addCharData(cd);
        }

        /// <summary>
        /// 预处理字符数据
        /// </summary>
        public void configChars() {
            var handler = info?.handler;
            if (handler != null)
                foreach (var cd in charData) {
                    handler.config(cd, this);
                    if (handler.block()) break;
                }
            foreach (var c in children)
                c.configChars();
        }

        /// <summary>
        /// 调整字符数据
        /// </summary>
        public void adjustChars() {
            var handler = info?.handler;
            if (handler != null)
                foreach (var cd in charData) {
                    handler.adjust(cd, this);
                    if (handler.block()) break;
                }
            foreach (var c in children)
                c.adjustChars();
        }

        /// <summary>
        /// 处理字符数据
        /// </summary>
        public void handleChars() {
            var handler = info?.handler;
            if (handler != null)
                foreach (var cd in charData) {
                    handler.handle(cd, this);
                    if (handler.block()) break;
                }
            foreach (var c in children)
                c.handleChars();
        }
    }

    /*
    /// <summary>
    /// 字符数据池
    /// </summary>
    public class CharDataPool {


        /// <summary>
        /// 数据
        /// </summary>
        List<CharData> charData; // 字符数据

        /// <summary>
        /// 绑定顶点
        /// </summary>
        public void bindVertices(string text, IList<UIVertex> verts) {
            // Last 4 verts are always a new line... (\n)
            var vcnt = verts.Count;// - 4;
            charData = new List<CharData>();

            QuestionText.TestLog("bindVertices: " + text);
            QuestionText.TestLog("text.Length = " + text.Length);
            QuestionText.TestLog("verts.Count = " + verts.Count);

            for (int i = 0; i < vcnt / 4; ++i) {
                var c = text[i];
                var v1 = verts[i * 4];
                var v2 = verts[i * 4 + 1];
                var v3 = verts[i * 4 + 2];
                var v4 = verts[i * 4 + 3];
                var cd = new CharData(c, v1, v2, v3, v4);
                charData.Add(cd);

                QuestionText.TestLog(i + ":" + c + ": mid: " + cd.midPoint()
                     + "\nv1:" + v1.position + "\nv2:" + v2.position
                     + "\nv3:" + v3.position + "\nv4:" + v4.position);
            }
        }

        /// <summary>
        /// 分析行数据
        /// </summary>
        public void parseLines() {

        }
    }
    */
}