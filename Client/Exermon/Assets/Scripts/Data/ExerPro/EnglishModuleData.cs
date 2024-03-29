﻿
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using LitJson;

using Core.Data;
using Core.Data.Loaders;

using GameModule.Data;
using GameModule.Services;

using ItemModule.Data;
using PlayerModule.Data;
using ExermonModule.Data;
using QuestionModule.Data;

using PlayerModule.Services;

using UI.Common.Controls.ParamDisplays;
using System.Text.RegularExpressions;

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
namespace ExerPro.EnglishModule.Data {

    using EnglishModule.Services;

    #region 题目


    /// <summary>
    /// 听力小题
    /// </summary>
    public class ListeningSubQuestion : BaseQuestion {
        /// <summary>
        /// 加载选项Item属性
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>

		/// <summary>
		/// 构造函数
		/// </summary>
		public ListeningSubQuestion() {

		}
	}

    /// <summary>
    /// 听力题
    /// </summary>
    public class ListeningQuestion : GroupQuestion<ListeningSubQuestion> {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public AudioClip audio { get; protected set; }

        /// <summary>
        /// 属性
        /// </summary>
        //[AutoConvert]
        //public string eventName { get; protected set; }
        //[AutoConvert]
        //public Texture2D picture { get; protected set; }
		[AutoConvert]
		public int times { get; protected set; }

        }

    /*
    /// <summary>
    /// 阅读小题
    /// </summary>
    public class ReadingSubQuestion : BaseQuestion { }

    /// <summary>
    /// 阅读题
    /// </summary>
    public class ReadingQuestion : GroupQuestion<ReadingSubQuestion> { }
    */

    /// <summary>
    /// 短语题目
    /// </summary>
    public class PhraseQuestion : BaseData {

        /// <summary>
		/// 删除率
		/// </summary>
		const int DropoutRate = 50;
		const int DropoutThreshold = 5;

		/// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public string word { get; protected set; }
        [AutoConvert]
        public string chinese { get; protected set; }
        [AutoConvert]
        public string phrase { get; protected set; }
        [AutoConvert]
        public int type { get; protected set; }
        public string[] option1 = {
            "sb. to do sth.", "sb. do sth.", "sb. doing sth.", "sb. into doing sth.", "sb. for doing sth.", "sb. of sth.", "sth. for sb." };
        public string[] option2 = {
			"doing sth.", "to do sth.", "to doing sth." };
        public string[] option3 = {
			"about", "at", "for", "from", "in", "of", "with", "to" };
        public string[] option = {
			"about", "at", "for", "from", "in", "of", "with", "to" ,
            "doing sth.", "to do sth." ,"to doing sth.", "sb. to do sth.", "sb. doing sth.",
            "sb. into doing sth.", "sb. of sth", "sth. for sb." };

		/// <summary>
		/// 缓存选项
		/// </summary>
		string[] tmpOptions = null;

		/// <summary>
		/// 生成选项
		/// </summary>
		/// <returns></returns>
        public string[] options() {
			if (tmpOptions == null) {
				if (option1.ToList().Contains(phrase))
					tmpOptions = option1;
				else if (option2.ToList().Contains(phrase))
					tmpOptions = option2;
				else if (option3.ToList().Contains(phrase))
					tmpOptions = option3;
				else tmpOptions = option;
        } 
			return dropout(tmpOptions);
		}

		/// <summary>
		/// 随机删除
		/// </summary>
		/// <param name="options"></param>
		/// <returns></returns>
		string[] dropout(string[] options) {
			var res = new List<string>();
			foreach (var opt in options) {
				var rand = UnityEngine.Random.Range(0, 100);
				if (res.Count >= DropoutThreshold) {
					if (rand >= DropoutRate) res.Add(opt);
			}
                else res.Add(opt);
            }
			if (!res.Contains(phrase)) res.Add(phrase);
			return res.ToArray();
		}

		/// <summary>
		/// 清除缓存选项
		/// </summary>
		public void resetOptions() {
			tmpOptions = null;
		}

		/// <summary>
		/// 测试
		/// </summary>
		/// <returns></returns>
        public static PhraseQuestion sample() {
            long i = UnityEngine.Random.Range(0, 10000);

            PhraseQuestion question = new PhraseQuestion();
            switch (i % 2) {
                case 0:
                    question.word = "persuade";
                    question.chinese = "说服某人做某事";
                    question.phrase = "sb. into doing sth.";
                    return question;
                case 2:
                    question.word = "hesitate";
                    question.chinese = "犹豫做某事";
                    question.phrase = "to do sth.";
                    return question;
                case 1:
                    question.word = "be certain";
                    question.chinese = "确信……";
                    question.phrase = "about";
                    return question;
                case 3:
                    question.word = "in [with] reference";
                    question.chinese = "关于";
                    question.phrase = "to";
                    return question;
                default:
                    question.word = "hope";
                    question.chinese = "希望某人做某事";
                    question.phrase = "sb. to do sth.";
                    return question;
            }
        }
    }

    /// <summary>
    /// 改错题
    /// </summary>
    public class CorrectionQuestion : BaseData {
        /// <summary>
        /// 错误项
        /// </summary>
        public class WrongItem : BaseData {

			/// <summary>
			/// 类型枚举
			/// </summary>
			public enum Type {
				Add = 1, Edit = 2, Delete = 3
			}

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

			/// <summary>
			/// 对应题目
			/// </summary>
			public CorrectionQuestion question { get; set; }

			/// <summary>
			/// 缓存转换结果
			/// </summary>
			FrontendWrongItem tempFrontendWrongItem = null;

			/// <summary>
			/// 类型文本
			/// </summary>
			/// <returns></returns>
			public string typeText() {
                return DataService.get().correctType(type).Item2;
            }

			/// <summary>
			/// 类型枚举
			/// </summary>
			/// <returns></returns>
			public Type typeEnum() {
				return (Type)type;
			}

			/// <summary>
			/// 转化为前端错误项
			/// </summary>
			/// <returns></returns>
			public FrontendWrongItem convertToFrontendWrongItem() {
				return tempFrontendWrongItem = tempFrontendWrongItem ?? 
					new FrontendWrongItem(sentenceIndex, wordIndex, convertFrontentWord());
			}

			/// <summary>
			/// 获取原始单词
			/// </summary>
			/// <returns></returns>
			string oriWord() {
				return question.word(sentenceIndex, wordIndex);
			}

			/// <summary>
			/// 获取下一单词
			/// </summary>
			/// <returns></returns>
			string nextWord() {
				return question.word(sentenceIndex, wordIndex+1);
			}

			/// <summary>
			/// 转化为前端文本
			/// </summary>
			/// <returns></returns>
			string convertFrontentWord() {
				switch (typeEnum()) {
					case Type.Delete: return "";
					case Type.Edit: return word;
					case Type.Add:
						if (wordIndex == 0) // 句首
							return word + " " + nextWord();
						else return oriWord() + " " + word;
				}
				return word;
			}

		}

		/// <summary>
		/// 前端错误项
		/// </summary>
		public class FrontendWrongItem : BaseData {
            /// <summary>
            /// 属性
            /// </summary>
            [AutoConvert]
            public int sid { get; set; }
            [AutoConvert]
            public int wid { get; set; }
            [AutoConvert]
            public string word { get; set; }

			/// <summary>
			/// 构造函数
			/// </summary>
			public FrontendWrongItem() { }
			public FrontendWrongItem(int sid, int wid, string word) {
				this.sid = sid; this.wid = wid;
				this.word = word;
            }
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

		/// <summary>
		/// 缓存句子
		/// </summary>
		string[] tmpSentences = null;

		/// <summary>
		/// 读取自定义属性
		/// </summary>
		/// <param name="json"></param>
		protected override void loadCustomAttributes(JsonData json) {
			base.loadCustomAttributes(json);
			foreach (var item in wrongItems)
				item.question = this;
		}

		/// <summary>
		/// 获取指定位置单词
		/// </summary>
		/// <param name="sid">句子ID（从1开始）</param>
		/// <param name="wid">单词ID（从1开始，0表示句首）</param>
		/// <returns></returns>
		public string word(int sid, int wid) {
			if (wid == 0) return "";
			var words = this.words(sid);
			if (wid > words.Length) return "";
			return words[wid - 1];
		}

		/// <summary>
		/// 获取句子数组
		/// </summary>
		/// <returns></returns>
		public string[] sentences() {
			if (tmpSentences == null) {
                //article = "I hardly remember my grandmother. a, “asdd.”She 12:20 a.m. Mr. Miss. 12:20 Mr. used to holding me ono her knees and sing old songs. I was only four when she passes away. She is just a distant memory for me now. I remember my grandfather very much. He was tall, with broad shoulder and a beard that turned from black toward gray over the years. He had a deep voice, which set himself apart from others in our small town, he was strong and powerful. In a fact, he even scared my classmates away during they came over to play or do homework with me. However, he was the gentlest man I have never known.";
                //      article = "Dear Diary, " +
                //"Here I am in the middle of a city, 350 miles far away from our farmhouse. Do you want to know why we move last week? Dad lost his job, and as Mom explained, “He was lucky to find other one.” His new job meant I had to say goodbye to my classmate, my school or just everything else I love in the world. To make matters bad, now I have to share a room with my younger sister, Maggie. Tomorrow is first day of school. I am awfully tiring, but I know I’ll never fall sleep." +
                // "Good night and remember, you, dear diary, is my only souvenir from my past life and my only friend.";
                //article = "My uncle is the owner of a restaurant close to that I live.|Though not very big, but the restaurant is popular in our area.|It is always crowded with customers at meal times.|Some people even had to wait outside.|My uncle tells me that the key to his success is honest.|Every day he makes sure that fresh vegetables or high quality oil are using for cooking.|My uncle says that he never dreams becoming rich in the short period of time.|Instead, he hopes that our business will grow steady.";
                //article = "Mr Johnson is a hardworking teacher.|Every day, he spends too much time with his work.|With little sleep and hardly any break, so he works from morning till night.|Hard work have made him very ill.|“He has ruined his healthy.|We are worried about him.”|That is which other teachers say.|Yesterday afternoon, I paid visit to Mr.|Johnson.|I was eager to see him, but outside her room I stopped.|I had to calm myself down.|Quietly I step into the room.|I saw him lying in bed, looking at some of the picture we had taken together.|I understood that he missed us just as many as we missed him.";

                string temp = article.Replace("\n", "");
				temp = temp.Replace("\t", "");
				temp = temp.Replace("\r", "");

				//Queue<KeyValuePair<string, string>> matchWordList = new Queue<KeyValuePair<string, string>>();
				//List<string> wordList = new List<string>();
				//using (StreamReader sr = new StreamReader("word.txt")) {
				//	string line;
				//	while ((line = sr.ReadLine()) != null) {
				//		wordList.Add(line);
				//	}
				//}

				//foreach (string word in wordList) {
				//	Regex regex = new Regex(word);
				//	if (regex.IsMatch(temp)) {
				//		string match = Regex.Match(temp, word).Value;
				//		string hashCode = match.GetHashCode().ToString();
				//		temp = temp.Replace(match, hashCode);
				//		matchWordList.Enqueue(new KeyValuePair<string, string>(match, hashCode));
				//	}
				//}


				//temp = temp.Replace("”", "\"");
				//temp = temp.Replace("“", "\"");

				//temp = Regex.Replace(temp, @"(?<str>[A-Z][^ ]*?)(\.)", "${str}#");

				//temp = Regex.Replace(temp, @"(?<str>[^A-Za-z0-9| |\-|’|#])", "${str} ");
				//temp = Regex.Replace(temp, @"(?<str>[^A-Za-z0-9| |\-|’|#])  ", "${str} ");
				//temp = Regex.Replace(temp, @"(?<str>[^A-Za-z0-9| |\-|’|#]) ", " ${str} ");

				//temp = temp.Replace(",  \"", ",\"&");
				//temp = temp.Replace(".  \"", ".\"&");
				//temp = temp.Replace(" ? ", " ?&");
				//temp = temp.Replace(" . ", " .&");
				//temp = temp.Replace(" ! ", " !&");
				//Debug.Log("aaa" + temp);

				//temp = temp.Replace("#", ".");

				//while (matchWordList.Count != 0) {
				//	KeyValuePair<string, string> keyValuePair = matchWordList.Dequeue();
				//	temp = temp.Replace(keyValuePair.Value, keyValuePair.Key);
				//}

				////去除重复空格
				//temp = Regex.Replace(temp, " {2,}", " ");

				tmpSentences = temp.Split(new char[1] { '|' }, StringSplitOptions.RemoveEmptyEntries);
			}
			return tmpSentences;
		}

		/// <summary>
		/// 获取单词数组
		/// </summary>
		/// <param name="sid">句子ID（从1开始）</param>
		/// <returns></returns>
		public string[] words(int sid) {
			if (sid <= 0) return new string[0];
			var sentence = sentences()[sid - 1];
			return sentence.Trim().Split(' ');
		}
	}
	
	/// <summary>
	/// 剧情题目
	/// </summary>
	public class PlotQuestion : BaseQuestion {

		/// <summary>
		/// 选项
		/// </summary>
		public new class Choice : BaseQuestion.Choice {

			/// <summary>
			/// 属性
			/// </summary>
			[AutoConvert]
			public ExerProEffectData[] effects { get; protected set; }
			[AutoConvert]
			public int gold { get; protected set; }
			[AutoConvert]
			public string resultText { get; protected set; }

		}

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public string eventName { get; protected set; }
		[AutoConvert]
		public Texture2D picture { get; protected set; }

		/// <summary>
		/// 加载选项Item属性
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		protected override BaseQuestion.Choice loadChoice(JsonData data) {
			return DataLoader.load<Choice>(data) as BaseQuestion.Choice;
		}

		/// <summary>
		/// 选项
		/// </summary>
		public new Choice[] choices {
			get {
				var choices = base.choices;
				List<Choice> listTemp = new List<Choice>();
				foreach (var choice in choices)
					listTemp.Add(choice as Choice);
				return listTemp.ToArray();
			}
		}
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
        [AutoConvert]
        public bool current { get; protected set; }
        [AutoConvert]
        public bool currentCorrect { get; protected set; }
        [AutoConvert]
        public bool currentDone { get; protected set; }

        /// <summary>
        /// 获取单词实例
        /// </summary>
        /// <returns></returns>
        public Word word() {
            return EnglishService.get().getQuestion<Word>(wordId);
        }

		/// <summary>
		/// 回答
		/// </summary>
		/// <param name="correct"></param>
		public void answer(bool correct) {
			currentDone = true;
			currentCorrect = correct;
		}

		/// <summary>
		/// 是否当前轮正确单词
		/// </summary>
		/// <returns></returns>
		public bool isCurrentCorrect() {
            return currentDone && currentCorrect;
        }

        /// <summary>
        /// 是否当前轮错误单词
        /// </summary>
        /// <returns></returns>
        public bool isCurrentWrong() {
            return currentDone && !currentCorrect;
        }
    }

    /// <summary>
    /// 反义词表
    /// </summary>
    public class Antonym : TypeData {
        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public string cardWord { get; protected set; }
        [AutoConvert]
        public string enemyWord { get; protected set; }
        [AutoConvert]
        public double hurtRate { get; protected set; }

    }

