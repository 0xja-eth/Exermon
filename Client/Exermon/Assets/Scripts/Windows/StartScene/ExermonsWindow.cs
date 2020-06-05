using System.Collections.Generic;

using Core.Systems;
using Core.UI;
using Core.UI.Utils;

using GameModule.Services;
using PlayerModule.Services;

using ExermonModule.Data;

using UI.Common.Windows;
using UI.StartScene.Controls.Exermon;

namespace UI.StartScene.Windows {

    /// <summary>
    /// 选择艾瑟萌窗口
    /// </summary>
    public class ExermonsWindow : BaseWindow {

        /// <summary>
        /// 文本常量定义
        /// </summary>
        // const string InvalidInputAlertText = "请检查输入格式正确后再提交！";

        const string InvalidSelectionAlertText = "请从语数英以外的6个艾瑟萌中选择3个作为您的初始艾瑟萌！";
        const string ConfirmTextFormat = "您选择的艾瑟萌为以下科目：{0}，确认？";

        const string CreateSuccessText = "选择艾瑟萌完毕！";

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public ExermonsContainer exermons; // 艾瑟萌集

        /// <summary>
        /// 场景组件引用
        /// </summary>
        StartScene scene;

        /// <summary>
        /// 外部系统引用
        /// </summary>
        GameSystem gameSys = null;
        DataService dataSer = null;
        PlayerService playerSer = null;

        #region 初始化

        /// <summary>
        /// 初次初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            configureSubViews();
        }

        /// <summary>
        /// 初始化场景
        /// </summary>
        protected override void initializeScene() {
            scene = SceneUtils.getCurrentScene<StartScene>();
        }

        /// <summary>
        /// 初始化系统/服务
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            gameSys = GameSystem.get();
            playerSer = PlayerService.get();
            dataSer = DataService.get();
        }

        /// <summary>
        /// 配置子组件
        /// </summary>
        void configureSubViews() {
            var exermons = dataSer.staticData.data.exermons;
            var initExers = new List<Exermon>();
            foreach (var exer in exermons)
                if (exer.eType == 1) initExers.Add(exer);
            this.exermons.configure(initExers);
            this.exermons.select(0);
        }

        #endregion

        #region 流程控制

        /// <summary>
        /// 创建角色
        /// </summary>
        public void create() {
            if (check()) confirmCreate();
            else onCheckFailed();
        }

        /// <summary>
        /// 不正确的格式
        /// </summary>
        void onCheckFailed() {
            gameSys.requestAlert(InvalidSelectionAlertText);
        }

        /// <summary>
        /// 确认
        /// </summary>
        void confirmCreate() {
            var text = generateSelectedSubjectsText();
            gameSys.requestAlert(string.Format(ConfirmTextFormat, text),
                AlertWindow.Type.YesOrNo, doCreate);
        }

        /// <summary>
        /// 生成选择科目文本
        /// </summary>
        /// <returns>文本</returns>
        string generateSelectedSubjectsText() {
            var subjects = new List<string>();
            var results = exermons.getResult();
            foreach (var res in results) subjects.Add(res.subject().name);
            return string.Join(" ", subjects);
        }

        /// <summary>
        /// 执行创建
        /// </summary>
        void doCreate() {
            int[] eids; string[] enames;

            exermons.getResult(out eids, out enames);

            playerSer.createExermons(eids, enames, onCreateSuccess);
        }

        /// <summary>
        /// 选择艾瑟萌成功回调
        /// </summary>
        void onCreateSuccess() {
            gameSys.requestAlert(CreateSuccessText);
            scene.refresh();
        }

        #region 数据校验

        /// <summary>
        /// 检查是否可以登陆
        /// </summary>
        bool check() {
            return exermons.checkSelection();
        }

        #endregion

        #endregion

        #region 界面控制

        #endregion
    }
}