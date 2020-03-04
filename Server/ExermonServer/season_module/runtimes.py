from season_module import models as Season
from utils import exception
from utils.runtime_manager import RuntimeData,RuntimeManager
from game_module.consumer import GameConsumer
import datetime

class SeasonManager(RuntimeManager):

    currentSeason = RuntimeData()

    #计算当前赛季
    @classmethod
    def computeSeason(cls):

        #获取所有赛季
        allSeason = Season.CompSeason.objs()

        #当前日期和时间
        cDateTime = datetime.datetime.now()

        #按照时间顺序从将来往以前读取赛季信息
        try:
            for i in allSeason:
                #当前日期和时间小于某个赛季的开始时间
                if cDateTime.__le__(i.start_time):
                    currentSeason = i-1
                    return currentSeason
        except exception.ErrorType.DatabaseError:
            return None


    #维护赛季，即检查日期
    async def maintainSeason(self):
        
        cDateTime = datetime.datetime.now()

        #超过了目前赛季的终止日期
        if cDateTime.__ge__(self.currentSeason.end_time):
            await self.onSeasonChanged()
            #广播赛季切换信息
            GameConsumer.broadcast('link',"赛季切换信息广播……")


    #切换赛季
    def onSeasonChanged(self,cls):
        self.currentSeason = cls.computeSeason()


RuntimeManager.register(SeasonManager)
RuntimeManager.registerEvent(SeasonManager.maintainSeason)


