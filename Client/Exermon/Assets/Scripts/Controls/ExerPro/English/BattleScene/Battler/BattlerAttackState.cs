
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

using Core.UI;
using Core.UI.Utils;

namespace UI.ExerPro.EnglishPro.BattleScene.Controls.Battler {

	/// <summary>
	/// 战斗答题显示状态
	/// </summary>
	public class BattlerAttackState : BaseStateBehaviour {

		/// <summary>
		/// 内部变量
		/// </summary>
		BattlerDisplay battlerDisplay;

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="go"></param>
		protected override void setup(GameObject go) {
			base.setup(go);
			var receiver = SceneUtils.get<BattlerAnimationReceiver>(go);
			battlerDisplay = receiver.battler;
			Debug.Log("setup: battlerDisplay: " + battlerDisplay);
		}

		/// <summary>
		/// 状态结束
		/// </summary>
		protected override void onStatusExit() {
			battlerDisplay?.resetPosition();
			Debug.Log("onStatusExit: " + battlerDisplay);
		}

	}

}
