
using ItemModule.Data;
using RecordModule.Data;

using UI.Common.Controls.ItemDisplays;

namespace UI.ExerciseScene.Controls.Result {

    /// <summary>
    /// 物品奖励容器显示
    /// </summary>
    public class ItemRewardContainer : 
        ContainerDisplay<QuestionSetReward> {

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            registerUpdateLayout(container);
        }

        #endregion

        #region 数据控制
        
        #endregion
    }
}