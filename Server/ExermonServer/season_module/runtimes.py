from season_module.models import CompSeason
from utils.runtime_manager import RuntimeManager
from game_module.consumer import EmitType, GameConsumer
import datetime


# ===================================================
# 赛季运行时管理类
# ===================================================
class SeasonManager:

    current_season: CompSeason = None

    # 计算当前赛季
    @classmethod
    def computeSeason(cls):

        # 获取所有赛季
        seasons = CompSeason.objs()

        # 当前日期和时间
        now = datetime.datetime.now()

        # 按照时间顺序从将来往以前读取赛季信息
        for i in seasons:
            # 当前日期和时间小于某个赛季的开始时间
            if now <= i.start_time:
                current_season = i-1
                return current_season

        return None

    # 维护赛季，即检查日期
    @classmethod
    async def maintainSeason(cls):
        
        now = datetime.datetime.now()

        # 超过了目前赛季的终止日期
        if now >= cls.current_season.end_time:
            cls.onSeasonChanged()



            # 广播赛季切换信息
            await GameConsumer.broadcast(EmitType.SeasonSwitch,
                                         "赛季切换信息广播……")

    # 生成最新赛季数据
    @classmethod
    def generateSeasonData(cls):
        return cls.current_season.convertToDict()

    # 切换赛季
    @classmethod
    def onSeasonChanged(cls):
        cls.current_season = cls.computeSeason()


RuntimeManager.registerEvent(SeasonManager.maintainSeason)


