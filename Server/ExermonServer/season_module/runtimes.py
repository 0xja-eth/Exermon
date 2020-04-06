from season_module.models import CompSeason
from utils.runtime_manager import RuntimeManager
from game_module.consumer import EmitType, GameConsumer
import datetime


# ===================================================
# 赛季运行时管理类
# ===================================================
class SeasonManager:

    # 当前赛季
    CurrentSeason: CompSeason = None

    # 赛季切换回调事件列表
    OnSeasonChanged: list = None

    # 当前赛季排名最高的 n 个用户-lgy4.2
    TopRank : list = None

    # TopRank的更新时间
    TopRankUpdateTime : datetime.datetime = None

    @classmethod
    def computeSeason(cls) -> CompSeason:
        """
        计算当前赛季
        Returns:
            返回当前赛季，没有则返回 None
        """
        # 获取所有赛季
        seasons = CompSeason.objs()

        # 当前日期和时间
        now = datetime.datetime.now()

        # 按照时间顺序从将来往以前读取赛季信息
        for season in seasons:
            # 当前日期和时间小于某个赛季的开始时间
            if season.start_time <= now < season.end_time:
                current_season = season
                return current_season

        return None

    @classmethod
    def refreshSeason(cls):
        """
        刷新当前赛季
        """
        cls.CurrentSeason = cls.computeSeason()

    @classmethod
    def getCurrentSeason(cls) -> CompSeason:
        """
        获取缓存的当前赛季（若无缓存，自动计算）
        Returns:
            返回当前赛季
        """
        if cls.CurrentSeason is None:
            cls.refreshSeason()

        return cls.CurrentSeason

    @classmethod
    async def maintainSeason(cls):

        now = datetime.datetime.now()

        season = cls.getCurrentSeason()

        if season is None: return

        # 超过了目前赛季的终止日期
        if now >= season.end_time:
            await cls.onSeasonChanged()

    @classmethod
    async def onSeasonChanged(cls):
        """
        赛季切换回调
        """
        cls.refreshSeason()
        await cls.broadcastSeasonChange()

    @classmethod
    async def broadcastSeasonChange(cls):
        """
        广播赛季切换信息
        """
        await GameConsumer.broadcast(EmitType.SeasonSwitch,
                               {'season_id': cls.CurrentSeason.id})

    @classmethod
    async def topRank(cls,server_on=False):
        '''
        管理赛季高分排行榜
        '''
        now = datetime.datetime.now()

        if server_on==False or now.minute==0 or now.minute==30:

            server_on = True

            # TopRank是一个列表，里面每一条赛季记录是一个字典
            # [ {},{},...,{} ]
            cls.TopRank = CompSeason._convertRanksData(CompSeason.MAX_RANK)['ranks']
            cls.TopRankUpdateTime = now

    @classmethod
    async def getTopRank(cls, num=None):

        return cls.TopRank[:num+1]


RuntimeManager.registerEvent(SeasonManager.maintainSeason)
RuntimeManager.registerEvent(SeasonManager.topRankManager)


