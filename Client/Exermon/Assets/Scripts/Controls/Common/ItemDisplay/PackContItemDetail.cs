using System;
using System.Collections.Generic;

using UnityEngine.UI;
using UnityEngine.Events;

using ItemModule.Data;

namespace UI.Common.Controls.ItemDisplays {

	/// <summary>
	/// 容器项详情显示
	/// </summary
	public class PackContItemDetail : ItemDetailDisplay<PackContItem> {

		/// <summary>
		/// 外部组件设置
		/// </summary>

		/// <summary>
		/// 绘制函数
		/// </summary>
		Dictionary<int, UnityAction<PackContItem>> drawFuncs =
			new Dictionary<int, UnityAction<PackContItem>>();

		#region 初始化

		/// <summary>
		/// 初始化
		/// </summary>
		protected override void initializeOnce() {
			base.initializeOnce();
			initializeDrawFuncs();
		}

		/// <summary>
		/// 初始化绘制函数
		/// </summary>
		protected virtual void initializeDrawFuncs() { }

		#endregion

		#region 绘制函数控制

		/// <summary>
		/// 注册物品类型
		/// </summary>
		/// <typeparam name="T">物品类型</typeparam>
		/// <param name="func">绘制函数</param>
		public virtual void registerItemType<T>(
			UnityAction<T> func) where T : PackContItem {
			UnityAction<PackContItem> func_ =
				(item) => func?.Invoke((T)item);

			var typeName = typeof(T).Name;
			var enumType = typeof(BaseContItem.Type);
			var type = (int)Enum.Parse(enumType, typeName);
			drawFuncs.Add(type, func_);
		}

		#endregion

		#region 数据控制

		/// <summary>
		/// 获取包含的道具
		/// </summary>
		/// <returns></returns>
		public virtual T getContainedItem<T>() where T : BaseItem {
			var _item = item as PackContItem<T>;
			if (_item != null) return (_item).item();
			return null;
		}

		/// <summary>
		/// 是否为空物品
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public override bool isNullItem(PackContItem item) {
			return base.isNullItem(item) || item.isNullItem() ||
				!drawFuncs.ContainsKey(item.type);
		}

		#endregion

		#region 界面控制

		/// <summary>
		/// 绘制确切物品
		/// </summary>
		/// <param name="item">物品</param>
		protected override void drawExactlyItem(PackContItem item) {
			base.drawExactlyItem(item);
			drawFuncs[item.type]?.Invoke(item);
		}

		#endregion
	}

	/// <summary>
	/// 容器项详情显示
	/// </summary
	public class PackContItemDetail<I> : ItemDetailDisplay<PackContItem<I>> where I : BaseItem {

		/// <summary>
		/// 外部组件设置
		/// </summary>

		#region 数据控制

		/// <summary>
		/// 获取包含的道具
		/// </summary>
		/// <returns></returns>
		public virtual I getContainedItem() {
			if (item != null) return item.item();
			return null;
		}

		/// <summary>
		/// 是否为空物品
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public override bool isNullItem(PackContItem<I> item) {
			return base.isNullItem(item) || item.isNullItem();
		}

		#endregion

		#region 界面控制

		/// <summary>
		/// 绘制容器项
		/// </summary>
		/// <param name="item"></param>
		protected override void drawExactlyItem(PackContItem<I> item) {
			base.drawExactlyItem(item);
			drawItem(item.item());
		}

		/// <summary>
		/// 绘制物品
		/// </summary>
		/// <param name="item"></param>
		protected virtual void drawItem(I item) { }

		#endregion
	}

	/// <summary>
	/// 容器项详情显示
	/// </summary
	public class PackContItemDetail<P, I> : ItemDetailDisplay<P> 
		where P: PackContItem<I> where I : BaseItem {

		/// <summary>
		/// 外部组件设置
		/// </summary>

		#region 数据控制

		/// <summary>
		/// 获取包含的道具
		/// </summary>
		/// <returns></returns>
		public virtual I getContainedItem() {
			if (item != null) return item.item();
			return null;
		}

		/// <summary>
		/// 是否为空物品
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public override bool isNullItem(P item) {
			return base.isNullItem(item) || item.isNullItem();
		}

		#endregion

		#region 界面控制

		/// <summary>
		/// 绘制容器项
		/// </summary>
		/// <param name="item"></param>
		protected override void drawExactlyItem(P item) {
			base.drawExactlyItem(item);
			drawItem(item.item());
		}

		/// <summary>
		/// 绘制物品
		/// </summary>
		/// <param name="item"></param>
		protected virtual void drawItem(I item) { }

		#endregion
	}
}