# chat/routing.py
from django.conf.urls import re_path
from game_module.views import Service as Game
from player_module.views import Service as Player
from item_module.views import Service as Item
from exermon_module.views import Service as Exermon
from record_module.views import Service as Record
from question_module.views import Service as Question
from season_module.views import Service as Season
from battle_module.views import Service as Battle
from english_pro_module.views import Service as EnglishPro

from .consumer import ChannelLayerTag, GameConsumer

websocket_urlpatterns = [
    re_path('game/', GameConsumer),
]

DISCONNECT_ROUTE = 'player/player/disconnect'

WEBSOCKET_METHOD_ROUTER = {
    # 获取静态数据
    'game/data/static': [[
        ['main_version', 'str'],
        ['sub_version', 'str'],
        ['cached', 'bool']
    ],
        Game.getStaticData,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 获取动态数据
    'game/data/dynamic': [[],
        Game.getDynamicData,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 玩家注册
    'player/player/register': [[
        ['un', 'str'],
        ['pw', 'str'],
        ['email', 'str'],
        ['code', 'str']
    ],
        Player.register,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 玩家登陆
    'player/player/login': [[
        ['un', 'str'],
        ['pw', 'str']
    ],
        Player.login,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 玩家忘记密码
    'player/player/retrieve': [[
        ['un', 'str'],
        ['pw', 'str'],
        ['email', 'str'],
        ['code', 'str']
    ],
        Player.retrieve,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 发送验证码
    'player/player/code': [[
        ['un', 'str'],
        ['email', 'str'],
        ['type', 'str']
    ],
        Player.sendCode,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 登出（客户端调用）
    'player/player/logout': [[
        ['uid', 'int']
    ],
        Player.logout,  # 处理函数
        ChannelLayerTag.NoLayer  # 是否需要响应
    ],
    # 断开连接（内部调用）
    'player/player/disconnect': [[
        ['uid', 'int'],
        ['auth', 'str']
    ],
        Player.disconnect,  # 处理函数
        ChannelLayerTag.NoLayer  # 是否需要响应
    ],
    # 玩家重连
    'player/player/reconnect': [[
        ['un', 'str'],
        ['pw', 'str'],
    ],
        Player.reconnect,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 选择玩家形象
    'player/create/character': [[
        ['uid', 'int'],
        ['name', 'str'],
        ['grade', 'int'],
        ['cid', 'int'],
    ],
        Player.createCharacter,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 选择艾瑟萌
    'player/create/exermons': [[
        ['uid', 'int'],
        ['eids', 'int[]'],
        ['enames', 'str[]'],
    ],
        Player.createExermons,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 选择艾瑟萌天赋
    'player/create/gifts': [[
        ['uid', 'int'],
        ['gids', 'int[]'],
    ],
        Player.createGifts,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 完善个人信息
    'player/create/info': [[
        ['uid', 'int'],
        ['birth', 'var'],
        ['school', 'var'],
        ['city', 'var'],
        ['contact', 'var'],
        ['description', 'var'],
    ],
        Player.createInfo,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 获取玩家基本信息
    'player/get/basic': [[
        ['uid', 'int'],
        ['get_uid', 'int'],
    ],
        Player.getBasic,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 获取玩家状态界面信息
    'player/get/status': [[
        ['uid', 'int'],
        ['get_uid', 'int'],
    ],
        Player.getStatus,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 获取玩家对战界面信息
    'player/get/battle': [[
        ['uid', 'int'],
        ['get_uid', 'int'],
    ],
        Player.getBattle,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 获取玩家对战界面信息
    'player/get/pack': [[
        ['uid', 'int'],
        ['get_uid', 'int'],
    ],
        Player.getPack,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 玩家修改昵称
    'player/edit/name': [[
        ['uid', 'int'],
        ['name', 'str'],
    ],
        Player.editName,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 人类装备槽装备
    'player/equipslot/equip': [[
        ['uid', 'int'],
        ['heid', 'int'],
    ],
        Player.equipSlotEquip,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 人类装备槽卸下
    'player/equipslot/dequip': [[
        ['uid', 'int'],
        ['type', 'int'],
    ],
        Player.equipSlotDequip,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 玩家修改个人信息
    'player/edit/info': [[
        ['uid', 'int'],
        ['grade', 'int'],
        ['birth', 'var'],
        ['school', 'var'],
        ['city', 'var'],
        ['contact', 'var'],
        ['description', 'var'],
    ],
        Player.editInfo,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 获取背包类容器项数据
    'item/packcontainer/get': [[
        ['uid', 'int'],
        ['type', 'int'],
        ['cid', 'var'],
    ],
        Item.packContainerGet,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 获取背包类容器项数据
    'item/slotcontainer/get': [[
        ['uid', 'int'],
        ['type', 'int'],
        ['cid', 'var'],
    ],
        Item.slotContainerGet,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 背包类容器获得物品
    'item/packcontainer/gain': [[
        ['uid', 'int'],
        ['type', 'int'],
        ['i_type', 'int'],
        ['item_id', 'int'],
        ['count', 'int'],
        ['refresh', 'bool'],
    ],
        Item.packContainerGain,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 背包类容器获得容器项
    'item/packcontainer/gain_contitems': [[
        ['uid', 'int'],
        ['type', 'int'],
        ['ci_types', 'int[]'],
        ['contitem_ids', 'int[]'],
        ['fixed', 'bool'],
        ['refresh', 'bool'],
    ],
        Item.packContainerGainContItems,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 背包类容器失去容器项
    'item/packcontainer/lost_contitems': [[
        ['uid', 'int'],
        ['type', 'int'],
        ['ci_types', 'int[]'],
        ['contitem_ids', 'int[]'],
        ['counts', 'var'],
        ['fixed', 'bool'],
        ['refresh', 'bool'],
    ],
        Item.packContainerLostContItems,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 背包类容器转移
    'item/packcontainer/transfer': [[
        ['uid', 'int'],
        ['type', 'int'],
        ['target_cid', 'int'],
        ['ci_types', 'int[]'],
        ['contitem_id', 'int[]'],
        ['count', 'var'],
    ],
        Item.packContainerTransfer,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 背包类容器拆分
    'item/packcontainer/split': [[
        ['uid', 'int'],
        ['type', 'int'],
        ['ci_type', 'int'],
        ['contitem_id', 'int'],
        ['count', 'int'],
    ],
        Item.packContainerSplit,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 背包类容器组合
    'item/packcontainer/merge': [[
        ['uid', 'int'],
        ['type', 'int'],
        ['ci_type', 'int'],
        ['contitem_ids', 'int[]'],
    ],
        Item.packContainerMerge,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 背包类容器使用
    'item/packcontainer/use': [[
        ['uid', 'int'],
        ['type', 'int'],
        ['ci_type', 'int'],
        ['contitem_id', 'int'],
        ['count', 'int'],
        ['occasion', 'int'],
        ['target', 'var'],
    ],
        Item.packContainerUse,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 背包类容器丢弃
    'item/packcontainer/discard': [[
        ['uid', 'int'],
        ['type', 'int'],
        ['ci_type', 'int'],
        ['contitem_id', 'int'],
        ['count', 'int'],
    ],
        Item.packContainerDiscard,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 背包类容器出售
    'item/packcontainer/sell': [[
        ['uid', 'int'],
        ['type', 'int'],
        ['ci_type', 'int'],
        ['contitem_id', 'int'],
        ['count', 'int'],
    ],
        Item.packContainerSell,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 艾瑟萌槽装备艾瑟萌
    'exermon/equip/playerexer': [[
        ['uid', 'int'],
        ['sid', 'int'],
        ['peid', 'int'],
    ],
        Exermon.equipPlayerExer,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 艾瑟萌槽装备天赋
    'exermon/equip/playergift': [[
        ['uid', 'int'],
        ['sid', 'int'],
        ['pgid', 'int'],
    ],
        Exermon.equipPlayerGift,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 艾瑟萌装备槽装备
    'exermon/equip/exerequip': [[
        ['uid', 'int'],
        ['sid', 'int'],
        ['eeid', 'int'],
    ],
        Exermon.equipExerEquip,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 艾瑟萌装备槽装备
    'exermon/dequip/exerequip': [[
        ['uid', 'int'],
        ['sid', 'int'],
        ['type', 'int'],
    ],
        Exermon.dequipExerEquip,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 艾瑟萌改名
    'exermon/edit/nickname': [[
        ['uid', 'int'],
        ['peid', 'int'],
        ['name', 'str']
    ],
        Exermon.editNickname,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 查询记录
    'record/record/get': [[
        ['uid', 'int']
    ],
        Record.get,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 收藏/解除收藏题目
    'record/question/collect': [[
        ['uid', 'int'],
        ['qid', 'int']
    ],
        Record.collect,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 解除错题
    'record/question/unwrong': [[
        ['uid', 'int'],
        ['qid', 'int']
    ],
        Record.unwrong,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 添加备注
    'record/question/note': [[
        ['uid', 'int'],
        ['qid', 'int'],
        ['note', 'str']
    ],
        Record.note,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 生成刷题
    'record/exercise/generate': [[
        ['uid', 'int'],
        ['sid', 'int'],
        ['gen_type', 'int'],
        ['count', 'int'],
    ],
        Record.exerciseGenerate,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 开始答题
    'record/exercise/start': [[
        ['uid', 'int'],
        ['qid', 'int'],
    ],
        Record.exerciseStart,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 作答刷题题目
    'record/exercise/answer': [[
        ['uid', 'int'],
        ['qid', 'int'],
        ['selection', 'int[]'],
        ['timespan', 'int'],
        ['terminate', 'bool']
    ],
        Record.exerciseAnswer,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 查询题目
    'question/question/get': [[
        ['uid', 'int'],
        ['qids', 'int[]']
    ],
        Question.get,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 查询题目详情
    'question/detail/get': [[
        ['uid', 'int'],
        ['qid', 'int'],
    ],
        Question.getDetail,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 装备对战物资槽
    'battle/equip/item': [[
        ['uid', 'int'],
        ['index', 'int'],
        ['contitem_id', 'int'],
    ],
        Battle.equipItem,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],

    # 卸下对战物资槽
    'battle/dequip/item': [[
        ['uid', 'int'],
        ['index', 'int'],
    ],
        Battle.dequipItem,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],

    # 对战开始匹配
    'battle/match/start': [[
        ['uid', 'int'],
        ['mode', 'int'],
    ],
        Battle.matchStart,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 对战取消匹配
    'battle/match/cancel': [[
        ['uid', 'int'],
    ],
        Battle.matchCancel,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 对战匹配进度
    'battle/match/progress': [[
        ['uid', 'int'],
        ['progress', 'int'],
    ],
        Battle.matchProgress,  # 处理函数
        ChannelLayerTag.NoLayer  # 是否需要响应
    ],
    # 对战准备阶段完成
    'battle/prepare/complete': [[
        ['uid', 'int'],
        ['type', 'var'],
        ['contitem_id', 'var'],
    ],
        Battle.prepareComplete,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 对战作答题目
    'battle/question/answer': [[
        ['uid', 'int'],
        ['selection', 'int[]'],
        ['timespan', 'int'],
    ],
        Battle.questionAnswer,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 题目结果查看完成
    'battle/question/complete': [[
        ['uid', 'int'],
    ],
        Battle.questionComplete,  # 处理函数
        ChannelLayerTag.NoLayer  # 是否需要响应
    ],
    # 对战行动分镜播放完成
    'battle/action/complete': [[
        ['uid', 'int'],
    ],
        Battle.actionComplete,  # 处理函数
        ChannelLayerTag.NoLayer  # 是否需要响应
    ],
    # 对战结果播放完成
    'battle/result/complete': [[
        ['uid', 'int'],
    ],
        Battle.resultComplete,  # 处理函数
        ChannelLayerTag.NoLayer  # 是否需要响应
    ],

    # 查询玩家题目反馈
    'question/report/get': [[
        ['uid', 'int'],
    ],
        Question.getReports,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],

    # 提交题目反馈
    'question/report/push': [[
        ['uid', 'int'],
        ['qid', 'int'],
        ['type', 'int'],
        ['description', 'str'],
    ],
        Question.pushReport,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],

    # 查询赛季记录
    'season/record/get': [[
        ['uid', 'int'],
        ['sid', 'int'],
    ],
        Season.getRecord,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 查询赛季排行
    'season/rank/get': [[
        ['uid', 'int'],
        ['sid', 'int'],
        ['count', 'int'],
    ],
        Season.getRanks,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 购买物品
    'item/shop/buy': [[
        ['uid', 'int'],
        ['type', 'int'],
        ['item_id', 'int'],
        ['count', 'int'],
        ['buy_type', 'int'],
    ],
        Item.shopBuy,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 获取商品
    'item/shop/get': [[
        ['uid', 'int'],
        ['type', 'int'],
    ],
        Item.shopGet,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 开始英语特训
    'engpro/record/start': [[
        ['uid', 'int'],
        ['mid', 'int'],
    ],
        EnglishPro.startRecord,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 保存英语特训
    'engpro/record/save': [[
        ['uid', 'int'],
        ['record', 'dict'],
        ['terminate', 'bool'],
    ],
        EnglishPro.saveRecord,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 生成英语特训题目
    'engpro/question/generate': [[
        ['uid', 'int'],
        ['type', 'int'],
        ['count', 'int'],
    ],
        EnglishPro.generateQuestions,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 查询英语特训题目
    'engpro/question/get': [[
        ['uid', 'int'],
        ['type', 'int'],
        ['qids', 'int[]'],
    ],
        EnglishPro.getQuestions,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 生成当前轮单词
    'engpro/word/generate': [[
        ['uid', 'int'],
    ],
        EnglishPro.generateWords,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 回答当前轮单词
    'engpro/word/answer': [[
        ['uid', 'int'],
        ['wid', 'int'],
        ['chinese', 'str']
    ],
        EnglishPro.answerWord,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 查询单词
    'engpro/word/get': [[
        ['uid', 'int'],
        ['wids', 'int[]']
    ],
        EnglishPro.getWords,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 查询单词记录
    'engpro/record/get': [[
        ['uid', 'int'],
    ],
        EnglishPro.getRecords,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # # 查询当前轮单词
    # 'engpro/word/query': [[
    #     ['uid', 'int'],
    # ],
    #     EnglishPro.queryWords,  # 处理函数
    #     ChannelLayerTag.Self  # 是否需要响应
    # ],

    # 商品生成
    'engpro/shop/generate': [[
        ['uid', 'int'],
        ['type', 'int'],
    ],
        EnglishPro.shopGenerate,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 商品购买校验
    'engpro/shop/buy': [[
        ['uid', 'int'],
        ['type', 'int'],
        ['order', 'int'],
    ],
        EnglishPro.shopBuy,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 回答短语题目
    'engpro/answer/phrase': [[
        ['uid', 'int'],
        ['qids', 'int[]'],
        ['answers', 'str[]'],
    ],
        EnglishPro.answerPhrase,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 回答改错题目
    'engpro/answer/correction': [[
        ['uid', 'int'],
        ['qid', 'int'],
        ['answers', 'dict'],
    ],
        EnglishPro.answerCorrection,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
    # 回答听力题目
    'engpro/answer/listening': [[
        ['uid', 'int'],
        ['qid', 'int'],
        ['answers', 'int[]'],
    ],
        EnglishPro.answerListening,  # 处理函数
        ChannelLayerTag.Self  # 是否需要响应
    ],
}