	#endregion

	#region 物品

	/// <summary>
	/// 特训物品星级数据
	/// </summary>
	public class ExerProItemStar : TypeData {

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public Color color { get; protected set; }
	}

	/// <summary>
	/// 特训效果数据
	/// </summary>
	public class ExerProEffectData : BaseData {

		/// <summary>
		/// 效果代码枚举
		/// </summary>
		public enum Code {
            Unset = 0, // 空

            Attack = 1, // 造成伤害
            AttackSlash = 2, // 造成伤害（完美斩击）
            AttackBlack = 3, // 造成伤害（黑旋风）
            AttackWave = 4, // 造成伤害（波动拳）
            AttackRite = 5, // 造成伤害（仪式匕首）

            Recover = 100, // 回复体力值

            AddParam = 200, // 增加能力值
            AddMHP = 201, // 获得MHP
            AddPower = 202, // 获得力量
            AddDefense = 203, // 获得格挡
            AddAgile = 204, // 获得敏捷
            AddParamUrgent = 205, // 增加能力值（紧急按钮）

            TempAddParam = 210, // 临时增加能力值
            TempAddMHP = 211, // 临时获得MHP
            TempAddPower = 212, // 临时获得力量
            TempAddDefense = 213, // 临时获得格挡
            TempAddAgile = 214, // 临时获得敏捷

            AddState = 220, // 增加状态
            RemoveState = 221, // 移除状态
            RemoveNegaState = 222, // 移除消极状态

            AddEnergy = 230, // 回复能量

            DrawCards = 300, // 抽取卡牌
            ConsumeCards = 310, // 消耗卡牌

            ChangeCost = 400, // 更改耗能
            ChangeCostDisc = 401, // 更改耗能（发现）
            ChangeCostCrazy = 402, // 更改耗能（疯狂）

			GainGold = 500, // 获得金币
			GainCard = 510, // 获得卡牌
			DropCard = 511, // 失去卡牌
			CopyCard = 512, // 复制卡牌
			GainItem = 520, // 获得道具
			LoseItem = 521, // 失去道具
			GainPotion = 530, // 获得药水
			LosePotion = 531, // 失去药水

		}

		/// <summary>
		/// 效果范围
		/// </summary>
		static readonly int[] AttackEffectRange = new int[] { 1, 99 };
		static readonly int[] RecoverEffectRange = new int[] { 100, 199 };
		static readonly int[] BuffEffectRange = new int[] { 200, 219 };
		static readonly int[] StateEffectRange = new int[] { 220, 229 };
		static readonly int[] EnergyEffectRange = new int[] { 230, 239 };
		static readonly int[] CardEffectRange = new int[] { 300, 399 };
		static readonly int[] CostEffectRange = new int[] { 400, 499 };

		static readonly int[] PlotEffectRange = new int[] { 500, 599 };

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public int code { get; protected set; }
		[AutoConvert("params")]
		public JsonData params_ { get; protected set; } // 参数（数组）

		#region 数据获取

		/// <summary>
		/// 获取指定下标下的数据
		/// </summary>
		/// <typeparam name="T">类型</typeparam>
		/// <param name="index">下标</param>
		/// <param name="default_">默认值</param>
		/// <returns></returns>
		public T get<T>(int index, T default_ = default) {
			if (params_ == null || !params_.IsArray) return default_;
			if (index < params_.Count) return DataLoader.load<T>(params_[index]);
			return default_;
		}

		/// <summary>
		/// 效果代码枚举
		/// </summary>
		/// <returns></returns>
		public Code codeEnum() {
			return (Code)code;
		}

		#endregion

		#region 效果判断

		/// <summary>
		/// 效果代码是否属于某范围
		/// </summary>
		/// <param name="range">范围（最小值，最大值）</param>
		/// <returns></returns>
		public bool isCodeInsideRange(int[] range) {
			return range[0] <= code && code <= range[1];
		}

		/// <summary>
		/// 是否攻击效果
		/// </summary>
		/// <returns></returns>
		public bool isAttackEffect() {
			return isCodeInsideRange(AttackEffectRange);
		}

		/// <summary>
		/// 是否回复效果
		/// </summary>
		/// <returns></returns>
		public bool isRecoverEffect() {
			return isCodeInsideRange(RecoverEffectRange);
		}

		/// <summary>
		/// 是否BUFF效果
		/// </summary>
		/// <returns></returns>
		public bool isBuffEffect() {
			return isCodeInsideRange(BuffEffectRange);
		}

		/// <summary>
		/// 是否状态效果
		/// </summary>
		/// <returns></returns>
		public bool isStateEffect() {
			return isCodeInsideRange(StateEffectRange);
		}

		/// <summary>
		/// 是否能量效果
		/// </summary>
		/// <returns></returns>
		public bool isEnergyEffect() {
			return isCodeInsideRange(EnergyEffectRange);
		}

		/// <summary>
		/// 是否卡牌效果
		/// </summary>
		/// <returns></returns>
		public bool isCardEffect() {
			return isCodeInsideRange(CardEffectRange);
		}

		/// <summary>
		/// 是否耗能效果
		/// </summary>
		/// <returns></returns>
		public bool isCostEffect() {
			return isCodeInsideRange(CostEffectRange);
		}

		/// <summary>
		/// 是否剧情效果
		/// </summary>
		/// <returns></returns>
		public bool isPlotEffect() {
			return isCodeInsideRange(PlotEffectRange);
		}

		#endregion

		/// <summary>
		/// 构造函数
		/// </summary>
		public ExerProEffectData() { }
		public ExerProEffectData(Code code, JsonData params_) {
			this.code = (int)code; this.params_ = params_;
		}

	}

	/// <summary>
	/// 特训效果数据
	/// </summary>
	public class ExerProTraitData : BaseData {

		/// <summary>
		/// 效果代码枚举
		/// </summary>
		public enum Code {
			Unset = 0, // 空

			DamagePlus = 1, // 攻击伤害加成
			HurtPlus = 2, // 受到伤害加成
			RecoverPlus = 3, // 回复加成

			RestRecoverPlus = 4, // 回复加成（休息据点）

			RoundEndRecover = 5, // 回合结束HP回复
			RoundStartRecover = 6, // 回合开始HP回复
			BattleEndRecover = 7, // 战斗结束HP回复
			BattleStartRecover = 8, // 战斗开始HP回复

			ParamAdd = 9, // 属性加成值
			ParamRoundAdd = 10, // 回合属性加成值
			ParamBattleAdd = 11, // 战斗属性加成值

			RoundDrawCards = 12, // 回合开始抽牌数加成
			BattleDrawCards = 13, // 战斗开始抽牌数加成
		}

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public int code { get; protected set; }
		[AutoConvert("params")]
		public JsonData params_ { get; protected set; } // 参数（数组）

		#region 数据获取

		/// <summary>
		/// 获取指定下标下的数据
		/// </summary>
		/// <typeparam name="T">类型</typeparam>
		/// <param name="index">下标</param>
		/// <param name="default_">默认值</param>
		/// <returns></returns>
		public T get<T>(int index, T default_ = default) {
			if (params_ == null || !params_.IsArray) return default_;
			if (index < params_.Count) return DataLoader.load<T>(params_[index]);
			return default_;
		}

		/// <summary>
		/// 特性代码枚举
		/// </summary>
		/// <returns></returns>
		public Code codeEnum() {
			return (Code)code;
		}

		#endregion

		/// <summary>
		/// 构造函数
		/// </summary>
		public ExerProTraitData() { }
		public ExerProTraitData(Code code, JsonData params_) {
			this.code = (int)code; this.params_ = params_;
		}

	}

	/// <summary>
	/// 特训物品数据
	/// </summary>
	public abstract class BaseExerProItem : BaseItem {

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public int iconIndex { get; protected set; }
		[AutoConvert]
		public int startAniIndex { get; protected set; }
		[AutoConvert]
		public int targetAniIndex { get; protected set; }
		[AutoConvert]
        public int starId { get; protected set; }
        [AutoConvert]
        public ExerProEffectData[] effects { get; protected set; }

		/*
		/// <summary>
		/// 物品价格，用于商店
		/// </summary>
		protected int _gold = 0;
        public int gold {
            get {
                if (_gold == 0)
                    _gold = generatePrice();
                return _gold;
            }
            set {
                _gold = 0;
            }
        }
        /// <summary>
        /// 子类重载该函数用于制定价格策略
        /// </summary>
        /// <returns></returns>
        protected virtual int generatePrice() { return 0; }
		*/

		/// <summary>
		/// 起手动画/目标动画
		/// </summary>
		public AnimationClip startAni { get; protected set; } = null;
		public AnimationClip targetAni { get; protected set; } = null;

		/// <summary>
		/// 物品图标
		/// </summary>
		public Sprite icon { get; protected set; }

		/// <summary>
		/// 读取函数
		/// </summary>
		/// <param name="index">索引</param>
		/// <returns>返回裁剪后的精灵</returns>
		public delegate Sprite IconLoadFun(int index);

		/// <summary>
		/// 读取函数
		/// </summary>
		/// <param name="index">索引</param>
		/// <returns>返回裁剪后的精灵</returns>
		public delegate AnimationClip AniLoadFun(int index, bool start);

		/// <summary>
		/// 获取读取函数
		/// </summary>
		/// <returns>读取函数</returns>
		protected abstract IconLoadFun iconLoadFun();
		protected virtual AniLoadFun aniLoadFun() { return null; }

        /// <summary>
        /// 物品星级
        /// </summary>
        /// <returns>返回物品星级对象</returns>
        public ExerProItemStar star() {
            return DataService.get().exerProItemStar(starId);
        }

		/// <summary>
		/// 重复次数
		/// </summary>
		/// <returns></returns>
		public int repeats() {
			var res = 1;
			var effects = attackEffects();
			foreach (var effect in effects)
				res = Math.Max(res, effect.get(1, 1));
			return res;
		}

		/// <summary>
		/// 攻击类效果
		/// </summary>
		/// <returns></returns>
		public ExerProEffectData[] attackEffects() {
			var res = new List<ExerProEffectData>();
			foreach (var effect in effects)
				if (effect.isAttackEffect()) res.Add(effect);
			return res.ToArray();
		}

		/// <summary>
		/// 加载自定义属性
		/// </summary>
		/// <param name="json"></param>
		protected override void loadCustomAttributes(JsonData json) {
			base.loadCustomAttributes(json);

			icon = iconLoadFun()?.Invoke(iconIndex);
			startAni = AssetLoader.loadAnimation(startAniIndex);
			targetAni = AssetLoader.loadAnimation(targetAniIndex);
		}
	}

    /// <summary>
    /// 特训物品数据
    /// </summary>
    public class ExerProItem : BaseExerProItem {

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public ExerProTraitData[] traits { get; protected set; }

		/// <summary>
		/// 获取读取函数
		/// </summary>
		/// <returns>读取函数</returns>
		protected override IconLoadFun iconLoadFun() {
			return AssetLoader.getExerProItemIconSprite;
		}

        public static ExerProItem sample() {
            int index = UnityEngine.Random.Range(0, 9);
            int[] a = { 1, 2, 4, 10, 14, 16, 19, 32, 33 };
            return DataService.get().exerProItem(a[index]);
	}
    }

    /// <summary>
    /// 特训药水数据
    /// </summary>
    public class ExerProPotion : BaseExerProItem {

		/// <summary>
		/// 属性
		/// </summary>

		/// <summary>
		/// 获取读取函数
		/// </summary>
		/// <returns>读取函数</returns>
		protected override IconLoadFun iconLoadFun() {
			return AssetLoader.getExerProItemIconSprite;
		}
        public static ExerProPotion sample() {
            int index = UnityEngine.Random.Range(0, 8);
            int[] a = { 4, 5, 6, 7, 14, 15, 16, 25 };
            return DataService.get().exerProPotion(a[index]);
        }
		/*
        /// <summary>
        /// 药水定价策略
        /// </summary>
        /// <returns></returns>
        protected override int generatePrice() {
            return CalcService.ExerProItemGenerator.generatePotionPrice(this);
        }
		*/
    }

    /// <summary>
    /// 特训卡片数据
    /// </summary>
    public class ExerProCard : BaseExerProItem {

        /// <summary>
        /// 目标
        /// </summary>
        public enum Target {
            Default = 0,  // 默认
            One = 1,  // 敌方单体
            All = 2,  // 敌方全体
        }

		/// <summary>
		/// 卡牌类型
		/// </summary>
		public new enum Type {
			Attack = 1, // 攻击
			Skill = 2, // 技能
			Ability = 3, // 能力
			Evil = 4 // 诅咒
		}

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int cost { get; protected set; }
        [AutoConvert]
        public int cardType { get; protected set; }
		[AutoConvert]
		public int skinIndex { get; protected set; }
		[AutoConvert]
        public bool inherent { get; protected set; }
        [AutoConvert]
        public bool disposable { get; protected set; }
		[AutoConvert("character")]
		public string _character { get; protected set; }

		/// <summary>
		/// 首字母大写
		/// </summary>
		public string character {
			get {
				if (_character == "") return "";
				var s = _character;
				return s.Substring(0, 1).ToUpper() + s.Substring(1);
			}
		}

		[AutoConvert]
        public int target { get; protected set; }

		/// <summary>
		/// 图片
		/// </summary>
		public Texture2D skin { get; protected set; }
		public Texture2D charFrame { get; protected set; }
		public Texture2D typeIcon { get; protected set; }

		/// <summary>
		/// 类型文本
		/// </summary>
		/// <returns></returns>
		public string typeText() {
			return DataService.get().cardType(cardType).Item2;
		}
		/*
        /// <summary>
        /// 卡牌定价策略
        /// </summary>
        /// <returns></returns>
        protected override int generatePrice() {
            return CalcService.ExerProItemGenerator.generateCardPrice(this);
        }
		*/
		/// <summary>
		/// 获取读取函数
		/// </summary>
		/// <returns>读取函数</returns>
		protected override IconLoadFun iconLoadFun() {
			return AssetLoader.getExerProCardIconSprite;
		}

		/// <summary>
		/// 读取自定义属性
		/// </summary>
		/// <param name="json"></param>
		protected override void loadCustomAttributes(JsonData json) {
			base.loadCustomAttributes(json);

			skin = AssetLoader.loadCardSkin(starId, cardType, skinIndex);
			charFrame = AssetLoader.loadCardCharFrame(starId, skinIndex);
			typeIcon = AssetLoader.loadCardTypeIcon(cardType);
		}

        public static ExerProCard sample() {
            ExerProCard card = new ExerProCard();
            card.cost = 1;
            card.skinIndex = 0;
            card.cardType = 0;
            card.inherent = false;
            card.disposable = false;
            card._character = "character";
            card.target = 0;
            card.type = 1;
            card.iconIndex = 0;
            return card;
	}

    }

	/// <summary>
	/// 初始卡组
	/// </summary>
    public class FirstCardGroup : TypeData {

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public int[] cards { get; protected set; }

		/// <summary>
		/// 获取所有卡牌
		/// </summary>
		/// <returns></returns>
		public ExerProCard[] getCards() {
			var cnt = cards.Length;
			var res = new ExerProCard[cnt];
			for (int i = 0; i < cnt; ++i)
				res[i] = DataService.get().exerProCard(cards[i]);
			return res;
		}
	}

