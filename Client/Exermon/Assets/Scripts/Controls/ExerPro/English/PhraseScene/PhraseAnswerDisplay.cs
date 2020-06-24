using Core.UI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Common.Controls.ItemDisplays;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Assets.Scripts.Scenes.ExerPro.EnglishPro;
using UI.PhraseScene.Windows;

namespace UI.ExerPro.EnglishPro.PhraseScene.Controls {

    /// <summary>
    /// 物品容器接口
    /// </summary>
    public interface IPhraseAnswerDisplay : IDropHandler { }

    class PhraseAnswerDisplay : MonoBehaviour, IPhraseAnswerDisplay {

        public Text Phrase;
        public OptionAreaDisplay areaDisplay;
        public ConfirmWindow window;

        #region 事件控制

        /// <summary>
        /// 拖拽物品放下回
        /// </summary>
        /// <param name="data">事件数据</param>
        public void OnDrop(PointerEventData data) {
            processItemDrop(getDraggingItemDisplay(data));
        }

        /// <summary>
        /// 获取拖拽中的物品显示项
        /// </summary>
        /// <param name="data">事件数据</param>
        /// <returns>物品显示项</returns>
        DraggableItemDisplay<string> getDraggingItemDisplay(PointerEventData data) {
            var obj = data.pointerDrag;
            if (obj == null) return null;
            return SceneUtils.get<DraggableItemDisplay<string>>(obj);
        }

        /// <summary>
        /// 处理物品放下
        /// </summary>
        protected void processItemDrop(DraggableItemDisplay<string> display) {
            if (display == null) return;
            Phrase.text = areaDisplay.question.word + " " + display.getItem();
            //回答正确
            if (display.getItem() == areaDisplay.question.phrase)
                window.initView(areaDisplay.question.word, "");
            //回答错误
            else window.initView(areaDisplay.question.word, areaDisplay.question.phrase);
        }

        #endregion


    }
}
