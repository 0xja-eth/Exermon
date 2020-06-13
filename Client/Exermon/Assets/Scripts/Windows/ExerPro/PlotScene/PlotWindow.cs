using UnityEngine;
using Core.UI;
using Core.UI.Utils;
using UI.ExerPro.EnglishPro.PlotScene.Controls;
using ExerPro.EnglishModule.Data;
using ExerPro.EnglishModule.Services;
using Core.Data.Loaders;
using System.IO;

namespace UI.ExerPro.EnglishPro.PlotScene.Windows {
    /// <summary>
    /// 剧情窗口
    /// </summary>
    public class PlotWindow : BaseWindow {
        /// <summary>
        /// 外部变量
        /// </summary>
        public PlotQuestionDisplay questionDisplay;

        /// <summary>
        /// 外部系统
        /// </summary>
        EnglishService engServ;

        /// <summary>
        /// 场景组件引用
        /// </summary>
        protected PlotScene scene;

        /// <summary>
        /// 内部变量
        /// </summary>
        PlotQuestion question;

        #region 初始化
        /// 初始化场景
        /// </summary>
        protected override void initializeScene() {
            scene = SceneUtils.getCurrentScene<PlotScene>();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            engServ = EnglishService.get();
            configureQuestion();
        }

        /// <summary>
        /// 配置题目
        /// </summary>
        void configureQuestion() {
            engServ.generateQuestions<PlotQuestion>(1, onGetQuestionSuccess, onGetQuestionFailed);
            onGetQuestionFailed();
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 当前题目
        /// </summary>
        /// <returns></returns>
        public PlotQuestion currentQuestion() {
            return questionDisplay.getItem();
        }

        #endregion

        #region 回调
        /// <summary>
        /// 获取题目回调
        /// </summary>
        /// <param name="questions"></param>
        void onGetQuestionSuccess(PlotQuestion[] questions) {
            if (questions.Length <= 0) {
                Debug.Log("Plot Quesion get failed!");
                return;
            }
            question = questions[0];
            questionDisplay.startView(question);
        }

        void onGetQuestionFailed() {
            //test
            PlotQuestion testQuestion = generateTestData();
            question = testQuestion;
            questionDisplay.startView(question);
        }
        #endregion

        PlotQuestion generateTestData() {
            LitJson.JsonData jsonData = new LitJson.JsonData();
            jsonData["id"] = 100;
            jsonData["title"] = "你走进一间房间，看见地上有一个大洞。当你靠近洞时，一条巨大的蛇形生物从里面钻了出来。\n\n" +
                "“~嚯嚯嚯！你好，你好啊！这是谁啊？”\n" +
                "你同意吗？";
            jsonData["event_name"] = "蛇";
            var loadPath = System.Environment.CurrentDirectory + "\\Assets\\Sprites\\ExerPro\\PlotScene\\Snake.png";
            jsonData["picture"] = DataLoader.convert(loadPictureHelp(loadPath));

            PlotQuestion.Choice[] tempArray = new PlotQuestion.Choice[5];
            LitJson.JsonData choiceData = new LitJson.JsonData();
            choiceData["text"] = "【同意】 得到 175 金币。被诅咒——疑虑。";
            choiceData["result_text"] = "“对～！\n" +
                "这会很值～得的。\n" +
                "嘶……嘶～嘶……”\n" +
                "蛇抬起头，往上喷出了一堆金币！\n" +
                "这令人震惊又有点可怕。\n" +
                "你把金币收好，谢过蛇后，重新上路。\n";
            PlotQuestion.Choice temp = DataLoader.load<PlotQuestion.Choice>(choiceData);
            tempArray[0] = temp;
            choiceData["text"] = "【拒绝】";
            choiceData["result_text"] = "蛇非常失望地看着你。";
            PlotQuestion.Choice temp1 = DataLoader.load<PlotQuestion.Choice>(choiceData);
            tempArray[1] = temp1;
            choiceData["text"] = "【拒绝】";
            choiceData["result_text"] = "蛇非常失望地看着你。";
            PlotQuestion.Choice temp2 = DataLoader.load<PlotQuestion.Choice>(choiceData);
            tempArray[2] = temp2;
            tempArray[3] = temp2;
            tempArray[4] = temp2;

            jsonData["choices"] = DataLoader.convert(tempArray);

            PlotQuestion testQuestion = DataLoader.load<PlotQuestion>(jsonData);
            return testQuestion;
        }

        Texture2D loadPictureHelp(string fileName) {
            //创建文件读取流
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            fileStream.Seek(0, SeekOrigin.Begin);
            //创建文件长度缓冲区
            byte[] bytes = new byte[fileStream.Length];
            //读取文件
            fileStream.Read(bytes, 0, (int)fileStream.Length);
            //释放文件读取流
            fileStream.Close();
            fileStream.Dispose();
            fileStream = null;

            //创建Texture
            int width = 300;
            int height = 372;
            Texture2D texture = new Texture2D(width, height);
            texture.LoadImage(bytes);
            return texture;
        }
    }
}
