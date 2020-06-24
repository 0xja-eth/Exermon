using System;
using Core.UI.Utils;
using ExerPro.EnglishModule.Data;
using UI.Common.Controls.ItemDisplays;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ExerPro.EnglishPro.PhraseScene.Controls {
    public class OptionAreaDisplay : SelectableContainerDisplay<string>,
         IItemDisplay<PhraseQuestion> {
        public PhraseQuestion question;
        public Text chineseDisplay;
        public Text phraseDispaly;
        public RectTransform rect;

        string blank = " ____________";


        public void setItem(PhraseQuestion item, bool force = false) {
            question = item; base.setItems(item.options());
        }


        public void startView(PhraseQuestion item) {
            chineseDisplay.text = item.chinese;
            phraseDispaly.text = item.word;
            setItem(item); base.startView();
        }

        //protected override void onSelectChanged() {
        //base.onSelectChanged();
        //if (getSelectedIndex() != -1)
        //phraseDispaly.text = question.word + " " + items[getSelectedIndex()];
        //}

        public PhraseQuestion getItem() {
            return question;
        }

        /// <summary>
        /// 子节点创建回调
        /// </summary>
        /// <param name="sub"></param>
        /// <param name="index"></param>
        protected override void onSubViewCreated(SelectableItemDisplay<string> sub, int index) {
            ((OptionDisplay)sub).draggingParent = rect;
            base.onSubViewCreated(sub, index);

            var pos = getPosition(sub);
            var rt = sub.transform as RectTransform;
            rt.anchoredPosition = pos;

            while (IsRectTransformOverlap(rt, sub)) {
                pos = getPosition(sub);
                rt = sub.transform as RectTransform;
                rt.anchoredPosition = pos;
            }

        }

        Vector2 getPosition(SelectableItemDisplay<string> sub) {
            int x = UnityEngine.Random.Range(-400, 400);
            int yp = UnityEngine.Random.Range(70, 250);
            int yn = UnityEngine.Random.Range(-250, -70);
            return new Vector2(x, UnityEngine.Random.Range(0, 1000) % 2 == 0 ? yn : yp);
        }

        public bool IsRectTransformOverlap(RectTransform rect1, SelectableItemDisplay<string> sub) {

            foreach (var subview in getSubViews()) {
                if (subview == sub)
                    continue;
                float rect1MinX = rect1.position.x - 300 / 2;
                float rect1MaxX = rect1.position.x + 300 / 2;
                float rect1MinY = rect1.position.y - rect1.rect.height / 2;
                float rect1MaxY = rect1.position.y + rect1.rect.height / 2;

                RectTransform rect2 = subview.transform as RectTransform;
                float rect2MinX = rect2.position.x - 300 / 2;
                float rect2MaxX = rect2.position.x + 300 / 2;
                float rect2MinY = rect2.position.y - rect2.rect.height / 2;
                float rect2MaxY = rect2.position.y + rect2.rect.height / 2;


                if (rect1MinX < rect2MaxX &&
                   rect2MinX < rect1MaxX &&
                   rect1MinY < rect2MaxY &&
                   rect2MinY < rect1MaxY)
                    return true;
            }

            return false;
        }
    }
}