	/// <summary>
	/// 特训敌人数据
	/// </summary>
	public class ExerProEnemy : BaseItem {

		/// <summary>
		/// BOSS设置
		/// </summary>
		public const int BOSS1 = 14; // 中二少年
		public const int BOSS2 = 13; // 乌龟大师
		public const int BOSS3 = 15; // 龙王

		/// <summary>
		/// 类型
		/// </summary>
		public enum EnemyType {
            Normal = 1, Elite = 2, Boss = 3
        }

        /// <summary>
        /// 行动数据
        /// </summary>
        public class Action : BaseData {

            /// <summary>
            /// 类型
            /// </summary>
            public enum Type {
                Attack = 1,
				PowerUp = 2, PosStates = 3,
				PowerDown = 4, NegStates = 5,
				Escape = 6, Unset = 0
            }

            /// <summary>
            /// 属性
            /// </summary>
            [AutoConvert]
            public int[] rounds { get; protected set; }
            [AutoConvert]
            public int type { get; protected set; }
            [AutoConvert("params")]
            public JsonData params_ { get; protected set; }
            [AutoConvert]
            public int rate { get; protected set; }

            /// <summary>
            /// 获取类型枚举
            /// </summary>
            /// <returns></returns>
            public Type typeEnum() {
                return (Type)type;
            }

            /// <summary>
            /// 是否为逃跑
            /// </summary>
            /// <returns></returns>
            public bool isEscape() {
                return type == (int)Type.Escape;
            }

            /// <summary>
            /// 是否无操作
            /// </summary>
            /// <returns></returns>
            public bool isUnset() {
                return type == (int)Type.Unset;
            }

            /// <summary>
            /// 测试回合
            /// </summary>
            /// <param name="round">回合</param>
            /// <returns>返回指定回合能否发动本行动</returns>
            public bool testRound(int round) {
                if (rounds.Length <= 0) return true;
                foreach (var r in rounds) if (r == round) return true;
                return false;
            }
        }

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int mhp { get; protected set; }
        [AutoConvert]
        public int power { get; protected set; }
        [AutoConvert]
        public int defense { get; protected set; }
        [AutoConvert("type_")]
        public int type_ { get; protected set; }
		[AutoConvert("character")]
		public string _character { get; protected set; } // 性格

		/// <summary>
		/// 首字母大写
		/// </summary>
		public string character {
			get {
				if (_character == "") return "";
				var s = _character;
				return s.Substring(0, 1).ToUpper() + s.Substring(1);
			}
		}

		[AutoConvert]
        public Action[] actions { get; protected set; }
        [AutoConvert]
        public ExerProEffectData[] effects { get; protected set; }

        public Texture2D battle { get; protected set; }

        /// <summary>
        /// 类型文本
        /// </summary>
        /// <returns></returns>
        public string typeText() {
            return DataService.get().enemyType(type_).Item2;
        }

        /// <summary>
        /// 数据加载
        /// </summary>
        /// <param name="json">数据</param>
        protected override void loadCustomAttributes(JsonData json) {
            base.loadCustomAttributes(json);
            battle = AssetLoader.loadExerProEnemyBattle(id);
        }
    }

	/// <summary>
	/// 特训状态数据
	/// </summary>
	public class ExerProState : BaseItem {

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public int iconIndex { get; protected set; }
		[AutoConvert]
		public int maxTurns { get; protected set; } // 最大状态叠加回合数
		[AutoConvert]
		public bool isNega { get; protected set; } // 是否负面

		[AutoConvert]
		public ExerProTraitData[] traits { get; protected set; }


        /// <summary>
        /// 物品图标
        /// </summary>
        public Sprite icon { get; protected set; }

        /// <summary>
        /// 加载自定义属性
        /// </summary>
        /// <param name="json"></param>
        protected override void loadCustomAttributes(JsonData json) {
            base.loadCustomAttributes(json);
            icon = AssetLoader.getExerProStateIconSprite(iconIndex);
        }
    }

    #endregion

    #region 容器项

