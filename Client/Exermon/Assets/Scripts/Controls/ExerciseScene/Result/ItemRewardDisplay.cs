
using UnityEngine.UI;

using PlayerModule.Data;
using ExermonModule.Data;
using QuestionModule.Data;
using RecordModule.Data;

using UI.Common.Controls.ItemDisplays;

/// <summary>
/// 刷题场景结算界面控件
/// </summary>
namespace UI.ExerciseScene.Controls.Result {

    /// <summary>
    /// 对战物资槽物品显示
    /// </summary
    public class ItemRewardDisplay : ItemDisplay<QuestionSetReward> {

        /// <summary>
        /// 常量定义
        /// </summary>
        const string FreezeFormat = "冻结回合：{0}";

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Text name, count;
        public Image icon; // 图片

        public BaseItemDisplay itemDisplay;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            initializeItemDisplay();
        }

        /// <summary>
        /// 初始化道具显示控件
        /// </summary>
        void initializeItemDisplay() {
            itemDisplay.registerItemType<HumanItem>(drawHumanItem);
            itemDisplay.registerItemType<ExerEquip>(drawExerEquip);
            itemDisplay.registerItemType<QuesSugar>(drawQuesSugar);
        }

        #endregion

        #region 界面控制

        /// <summary>
        /// 绘制确切物品
        /// </summary>
        /// <param name="item">运行时物品</param>
        protected override void drawExactlyItem(QuestionSetReward item) {
            base.drawExactlyItem(item);
            itemDisplay.setItem(item.item());
            count.text = item.count.ToString();
        }

        /// <summary>
        /// 绘制人类物品
        /// </summary>
        /// <param name="item"></param>
        void drawHumanItem(HumanItem item) {
            name.text = item.name;
            icon.gameObject.SetActive(true);
            icon.overrideSprite = item.icon;
        }

        /// <summary>
        /// 绘制艾瑟萌装备
        /// </summary>
        /// <param name="item"></param>
        void drawExerEquip(ExerEquip item) {
            name.text = item.name;
            icon.gameObject.SetActive(true);
            icon.overrideSprite = item.icon;
        }

        /// <summary>
        /// 绘制题目糖
        /// </summary>
        /// <param name="item"></param>
        void drawQuesSugar(QuesSugar item) {
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void clearItem() {
            name.text = count.text = "";
            icon.gameObject.SetActive(false);
            itemDisplay.requestClear(true);
        }

        #endregion
    }
}