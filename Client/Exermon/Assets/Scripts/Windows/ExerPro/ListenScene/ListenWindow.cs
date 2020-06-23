using UnityEngine;
using Core.UI;
using Core.UI.Utils;
using UI.ExerPro.EnglishPro.ListenScene.Controls;
using ExerPro.EnglishModule.Data;
using ExerPro.EnglishModule.Services;
using Core.Data.Loaders;
using System.IO;
using Core.Systems;
using GameModule.Services;
using System.Text.RegularExpressions;
using QuestionModule.Data;
using Core.Data;
using System.Linq;
using PlayerModule.Services;

namespace UI.ExerPro.EnglishPro.ListenScene.Windows {
    /// <summary>
    /// 剧情窗口
    /// </summary>
    public class ListenWindow : BaseWindow {
        /// <summary>
        /// 外部变量
        /// </summary>
        public ListenQuestionDisplay questionDisplay;

        /// <summary>
        /// 外部系统
        /// </summary>
        EnglishService engServ;

        /// <summary>
        /// 场景组件引用
        /// </summary>
        protected ListenScene scene;

        /// <summary>
        /// 内部变量
        /// </summary>
        ListeningQuestion question;
        ListeningSubQuestion[] questions;
        #region 初始化
        /// 初始化场景
        /// </summary>
        protected override void initializeScene() {
            scene = SceneUtils.getCurrentScene<ListenScene>();
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
            engServ.generateQuestion<ListeningQuestion>(onGetQuestionSuccess, onGetQuestionFailed);
            //onGetQuestionFailed();
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 当前题目
        /// </summary>
        /// <returns></returns>
        public ListeningQuestion currentQuestion() {
            return questionDisplay.getItem();
        }

        #endregion

        #region 回调
        /// <summary>
        /// 获取题目回调
        /// </summary>
        /// <param name="questions"></param>
        void onGetQuestionSuccess(ListeningQuestion remoteQuestion) {
            Debug.Log("onGetQuestionSuccess: " + remoteQuestion.toJson().ToJson());
            question = remoteQuestion;
            questions = question.subQuestions;
            questionDisplay.startView(question);
        }

        void onGetQuestionFailed() {
			Debug.Log("onGetQuestionFailed: ");

			//test
			ListeningQuestion testQuestion = generateTestData();
            question = testQuestion;
            questionDisplay.startView(question);
        }
        #endregion

        ListeningQuestion generateTestData() {
            ListeningQuestion ListeningSample = ListeningQuestion.sample();
            return ListeningSample;
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
