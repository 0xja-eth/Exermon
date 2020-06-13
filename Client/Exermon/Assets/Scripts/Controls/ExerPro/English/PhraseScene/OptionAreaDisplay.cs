using Core.UI.Utils;
using ExerPro.EnglishModule.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Common.Controls.ItemDisplays;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Controls.ExerPro.English.PhraseScene {
    class OptionAreaDisplay : SelectableContainerDisplay<string>,
         IItemDisplay<PhraseQuestion> {
        public PhraseQuestion question;
        public Text chineseDisplay;
        public Text phraseDispaly;
        string blank = " ____________";


        public void setItem(PhraseQuestion item, bool force = false) {
            question = item;
            base.setItems(item.options());
        }

        public void startView(PhraseQuestion item) {
            base.startView();
            chineseDisplay.text = item.chinese;
            phraseDispaly.text = item.word + blank;
            setItem(item);
        }

        protected override void onSelectChanged() {
            base.onSelectChanged();
            if (getSelectedIndex() != -1)
                phraseDispaly.text = question.word + " " + items[getSelectedIndex()];
        }

        public PhraseQuestion getItem() {
            return question;
        }

        /// <summary>
        /// 子节点创建回调
        /// </summary>
        /// <param name="sub"></param>
        /// <param name="index"></param>
        protected override void onSubViewCreated(SelectableItemDisplay<string> sub, int index) {
            base.onSubViewCreated(sub, index);
            SceneUtils.get<Text>(subViews[index].gameObject).text = items[index];
        }

    }
}
