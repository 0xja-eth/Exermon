
using UnityEngine;
using UnityEngine.UI;

using GameModule.Services;

using UI.Common.Controls.InputFields;
using UI.Common.Controls.ItemDisplays;
using UI.Common.Controls.ParamDisplays;

/// <summary>
/// 开始场景中艾瑟萌窗口控件
/// </summary>
namespace UI.StartScene.Controls.Exermon {

    using ExermonModule.Data;

    /// <summary>
    /// 艾瑟萌详细信息
    /// </summary>
    public class ExermonDetail : ItemDetailDisplay<Exermon> {

        /// <summary>
        /// 常量设置
        /// </summary>

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Image full;
        public Text name, subject, animal;
        public ParamDisplaysGroup paramsView;

        public TextInputField nicknameInput;
        public GameObject oriNameView;

        /// <summary>
        /// 内部变量设置
        /// </summary>
        string nickname;

        #region 初始化

        /// <summary>
        /// 初次打开时初始化（子类中重载）
        /// </summary>
        protected override void initializeOnce() {
            nicknameInput.onChanged = onNicknameChanged;
            setupParamsView();
        }

        /// <summary>
        /// 配置属性组
        /// </summary>
        void setupParamsView() {
            var params_ = DataService.get().staticData.configure.baseParams;
            paramsView.configure(params_);
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 物品改变回调
        /// </summary>
        protected override void onItemChanged() {
            nickname = item.name;
            base.onItemChanged();
        }

        /// <summary>
        /// 获取容器
        /// </summary>
        /// <returns></returns>
        public new ExermonsContainer getContainer() {
            return base.getContainer() as ExermonsContainer;
        }

        #endregion

        #region 界面控制

        /// <summary>
        /// 绘制确切的物品
        /// </summary>
        /// <param name="exermon">物品</param>
        protected override void drawExactlyItem(Exermon exermon) {
            completeNicknameText();
            drawInfoView(exermon);
            drawParamsView(exermon);
        }

        /// <summary>
        /// 绘制全身像
        /// </summary>
        /// <param name="exermon">物品</param>
        void drawFull(Exermon exermon) {
            var full = exermon.full;
            var rect = new Rect(0, 0, full.width, full.height);
            this.full.overrideSprite = Sprite.Create(
                full, rect, new Vector2(0.5f, 0.5f));
            this.full.overrideSprite.name = full.name;
        }

        /// <summary>
        /// 绘制基本信息
        /// </summary>
        /// <param name="exermon">物品</param>
        void drawInfoView(Exermon exermon) {
            drawFull(exermon);

            var starText = exermon.star().name;
            var typeText = exermon.typeText();

            name.text = nickname;
            subject.text = exermon.subject().name;
            animal.text = exermon.animal;
        }

        /// <summary>
        /// 绘制属性信息
        /// </summary>
        /// <param name="exermon">物品</param>
        void drawParamsView(Exermon exermon) {
            paramsView.setValues(exermon);
        }

        /// <summary>
        /// 自动读取设定好的昵称
        /// </summary>
        void completeNicknameText() {
            var container = getContainer();
            nickname = container.getNickname(index);
            nicknameInput.setValue(nickname, false, false);
        }

        /// <summary>
        /// 清除信息视图
        /// </summary>
        void clearInfoView() {
            full.overrideSprite = null;
            name.text = subject.text = animal.text = "";
        }

        /// <summary>
        /// 清除属性信息
        /// </summary>
        void clearParamsView() {
            paramsView.clearValues();
        }

        /// <summary>
        /// 清空昵称输入
        /// </summary>
        void clearNicknameText() {
            nicknameInput.setValue("", false, false);
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void clearItem() {
            clearInfoView();
            clearParamsView();
            clearNicknameText();
        }

        #endregion

        #region 流程控制

        /// <summary>
        /// 开始昵称输入
        /// </summary>
        public void startNicknameInput() {
            oriNameView.SetActive(false);
            nicknameInput.setValue(nickname, false, false);
            nicknameInput.startView();
            nicknameInput.activate();
        }

        /// <summary>
        /// 结束昵称输入
        /// </summary>
        void terminateNicknameInput() {
            oriNameView.SetActive(true);
            nicknameInput.terminateView();
            requestRefresh();
        }

        /// <summary>
        /// 昵称改变回调事件
        /// </summary>
        public void onNicknameChanged(string value) {
            nickname = (value == "" ? item.name : value);
            getContainer().changeNickname(index, nickname);
            terminateNicknameInput();
        }

        #endregion
    }
}