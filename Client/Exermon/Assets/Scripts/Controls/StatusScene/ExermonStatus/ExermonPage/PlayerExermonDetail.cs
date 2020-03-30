
using UnityEngine;
using UnityEngine.UI;

using ExermonModule.Data;
using ExermonModule.Services;

using UI.Common.Controls.ParamDisplays;
using UI.Common.Controls.InputFields;

namespace UI.StatusScene.Controls.ExermonStatus.ExermonPage {

    /// <summary>
    /// 状态窗口艾瑟萌页属性信息显示
    /// </summary>
    public class PlayerExermonDetail : ExermonStatusExerSlotDetail<PlayerExermon> {

        /// <summary>
        /// 文本常量定义
        /// </summary>
        const string AnimalTextFormat = "物种：{0}";
        const string TypeTextFormat = "类型：{0}";

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Image full;
        public StarsDisplay stars;

        public Text animal, type, nickname, description;
        public TextInputField nicknameInput;

        public ParamDisplay expBar;

        public GameObject equipedFlag, editButton;

        /// <summary>
        /// 外部系统设置
        /// </summary>
        ExermonService exerSer;

        #region 界面绘制

        /// <summary>
        /// 绘制主体信息
        /// </summary>
        /// <param name="slotItem">艾瑟萌槽项</param>
        protected override void drawMainInfo(PlayerExermon playerExer) {
            base.drawMainInfo(playerExer);
            drawFullImage(playerExer);
            drawBaseInfo(playerExer);
            drawExpInfo(playerExer);
        }

        /// <summary>
        /// 绘制艾瑟萌图像
        /// </summary>
        /// <param name="slotItem">艾瑟萌槽项</param>
        void drawFullImage(PlayerExermon playerExer) {
            var exermon = playerExer.exermon();
            var full = exermon.full;
            var rect = new Rect(0, 0, full.width, full.height);
            this.full.gameObject.SetActive(true);
            this.full.overrideSprite = Sprite.Create(
                full, rect, new Vector2(0.5f, 0.5f));
            this.full.overrideSprite.name = full.name;

            stars.setValue(exermon.starId);
            equipedFlag?.SetActive(playerExer.equiped);
        }

        /// <summary>
        /// 绘制基本信息
        /// </summary>
        /// <param name="slotItem">艾瑟萌槽项</param>
        void drawBaseInfo(PlayerExermon playerExer) {
            var exermon = playerExer.exermon();
            animal.text = string.Format(AnimalTextFormat, exermon.animal);
            type.text = string.Format(TypeTextFormat, exermon.typeText());
            nickname.text = playerExer.name();
            description.text = exermon.description;
        }

        /// <summary>
        /// 绘制经验信息
        /// </summary>
        /// <param name="slotItem">艾瑟萌槽项</param>
        void drawExpInfo(PlayerExermon playerExer) {
            expBar.setValue(playerExer, "exp");
        }

        /// <summary>
        /// 绘制纯物品属性数据
        /// </summary>
        /// <param name="contItem"></param>
        protected override void drawContItemParamsInfo(PlayerExermon playerExer) {
            paramInfo?.setValues(playerExer, "params");
            battlePoint?.setValue(playerExer, "battle_point");
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void clearMainInfo() {
            base.clearMainInfo();
            animal.text = type.text = "";
            nickname.text = description.text = "";
            full.gameObject.SetActive(false);
            expBar.clearValue();
            stars.clearValue();
            equipedFlag?.SetActive(false);
        }

        #endregion

        #region 流程控制

        /// <summary>
        /// 修改昵称点击回调
        /// </summary>
        public void onEditClick() {
            nickname.enabled = false;
            editButton.SetActive(false);
            nicknameInput.startView(item.name());
            nicknameInput.activate();
        }

        /// <summary>
        /// 结束昵称输入
        /// </summary>
        void terminateNicknameInput() {
            nickname.enabled = true;
            editButton.SetActive(true);
            nicknameInput.terminateView();
            requestRefresh();
        }

        /// <summary>
        /// 昵称改变回调事件
        /// </summary>
        public void onNicknameChanged(string value) {
            var name = item.name();
            requestRename(value == "" ? name : value);
        }

        #endregion

        #region 请求控制

        /// <summary>
        /// 请求更改昵称
        /// </summary>
        void requestRename(string nickname) {
            exerSer.editNickname(item, nickname, terminateNicknameInput);
        }

        #endregion
    }
}