
using System;
using System.Collections.Generic;
using UnityEngine;

using LitJson;
using System.Linq;

using Core.Data;
using Core.Data.Loaders;

using GameModule.Data;
using GameModule.Services;

using QuestionModule.Data;

using ItemModule.Data;
using PlayerModule.Data;

using UI.Common.Controls.ParamDisplays;

/// <summary>
/// 艾瑟萌特训模块
/// </summary>
namespace ExerPro { }

/// <summary>
/// 英语特训模块
/// </summary>
namespace ExerPro.EnglishModule { }

/// <summary>
/// 英语特训模块数据
/// </summary>
namespace EnglishModule.Data {

    /// <summary>
    /// 听力小题
    /// </summary>
    public class ListeningSubQuestion : BaseQuestion { }

    /// <summary>
    /// 听力题
    /// </summary>
    public class ListeningQuestion : GroupQuestion<ListeningSubQuestion> {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public AudioClip audio { get; protected set; }

    }

    /// <summary>
    /// 阅读小题
    /// </summary>
    public class ReadingSubQuestion : BaseQuestion { }

    /// <summary>
    /// 听力题
    /// </summary>
    public class ReadingQuestion : GroupQuestion<ReadingSubQuestion> { }

    /// <summary>
    /// 改错题
    /// </summary>
    public class CorrectionQuestion : BaseData {

        /// <summary>
        /// 错误项
        /// </summary>
        public class WrongItem : BaseData {

            /// <summary>
            /// 属性
            /// </summary>
            [AutoConvert]
            public int sentenceIndex { get; protected set; }
            [AutoConvert]
            public int wordIndex { get; protected set; }
            [AutoConvert]
            public int type { get; protected set; }
            [AutoConvert]
            public string word { get; protected set; }

        }

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public string article { get; protected set; }
        [AutoConvert]
        public string description { get; protected set; }

        [AutoConvert]
        public WrongItem[] wrongItems { get; protected set; }
    }

    /// <summary>
    /// 单词
    /// </summary>
    public class Word : BaseData {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public string english { get; protected set; }
        [AutoConvert]
        public string chinese { get; protected set; }
        [AutoConvert]
        public string type { get; protected set; }
        [AutoConvert]
        public int level { get; protected set; }
        [AutoConvert]
        public bool isMiddle { get; protected set; }
        [AutoConvert]
        public bool isHigh { get; protected set; }

    }

    /// <summary>
    /// 单词记录
    /// </summary>
    public class WordRecord : BaseData {
        
        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int wordId { get; protected set; }
        [AutoConvert]
        public int count { get; protected set; }
        [AutoConvert]
        public int correct { get; protected set; }
        [AutoConvert]
        public DateTime firstDate { get; protected set; }
        [AutoConvert]
        public DateTime lastDate { get; protected set; }
        [AutoConvert]
        public bool collected { get; protected set; }
        [AutoConvert]
        public bool wrong { get; protected set; }

    }
}