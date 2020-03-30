
using PlayerModule.Data;
using ItemModule.Data;

namespace UI.Common.Controls.ItemDisplays {

    /// <summary>
    /// 人类背包显示
    /// </summary>
    public class HumanPackDisplay : PackContainerDisplay<PackContItem> {

        /// <summary>
        /// 常量设置
        /// </summary>

        /// <summary>
        /// 外部组件设置
        /// </summary>

        /// <summary>
        /// 内部变量声明
        /// </summary>

        #region 数据控制

        /// <summary>
        /// 是否需要判断具体的类型
        /// </summary>
        /// <returns></returns>
        protected override bool isNeedJudgeType() {
            return true;
        }

        /// <summary>
        /// 可接受的类型列表
        /// </summary>
        /// <returns>返回可接受的物品类型列表</returns>
        protected override BaseContItem.Type[] acceptableTypes() {
            return new BaseContItem.Type[] {
                BaseContItem.Type.HumanPackItem,
                BaseContItem.Type.HumanPackEquip,
            };
        }

        /// <summary>
        /// 是否包含物品
        /// </summary>
        /// <param name="packItem">物品</param>
        /// <param name="type">指定的类型</param>
        /// <returns>返回指定物品能否包含在容器中</returns>
        protected override bool isIncluded(PackContItem packItem, BaseContItem.Type type) {
            switch(type) {
                case BaseContItem.Type.HumanPackItem:
                    return isItemIncluded((HumanPackItem)packItem);
                case BaseContItem.Type.HumanPackEquip:
                    return isEquipIncluded((HumanPackEquip)packItem);
            }
            return false;
        }

        /// <summary>
        /// 是否包含物品
        /// </summary>
        /// <param name="packItem">物品</param>
        /// <returns>返回指定物品能否包含在容器中</returns>
        protected virtual bool isItemIncluded(HumanPackItem packItem) {
            return true;
        }

        /// <summary>
        /// 是否包含装备
        /// </summary>
        /// <param name="packEquip">装备</param>
        /// <returns>返回指定装备能否包含在容器中</returns>
        protected virtual bool isEquipIncluded(HumanPackEquip packEquip) {
            return true;
        }

        #endregion
    }
}