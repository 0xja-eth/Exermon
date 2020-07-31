using System;
using System.Collections.Generic;
using UnityEngine.Events;

using LitJson;

using Core.Data;
using Core.Data.Loaders;
using Core.Systems;
using Core.Services;

using GameModule.Data;
using PlayerModule.Data;
using ExermonModule.Data;
using QuestionModule.Data;
using SeasonModule.Data;
using BattleModule.Data;

using ExerPro.EnglishModule.Data;

namespace GameModule.Services {

    /// <summary>
    /// 系统数据控制类
    /// </summary>
    /// <remarks>
    /// 控制游戏系统数据的读取、刷新等
    /// </remarks>
    public class DataService : BaseService<DataService> {

        /// <summary>
        /// 操作文本设定
        /// </summary>
        public new const string FailTextFormat = "{0}发生错误，错误信息：\n{{0}}"; // \n选择“重试”进行重试，选择“取消”退出游戏。";

        public const string Initializing = "初始化数据";
        public const string Refresh = "刷新数据";

        /// <summary>
        /// 业务操作
        /// </summary>
        public enum Oper {
            Static, Dynamic, Refresh
        }

        /// <summary>
        /// 状态
        /// </summary>
        public enum State {
            Unload,
            LoadingStatic,
            LoadingDynamic,
            Loaded,
        }
        public bool isLoaded() {
            return state == (int)State.Loaded;
        }

        /// <summary>
        /// 游戏数据
        /// </summary>
        public GameStaticData staticData { get; protected set; } = new GameStaticData();
        public GameDynamicData dynamicData { get; protected set; } = new GameDynamicData();

        /// <summary>
        /// 接受失败函数（初始加载用）
        /// </summary>
        UnityAction unacceptFunc = null;

        /// <summary>
        /// 外部系统
        /// </summary>
        StorageSystem storageSys;

        #region 初始化

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            storageSys = StorageSystem.get();
        }

        /// <summary>
        /// 初始化状态字典
        /// </summary>
        protected override void initializeStateDict() {
            base.initializeStateDict();
            addStateDict(State.Unload);
            addStateDict(State.LoadingStatic);
            addStateDict(State.LoadingDynamic);
            addStateDict(State.Loaded);
        }

        /// <summary>
        /// 初始化操作字典
        /// </summary>
        protected override void initializeOperDict() {
            base.initializeOperDict();
            addOperDict(Oper.Static, Initializing, NetworkSystem.Interfaces.LoadStaticData);
            addOperDict(Oper.Dynamic, Initializing, NetworkSystem.Interfaces.LoadDynamicData);
            addOperDict(Oper.Refresh, Refresh, NetworkSystem.Interfaces.LoadDynamicData);
        }

        /// <summary>
        /// 其他初始化工作
        /// </summary>
        protected override void initializeOthers() {
            base.initializeOthers();
            changeState(State.Unload);
        }

        #endregion

        #region 操作控制

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="unaccept">接受失败函数</param>
        public void load(UnityAction unaccept = null) {
            unacceptFunc = unaccept;
            switch ((State)state) {
                case State.Unload:
                case State.LoadingStatic:
                    loadStaticData(); break;
                case State.LoadingDynamic:
                    loadDynamicData(); break;
            }
        }

        /// <summary>
        /// 读取静态数据
        /// </summary>
        void loadStaticData() {
            var cached = isStaticDataCached();
            changeState(State.LoadingStatic);
            JsonData data = new JsonData();
            data["main_version"] = GameStaticData.LocalMainVersion;
            data["sub_version"] = GameStaticData.LocalSubVersion;
            data["cached"] = cached; // 是否有缓存

            sendRequest(Oper.Static, data, onStaticDataLoaded,
                unacceptFunc, failFormat: FailTextFormat);
        }

        /// <summary>
        /// 本地静态数据是否已有缓存
        /// </summary>
        bool isStaticDataCached() {
            return staticData.isLoaded();
        }

