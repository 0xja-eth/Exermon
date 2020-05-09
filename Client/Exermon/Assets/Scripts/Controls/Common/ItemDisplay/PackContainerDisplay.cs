
using Core.Data.Loaders;

using PlayerModule.Data;
using ItemModule.Data;

using ItemModule.Services;

namespace UI.Common.Controls.ItemDisplays {

    /// <summary>
    /// 背包容器显示控件
    /// </summary>
    /// <remarks>
    /// 专门用于显示 PackContainer 的组件
    /// </remarks>
    public class PackContainerDisplay<T> : ContainerDisplay<T> where T : PackContItem, new() {

        /// <summary>
        /// 常量设置
        /// </summary>

        /// <summary>
        /// 内部变量声明
        /// </summary>
        PackContainer<T> packData;

        /// <summary>
        /// 内部系统定义
        /// </summary>
        ItemService itemSer;

        #region 初始化

        /// <summary>
        /// 初始化系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            itemSer = ItemService.get();
        }

        /// <summary>
        /// 配置
        /// </summary>
        public void configure(PackContainer<T> packData) {
            base.configure();
            setPackData(packData);
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 设置容器数据源
        /// </summary>
        /// <param name="packData">设置容器数据源</param>
        public void setPackData(PackContainer<T> packData) {
            this.packData = packData;
            refreshItems();
        }

        /// <summary>
        /// 获取容器
        /// </summary>
        /// <returns>返回对应的容器</returns>
        public PackContainer<T> getPackData() {
            return packData;
        }

        /// <summary>
        /// 是否需要判断具体的类型
        /// </summary>
        /// <returns></returns>
        protected virtual bool isNeedJudgeType() {
            return false; // typeof(T).IsAbstract || typeof(T) == typeof(PackContItem);
        }

        /// <summary>
        /// 可接受的类型列表
        /// </summary>
        /// <returns>返回可接受的物品类型列表</returns>
        protected virtual BaseContItem.Type[] acceptableTypes() {
            return new BaseContItem.Type[] { };
        }

        /// <summary>
        /// 是否包含物品
        /// </summary>
        /// <param name="packItem">物品</param>
        /// <param name="type">指定的类型</param>
        /// <returns>返回指定物品能否包含在容器中</returns>
        protected virtual bool isIncluded(T packItem, BaseContItem.Type type) {
            return true;
        }

        /// <summary>
        /// 是否包含物品
        /// </summary>
        /// <param name="packItem">物品</param>
        /// <returns>返回指定物品能否包含在容器中</returns>
        protected override bool isIncluded(T packItem) {
            if (!base.isIncluded(packItem)) return false;
            if (packItem.isNullItem()) return includeEmpty();

            if (isNeedJudgeType()) 
                foreach(var type in acceptableTypes()) 
                    if (packItem.type == (int)type)
                        return isIncluded(packItem, type);
            return true;
        }

        /// <summary>
        /// 增加物品
        /// </summary>
        /// <param name="item">物品</param>
        public override void addItem(T item) {
            addItem(item, false);
        }
        /// <param name="fixed_">整体操作</param>
        public void addItem(T item, bool fixed_) {
            itemSer.gainContItem(packData, item, true, fixed_, refreshItems);
        }

        /// <summary>
        /// 增加多个物品
        /// </summary>
        /// <param name="items">物品数组</param>
        /// <param name="fixed_">整体操作</param>
        public void addItems(T[] items, bool fixed_) {
            itemSer.gainContItems(packData, items, true, fixed_, refreshItems);
        }

        /// <summary>
        /// 移除物品
        /// </summary>
        /// <param name="item">物品</param>
        public override void removeItem(T item) {
            removeItem(item, -1);
        }
        /// <param name="count">移除数量</param>
        /// <param name="fixed_">整体操作</param>
        public void removeItem(T item, int count, bool fixed_ = true) {
            itemSer.lostContItem(packData, item, count, true, fixed_, refreshItems);
        }

        /// <summary>
        /// 移除多个物品
        /// </summary>
        /// <param name="items">物品数组</param>
        /// <param name="counts">分别 移除数量</param>
        /// <param name="fixed_">整体操作</param>
        public void removeItems(T[] items, int[] counts = null, bool fixed_ = true) {
            itemSer.lostContItems(packData, items, counts, true, fixed_, refreshItems);
        }

        /// <summary>
        /// 拆分物品
        /// </summary>
        /// <param name="item">物品</param>
        /// <param name="count">拆分数量</param>
        public virtual void splitItem(T item, int count) {
            itemSer.splitItem(packData, item, count, refreshItems);
        }

        /// <summary>
        /// 合并物品
        /// </summary>
        /// <param name="items">物品数组</param>
        public virtual void mergeItems(T[] items) {
            itemSer.mergeItems(packData, items, refreshItems);
        }

        /// <summary>
        /// 接受转移
        /// </summary>
        /// <param name="item">物品</param>
        protected override T prepareTransfer(T item) {
            base.removeItem(item);
            return item;
        }

        /// <summary>
        /// 接受转移
        /// </summary>
        /// <param name="container">容器</param>
        /// <param name="item">物品</param>
        protected override void acceptTransfer(ContainerDisplay<T> container, T item) {
            var packContainer = getPackDisplay(container);
            if (packContainer != null)
                acceptTransfer(packContainer, item);
            else base.addItem(item);
        }
        protected void acceptTransfer(PackContainerDisplay<T> container, T item) {
            var packData = container.packData;
            itemSer.transferItem(packData, this.packData, item, 
                onSuccess: () => onItemTransferred(container, item));
        }

        /// <summary>
        /// 获取背包容器显示组件
        /// </summary>
        /// <returns>返回背包容器</returns>
        PackContainerDisplay<T> getPackDisplay(
            ContainerDisplay<T> container = null) {
            return DataLoader.cast<PackContainerDisplay<T>>(container);
        }

        /// <summary>
        /// 物品转移回调
        /// </summary>
        protected virtual void onItemTransferred(PackContainerDisplay<T> container, T item) {
            container.refreshItems(); refreshItems();
        }

        /// <summary>
        /// 刷新物品
        /// </summary>
        public override void refreshItems() {
            if (packData == null) clearItems();
            else setItems(packData.items);
        }

        #endregion
    }
}