    /// <summary>
    /// 特训背包物品
    /// </summary>
    public class ExerProPackItem<T> : PackContItem<T>
		where T : BaseExerProItem {

        /// <summary>
        /// 使用对象池
        /// </summary>
        /// <returns></returns>
        protected override bool useObjectsPool() {
            return true;
        }

        /// <summary>
        /// 自动分配ID
        /// </summary>
        /// <returns></returns>
        protected override bool autoId() {
            return true;
        }

        /// <summary>
        /// 获取效果数组
        /// </summary>
        /// <returns></returns>
        public ExerProEffectData[] effects() {
            return item().effects;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExerProPackItem() : base() { }
        /// <param name="itemId">物品ID</param>
        /// <param name="count">数量</param>
        public ExerProPackItem(int itemId, int count = 1) : base(itemId, count) { }
        /// <param name="item">物品</param>
        public ExerProPackItem(T item, int count = 1) : base(item, count) { }

    }

    /// <summary>
    /// 特训背包道具
    /// </summary>
    public class ExerProPackItem : ExerProPackItem<ExerProItem> {

		/// <summary>
		/// 特性
		/// </summary>
		/// <returns></returns>
		public ExerProTraitData[] traits() {
			return item().traits;
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		public ExerProPackItem() { }
		public ExerProPackItem(ExerProItem item) : base(item) { }
        public static ExerProPackItem sample() {

            ExerProPackItem sample = new ExerProPackItem(ExerProItem.sample());
            sample.count = UnityEngine.Random.Range(1, 10);
            return sample;
	}


    }

    /// <summary>
    /// 特训背包药水
    /// </summary>
    public class ExerProPackPotion : ExerProPackItem<ExerProPotion> {

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExerProPackPotion() { }
        public ExerProPackPotion(ExerProPotion potion) : base(potion) { }
        public static ExerProPackPotion sample() {
            ExerProPackPotion sample = new ExerProPackPotion(ExerProPotion.sample());

            return sample;
    }
    }

    /// <summary>
    /// 特训背包卡片
    /// </summary>
    public class ExerProPackCard : ExerProPackItem<ExerProCard> {
		/// <summary>
		/// 消耗能量
		/// </summary>
		/// <returns></returns>
		public int cost() {
			var item = this.item();
			if (item == null) return 0;
			return item.cost;
		}

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExerProPackCard() { }
        public ExerProPackCard(ExerProCard card) : base(card) { }

        public static ExerProPackCard sample() {
            ExerProPackCard sample = new ExerProPackCard(ExerProCard.sample());

            return sample;
    }
        public static ExerProPackCard[] ExerProCard2ExerProPackCard(ExerProCard[] cards) {
            var cnt = cards.Length;
            var packCards = new ExerProPackCard[cnt];
            for (int i = 0; i < cnt; ++i)
                packCards[i] = new ExerProPackCard(cards[i]);
            return packCards;
    }
    }

    /// <summary>
    /// 特训药水槽项
    /// </summary>
    public class ExerProSlotPotion : SlotContItem<ExerProPackPotion> {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int packPotionId { get; protected set; }

        /// <summary>
        /// 药水槽
        /// </summary>
        public ExerProPotionSlot potionSlot { get; set; }

        /// <summary>
        /// 玩家艾瑟萌实例
        /// </summary>
        ExerProPackPotion _packPotion;
        public ExerProPackPotion packPotion {
            get {
                if (_packPotion == null) {
                    var potionPack = potionSlot.actor.potionPack;
                    _packPotion = potionPack.findItem(item => item.id == packPotionId);
                }
                return _packPotion;
            }
            set {
                if (value == null) packPotionId = 0;
                else packPotionId = value.id;
                _packPotion = null;
            }
        }

		/// <summary>
		/// 装备
		/// </summary>
		public override ExerProPackPotion equip1 {
			get { return packPotion; }
			protected set { packPotion = value; }
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		public ExerProSlotPotion() { }
		public ExerProSlotPotion(ExerProPotionSlot slot, int index) {
			potionSlot = slot; this.index = index;
		}
	}

    #endregion

    #region 容器

	/// <summary>
	/// 特训物品背包
	/// </summary>
	public class ExerProItemPack : PackContainer<ExerProPackItem> {

		/// <summary>
		/// 特性
		/// </summary>
		/// <returns></returns>
		public List<ExerProTraitData> traits() {
			var res = new List<ExerProTraitData>();
			foreach (var item in items)
				res.AddRange(item.traits());
			return res;
		}

	}

	/// <summary>
	/// 特训药水背包
	/// </summary>
	public class ExerProPotionPack : PackContainer<ExerProPackPotion> { }

    /// <summary>
    /// 特训抽牌堆
    /// </summary>
    public class ExerProCardDrawGroup : PackContainer<ExerProPackCard> {

        /// <summary>
        /// 卡组
        /// </summary>
        public ExerProCardGroup cardGroup { get; set; }

        /// <summary>
        /// 接受物品
        /// </summary>
        /// <param name="item"></param>
        protected override bool acceptItem(ExerProPackCard item) {
            // 卡牌进入的时候洗牌
            if (isFull()) return false;
            if (containItem(item)) return false;
            var index = UnityEngine.Random.Range(0, items.Count);
            items.Insert(index, item);
            return true;
        }

        /// <summary>
        /// 洗牌
        /// </summary>
        public void shuffle() {
            var tmpItems = items.ToArray();
            foreach (var item in tmpItems)
                transferItem(this, item);
        }

        /// <summary>
        /// 获取第一个卡牌（最后一张）
        /// </summary>
        /// <returns></returns>
        public ExerProPackCard firstCard() {
			if (items.Count <= 0) return null;
            return items[items.Count - 1];
        }
    }

    /// <summary>
    /// 特训弃牌堆
    /// </summary>
    public class ExerProCardDiscardGroup : PackContainer<ExerProPackCard> {

        /// <summary>
        /// 卡组
        /// </summary>
        public ExerProCardGroup cardGroup { get; set; }
    }

    /// <summary>
    /// 特训手牌
    /// </summary>
    public class ExerProCardHandGroup : PackContainer<ExerProPackCard> {

        /// <summary>
        /// 常量定义
        /// </summary>
        public const int DefaultCapacity = 10;

        /// <summary>
        /// 卡组
        /// </summary>
        public ExerProCardGroup cardGroup { get; set; }

        /// <summary>
        /// 默认容量
        /// </summary>
        /// <returns></returns>
        public override int defaultCapacity() {
            return DefaultCapacity;
        }

        /// <summary>
        /// 抽出的牌
        /// </summary>
        List<ExerProPackCard> drawnCards = new List<ExerProPackCard>();

		/// <summary>
		/// 获取并清除抽取的卡牌
		/// </summary>
		public ExerProPackCard[] getDrawnCards() {
			var res = drawnCards.ToArray();
			clearDrawnCards(); return res;
		}

		/// <summary>
		/// 获取并清除抽取的卡牌
		/// </summary>
		public void clearDrawnCards() {
			drawnCards.Clear();
		}

		/// <summary>
		/// 接收物品
		/// </summary>
		/// <param name="item"></param>
		protected override bool acceptItem(ExerProPackCard item) {
            var res = base.acceptItem(item);
            if (res) drawnCards.Add(item);
            return res;
        }
    }

    /// <summary>
    /// 特训卡组
    /// </summary>
    public class ExerProCardGroup : PackContainer<ExerProPackCard> {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public ExerProCardDrawGroup drawGroup { get; protected set; } = new ExerProCardDrawGroup();
        [AutoConvert]
        public ExerProCardDiscardGroup discardGroup { get; protected set; } = new ExerProCardDiscardGroup();
        [AutoConvert]
        public ExerProCardHandGroup handGroup { get; protected set; } = new ExerProCardHandGroup();

		#region 记录控制

		/// <summary>
		/// 出牌记录
		/// </summary>
		[AutoConvert("cardRecord")]
		public List<int> _cardRecord { get; protected set; } = new List<int>();

		/// <summary>
		/// 获取出牌记录的卡牌实例
		/// </summary>
		/// <returns></returns>
		public List<ExerProCard> cardRecord() {
			Debug.Log("_cardRecord: " + string.Join(",", _cardRecord));
			var res = new List<ExerProCard>(_cardRecord.Count);
			foreach (var cid in _cardRecord)
				res.Add(DataService.get().exerProCard(cid));
			return res;
		}

		#endregion

		#region 加入卡组

		/// <summary>
		/// 加入卡组
		/// </summary>
		public void addCard(ExerProCard card) {
            pushItem(new ExerProPackCard(card));
        }

        #endregion

        #region 卡牌转移

        /// <summary>
        /// 战斗开始，生成牌堆
        /// </summary>
        public void onBattleStart() {
            var tmpItems = items.ToArray();
            foreach (var item in tmpItems)
                if (item.item().inherent) // 固有牌
                    transferItem(handGroup, item);
                else
                    transferItem(drawGroup, item);
			handGroup.clearDrawnCards();
		}

        /// <summary>
        /// 战斗结束，回收牌堆
        /// </summary>
        public void onBattleEnd() {
            recycle(drawGroup); recycle(discardGroup); recycle(handGroup);
        }

        /// <summary>
        /// 回合结束，自动弃牌
        /// </summary>
        public void onRoundEnd() {
            var tmpItems = handGroup.items.ToArray();
            foreach (var item in tmpItems)
                handGroup.transferItem(discardGroup, item);
        }

        /// <summary>
        /// 回收牌堆
        /// </summary>
        public void recycle(PackContainer<ExerProPackCard> container) {
            var tmpItems = container.items.ToArray();
            foreach (var item in tmpItems)
                container.transferItem(this, item);
        }

        /// <summary>
        /// 抽牌
        /// </summary>
        public void drawCard() {
			if (drawGroup.items.Count <= 0) recycleDiscard();
			var card = drawGroup.firstCard();
			drawGroup.transferItem(handGroup, card);
			debugShow("drawCard: " + card.item().name);
		}

		/// <summary>
		/// 回收弃牌
		/// </summary>
		public void recycleDiscard() {
			var tmpItems = discardGroup.items.ToArray();
			foreach (var item in tmpItems)
				discardGroup.transferItem(drawGroup, item);
		}

		/// <summary>
		/// 使用牌
		/// </summary>
		public void useCard(ExerProPackCard card) {
			debugShow("useCard: " + card.item().name);
			_cardRecord.Add(card.itemId);
			if (card.item().disposable) consumeCard(card);
            else discardCard(card);
        }

        /// <summary>
        /// 弃牌
        /// </summary>
        public void discardCard(ExerProPackCard card) {
			handGroup.transferItem(discardGroup, card);
			debugShow("discardCard: " + card.item().name);
		}
		public void discardCard() {
            var card = handGroup.getRandomItem(); // 随机
            if (card == null) return;
			discardCard(card);
			//handGroup.transferItem(discardGroup, card);
        }

        /// <summary>
        /// 消耗牌（本次战斗不再出现）
        /// </summary>
        public void consumeCard(ExerProPackCard card) {
			handGroup.transferItem(this, card);
			debugShow("consumeCard: " + card.item().name);
		}
		public void consumeCard() {
            var card = handGroup.getRandomItem(); // 随机
            if (card == null) return;
			consumeCard(card);
        }

		/// <summary>
		/// 显示调试信息
		/// </summary>
		void debugShow(string oper = "") {
			var format = "Debug: {0}\nCard Group: {1}\n" +
				"Draw Group: {2}\nDiscard Group: {3}\nHand Group: {4}";
			Debug.Log(string.Format(format, oper, debugCards(this), 
				debugCards(drawGroup), debugCards(discardGroup), debugCards(handGroup)));
		}

		/// <summary>
		/// 显示调试卡牌
		/// </summary>
		/// <param name="container"></param>
		/// <returns></returns>
		string debugCards(PackContainer<ExerProPackCard> container) {
			var res = container.items.Count.ToString();
			foreach (var card in container.items)
				res += " " + card.item().name;
			return res;
		}

        #endregion

        /// <summary>
        /// 读取自定义数据
        /// </summary>
        /// <param name="json"></param>
        protected override void loadCustomAttributes(JsonData json) {
            base.loadCustomAttributes(json);
            drawGroup.cardGroup = discardGroup.cardGroup = handGroup.cardGroup = this;
        }

        /// <summary>
        /// 获取玩家卡组数目接口
        /// </summary>
        /// <returns></returns>
        public int getCardNumber() {
            return items.Count + handGroup.items.Count +
				drawGroup.items.Count + discardGroup.items.Count;
        }
    }

    /// <summary>
    /// 特训药水槽
    /// </summary>
    public class ExerProPotionSlot : SlotContainer<ExerProSlotPotion> {

		/// <summary>
		/// 槽数
		/// </summary>
		public const int SlotItemCount = 3;

		/// <summary>
		/// 所属玩家
		/// </summary>
		public RuntimeActor actor { get; set; }

		/// <summary>
		/// 默认容量
		/// </summary>
		/// <returns></returns>
		public override int defaultCapacity() {
			return SlotItemCount;
		}

		/// <summary>
		/// 配置槽项（初始化）
		/// </summary>
		void setupSlotItems() {
            for (int i = 0; i < SlotItemCount; ++i)
				items.Add(new ExerProSlotPotion(this, i));
		}

        /// <summary>
        /// 通过装备物品获取槽ID
        /// </summary>
        /// <typeparam name="E">装备物品类型</typeparam>
        /// <param name="equipItem">装备物品</param>
        /// <returns>槽ID</returns>
        public override ExerProSlotPotion getSlotItemByEquipItem<E>(E equipItem) {
			foreach (var slotItem in items)
				if (!slotItem.isNullItem() && slotItem.packPotionId == equipItem.id)
					return slotItem;
            return emptySlotItem();
        }

        /// <summary>
        /// 读取物品
        /// </summary>
        /// <param name="json">数据</param>
        /// <returns>物品对象</returns>
        protected override ExerProSlotPotion loadItem(JsonData json) {
            var slotItem = base.loadItem(json);
            slotItem.potionSlot = this;
            return slotItem;
        }

		/// <summary>
		/// 构造函数
		/// </summary>
		public ExerProPotionSlot() { }
		public ExerProPotionSlot(RuntimeActor actor) {
			this.actor = actor; setupSlotItems();
		}
	}

    #endregion

    #region 地图

    /// <summary>
    /// 地图阶段数据
    /// </summary>
    public class ExerProMap : BaseData {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public string name { get; protected set; }
        [AutoConvert]
        public string description { get; protected set; }
        [AutoConvert]
        public int level { get; protected set; }
        [AutoConvert]
        public int minLevel { get; protected set; }

        [AutoConvert]
        public ExerProMapStage[] stages { get; protected set; }

        /// <summary>
        /// 获取关卡
        /// </summary>
        /// <param name="order">序号</param>
        /// <returns>返回对应序号的关卡</returns>
        public ExerProMapStage stage(int order) {
            foreach (var stage in stages)
                if (stage.order == order) return stage;
            return null;
        }

        /// <summary>
        /// 读取自定义数据
        /// </summary>
        /// <param name="json"></param>
        protected override void loadCustomAttributes(JsonData json) {
            base.loadCustomAttributes(json);
            foreach (var stage in stages) stage.map = this;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExerProMap() { }
        public ExerProMap(string name, int level, int minLevel, ExerProMapStage[] stages) {
            this.name = name; this.level = level; this.minLevel = minLevel;
            this.stages = stages;
        }
    }

    /// <summary>
    /// 地图关卡
    /// </summary>
    public class ExerProMapStage : BaseData {

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
        public int order { get; protected set; }
        [AutoConvert("enemies")]
        public int[] _enemies { get; protected set; }
        [AutoConvert]
        public int maxBattleEnemies { get; protected set; }
        [AutoConvert]
        public int[] steps { get; protected set; }
        [AutoConvert]
        public int maxForkNode { get; protected set; }
        [AutoConvert]
        public int maxFork { get; protected set; }
        [AutoConvert]
        public int[] nodeRate { get; protected set; }

        /// <summary>
        /// 地图
        /// </summary>
        public ExerProMap map { get; set; }

        /// <summary>
        /// 缓存敌人集合
        /// </summary>
        List<ExerProEnemy> tmpEnemies;

        /// <summary>
        /// 敌人数组
        /// </summary>
        /// <returns>返回敌人数组</returns>
        public List<ExerProEnemy> enemies() {
            if (tmpEnemies == null) {
				if (_enemies == null) return null;
				tmpEnemies = new List<ExerProEnemy>(_enemies.Length);
                foreach (var enemy in _enemies)
					tmpEnemies.Add(DataService.get().exerProEnemy(enemy));
			}
            return tmpEnemies;
        }

        /// <summary>
        /// BOSS数组
        /// </summary>
        /// <returns>返回BOSS敌人数组</returns>
        public List<ExerProEnemy> bosses() {
			return enemies().FindAll(e => e.type_ == (int)ExerProEnemy.EnemyType.Boss);
        }

        /// <summary>
        /// 普通敌人数组
        /// </summary>
        /// <returns>返回普通敌人数组</returns>
        public List<ExerProEnemy> normalEnemies() {
			return enemies().FindAll(e => e.type_ == (int)ExerProEnemy.EnemyType.Normal);
        }

        /// <summary>
        /// 精英数组
        /// </summary>
        /// <returns>返回精英敌人数组</returns>
        public List<ExerProEnemy> eliteEnemies() {
			return enemies().FindAll(e => e.type_ == (int)ExerProEnemy.EnemyType.Elite);
        }

		/// <summary>
		/// 测试
		/// </summary>
		/// <param name="json"></param>
		protected override void loadCustomAttributes(JsonData json) {
			base.loadCustomAttributes(json);
            Debug.Log("loadCustomAttributes: " + json.ToJson());
		}

        /*
        /// <summary>
        /// 构造函数
        /// </summary>
        public ExerProMapStage() { }
        public ExerProMapStage(int order, int[] _enemies, int maxBattleEnemies,
            int[] steps, int maxForkNode, int maxFork, int[] nodeRate) {
            this.order = order; this._enemies = _enemies;
            this.maxBattleEnemies = maxBattleEnemies;
            this.steps = steps; this.maxFork = maxFork;
            this.maxForkNode = maxForkNode;
            this.nodeRate = nodeRate;
        }
        */

        #region Unit Test
        /// <summary>
        /// 测试变量
        /// 其中index = 0为默认情况（数据库定义）；
        /// index = 1对应小规模地图，分布较均匀，参数约束较严格
        /// index = 2对应小规模地图，分布波动非常大，参数约束中等
        /// index = 3对应大规模地图，
        /// index = 4对应大规模地图，
        /// index = 5对应大规模地图，
        /// </summary>
        List<int[]> testSteps = new List<int[]> { new int[] { 3, 4, 5, 2, 3, 5, 4, 3, 4, 3, 2, 4, 4, 2, 5, 1 },
            new int[] { 4, 1 }, new int[] { 2, 5, 1, 4, 3, 6, 1 },
            new int[] { 3, 5, 7, 4, 2, 3, 5, 4, 6, 3, 3, 4, 2, 1 },
            new int[] { 3, 5, 7, 4, 2, 3, 5, 4, 6, 3, 3, 4, 2, 1 },
            new int[] { 3, 5, 7, 4, 2, 3, 5, 4, 6, 3, 3, 4, 2, 1 },};
        List<int> testMaxForkNodes = new List<int> { 10, 2, 5, 5, 8, 10};
        List<int> testMaxForks = new List<int> { 3, 2, 3, 3, 3, 3};

        List<int[]> testNodeRates = new List<int[]> { new int[] { 2, 1, 1, 1, 1, 4 },
            new int[] { 2, 0, 0, 0, 0, 0 }, new int[]{ 0, 1} , new int[]{ 0, 0,1}  };

        static int index = 0;
        /// <summary>
        /// 改变内置参数进行测试
        /// </summary>
        /// <param name="steps"></param>
        /// <param name="maxForkNode"></param>
        /// <param name="maxFork"></param>
        public void changeStageParam() {
            this.steps = testSteps[index];
            this.maxForkNode = testMaxForkNodes[index];
            this.maxFork = testMaxForks[index];
            index++;
        }

        public void changeNodeRate() {
            this.nodeRate = testNodeRates[index];
            index++;
        }
        #endregion
    }

    #endregion

    #region 储存信息

    /// <summary>
    /// 特训记录
    /// </summary>
    public class ExerProRecord : BaseData, ParamDisplay.IDisplayDataConvertable {

		/// <summary>
		/// 常量定义 
		/// </summary>
        public const int DefaultGold = 100; // 初始金币

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int mapId { get; protected set; }
        [AutoConvert]
        public int stageOrder { get; protected set; }

        /// <summary>
        /// 是否开始（游戏是否一开始，尚未结束）
        /// </summary>
        [AutoConvert]
        public bool started { get; protected set; } = false;

        /// <summary>
        /// 地图是否生成（本关卡的地图是否已经生成）
        /// </summary>
        [AutoConvert]
        public bool generated { get; protected set; } = false;

        /// <summary>
        /// 生成的据点
        /// </summary>
        [AutoConvert]
        public List<ExerProMapNode> nodes { get; protected set; } = new List<ExerProMapNode>();

        /// <summary>
        /// 角色属性
        /// </summary>
        [AutoConvert]
        public int curIndex { get; protected set; } = -1; // 当前节点索引
        [AutoConvert]
		public int gold { get; protected set; } = DefaultGold; // 金币
		[AutoConvert]
        public bool nodeFlag { get; set; } = false; // 是否完成据点事件
        [AutoConvert]
        public RuntimeActor actor { get; protected set; } = null;

        /// <summary>
        /// 单词轮属性
        /// </summary>
        [AutoConvert]
        public int wordLevel { get; protected set; } = 1;

        [AutoConvert(autoConvert: false)]
        public List<WordRecord> wordRecords { get; protected set; }

        /// <summary>
		/// 积分记录
		/// </summary>
		[AutoConvert]
		public ScoreRecord scoreRecord { get; protected set; } = new ScoreRecord();

		/// <summary>
        /// 下一单词ID
        /// </summary>
        public int next { get; set; } = 0;

        /*
        [AutoConvert]
        public int sum { get; protected set; }
        [AutoConvert]
        public int correct { get; protected set; }
        [AutoConvert]
        public int wrong { get; protected set; }
        */

        /// <summary>
        /// 敌人（缓存用）
        /// </summary>
        List<ExerProEnemy> _enemies = null;

        #region 数据转化

        /// <summary>
        /// 转化为显示数据
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public JsonData convertToDisplayData(string type = "") {
            switch (type) {
                case "map_progress": return convertMapProgressData();
                case "word_progress": return convertWordProgressData();
                default: return toJson();
            }
        }

        /// <summary>
        /// 转化地图进度数据
        /// </summary>
        /// <returns></returns>
        JsonData convertMapProgressData() {
            var node = currentNode();
            var last = lastNode();
            var res = new JsonData();

            if (node != null && last != null)
                res["rate"] = (node.xOrder * 1.0 / last.xOrder);

            return res;
        }

        /// <summary>
        /// 转化单词进度数据
        /// </summary>
        /// <returns></returns>
        JsonData convertWordProgressData() {
            var res = new JsonData();
            var sum = wordRecords.Count;
            var corrCnt = getCorrCnt();
            var wrongCnt = getWrongCnt();

            var corrRate = corrCnt * 1.0 / sum;
            var wrongRate = corrRate + wrongCnt * 1.0 / sum;
            var rest = sum - corrCnt - wrongCnt;

            res["level"] = wordLevel;
            res["sum"] = sum;
            res["corr_cnt"] = corrCnt;
            res["wrong_cnt"] = wrongCnt;
            res["rest"] = rest;

            res["corr_rate"] = corrRate;
            res["wrong_rate"] = wrongRate;

            return res;
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 获取关卡
        /// </summary>
        /// <param name="order">序号</param>
        /// <returns>返回对应序号的关卡</returns>
        public ExerProMap map() {
            return DataService.get().exerProMap(mapId);
        }

        /// <summary>
        /// 获取关卡
        /// </summary>
        /// <param name="order">序号</param>
        /// <returns>返回对应序号的关卡</returns>
        public ExerProMapStage stage() {
            return map()?.stage(stageOrder);
        }

        /// <summary>
		/// 得到金钱
        /// </summary>
		/// <param name="val"></param>
		public void gainGold(int val) {
			gold = Math.Max(gold + val, 0);
        } 

        #endregion

        #region 据点控制

        /// <summary>
        /// 生成地图
        /// </summary>
        void generate() {
			if (generated) return;
            generated = CalcService.NodeGenerator.generate(this);
			setupCards(); refreshNodeStatuses();
        }

        /// <summary>
		/// 创建据点
        /// </summary>
		/// <param name="xOrder">X序号</param>
		/// <param name="yOrder">Y序号</param>
		/// <param name="type">据点类型</param>
		public ExerProMapNode createNode(int xOrder, int yOrder, ExerProMapNode.Type type) {
			var node = new ExerProMapNode(nodes.Count, this, xOrder, yOrder, type);
			nodes.Add(node); return node;
        }

        /// <summary>
        /// 获取据点对象
        /// </summary>
        /// <param name="id">据点ID</param>
        /// <returns>返回据点对象</returns>
        public ExerProMapNode getNode(int id) {
            if (id < 0 || id >= nodes.Count) return null;
            return nodes[id]; // nodes.Find(node => node.id == id);
        }
        /// <param name="xOrder">X顺序</param>
        /// <param name="yOrder">Y顺序</param>
        public ExerProMapNode getNode(int xOrder, int yOrder) {
            return nodes.Find(node => node.xOrder == xOrder &&
                node.yOrder == yOrder);
        }

        /// <summary>
        /// 初始据点
        /// </summary>
        /// <returns></returns>
        public List<ExerProMapNode> firstNodes() {
            return nodes.FindAll(node => node.xOrder == 0);
        }

        /// <summary>
        /// 最后据点
        /// </summary>
        /// <returns></returns>
        public ExerProMapNode lastNode() {
            if (nodes.Count <= 0) return null;
            return nodes[nodes.Count - 1];
        }

        /// <summary>
        /// 获取当前据点
        /// </summary>
        /// <returns>返回当前据点对象</returns>
        public ExerProMapNode currentNode() {
            return getNode(curIndex);
        }

        /// <summary>
        /// 是否已选择起点
        /// </summary>
        /// <returns></returns>
        public bool isFirstSelected() {
            return curIndex >= 0;
        }

		#endregion

		#region 记录控制

        /// <summary>
        /// 下一个单词
        /// </summary>
        /// <returns></returns>
        public Word nextWord() {
			Debug.Log("nextWord: " + next);
            if (next <= 0) return wordRecords[0].word();
            return EnglishService.get().getQuestion<Word>(next);
        }

		/// <summary>
		/// 获取指定ID的单词记录
		/// </summary>
		/// <param name="wid"></param>
		/// <returns></returns>
		public WordRecord wordRecord(int wid) {
			return wordRecords.Find(rec => rec.wordId == wid);
		}

        /// <summary>
		/// 已有记录的单词ID
        /// </summary>
        /// <returns></returns>
        public int[] recordWordIds() {
            var cnt = wordRecords.Count;
            var res = new int[cnt];
            for (int i = 0; i < cnt; ++i)
                res[i] = wordRecords[i].wordId;
            return res;
        }

		/// <summary>
		/// 回答单词
		/// </summary>
		/// <param name="wid">单词ID</param>
		/// <param name="correct">是否正确</param>
		public void answerWord(int wid, bool correct, int next) {
			this.next = next;
			var rec = wordRecord(wid);
			rec.answer(correct);
		}

        /// <summary>
        /// 获取本轮单词正确数量
        /// </summary>
        /// <returns></returns>
        public int getCorrCnt() {
            int cnt = 0;
            foreach (var record in wordRecords)
                if (record.isCurrentCorrect()) cnt++;
            return cnt;
        }

        /// <summary>
        /// 获取本轮单词错误数量
        /// </summary>
        /// <returns></returns>
        public int getWrongCnt() {
            int cnt = 0;
            foreach (var record in wordRecords)
                if (record.isCurrentWrong()) cnt++;
            return cnt;
        }

        #endregion
       
        #region 游戏控制

        /// <summary>
        /// 开始特训
        /// </summary>
        public void start() {
			createActor(); generate();
        }

        /*
        /// <summary>
        /// 设置地图
        /// </summary>
        public void setup(bool restart = false) {
            reset(restart); createActor(); generate();
        }

        /// <summary>
        /// 重置地图
        /// </summary>
        /// <param name="restart">重新开始</param>
        void reset(bool restart = false) {
            if (restart) {
                started = false; actor = null;
            }
            resetMap();
        }

        /// <summary>
        /// 重置地图
        /// </summary>
        /// <param name="restart">重新开始</param>
        void resetMap() {
            _enemies = null; generated = false; nodes.Clear();
        }
        */

		/// <summary>
		/// 配置初始卡牌
		/// </summary>
		public void setupCards() {
			var group = DataService.get().firstCardGroup(1);
			var cards = group.getCards();
			foreach (var card in cards)
				actor.cardGroup.addCard(card);
		}

        /// <summary>
        /// 重置单词
        /// </summary>
        /// <param name="words">单词集合</param>
        public void setupWords(Word[] words) {
            if (words.Length > 0) next = words[0].id;
            else next = 0;
        }

        /// <summary>
        /// 走到下一步
        /// </summary>
        /// <param name="id">下一步的据点ID</param>
        /// <param name="force">强制转移</param>
        /// <param name="emit">是否发射事件</param>
        public void moveNext(int id, bool force = false) {
            if (force) changePosition(id);
            else {
                var node = currentNode();
                if (node == null) return;
                foreach (var next in node.nexts)
                    if (id == next) changePosition(id);
            }
        }

        /// <summary>
        /// 更改位置
        /// </summary>
        /// <param name="id">新结点ID</param>
        /// <param name="emit">是否发射事件</param>
        void changePosition(int id) {
            curIndex = id; onMoved();
        }

        /// <summary>
        /// 移动结束回调
        /// </summary>
        void onMoved() {
            refreshNodeStatuses();
        }

        /// <summary>
        /// 刷新据点状态
        /// </summary>
        void refreshNodeStatuses() {
            var current = currentNode();
            foreach (var node in nodes) {
                if (node.status == (int)ExerProMapNode.Status.Passed) continue;

                var status = ExerProMapNode.Status.Deactive;

                if (current != null) {
                    if (node == current)
                        status = ExerProMapNode.Status.Current;
                    else if (node.xOrder < current.xOrder)
                        status = ExerProMapNode.Status.Over;
                    else if (node.status == (int)ExerProMapNode.Status.Current)
                        status = ExerProMapNode.Status.Passed;
                    else if (current.nexts.Contains(node.id))
                        status = ExerProMapNode.Status.Active;
                }
                else if (node.xOrder <= 0)
                    status = ExerProMapNode.Status.Active;

                node.setStatus(status);
            }
        }

        #endregion

        #region 数据读取

        /// <summary>
        /// 读取自定义数据
        /// </summary>
        /// <param name="json"></param>
        protected override void loadCustomAttributes(JsonData json) {
            base.loadCustomAttributes(json);

            nodes = nodes ?? new List<ExerProMapNode>();

            for (int i = 0; i < nodes.Count; ++i) {
                nodes[i].setId(i); nodes[i].stage = this;
            }

            if (DataLoader.contains(json, "words")) {
                var words = DataLoader.load<Word[]>(json, "words");
                EnglishService.get().questionCache.addQuestions(words);
                setupWords(words);
            }
            // generate();
        }

        /// <summary>
        /// 创建一个Actor
        /// </summary>
        public void createActor() {
            var player = PlayerService.get().player;
            if (player == null) return;

			if (actor == null)
				actor = new RuntimeActor(this, player);
            else actor.setupPlayer(player);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExerProRecord() { }
        public ExerProRecord(ExerProMapStage stage) : this(stage.map.id, stage.order) { }
        public ExerProRecord(ExerProMap map, int order) : this(map.id, order) { }

        public ExerProRecord(int mapId, int stageOrder) {
            this.mapId = mapId; this.stageOrder = stageOrder;
            generate();
        }

        #endregion
    }

	/// <summary>
	/// 积分记录
	/// </summary>
    public class ScoreRecord : BaseData {

        /// <summary>
        /// 积分计算的相关数据
        /// </summary>
        [AutoConvert]
        public int killEnemyAccmu { get; set; } = 0;
        [AutoConvert]
        public int killBossAccmu { get; set; } = 0;
        [AutoConvert]
        public int perfectNumber { get; set; } = 0;
        [AutoConvert]
        public int stageOrderAccumu { get; set; } = 0;
    }

    /// <summary>
    /// 据点类型
    /// </summary>
    public class NodeType : TypeData {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public string quesTypes { get; protected set; }

        /// <summary>
        /// 图标
        /// </summary>
        public Texture2D icon { get; protected set; }

        /// <summary>
        /// 读取自定义属性
        /// </summary>
        /// <param name="json"></param>
        protected override void loadCustomAttributes(JsonData json) {
            base.loadCustomAttributes(json);
            icon = AssetLoader.loadNodeIcon(id);
        }

    }

    /// <summary>
    /// 地图据点
    /// </summary>
    public class ExerProMapNode : BaseData {

        /// <summary>
        /// 最大偏移量
        /// </summary>
        const int MaxXOffset = 24;
        const int MaxYOffset = 16;

        /// <summary>
        /// 据点类型
        /// </summary>
        public enum Type {
            Rest = 1, //休息据点
            Treasure = 2, //藏宝据点
            Shop = 3, //商人据点
            Enemy = 4, //敌人据点
            Elite = 5, //精英据点
            Unknown = 6, //未知据点
            Boss = 7, // 最终BOSS
            Story = 8, // 剧情据点
        }

        /// <summary>
        /// 状态类型
        /// </summary>
        public enum Status {
            Deactive = 0, // 未激活（未到达）
            Active = 1, // 已激活（下一步）
            Current = 2, // 当前
            Passed = 3, // 已经过
            Over = 4, // 已结束
        }

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int xOrder { get; protected set; }
        [AutoConvert]
        public int yOrder { get; protected set; }
        [AutoConvert]
        public double xOffset { get; protected set; }
        [AutoConvert]
        public double yOffset { get; protected set; }
        [AutoConvert]
        public int typeId { get; protected set; }

        /// <summary>
		/// 实际类型ID
		/// </summary>
		public int realTypeId { get; set; } = 0;

		/// <summary>
        /// 下一个Y序号（数组）
        /// </summary>
        [AutoConvert]
        public HashSet<int> nexts { get; protected set; } = new HashSet<int>();
        [AutoConvert]
        public int status { get; protected set; }

        /// <summary>
        /// 地图关卡
        /// </summary>
        public ExerProRecord stage { get; set; }

        /// <summary>
        /// 分叉标记
        /// </summary>
        public bool fork = false;

        #region 生成

        /// <summary>
        /// 生成位置
        /// </summary>
        public void generatePosition() {
            xOffset = UnityEngine.Random.Range(-MaxXOffset, MaxXOffset);
            yOffset = UnityEngine.Random.Range(-MaxYOffset, MaxYOffset);
        }

        /// <summary>
        /// 设置ID
        /// </summary>
        /// <param name="id"></param>
        public void setId(int id) {
            this.id = id;
        }

        /// <summary>
        /// 添加下一步
        /// </summary>
        /// <param name="next"></param>
        public void addNext(int next) {
            nexts.Add(next);
        }
        public void addNext(ExerProMapNode next) {
            addNext(next.id);
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 获取类型
        /// </summary>
        /// <returns></returns>
        public NodeType type() {
            return DataService.get().nodeType(typeId);
        }

        /// <summary>
        /// 获取类型枚举
        /// </summary>
        /// <returns></returns>
		public Type typeEnum(bool real = true) {
			if (realTypeId > 0 && real)
				return (Type)realTypeId;
            return (Type)typeId;
        }

        /// <summary>
        /// 获取下一节点
        /// </summary>
        /// <returns></returns>
        public List<ExerProMapNode> getNexts() {
            if (stage == null) return null;
            var nodes = new List<ExerProMapNode>();
            foreach (var next in nexts)
                nodes.Add(stage.nodes[next]);
            return nodes;
        }

        /// <summary>
        /// 设置状态
        /// </summary>
        /// <param name="status">状态</param>
        public void setStatus(Status status) {
            this.status = (int)status;
        }
        public void setStatus(int status) {
            this.status = status;
        }

        /// <summary>
        /// 是否当前据点
        /// </summary>
        public bool isCurrent() {
            return status == (int)Status.Current;
        }

		/// <summary>
		/// 是否BOSS据点
		/// </summary>
		/// <returns></returns>
		public bool isBoss() {
			return typeId == (int)Type.Boss;
		}

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExerProMapNode() { }
        public ExerProMapNode(int id, ExerProRecord stage,
            int xOrder, int yOrder, Type type) {
            this.id = id; this.stage = stage;
            this.xOrder = xOrder; this.yOrder = yOrder;
            typeId = (int)type;

            generatePosition();
        }
    }

    #endregion

    #region 运行时数据

    /// <summary>
    /// 运行时BUFF
    /// </summary>
    public class RuntimeBuff : BaseData {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int paramId { get; protected set; } // 属性ID
        [AutoConvert]
        public int value { get; protected set; } // 改变数值
        [AutoConvert]
        public double rate { get; protected set; } // 改变比率
        [AutoConvert]
        public int turns { get; protected set; } // BUFF回合

        /// <summary>
        /// 是否为Debuff
        /// </summary>
        /// <returns></returns>
        public bool isDebuff() {
            return value < 0 || rate < 1;
        }

        /// <summary>
        /// BUFF是否过期
        /// </summary>
        /// <returns></returns>
        public bool isOutOfDate() {
            return turns == 0;
        }

        /// <summary>
        /// 回合结束回调
        /// </summary>
        public void onRoundEnd() {
            if (turns > 0) turns--;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public RuntimeBuff() { }
        public RuntimeBuff(int paramId,
            int value = 0, double rate = 1, int turns = 0) {
            this.paramId = paramId; this.value = value;
            this.rate = rate; this.turns = turns;
        }

    }

    /// <summary>
    /// 运行时状态
    /// </summary>
    public class RuntimeState : BaseData {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int stateId { get; protected set; } // 状态ID
        [AutoConvert]
        public int turns { get; protected set; } // 状态回合

		/// <summary>
		/// 状态对象
		/// </summary>
		ExerProState _state = null;

		/// <summary>
		/// 获取状态
		/// </summary>
		/// <returns></returns>
		public ExerProState state() {
            return _state = _state ?? DataService.get().exerProState(stateId);
        }

        /// <summary>
        /// 是否为负面状态
        /// </summary>
        /// <returns></returns>
        public bool isNega() {
            return state().isNega;
        }

        /// <summary>
        /// 状态是否过期
        /// </summary>
        /// <returns></returns>
        public bool isOutOfDate() {
            return turns <= 0;
        }

        /// <summary>
        /// 回合结束回调
        /// </summary>
        public void onRoundEnd() {
            if (turns > 0) turns--;
        }

        /// <summary>
        /// 移除状态回合
        /// </summary>
        /// <param name="turns">回合数</param>
        public void remove(int turns) {
            this.turns -= turns;
        }

        /// <summary>
        /// 增加状态回合
        /// </summary>
        /// <param name="turns">回合数</param>
        public void add(int turns) {
            this.turns += turns;
            var max = state().maxTurns;
            if (max > 0 && this.turns > max)
                this.turns = max;
        }

		/// <summary>
		/// 特性
		/// </summary>
		/// <returns></returns>
		public ExerProTraitData[] traits() {
			return state().traits;
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		public RuntimeState() { }
        public RuntimeState(int stateId, int turns = 0) {
            this.stateId = stateId; this.turns = turns;
        }

    }

    /// <summary>
    /// 运行时战斗者
    /// </summary>
    public abstract class RuntimeBattler : BaseData, ParamDisplay.IDisplayDataConvertable {

        /// <summary>
        /// 属性ID约定
        /// </summary>
        public const int MHPParamId = 1;
        public const int PowerParamId = 2;
        public const int DefenseParamId = 3;
		public const int AgileParamId = 4;

		public const int MaxParamCount = 4;

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
        public int hp { get; protected set; }

        /// <summary>
        /// 状态和BUFF
        /// </summary>
        //[AutoConvert]
        public Dictionary<int, RuntimeState> states { get; protected set; } = new Dictionary<int, RuntimeState>();
        [AutoConvert]
        public List<RuntimeBuff> buffs { get; protected set; } = new List<RuntimeBuff>();

        /// <summary>
        /// 是否逃跑
        /// </summary>
        public bool isEscaped = false;

        #region 数据转化

        /// <summary>
        /// 转化为显示数据
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public virtual JsonData convertToDisplayData(string type = "") {
            switch (type) {
                case "hp": return convertHp();
                default: return toJson();
            }
        }

        /// <summary>
        /// 转化状态
        /// </summary>
        /// <returns></returns>
        JsonData convertHp() {
            var res = new JsonData();

            res["hp"] = hp; res["mhp"] = mhp();
            res["rate"] = hp * 1.0 / mhp();

            return res;
        }

        #endregion

		#region 数据读取

		/// <summary>
		/// 读取自定义属性
		/// </summary>
		/// <param name="json"></param>
		protected override void loadCustomAttributes(JsonData json) {
			base.loadCustomAttributes(json);

			this.states.Clear();

			var states = DataLoader.load(json, "states");
			if (states != null) {
				Debug.Log("Load states: " + states.ToJson());
                states.SetJsonType(JsonType.Object);
                foreach (KeyValuePair<string, JsonData> pair in states) {
					var key = int.Parse(pair.Key);
					var data = DataLoader.load<RuntimeState>(pair.Value);
					Debug.Log("Load states: " + key + ", " + data);
					this.states.Add(key, data);
				}
			}

		}

		/// <summary>
		/// 转化自定义属性
		/// </summary>
		/// <param name="json"></param>
		protected override void convertCustomAttributes(ref JsonData json) {
			base.convertCustomAttributes(ref json);
			var states = new JsonData();

			states.SetJsonType(JsonType.Object);
			foreach (var pair in this.states)
				states[pair.Key.ToString()] = DataLoader.convert(pair.Value);

			json["states"] = states;
		}

		#endregion

		#region 数据控制

        /// <summary>
        /// 是否玩家
        /// </summary>
        /// <returns></returns>
        public virtual bool isActor() { return false; }

        /// <summary>
        /// 是否敌人
        /// </summary>
        /// <returns></returns>
        public virtual bool isEnemy() { return false; }

        /// <summary>
        /// 获取战斗图
        /// </summary>
        /// <returns></returns>
        public abstract Texture2D getBattlePicture();

		/// <summary>
		/// 当前性格/性质
		/// </summary>
		/// <returns></returns>
		public virtual string currentCharacter() { return ""; }

        #region 属性控制

        #region 属性快捷定义

        /// <summary>
        /// 最大生命值
        /// </summary>
        /// <returns></returns>
        public int mhp() {
            return param(MHPParamId);
        }

        /// <summary>
        /// 力量
        /// </summary>
        /// <returns></returns>
        public int power() {
            return param(PowerParamId);
        }

        /// <summary>
        /// 格挡
        /// </summary>
        /// <returns></returns>
        public int defense() {
            return param(DefenseParamId);
        }

        /// <summary>
        /// 敏捷
        /// </summary>
        /// <returns></returns>
        public int agile() {
            return param(AgileParamId);
        }

        /// <summary>
        /// 基础最大生命值
        /// </summary>
        /// <returns></returns>
        protected virtual int baseMHP() { return 0; }

        /// <summary>
        /// 基础力量
        /// </summary>
        /// <returns></returns>
        protected virtual int basePower() { return 0; }

        /// <summary>
        /// 基础格挡
        /// </summary>
        /// <returns></returns>
        protected virtual int baseDefense() { return 0; }

        /// <summary>
        /// 基础敏捷
        /// </summary>
        /// <returns></returns>
        protected virtual int baseAgile() { return 0; }

        #endregion

        #region HP控制

        #region HP变化显示

        /// <summary>
        /// HP该变量
        /// </summary>
        public class DeltaHP {

            public int value = 0; // 值

            public bool critical = false; // 是否暴击
            public bool miss = false; // 是否闪避

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="value"></param>
            public DeltaHP(int value = 0, bool critical = false, bool miss = false) {
                this.value = value; this.critical = critical; this.miss = miss;
            }
        }

        /// <summary>
        /// HP变化量
        /// </summary>
        DeltaHP _deltaHP = null;
        public DeltaHP deltaHP {
            get {
                var res = _deltaHP;
                _deltaHP = null; return res;
            }
        }

        /// <summary>
        /// 设置闪避标志
        /// </summary>
        public void setMissFlag() {
            _deltaHP = _deltaHP ?? new DeltaHP();
            _deltaHP.miss = true;
        }

        /// <summary>
        /// 设置暴击标志
        /// </summary>
        public void setCriticalFlag() {
            _deltaHP = _deltaHP ?? new DeltaHP();
            _deltaHP.critical = true;
        }

		/// <summary>
		/// 设置值变化
		/// </summary>
		/// <param name="value"></param>
		public void setHPChange(int value) {
			_deltaHP = _deltaHP ?? new DeltaHP();
			_deltaHP.value += value;

			Debug.Log("setHPChange: " + value + ", sum: " + _deltaHP.value);
		}

		#endregion

        /// <summary>
        /// 改变HP
        /// </summary>
        /// <param name="value">目标值</param>
        /// <param name="show">是否显示</param>
        public void changeHP(int value, bool show = true) {
            var oriHp = hp;
            hp = Mathf.Clamp(value, 0, mhp());
            if (show) setHPChange(hp - oriHp);
            if (hp <= 0) onDie();
        }

        /// <summary>
        /// 增加HP
        /// </summary>
        /// <param name="value">增加值</param>
        /// <param name="show">是否显示</param>
        public void addHP(int value, bool show = true) {
            changeHP(hp + value, show);
        }

        /// <summary>
        /// 增加HP
        /// </summary>
        /// <param name="rate">增加率</param>
        /// <param name="show">是否显示</param>
        public void addHP(double rate, bool show = true) {
			changeHP((int)Math.Round(hp + mhp() * rate), show);
		}

		/// <summary>
		/// 增加HP
		/// </summary>
		/// <param name="rate">增加率</param>
		/// <param name="show">是否显示</param>
		public void addHPInRestNode(double rate, bool show = true) {
			var traits = filterTraits(ExerProTraitData.Code.RestRecoverPlus);

			int val = sumTraits(traits);
			rate += sumTraits(traits, 1) / 100.0;
			addHP((int)Math.Round(mhp() * rate + val));
		}

		/// <summary>
		/// 回复所有HP
		/// </summary>
		/// <param name="show">是否显示</param>
		public void recoverAll(bool show = true) {
			changeHP(mhp(), show);
		}

		/// <summary>
		/// 是否死亡
		/// </summary>
		/// <returns></returns>
		public bool isDead() {
            return hp <= 0;
        }

		/// <summary>
		/// 是否失败
		/// </summary>
		/// <returns></returns>
		public bool isLost() {
			return isDead() || isEscaped;
		}

		#endregion

		#region 属性统一接口

		/// <summary>
		/// 基本属性值
		/// </summary>
		/// <param name="paramId">属性ID</param>
		/// <returns>属性值</returns>
		public virtual int baseParam(int paramId) {
            switch (paramId) {
                case MHPParamId: return baseMHP();
                case PowerParamId: return basePower();
                case DefenseParamId: return baseDefense();
                case AgileParamId: return baseAgile();
                default: return 0;
            }
        }

        /// <summary>
        /// Buff附加值
        /// </summary>
        /// <param name="paramId">属性ID</param>
        /// <returns></returns>
        public int buffValue(int paramId) {
            int value = 0;
            foreach (var buff in buffs)
                if (buff.paramId == paramId && !buff.isOutOfDate()) value += buff.value;
            return value;
        }

        /// <summary>
        /// Buff附加率
        /// </summary>
        /// <param name="paramId">属性ID</param>
        /// <returns></returns>
        public double buffRate(int paramId) {
            double rate = 1;
            foreach (var buff in buffs)
                if (buff.paramId == paramId && !buff.isOutOfDate()) rate *= buff.rate;
            return rate;
		}

		/// <summary>
		/// 特性属性
		/// </summary>
		/// <param name="paramId">属性ID</param>
		/// <returns></returns>
		public virtual int traitParamVal(int paramId) {
			return sumTraits<int>(ExerProTraitData.Code.ParamAdd, paramId);
		}

		/// <summary>
		/// 特性属性
		/// </summary>
		/// <param name="paramId">属性ID</param>
		/// <returns></returns>
		public virtual double traitParamRate(int paramId) {
			return multTraits<int>(ExerProTraitData.Code.ParamAdd, paramId);
		}

		/// <summary>
		/// 额外属性
		/// </summary>
		/// <param name="paramId">属性ID</param>
		/// <returns></returns>
		public virtual int extraParam(int paramId) {
			return 0;
		}

		/// <summary>
		/// 属性值
		/// </summary>
		/// <param name="paramId">属性ID</param>
		/// <returns>属性值</returns>
		public int param(int paramId) {
			var base_ = baseParam(paramId) + traitParamVal(paramId) + buffValue(paramId);
			var rate = buffRate(paramId) * traitParamRate(paramId);
			var extra = extraParam(paramId);
			return (int)Math.Round((base_) * rate + extra);
		}

		/// <summary>
		/// 属性相加
		/// </summary>
		/// <param name="paramId">属性ID</param>
		/// <param name="value">增加值</param>
		public void addParam(int paramId, int value) {
			addBuff(paramId, value);
		}
		public void addParam(int paramId, int value, double rate) {
			addBuff(paramId, value, rate);
		}

		/// <summary>
		/// 属性相乘
		/// </summary>
		/// <param name="paramId">属性ID</param>
		/// <param name="rate">比率</param>
		public void multParam(int paramId, double rate) {
            addBuff(paramId, 0, rate);
		}

		/// <summary>
		/// 伤害加成
		/// </summary>
		public int damagePlusVal() {
			return sumTraits(ExerProTraitData.Code.DamagePlus);
		}
		public double damagePlusRate() {
			return multTraits(ExerProTraitData.Code.DamagePlus);
		}

		/// <summary>
		/// 受伤加成
		/// </summary>
		public int hurtPlusVal() {
			return sumTraits(ExerProTraitData.Code.HurtPlus);
		}
		public double hurtPlusRate() {
			return multTraits(ExerProTraitData.Code.HurtPlus);
		}

		/// <summary>
		/// 回复加成
		/// </summary>
		public int recoverPlusVal() {
			return sumTraits(ExerProTraitData.Code.RecoverPlus);
		}
		public double recoverPlusRate() {
			return multTraits(ExerProTraitData.Code.RecoverPlus);
		}

		#endregion

		#endregion

		#region 特性控制

		/// <summary>
		/// 获取所有特性
		/// </summary>
		/// <returns></returns>
		public virtual List<ExerProTraitData> traits() {
			return statesTraits();
		}

		/// <summary>
		/// 所有状态特性
		/// </summary>
		List<ExerProTraitData> statesTraits() {
			var res = new List<ExerProTraitData>();
			foreach (var state in states)
				res.AddRange(state.Value.traits());
			return res;
		}

		/// <summary>
		/// 获取特定特性
		/// </summary>
		/// <param name="code">特性枚举</param>
		/// <returns>返回符合条件的特性</returns>
		public List<ExerProTraitData> filterTraits(ExerProTraitData.Code code) {
			return traits().FindAll(trait => trait.code == (int)code);
		}
		/// <param name="param">参数取值</param>
		/// <param name="id">参数下标</param>
		public List<ExerProTraitData> filterTraits<T>(ExerProTraitData.Code code, T param, int id = 0) {
            return traits().FindAll(trait => trait.code == (int)code
				&& Equals(trait.get<T>(id), param));
		}

		/// <summary>
		/// 特性值求和
		/// </summary>
		/// <param name="code">特性枚举</param>
		/// <param name="index">特性参数索引</param>
		/// <param name="base_">求和基础值</param>
		/// <returns></returns>
		public int sumTraits(ExerProTraitData.Code code, int index = 0, int base_ = 0) {
			return sumTraits(filterTraits(code), index, base_);
		}
		/// <param name="param">参数取值</param>
		/// <param name="id">参数下标</param>
		public int sumTraits<T>(ExerProTraitData.Code code, T param, int id = 0, int index = 1, int base_ = 0) {
			return sumTraits(filterTraits(code, param, id), index, base_);
		}
		public int sumTraits(List<ExerProTraitData> traits, int index = 0, int base_ = 0) {
			var res = base_;
			foreach (var trait in traits)
				res += trait.get(index, 0);
			return res;
		}

		/// <summary>
		/// 特性值求积
		/// </summary>
		/// <param name="code">特性枚举</param>
		/// <param name="index">特性参数索引</param>
		/// <param name="base_">概率基础值</param>
		/// <returns></returns>
		public double multTraits(ExerProTraitData.Code code, int index = 1, int base_ = 100) {
			return multTraits(filterTraits(code), index, base_);
		}
		/// <param name="param">参数取值</param>
		/// <param name="id">参数下标</param>
		public double multTraits<T>(ExerProTraitData.Code code, T param, int id = 0, int index = 2, int base_ = 100) {
			return multTraits(filterTraits(code, param, id), index, base_);
		}
		public double multTraits(List<ExerProTraitData> traits, int index = 1, int base_ = 100) {
			var res = 1.0;
			foreach (var trait in traits)
				res *= (base_ + trait.get(index, 0)) / 100.0;
			return res;
		}

		#endregion

		#region Buff控制

		/// <summary>
		/// 状态是否改变
		/// </summary>
		List<RuntimeBuff> _addedBuffs = new List<RuntimeBuff>();
		public List<RuntimeBuff> addedBuffs {
			get {
				var res = _addedBuffs;
				_addedBuffs.Clear(); return res;
			}
		}

		#region Buff变更

		/// <summary>
		/// 添加Buff
		/// </summary>
		/// <param name="paramId">属性ID</param>
		/// <param name="value">变化值</param>
		/// <param name="rate">变化率</param>
		/// <param name="turns">持续回合</param>
		/// <returns>返回添加的Buff</returns>
		public RuntimeBuff addBuff(int paramId,
            int value = 0, double rate = 1, int turns = 0) {
            return addBuff(new RuntimeBuff(paramId, value, rate, turns));
        }
        public RuntimeBuff addBuff(RuntimeBuff buff) {
            buffs.Add(buff); onBuffAdded(buff);
			_addedBuffs.Add(buff);

			return buff;
        }

        /// <summary>
        /// 移除Buff
        /// </summary>
        /// <param name="index">Buff索引</param>
        public void removeBuff(int index, bool force = false) {
            var buff = buffs[index];
            buffs.RemoveAt(index);
			_addedBuffs.Remove(buff);

			onBuffRemoved(buff, force);
        }
        /// <param name="buff">Buff对象</param>
        public void removeBuff(RuntimeBuff buff, bool force = false) {
            buffs.Remove(buff);
			_addedBuffs.Remove(buff);

			onBuffRemoved(buff, force);
        }

        /// <summary>
        /// 移除多个满足条件的Buff
        /// </summary>
        /// <param name="p">条件</param>
        public void removeBuffs(Predicate<RuntimeBuff> p, bool force = true) {
            for (int i = buffs.Count() - 1; i >= 0; --i)
                if (p(buffs[i])) removeBuff(i, force);
        }

        /// <summary>
        /// 清除所有Debuff
        /// </summary>
        public void removeDebuffs(bool force = true) {
            removeBuffs(buff => buff.isDebuff(), force);
        }

        /// <summary>
        /// 清除所有Buff
        /// </summary>
        public void clearBuffs(bool force = true) {
            for (int i = buffs.Count() - 1; i >= 0; --i)
                removeBuff(i, force);
        }

        /// <summary>
        /// 是否处于指定条件的Buff
        /// </summary>
        public bool containsBuff(int paramId) {
            return buffs.Exists(buff => buff.paramId == paramId);
        }
        public bool containsBuff(Predicate<RuntimeBuff> p) {
            return buffs.Exists(p);
        }

        #endregion

        #region Buff判断

        /// <summary>
        /// 是否处于指定条件的Debuff
        /// </summary>
        public bool containsDebuff(int paramId) {
            return buffs.Exists(buff => buff.isDebuff() && buff.paramId == paramId);
        }

        /// <summary>
        /// 是否存在Debuff
        /// </summary>
        public bool anyDebuff() {
            return buffs.Exists(buff => buff.isDebuff());
        }

        /// <summary>
        /// 获取指定条件的Buff
        /// </summary>
        public RuntimeBuff getBuff(int paramId) {
            return buffs.Find(buff => buff.paramId == paramId);
        }
        public RuntimeBuff getBuff(Predicate<RuntimeBuff> p) {
            return buffs.Find(p);
        }

        /// <summary>
        /// 获取指定条件的Buff（多个）
        /// </summary>
        public List<RuntimeBuff> getBuffs(Predicate<RuntimeBuff> p) {
            return buffs.FindAll(p);
        }

        #endregion

        #endregion

        #region 状态控制

        /// <summary>
        /// 状态是否改变
        /// </summary>
        bool _isStateChanged = false;
        public bool isStateChanged {
            get {
                var res = _isStateChanged;
                _isStateChanged = false; return res;
            }
        }

        /// <summary>
        /// 获取所有状态
        /// </summary>
        /// <returns></returns>
        public List<ExerProState> allStates() {
            var res = new List<ExerProState>(states.Count);
            foreach (var pair in states)
                res.Add(pair.Value.state());
            return res;
        }

        /// <summary>
        /// 获取所有运行时状态
        /// </summary>
        /// <returns></returns>
        public List<RuntimeState> allRuntimeStates() {
            var res = new List<RuntimeState>(states.Count);
            foreach (var pair in states) res.Add(pair.Value);
            return res;
        }

        #region 状态变更

        /// <summary>
        /// 添加状态
        /// </summary>
        /// <param name="paramId">属性ID</param>
        /// <param name="value">变化值</param>
        /// <param name="rate">变化率</param>
        /// <param name="turns">持续回合</param>
        /// <returns>返回添加的Buff</returns>
        public RuntimeState addState(int stateId, int turns = 0) {
            RuntimeState state;
            _isStateChanged = true;

            if (states.ContainsKey(stateId)) {
                state = states[stateId];
                state.add(turns);
            }
            else {
                state = new RuntimeState(stateId, turns);
                states.Add(stateId, state);
                onStateAdded(state);
            }

            return state;
        }

        /// <summary>
        /// 移除状态
        /// </summary>
        /// <param name="stateId">状态ID</param>
        /// <param name="turns">移除回合数</param>
        public RuntimeState removeState(int stateId, int turns = 0, bool force = false) {
            if (!containsState(stateId)) return null;
            var state = states[stateId];
            _isStateChanged = true;

            if (turns <= 0) {
                states.Remove(stateId);
                onStateRemoved(state, force);
            }
            else {
                state.remove(turns);
                if (state.isOutOfDate())
                    removeState(stateId, force: force);
            }

            return state;
        }
        /// <param name="buff">Buff对象</param>
        public RuntimeState removeState(RuntimeState state, int turns = 0, bool force = false) {
            return removeState(state.stateId, turns, force);
        }

        /// <summary>
        /// 移除多个满足条件的状态
        /// </summary>
        /// <param name="p">条件</param>
        public void removeStates(Predicate<RuntimeState> p, bool force = true) {
            for (int i = states.Count() - 1; i >= 0; --i)
                if (p(states[i])) removeState(i, force: force);
        }

        /// <summary>
        /// 清除所有Debuff
        /// </summary>
        public void removeNegeStates() {
            removeStates(state => state.isNega());
        }

        /// <summary>
        /// 清除所有状态
        /// </summary>
        public void clearStates(bool force = true) {
			var tmp = new Dictionary<int, RuntimeState>(states);
			foreach (var pair in tmp)
                removeState(pair.Value, force: force);
        }

        #endregion

        #region 状态判断

        /// <summary>
        /// 是否处于指定条件的状态
        /// </summary>
        public bool containsState(int stateId) {
            return states.ContainsKey(stateId);
        }
        public bool containsState(Predicate<RuntimeState> p) {
            foreach (var pair in states)
                if (p(pair.Value)) return true;
            return false;
        }

        /// <summary>
        /// 是否存在负面状态
        /// </summary>
        public bool anyNegaState() {
            return containsState(state => state.isNega());
        }

        /// <summary>
        /// 获取指定条件的状态
        /// </summary>
        public RuntimeState getState(int stateId) {
            return states[stateId];
        }
        public RuntimeState getState(Predicate<RuntimeState> p) {
            foreach (var pair in states)
                if (p(pair.Value)) return pair.Value;
            return null;
        }

        /// <summary>
        /// 获取指定条件的状态（多个）
        /// </summary>
        public List<RuntimeState> getStates(Predicate<RuntimeState> p) {
            var res = new List<RuntimeState>();
            foreach (var pair in states)
                if (p(pair.Value)) res.Add(pair.Value);
            return res;
        }

		/// <summary>
		/// 是否处于可移动状态
		/// </summary>
		/// <returns></returns>
		public bool isMovableState() {
			return !isEscaped && !isDead();
		}

        #endregion

        #endregion

        #region 结果控制

        /// <summary>
        /// 当前结果
        /// </summary>
        RuntimeActionResult currentResult = null;

        /// <summary>
        /// 应用结果
        /// </summary>
        /// <param name="result"></param>
        public void applyResult(RuntimeActionResult result) {
            currentResult = result;
            // TODO: 结果应用
            CalcService.ResultApplyCalc.apply(this, result);
        }

        /// <summary>
        /// 获取当前结果（用于显示）
        /// </summary>
        /// <returns></returns>
        public RuntimeActionResult getResult() {
            var res = currentResult;
			clearResult();
			return res;
        }

		/// <summary>
		/// 清除当前结果
		/// </summary>
		public void clearResult() {
			currentResult = null;
		}

		#endregion

        #region 行动控制

		/// <summary>
		/// 行动序列
		/// </summary>
		Queue<RuntimeAction> actions = new Queue<RuntimeAction>();

		/// <summary>
		/// 当前行动
		/// </summary>
		/// <returns></returns>
		public virtual RuntimeAction currentAction() {
			if (actions.Count <= 0) return null;
			var action = actions.Dequeue();
			if (!isMovableState()) return null;
			return action;
		}

		/// <summary>
		/// 增加行动
		/// </summary>
		/// <param name="action">行动</param>
		public void addAction(RuntimeAction action) {
			Debug.Log(this + " addAction " + action?.toJson().ToJson());
			actions.Enqueue(action);
		}

		/// <summary>
		/// 处理指定行动
		/// </summary>
		public virtual void processAction(RuntimeAction action) {
			if (!isMovableState()) return;
			action.generateResults();
			foreach (var obj in action.objects)
				obj.applyResult(action.result(obj));
		}

		/// <summary>
		/// 处理当前行动（处理后移出队列）
		/// </summary>
		/// <param name="action"></param>
		public virtual void processCurrentAction() {
			processAction(currentAction());
		}

		/// <summary>
		/// 清除所有行动
		/// </summary>
		public void clearActions() {
			actions.Clear();
		}

		#endregion

		#region 回调控制

		/// <summary>
		/// 战斗开始回调
		/// </summary>
		public virtual void onBattleStart() {
			CalcService.ExerProTraitsCalc.calcOnBattleStart(this);

			onRoundStart(0);
			clearStates();
			clearBuffs();
		}

		/// <summary>
		/// 回合开始回调
		/// </summary>
		/// <param name="round">回合数</param>
		public virtual void onRoundStart(int round) {
			if (round > 0)
				CalcService.ExerProTraitsCalc.calcOnRoundStart(this);

			_deltaHP = null;
			_addedBuffs.Clear();
			_isStateChanged = false;
			clearActions();
		}

		#region BUFF回调

		/// <summary>
		/// BUFF添加回调
		/// </summary>
		public virtual void onBuffAdded(RuntimeBuff buff) { }

		/// <summary>
		/// BUFF移除回调
		/// </summary>
		public virtual void onBuffRemoved(RuntimeBuff buff, bool force = false) { }

		#endregion

		#region 状态回调

		/// <summary>
		/// 状态添加回调
		/// </summary>
		public virtual void onStateAdded(RuntimeState state) { }

		/// <summary>
		/// 状态解除回调
		/// </summary>
		public virtual void onStateRemoved(RuntimeState state, bool force = false) { }

        #endregion

		/// <summary>
		/// 当前行动结束回调
		/// </summary>
		public virtual void onActionEnd(RuntimeAction action) { }

		/// <summary>
		/// 逃跑回调
		/// </summary>
		public virtual void onEscape() {
			isEscaped = true; onActionEnd(null);
		}

		/// <summary>
		/// 死亡回调
		/// </summary>
		protected virtual void onDie() { }

		/// <summary>
		/// 回合结束回调
		/// </summary>
		/// <param name="round">回合数</param>
		public virtual void onRoundEnd(int round) {
			if (round > 0)
				CalcService.ExerProTraitsCalc.calcOnRoundEnd(this);

            clearActions();

            processBuffsRoundEnd();
            processStatesRoundEnd();
        }

		/// <summary>
		/// 处理状态回合结束
		/// </summary>
		void processBuffsRoundEnd() {
            for (int i = buffs.Count - 1; i >= 0; --i) {
                var buff = buffs[i]; buff.onRoundEnd();
                if (buff.isOutOfDate()) removeBuff(i);
            }
        }

        /// <summary>
        /// 处理状态回合结束
        /// </summary>
        void processStatesRoundEnd() {
            var states = this.states.ToArray();

            foreach (var pair in states) {
                var state = pair.Value;
                state.onRoundEnd();
                _isStateChanged = true;
                if (state.isOutOfDate())
                    removeState(state.stateId);
            }
        }

        /// <summary>
        /// 战斗结束回调
        /// </summary>
        public virtual void onBattleEnd() {
			CalcService.ExerProTraitsCalc.calcOnBattleEnd(this);
			isEscaped = false;
		}

        #endregion

        /// <summary>
        /// 重置
        /// </summary>
        public virtual void reset() {
            hp = mhp();
            clearStates();
            clearBuffs();
        }

        #endregion

		/// <summary>
		/// 构造函数
		/// </summary>
		public RuntimeBattler() {
			//reset();
		}
	}

    /// <summary>
    /// 特训玩家
    /// </summary>
    public class RuntimeActor : RuntimeBattler {

        /// <summary>
        /// 默认属性
        /// </summary>
        public const int DefaultMHP = 100; // 初始体力值
        public const int DefaultPower = 5; // 初始力量
        public const int DefaultDefense = 0; // 初始格挡
        public const int DefaultAgile = 0; // 初始敏捷

		public const int DefaultEnergy = 4; // 默认能量

        const int EnglishSubjectId = 3; // 英语科目ID

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert("mhp")]
        public int _mhp { get; protected set; }
        [AutoConvert("power")]
        public int _power { get; protected set; }
        [AutoConvert("defense")]
        public int _defense { get; protected set; }
        [AutoConvert("agile")]
        public int _agile { get; protected set; }

		/// <summary>
		/// 能量
		/// </summary>
		[AutoConvert]
		public int energy { get; protected set; }

		/// <summary>
		/// 背包
		/// </summary>
		[AutoConvert]
        public ExerProItemPack itemPack { get; protected set; } = new ExerProItemPack();
        [AutoConvert]
        public ExerProPotionPack potionPack { get; protected set; } = new ExerProPotionPack();
        [AutoConvert]
        public ExerProCardGroup cardGroup { get; protected set; } = new ExerProCardGroup();

		/// <summary>
		/// 药水槽
		/// </summary>
		[AutoConvert]
		public ExerProPotionSlot potionSlot { get; protected set; }

		/// <summary>
		/// 对应的记录
		/// </summary>
		public ExerProRecord racord { get; protected set; } = null;

		/// <summary>
        /// 对应的艾瑟萌槽项
        /// </summary>
        public ExerSlotItem slotItem { get; protected set; } = null;

		#region 数据转化

		///// <summary>
		///// 转化为显示数据
		///// </summary>
		///// <param name="type">类型</param>
		///// <returns></returns>
		//public JsonData convertToDisplayData(string type = "") {
		//    switch (type) {
		//        case "hp": return convertHp();
		//        default: return toJson();
		//    }
		//}

		#endregion

		#region 数据控制

		/// <summary>
		/// 性质
		/// </summary>
		string character = "";

		/// <summary>
		/// 当前性格/性质
		/// </summary>
		/// <returns></returns>
		public override string currentCharacter() {
			return character;
		}

		/// <summary>
		/// 得到金钱
		/// </summary>
		/// <param name="val"></param>
		public void gainGold(int val) {
			racord?.gainGold(val);
		}

		/// <summary>
		/// 添加能量
		/// </summary>
		/// <param name="val"></param>
		public void addEnergy(int val) {
			energy = Math.Max(energy + val, 0);
		}

        /// <summary>
        /// 获取战斗图
        /// </summary>
        public override Texture2D getBattlePicture() {
            if (slotItem == null || slotItem.isNullItem()) return null;
            return slotItem.playerExer.exermon().battle;
        }

        /// <summary>
        /// 是否玩家
        /// </summary>
        /// <returns></returns>
        public override bool isActor() { return true; }

        /// <summary>
        /// 使用药水
        /// </summary>
        /// <param name="potion">药水</param>
        public void usePotion(ExerProPackPotion potion) {
            if (potion.equiped) {
                var slotItem = potionSlot.getSlotItemByEquipItem(potion);
                potionSlot.setEquip(slotItem, potionPack, null);
            }
            potionPack.removeItem(potion);
        }

		/// <summary>
		/// 使用卡牌
		/// </summary>
		/// <param name="card">卡牌</param>
		public void useCard(ExerProPackCard card) {
			character = card.item()._character;
			cardGroup.useCard(card);
			addEnergy(-card.cost());
		}

		/// <summary>
		/// 获得物品
		/// </summary>
		/// <typeparam name="T"><peparam>
		/// <param name="item">物品</param>
		public void gainItem<T>(T item) where T : BaseExerProItem {
			if (typeof(T) == typeof(ExerProItem)) gainItem(item as ExerProItem);
			if (typeof(T) == typeof(ExerProPotion)) gainPotion(item as ExerProPotion);
			if (typeof(T) == typeof(ExerProCard)) gainCard(item as ExerProCard);
		}
		public void gainItem<T>(int id) where T : BaseExerProItem {
			if (typeof(T) == typeof(ExerProItem)) gainItem(id);
			if (typeof(T) == typeof(ExerProPotion)) gainPotion(id);
			if (typeof(T) == typeof(ExerProCard)) gainCard(id);
		}

		/// <summary>
		/// 获得卡牌
		/// </summary>
		/// <param name="item"></param>
		public void gainCard(ExerProCard item) {
			var contItem = getContItem(item);
			if (contItem != null)
				cardGroup.pushItem(contItem as ExerProPackCard);
		}
		public void gainCard(int id) {
			gainCard(DataService.get().exerProCard(id));
		}

		/// <summary>
		/// 获得卡牌
		/// </summary>
		/// <param name="item"></param>
		public void gainItem(ExerProItem item) {
			var contItem = getContItem(item);
			if (contItem != null)
				itemPack.pushItem(contItem as ExerProPackItem);
		}
		public void gainItem(int id) {
			gainItem(DataService.get().exerProItem(id));
		}

		/// <summary>
		/// 获得卡牌
		/// </summary>
		/// <param name="item"></param>
		public void gainPotion(ExerProPotion item) {
			var contItem = getContItem(item);
			if (contItem != null)
				potionPack.pushItem(contItem as ExerProPackPotion);
		}
		public void gainPotion(int id) {
			gainPotion(DataService.get().exerProPotion(id));
		}

		/// <summary>
		/// 获取容器
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public ExerProPackItem<T> getContItem<T>(T item)
			where T : BaseExerProItem {
			if (item == null) return null;

			if (typeof(T) == typeof(ExerProItem))
                return new ExerProPackItem(item as ExerProItem) as ExerProPackItem<T>;
			if (typeof(T) == typeof(ExerProPotion))
				return new ExerProPackPotion(item as ExerProPotion) as ExerProPackItem<T>;
			if (typeof(T) == typeof(ExerProCard))
				return new ExerProPackCard(item as ExerProCard) as ExerProPackItem<T>;

			return null;
		}

		/// <summary>
		/// 获取容器
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
        public PackContainer<P> getContainer<P,T>()
            where P:ExerProPackItem<T>,new() where T : BaseExerProItem {
			if (typeof(T) == typeof(ExerProItem))
				return itemPack as PackContainer<P>;
			if (typeof(T) == typeof(ExerProPotion))
				return potionPack as PackContainer<P>;
			if (typeof(T) == typeof(ExerProCard))
				return cardGroup as PackContainer<P>;

			return null;
		}

		/// <summary>
		/// 获取所有特性
		/// </summary>
		/// <returns></returns>
		public override List<ExerProTraitData> traits() {
			var res = base.traits();
			res.AddRange(itemPack.traits());
			return res;
		}

		#region 属性定义

        /// <summary>
        /// 基础最大生命值
        /// </summary>
        /// <returns></returns>
        protected override int baseMHP() { return _mhp; }

        /// <summary>
        /// 基础力量
        /// </summary>
        /// <returns></returns>
        protected override int basePower() { return _power; }

        /// <summary>
        /// 基础格挡
        /// </summary>
        /// <returns></returns>
        protected override int baseDefense() { return _defense; }

        /// <summary>
        /// 基础敏捷
        /// </summary>
        /// <returns></returns>
        protected override int baseAgile() { return _agile; }

        #endregion

        #region 回调控制

		/// <summary>
		/// 战斗开始回调
		/// </summary>
		public override void onBattleStart() {
			base.onBattleStart();
			cardGroup.onBattleStart();
		}

		/// <summary>
		/// 回合开始回调
		/// </summary>
		public override void onRoundStart(int round) {
			base.onRoundStart(round);
			energy = DefaultEnergy;
		}

		#region 卡牌事件

		/// <summary>
		/// 卡牌使用回调
		/// </summary>
		/// <param name="card">卡牌</param>
		public virtual void onUseCard(ExerProPackCard card) {

		}

		/// <summary>
		/// 卡牌丢弃回调
		/// </summary>
		/// <param name="card">卡牌</param>
		public virtual void onDiscardCard(ExerProPackCard card) {

		}

		/// <summary>
		/// 卡牌消耗回调
		/// </summary>
		/// <param name="card">卡牌</param>
		public virtual void onConsumeCard(ExerProPackCard card) {

		}

		#endregion

		/// <summary>
		/// 回合结束回调
		/// </summary>
		public override void onRoundEnd(int round) {
			base.onRoundEnd(round);
			cardGroup.onRoundEnd();
		}

        /// <summary>
        /// 战斗结束回调
        /// </summary>
        public override void onBattleEnd() {
            base.onBattleEnd();
            cardGroup.onBattleEnd();
        }

        #endregion

        /// <summary>
        /// 重置
        /// </summary>
        public override void reset() {
            _mhp = DefaultMHP;
            _power = DefaultPower;
            _defense = DefaultDefense;
            _agile = DefaultAgile;

            base.reset();
        }

        /// <summary>
        /// 配置玩家
        /// </summary>
        /// <param name="player"></param>
        public void setupPlayer(Player player) {
            slotItem = player.getExerSlotItem(EnglishSubjectId);
        }

		/// <summary>
		/// 配置药水槽
		/// </summary>
		void createPotionSlot() {
			potionSlot = new ExerProPotionSlot(this);
		}

		#endregion

        /// <summary>
        /// 读取自定义属性
        /// </summary>
        /// <param name="json"></param>
        protected override void loadCustomAttributes(JsonData json) {
            base.loadCustomAttributes(json);
            potionSlot.actor = this;
        }

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="player"></param>
		public RuntimeActor() { }
		public RuntimeActor(ExerProRecord racord, Player player) {
			this.racord = racord;

			setupPlayer(player);
			createPotionSlot();
			reset();
		}
    }

    /// <summary>
    /// 运行时敌人数据
    /// </summary>
    public class RuntimeEnemy : RuntimeBattler {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int pos { get; protected set; }

        [AutoConvert]
        public int enemyId { get; protected set; }

        /// <summary>
        /// 当前行动状态
        /// </summary>
        public bool isActionEnd = false; // 当前行动是否结束
        public bool isActionStarted = false; // 当前行动是否开始

        /// <summary>
        /// 缓存敌人对象
        /// </summary>
        ExerProEnemy tempEnemy = null;

        #region 数据控制

        /// <summary>
        /// 获取战斗图
        /// </summary>
        public override Texture2D getBattlePicture() {
            var enemy = this.enemy();
            if (enemy == null) return null;
            return enemy.battle;
        }

		/// <summary>
		/// 当前性格/性质
		/// </summary>
		/// <returns></returns>
		public override string currentCharacter() {
			return enemy()._character;
		}

		/// <summary>
		/// 是否玩家
		/// </summary>
		/// <returns></returns>
		public override bool isEnemy() { return true; }

        #region 属性控制

        /// <summary>
        /// 最大体力值
        /// </summary>
        /// <returns></returns>
        protected override int baseMHP() {
            return enemy().mhp;
        }

        /// <summary>
        /// 力量
        /// </summary>
        /// <returns></returns>
        protected override int basePower() {
            return enemy().power;
        }

        /// <summary>
        /// 格挡
        /// </summary>
        /// <returns></returns>
        protected override int baseDefense() {
            return enemy().defense;
        }

        #endregion

        /// <summary>
        /// 敌人
        /// </summary>
        /// <returns></returns>
        public ExerProEnemy enemy() {
            if (tempEnemy != null) return tempEnemy;
            return tempEnemy = DataService.get().exerProEnemy(enemyId);
        }

        #endregion

        #region 行动控制

        /// <summary>
        /// 当前行动
        /// </summary>
        ExerProEnemy.Action _currentEnemyAction = null;
        public ExerProEnemy.Action currentEnemyAction {
            get {
                if (!isActionStarted) return null;
                var res = _currentEnemyAction;
                _currentEnemyAction = null;
                return res;
            }
        }

		/// <summary>
		/// 当前行动类型
		/// </summary>
		/// <returns></returns>
		public int currentActionType() {
			if (_currentEnemyAction == null) return 0;
			return _currentEnemyAction.type;
		}

		/// <summary>
		/// 当前行动类型枚举
		/// </summary>
		/// <returns></returns>
		public ExerProEnemy.Action.Type currentActionTypeEnum() {
			if (_currentEnemyAction == null) return ExerProEnemy.Action.Type.Unset;
			return _currentEnemyAction.typeEnum();
		}

		/// <summary>
		/// 当前行动参数
		/// </summary>
		/// <returns></returns>
		public JsonData currentActionParams() {
			if (_currentEnemyAction == null) return new JsonData();
			return _currentEnemyAction.params_;
		}

        /// <summary>
        /// 当前行动
        /// </summary>
        /// <returns></returns>
        public override RuntimeAction currentAction() {
            if (!isActionStarted) return null;
            return base.currentAction();
        }

		/// <summary>
		/// 更新行动
		/// </summary>
		public bool updateAction() {
            if (!isMovableState()) return isActionEnd = true;
            processEnemyAction(); return isActionEnd;
        }

        /// <summary>
        /// 计算下一步
        /// </summary>
        public void calcNext(int round) {
			RuntimeAction action;
			var actor = BattleService.get().actor();
            CalcService.EnemyNextCalc.calc(round, this, actor,
                out _currentEnemyAction, out action);
			if (action != null) addAction(action);
        }

        /// <summary>
        /// 处理敌人行动
        /// </summary>
        public void processEnemyAction() {
            if (!isActionStarted && (_currentEnemyAction == null ||
				_currentEnemyAction.isUnset())) isActionEnd = true;
			isActionStarted = true;
		}

		#endregion

		#region 回调控制

		/// <summary>
		/// 战斗开始回调
		/// </summary>
		public override void onBattleStart() {
			base.onBattleStart();
			reset();
		}

		/// <summary>
		/// 回合开始回调
		/// </summary>
		/// <param name="round"></param>
		public override void onRoundStart(int round) {
			base.onRoundStart(round);
			isActionStarted = isActionEnd = false;
			calcNext(round);
		}

		/// <summary>
		/// 行动结束回调
		/// </summary>
		public override void onActionEnd(RuntimeAction action) {
			base.onActionEnd(action);
			isActionEnd = true;
		}

		/// <summary>
		/// 回合结束回调
		/// </summary>
		public override void onRoundEnd(int round) {
			base.onRoundEnd(round);
			_currentEnemyAction = null;
		}

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public RuntimeEnemy() { }
        public RuntimeEnemy(int pos, ExerProEnemy enemy) {
            tempEnemy = enemy; enemyId = enemy.id;
            this.pos = pos;
        }

    }

    /// <summary>
    /// 运行时行动
    /// </summary>
    public class RuntimeAction : BaseData {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public RuntimeBattler subject { get; protected set; } // 主体
        [AutoConvert]
        public RuntimeBattler[] objects { get; protected set; } // 对象
        [AutoConvert]
        public ExerProEffectData[] effects { get; protected set; } // 效果

        /// <summary>
        /// 结果
        /// </summary>
        public RuntimeActionResult[] results { get; protected set; } = null;

		/// <summary>
		/// 起手动画/目标动画
		/// </summary>
		public AnimationClip startAni { get; protected set; } = null;
		public AnimationClip targetAni { get; protected set; } = null;

		/// <summary>
		/// 是否需要移动到对方位置
		/// </summary>
		public bool moveToTarget { get; set; } = false;

		/// <summary>
		/// 获取对应战斗者的结果
		/// </summary>
		/// <param name="battler">战斗者</param>
		/// <returns></returns>
		public RuntimeActionResult result(RuntimeBattler battler) {
			foreach (var result in results)
				if (result.object_ == battler) return result;
			return null;
		}

		/// <summary>
		/// 生成结果
		/// </summary>
		public void generateResults() {
            // TODO: 结果生成
            results = CalcService.ExerProActionResultGenerator.generate(this);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public RuntimeAction() { }
		public RuntimeAction(RuntimeBattler subject,
			RuntimeBattler[] object_, ExerProEffectData[] effects = null,
			AnimationClip startAni = null, AnimationClip targetAni = null,
			bool moveToTarget = false) {

			this.subject = subject; this.objects = object_;
			this.startAni = startAni; this.targetAni = targetAni;
			this.effects = effects ?? new ExerProEffectData[0];
			this.moveToTarget = moveToTarget;
		}
		public RuntimeAction(RuntimeBattler subject,
			RuntimeBattler object_, ExerProEffectData[] effects = null,
			AnimationClip startAni = null, AnimationClip targetAni = null,
            bool moveToTarget = false) : this(subject,
                new RuntimeBattler[] { object_ }, effects,
				startAni, targetAni, moveToTarget) { }
		public RuntimeAction(RuntimeBattler subject,
            RuntimeBattler[] object_, BaseExerProItem item) :
            this(subject, object_, item.effects,
				item.startAni, item.targetAni) { }
		public RuntimeAction(RuntimeBattler subject,
            RuntimeBattler object_, BaseExerProItem item) :
			this(subject, new RuntimeBattler[] { object_ }, item) { }
	}

	/// <summary>
	/// 运行时行动结果
	/// </summary>
	public class RuntimeActionResult : BaseData {

        /// <summary>
        /// 状态改变
        /// </summary>
        public class StateChange : RuntimeState {

            /// <summary>
            /// 属性
            /// </summary>
            [AutoConvert]
            public new bool remove { get; protected set; } // 是否移除状态（turns为0时移除全部回合）

            /// <summary>
            /// 构造函数
            /// </summary>
            public StateChange() { }
            public StateChange(int stateId, int turns = 0, bool remove = false) : base(stateId, turns) {
                this.remove = remove;
            }
        }

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int hpDamage { get; set; }
        [AutoConvert]
        public int hpRecover { get; set; }
        [AutoConvert]
        public int hpDrain { get; set; }

		/// <summary>
		/// 能量获得
		/// </summary>
		[AutoConvert]
		public int energyGain { get; set; }

        /// <summary>
        /// 状态/Buff变更
        /// </summary>
        [AutoConvert]
        public StateChange[] stateChanges { get; set; }
        [AutoConvert]
        public RuntimeBuff[] addBuffs { get; set; }

        /// <summary>
        /// 抽牌数据
        /// </summary>
        [AutoConvert]
        public int drawCardCnt { get; set; } // 抽牌数量
        [AutoConvert]
        public int consumeCardCnt { get; set; } // 消耗牌数量
        [AutoConvert]
        public bool consumeSelect { get; set; } // 消耗是否可选

		/// <summary>
		/// 剧情数据
		/// </summary>
		[AutoConvert]
		public int gainGold { get; set; } // 获得卡牌
		[AutoConvert]
		public int gainCard { get; set; } // 获得卡牌
		[AutoConvert]
		public int dropCard { get; set; } // 去除卡牌
		[AutoConvert]
		public int copyCard { get; set; } // 复制卡牌
		[AutoConvert]
		public int gainItem { get; set; } // 获得道具
		[AutoConvert]
		public int gainPotion { get; set; } // 获得药水
		[AutoConvert]
		public int loseItem { get; set; } // 失去道具
		[AutoConvert]
		public int losePotion { get; set; } // 失去药水

		/// <summary>
		/// 行动
		/// </summary>
		public RuntimeAction action { get; protected set; }

		/// <summary>
		/// 所属目标
		/// </summary>
		public RuntimeBattler object_ { get; protected set; }

		/// <summary>
		/// 构造函数
		/// </summary>
		public RuntimeActionResult() { }
        public RuntimeActionResult(RuntimeBattler object_, RuntimeAction action) {
			this.object_ = object_; this.action = action;
		}
    }

    #endregion

}