        /// <summary>
        /// 读取动态数据（读取静态数据之后）
        /// </summary>
        /// <param name="data">静态数据</param>
        void loadDynamicData() {
            changeState(State.LoadingDynamic);
            sendRequest(Oper.Dynamic, null, onDynamicDataLoaded,
                unacceptFunc, failFormat: FailTextFormat);
        }

        /// <summary>
        /// 刷新动态数据
        /// </summary>
        /// <param name="key">数据类型</param>
        /// <param name="accept">接受函数</param>
        /// <param name="unaccept">接受失败函数</param>
        /// <param name="waitText">等待文本</param>
        /// <param name="failText">失败文本</param>
        public void refreshDynamicData(string key, UnityAction accept, UnityAction unaccept = null) {
            Oper oper;
            // 成功回调，数据加载器
            NetworkSystem.RequestObject.SuccessAction onSuccess, loader;
            switch (key) {
                case "all":
                    loader = dynamicData.load;
                    oper = Oper.Refresh; break;
                default: return;
            }
            onSuccess = (data) => { loader.Invoke(data); accept.Invoke(); };
            sendRequest(oper, null, onSuccess, unaccept);
        }

        #endregion

        #region 获取数据

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="collection">数据集合</param>
        /// <param name="id">ID</param>
        /// <returns>目标数据</returns>
        public static T get<T>(T[] collection, int id) where T : BaseData {
            foreach (var element in collection)
                if (element.id == id) return element;
            return default;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="collection">数据集合</param>
        /// <param name="id">ID</param>
        /// <returns>目标数据</returns>
        public static T get<T>(List<T> collection, int id) where T : BaseData {
            return collection.Find((d) => d.id == id);
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="collection">数据集合</param>
        /// <param name="id">ID</param>
        /// <returns>目标数据</returns>
        public static BaseData get(BaseData[] collection, int id) {
            foreach (var element in collection)
                if (element.id == id) return element;
            return default;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="collection">数据集合</param>
        /// <param name="id">ID</param>
        /// <returns>目标数据</returns>
        public static Tuple<int, string> get(Tuple<int, string>[] collection, int id) {
            foreach (var element in collection)
                if (element.Item1 == id) return element;
            return default;
        }

        /// <summary>
        /// 获取数据索引
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="collection">数据集合</param>
        /// <param name="id">ID</param>
        /// <returns>目标数据索引</returns>
        public static int getIndex<T>(T[] collection, int id) where T : BaseData {
            for (int i = 0; i < collection.Length; ++i)
                if (collection[i].id == id) return i;
            return -1;
        }

        /// <summary>
        /// 获取数据索引
        /// </summary>
        /// <param name="collection">数据集合</param>
        /// <param name="id">ID</param>
        /// <returns>目标数据索引</returns>
        public static int getIndex(Tuple<int, string>[] collection, int id) {
            for (int i = 0; i < collection.Length; ++i)
                if (collection[i].Item1 == id) return i;
            return -1;
        }

        /// <summary>
        /// 获取静态/动态数据（不能获取组合术语数据）
        /// </summary>
        /// <typeparam name="attrName">数据名称</typeparam>
        /// <param name="id">索引</param>
        /// <param name="dataType">数据类型（0：数据库数据，1：配置数据，2：动态数据）</param>
        /// <returns>数据</returns>
        public BaseData get(string attrName, int id, int dataType = 0) {
            object obj;
            switch (dataType) {
                case 0: obj = staticData.data; break;
                case 1: obj = staticData.configure; break;
                case 2: obj = dynamicData; break;
                default: return null;
            }

            return get((BaseData[])obj.GetType().GetProperty(attrName).GetValue(obj), id);
        }

        /// <summary>
        /// 获取静态/动态数据（不能获取组合术语数据）
        /// </summary>
        /// <param name="type">数据类型</param>
        /// <param name="id">索引</param>
        /// <param name="dataType">数据类型（0：数据库数据，1：配置数据，2：动态数据）</param>
        public BaseData get(Type type, int id, int dataType = 0) {
            var attrType = type.Name;
            var attrName = char.ToLower(attrType[0]).ToString();
            attrName += attrType.Substring(1) + "s";
            return get(attrName, id, dataType);
        }
        
        /// <summary>
        /// 获取静态/动态数据（不能获取组合术语数据）
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="id">索引</param>
        /// <param name="dataType">数据类型（0：数据库数据，1：配置数据，2：动态数据）</param>
        public T get<T>(int id, int dataType = 0) where T : BaseData {
            return (T)get(typeof(T), id, dataType);
        }
        /*
        /// <summary>
        /// 获取组合术语数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="attrName">数据名称</param>
        /// <param name="id">索引</param>
        /// <returns>术语数据</returns>
        public Tuple<int, string> get(string attrName, int id) {
            var type = staticData.configure.GetType();
            return get((Tuple<int, string>[])type.
                GetProperty(attrName).GetValue(staticData.configure), id);
        }
        */
        public Tuple<int, string>[] typeDataToTuples(TypeData[] data) {
            var len = data.Length;
            var res = new Tuple<int, string>[len];
            for (int i = 0; i < len; ++i) 
                res[i] = new Tuple<int, string>(
                    data[i].id, data[i].name);
            return res;
        }

        /// <summary>
        /// 组合术语
        /// </summary>
        public Tuple<int, string> characterGender(int id) {
            return get(staticData.configure.characterGenders, id);
        }
        public Tuple<int, string> playerGrade(int id) {
            return get(staticData.configure.playerGrades, id);
        }
        public Tuple<int, string> playerStatus(int id) {
            return get(staticData.configure.playerStatuses, id);
        }
        public Tuple<int, string> playerType(int id) {
            return get(staticData.configure.playerTypes, id);
        }
        public Tuple<int, string> exermonType(int id) {
            return get(staticData.configure.exermonTypes, id);
        }
        public Tuple<int, string> exerSkillTargetType(int id) {
            return get(staticData.configure.exerSkillTargetTypes, id);
        }
        public Tuple<int, string> exerSkillHitType(int id) {
            return get(staticData.configure.exerSkillHitTypes, id);
        }
        public Tuple<int, string> questionType(int id) {
            return get(staticData.configure.questionTypes, id);
        }
        public Tuple<int, string> questionStatus(int id) {
            return get(staticData.configure.questionStatuses, id);
        }
        public Tuple<int, string> quesReportType(int id) {
            return get(staticData.configure.quesReportTypes, id);
        }
        public Tuple<int, string> recordSource(int id) {
            return get(staticData.configure.recordSources, id);
        }
        public Tuple<int, string> exerciseGenType(int id) {
            return get(staticData.configure.exerciseGenTypes, id);
        }
        public Tuple<int, string> battleMode(int id) {
            return get(staticData.configure.battleModes, id);
        }
        public Tuple<int, string> roundResultType(int id) {
            return get(staticData.configure.roundResultTypes, id);
        }
        public Tuple<int, string> battleResultType(int id) {
            return get(staticData.configure.battleResultTypes, id);
        }
        public Tuple<int, string> battleStatus(int id) {
            return get(staticData.configure.battleStatuses, id);
        }
        public Tuple<int, string> itemUseTargetType(int id) {
            return get(staticData.configure.itemUseTargetTypes, id);
        }
        public Tuple<int, string> correctType(int id) {
            return get(staticData.configure.correctTypes, id);
        }
        public Tuple<int, string> cardType(int id) {
            return get(staticData.configure.cardTypes, id);
        }
        public Tuple<int, string> enemyType(int id) {
            return get(staticData.configure.enemyTypes, id);
        }

        /// <summary>
        /// 组合配置
        /// </summary>
        public Subject subject(int id) {
            return get(staticData.configure.subjects, id);
        }
        public BaseParam baseParam(int id) {
            return get(staticData.configure.baseParams, id);
        }
        public UsableItemType usableItemType(int id) {
            return get(staticData.configure.usableItemTypes, id);
        }
        public HumanEquipType humanEquipType(int id) {
            return get(staticData.configure.humanEquipTypes, id);
        }
        public ExerEquipType exerEquipType(int id) {
            return get(staticData.configure.exerEquipTypes, id);
        }
        public ExerStar exerStar(int id) {
            return get(staticData.configure.exerStars, id);
        }
        public ExerGiftStar exerGiftStar(int id) {
            return get(staticData.configure.exerGiftStars, id);
        }
        public ItemStar itemStar(int id) {
            return get(staticData.configure.itemStars, id);
        }
        public QuestionStar questionStar(int id) {
            return get(staticData.configure.questionStars, id);
        }
        public CompRank compRank(int id) {
            return get(staticData.configure.compRanks, id);
        }
        public ResultJudge resultJudge(int id) {
            return get(staticData.configure.resultJudges, id);
		}
		public GameTip gameTip(int id) {
			return get(staticData.configure.gameTips, id);
		}

		public Antonym antonym(int id) {
            return get(staticData.configure.antonyms, id);
        }
        public ExerProItemStar exerProItemStar(int id) {
            return get(staticData.configure.exerProItemStars, id);
		}
		public NodeType nodeType(int id) {
			return get(staticData.configure.nodeTypes, id);
		}
		public FirstCardGroup firstCardGroup(int id) {
			return get(staticData.configure.firstCardGroups, id);
		}

		/// <summary>
		/// 数据库
		/// </summary>
		public Character character(int id) {
            return get(staticData.data.characters, id);
        }
        public Exermon exermon(int id) {
            return get(staticData.data.exermons, id);
        }
        public ExerFrag exerFrag(int id) {
            return get(staticData.data.exerFrags, id);
        }
        public ExerSkill exerSkill(int id) {
            return get(staticData.data.exerSkills, id);
        }
        public ExerGift exerGift(int id) {
            return get(staticData.data.exerGifts, id);
        }
        public HumanItem humanItem(int id) {
            return get(staticData.data.humanItems, id);
        }
        public HumanEquip humanEquip(int id) {
            return get(staticData.data.humanEquips, id);
        }
        public ExerItem exerItem(int id) {
            return get(staticData.data.exerItems, id);
        }
        public ExerEquip exerEquip(int id) {
            return get(staticData.data.exerEquips, id);
        }
        public QuesSugar quesSugar(int id) {
            return get(staticData.data.quesSugars, id);
        }
        public ExerProItem exerProItem(int id) {
            return get(staticData.data.exerProItems, id);
        }
        public ExerProPotion exerProPotion(int id) {
            return get(staticData.data.exerProPotions, id);
        }
        public ExerProCard exerProCard(int id) {
            return get(staticData.data.exerProCards, id);
        }
        public ExerProEnemy exerProEnemy(int id) {
            return get(staticData.data.exerProEnemies, id);
        }
        public ExerProState exerProState(int id) {
            return get(staticData.data.exerProStates, id);
        }
        public ExerProMap exerProMap(int id) {
            return get(staticData.data.exerProMaps, id);
        }

        /// <summary>
        /// 动态数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CompSeason season(int id) {
            return get(dynamicData.seasons, id);
        }

        #endregion

        #region 回调控制

        /// <summary>
        /// 静态数据读取回调
        /// </summary>
        /// <param name="data">静态数据</param>
        void onStaticDataLoaded(JsonData data) {
            staticData.load(DataLoader.load(data, "data"));
            storageSys.save();
            loadDynamicData();
        }

        /// <summary>
        /// 初始化成功回调
        /// </summary>
        /// <param name="data">数据</param>
        void onDynamicDataLoaded(JsonData data) {
            dynamicData.load(DataLoader.load(data, "data"));
            changeState(State.Loaded);
        }

        #endregion

    }
}