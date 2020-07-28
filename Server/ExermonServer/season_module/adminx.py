import xadmin

from .models import *
from utils.model_utils import AdminXHelper


@AdminXHelper.relatedModel(SeasonRecord)
class SeasonRecordAdmin(object):
    # list_display = ['id', 'player', 'season', 'point', 'star_num']
    #
    # list_editable = ['player', 'season', 'point', 'star_num']
    pass


@AdminXHelper.relatedModel(SuspensionRecord)
class SuspensionRecordAdmin(object):
    # list_display = ['id', 'season_rec', 'start_time', 'end_time']
    #
    # list_editable = ['season_rec', 'start_time', 'end_time']
    pass


@AdminXHelper.relatedModel(CompSeason)
class CompSeasonAdmin(object):
    # list_display = ['id', 'name', 'start_time', 'end_time']
    #
    # list_editable = ['name', 'start_time', 'end_time']
    pass


@AdminXHelper.relatedModel(CompRank)
class CompRankAdmin(object):
    # list_display = ['id', 'name', 'adminColor', 'sub_rank_num', 'score_factor', 'offset_factor']
    #
    # list_editable = ['name', 'sub_rank_num', 'score_factor', 'offset_factor']
    pass
