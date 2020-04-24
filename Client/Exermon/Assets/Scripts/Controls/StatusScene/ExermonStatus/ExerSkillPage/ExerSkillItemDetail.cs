
using UnityEngine;
using UnityEngine.UI;

using ExermonModule.Data;

using UI.Common.Controls.ItemDisplays;
using UI.Common.Controls.ParamDisplays;

/// <summary>
/// 状态场景中艾瑟萌状态-技能页控件
/// </summary>
namespace UI.StatusScene.Controls.ExermonStatus.ExerSkillPage {

    /// <summary>
    /// 状态窗口艾瑟萌页属性信息显示
    /// </summary>
    public class ExerSkillItemDetail : SelectableItemDisplay<ExerSkillSlotItem> {

        /// <summary>
        /// 文本常量定义
        /// </summary>
        const string UseCountTextFormat = "使用次数：{0}/{1}";

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public GameObject content, empty;

        public Image icon;

        public Text name, useCount;
        public GameObject useCountFrame;
        public GameObject positive, negative;

        public MultParamsDisplay detailInfo;

        #region 界面绘制

        /// <summary>
        /// 绘制主体信息
        /// </summary>
        /// <param name="slotItem">艾瑟萌槽项</param>
        protected override void drawExactlyItem(ExerSkillSlotItem skillItem) {
            base.drawExactlyItem(skillItem);
            if (skillItem.isNullItem()) 
                drawEmptyItem();  
            else {
                empty.SetActive(false);
                content.SetActive(true);
                drawIconImage(skillItem);
                drawDetailInfo(skillItem);
                drawTypeInfo(skillItem);
            }
        }

        /// <summary>
        /// 绘制艾瑟萌图像
        /// </summary>
        /// <param name="slotItem">艾瑟萌槽项</param>
        void drawIconImage(ExerSkillSlotItem skillItem) {
            var skill = skillItem.skill();
            var icon = skill.icon;
            var rect = new Rect(0, 0, icon.width, icon.height);
            this.icon.gameObject.SetActive(true);
            this.icon.overrideSprite = Sprite.Create(
                icon, rect, new Vector2(0.5f, 0.5f));
            this.icon.overrideSprite.name = icon.name;

            if (name) name.text = skill.name;

            useCountFrame?.SetActive(!skill.passive);
            if (!skill.passive && useCount) useCount.text = string.Format(
                UseCountTextFormat, skillItem.useCount, skill.needCount);
        }

        /// <summary>
        /// 绘制基本信息
        /// </summary>
        /// <param name="slotItem">艾瑟萌槽项</param>
        void drawDetailInfo(ExerSkillSlotItem skillItem) {
            detailInfo.setValue(item.skill());
        }

        /// <summary>
        /// 绘制基本信息
        /// </summary>
        /// <param name="slotItem">艾瑟萌槽项</param>
        void drawTypeInfo(ExerSkillSlotItem skillItem) {
            var skill = skillItem.skill();
            negative.SetActive(skill.passive);
            positive.SetActive(!skill.passive);
        }

        /// <summary>
        /// 绘制空物品
        /// </summary>
        protected override void drawEmptyItem() {
            empty.SetActive(true);
            content.SetActive(false);
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void clearItem() {
            empty.SetActive(false);
            content.SetActive(false);
        }

        #endregion
    }
}