using UnityEngine;
using UnityEngine.UI;

using Core.Systems;
using Core.UI;
using Core.UI.Utils;

using GameModule.Services;

using PlayerModule.Services;

using UI.Common.Controls.InputFields;

/// <summary>
/// 状态场景窗口
/// </summary>
namespace UI.StatusScene.Windows {

    /// <summary>
    /// 信息修改窗口
    /// </summary>
    public class InfoEditWindow : BaseWindow {

        /// <summary>
        /// 文本常量定义
        /// </summary>
        const string InvalidInputAlertText = "请检查输入格式正确后再提交！";

        const string EditSuccessText = "信息修改成功！";

        const string EditNameTitle = "修改昵称";
        const string EditInfoTitle = "修改信息";

        /// <summary>
        /// 类型枚举
        /// </summary>
        public enum Type {
            EditName, EditInfo,
        }

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public TextInputField name, school, city, contact, description;
        public DropdownField grade;
        public DateTimeField birth;

        public StatusWindow statusWindow;

        public Text title;

        /// <summary>
        /// 内部变量声明
        /// </summary>
        Type type;

        /// <summary>
        /// 场景组件引用
        /// </summary>
        StatusScene scene;

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
            scene = (StatusScene)SceneUtils.getSceneObject("Scene");
        }

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            gameSys = GameSystem.get();
            dataSer = DataService.get();
            playerSer = PlayerService.get();
        }

        /// <summary>
        /// 配置子组件
        /// </summary>
        void configureSubViews() {
            var min = dataSer.staticData.configure.minBirth;
            var grades = dataSer.staticData.configure.playerGrades;
            name.check = ValidateService.checkName;
            grade.configure(grades);
            birth.configure(min);
        }

        #endregion

        #region 更新控制

        /// <summary>
        /// 更新
        /// </summary>
        protected override void update() {
            base.update();
            updateLayout();
        }

        /// <summary>
        /// 更新布局
        /// </summary>
        void updateLayout() {
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
        }

        #endregion

        #region 开启控制

        /// <summary>
        /// 开始窗口
        /// </summary>
        public override void startWindow() {
            startWindow(Type.EditName);
        }

        /// <summary>
        /// 开始窗口
        /// </summary>
        public void startWindow(Type type) {
            base.startWindow();
            scene.pushConfirmCallback(onConfirm);
            scene.pushBackCallback(terminateWindow);
            setType(type);
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 切换视图
        /// </summary>
        public void setType(Type type) {
            this.type = type;
            var player = playerSer.player;
            clearInputFields();
            switch (type) {
                case Type.EditName:
                    name.startView(player.name);
                    title.text = EditNameTitle;
                    break;
                case Type.EditInfo:
                    grade.startView(player.grade);
                    birth.startView(player.birth);
                    city.startView(player.city);
                    school.startView(player.school);
                    contact.startView(player.contact);
                    description.startView(player.description);
                    title.text = EditInfoTitle;
                    break;
            }
        }

        #endregion

        #region 界面控制

        /// <summary>
        /// 清除视图
        /// </summary>
        public void clearInputFields() {
            name.terminateView();
            grade.terminateView();
            birth.terminateView();
            school.terminateView();
            city.terminateView();
            contact.terminateView();
            description.terminateView();
        }

        /// <summary>
        /// 清除窗口
        /// </summary>
        protected override void clear() {
            base.clear();
            clearInputFields();
        }

        #endregion

        #region 流程控制

        /// <summary>
        /// 确认
        /// </summary>
        public void onConfirm() {
            if (check()) doEdit();
            else onCheckFailed();
        }

        /// <summary>
        /// 执行修改
        /// </summary>
        void doEdit() {
            switch (type) {
                case Type.EditName: doEditName(); break;
                case Type.EditInfo: doEditInfo(); break;
            }
        }

        /// <summary>
        /// 修改昵称
        /// </summary>
        void doEditName() {
            var name = this.name.getValue();
            if (name != playerSer.player.name)
                playerSer.editName(name, onEditSuccess);
            else onEditSuccess();
        }

        /// <summary>
        /// 修改信息
        /// </summary>
        void doEditInfo() {
            var grade = this.grade.getValue().Item1;
            var birth = this.birth.getValue();
            var school = this.school.getValue();
            var city = this.city.getValue();
            var contact = this.contact.getValue();
            var description = this.description.getValue();
            playerSer.editInfo(grade, birth, school, city,
                contact, description, onEditSuccess);
        }

        /// <summary>
        /// 不正确的格式
        /// </summary>
        void onCheckFailed() {
            gameSys.requestAlert(InvalidInputAlertText);
        }

        /// <summary>
        /// 取消
        /// </summary>
        void onEditSuccess() {
            gameSys.requestAlert(EditSuccessText);
            statusWindow.requestRefresh(true);
            terminateWindow();
        }

        #region 数据校验

        /// <summary>
        /// 检查是否可以登陆
        /// </summary>
        bool check() {
            switch (type) {
                case Type.EditName:
                    return name.isCorrect();
                default: return true;
            }
        }

        #endregion

        #endregion
    }
}