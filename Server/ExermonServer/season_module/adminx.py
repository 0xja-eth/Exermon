import xadmin

from .models import *


@xadmin.sites.register(SeasonRecord)
class SeasonRecordAdmin(object):
    list_display = ['player','season','point','star_num']

    list_editable = ['player','season','point','star_num']



@xadmin.sites.register(SuspensionRecord)
class SuspensionRecordAdmin(object):
    list_display = ['season_rec','start_time','end_time']

    list_editable = ['season_rec','start_time','end_time']


@xadmin.sites.register(CompSeason)
class CompSeasonAdmin(object):
    list_display = ['season_name','start_time','end_time']

    list_editable = ['season_name','start_time','end_time']

@xadmin.sites.register(CompRank)
class CompRankAdmin(object):
    list_display = ['rank_name','sub_rank_num','score_factor','offset_factor']

    list_editable = ['rank_name','sub_rank_num','score_factor','offset_factor']