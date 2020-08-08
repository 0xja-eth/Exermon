

# ===================================================
# 根据星星数量计算出当前段位数和子段位
# ===================================================
class CompRankCalc:

    @classmethod
    def starNum2Rank(cls, star_num) -> ('CompRank', int, int):
        """
        原来：#def rank(self) -> ('CompRank', int):
        根据星星数计算当前实际段位
        Returns:
            返回实际段位对象（CompRank），子段位数目（从0开始）以及剩余星星数
        Examples:
            0 = > 学渣I(1, 0, 0)
            1 = > 学渣I(1, 0, 1)
            2 = > 学渣I(1, 0, 2)
            3 = > 学渣I(1, 0, 3)
            4 = > 学渣II(1, 1, 1)
            5 = > 学渣II(1, 1, 2)
            6 = > 学渣II(1, 1, 3)
            7 = > 学渣III(1, 2, 1)
            10 = > 学酥I(2, 1, 1)
        """
        from season_module.models import CompRank

        # ranks 储存了段位列表中的每一个段位的详细信息
        ranks = CompRank.objs()

        # 需要保证数据库的数据有序
        for rank in ranks:
            rank_stars = rank.rankStars()

            # 判断最后一个段位
            if rank_stars == 0:
                return rank, 0, star_num

            # 如果星星数目还可以扣
            if star_num > rank_stars:
                star_num -= rank_stars
            else:
                tmp_star = star_num - 1
                if tmp_star < 0:
                    sub_rank = star_num = 0
                else:
                    sub_rank = int(tmp_star / CompRank.STARS_PER_SUBRANK)
                    star_num = (tmp_star % CompRank.STARS_PER_SUBRANK) + 1

                return rank, sub_rank, star_num

        return None, 0, star_num

    @classmethod
    def rank2StarNum(cls, sub_rank: int, star_num: int = 0,
                     rank: 'CompRank' = None, rank_id: int = None) -> int:
        """
        根据段位信息计算星星数
        Args:
            sub_rank (int): 子短位数
            star_num (int): 剩余星星数
            rank (CompRank): 段位实例
            rank_id (int): 段位ID
        Returns:
            返回由段位计算出来的星星数量
        """
        from season_module.models import CompRank

        if rank_id is None: rank_id = rank.id

        # ranks 储存了段位列表中的每一个段位的详细信息
        ranks = CompRank.objs()
        rank_id_ = 1  # 用于标识大段位
        # star_num = star_num

        for rank_ in ranks:
            if rank_id_ < rank_id:
                star_num += rank_.rankStars()
                rank_id_ += 1

            # 大段位计算好了，剩下小段位
            if rank_id_ == rank_id:
                star_num += sub_rank * CompRank.STARS_PER_SUBRANK
                break

        return star_num


# ===================================================
# 赛季切换，根据当前的段位星星计算新赛季的初始段位星星-lgy4.15
# ===================================================
class NewSeasonStarNumCalc:

    @classmethod
    def calc(cls, star_num):

        rank, sub_rank, star_num = CompRankCalc.starNum2Rank(star_num)
        data = (rank.id, sub_rank)

        # 段位元组第三位，不满三个的星星直接清零了
        if data < (3, 2):
            new_rank = (1, 1, 0)
        elif data < (3, 4):
            new_rank = (3, 2, 0)
        elif data < (4, 2):
            new_rank = (3, 3, 0)
        elif data < (4, 4):
            new_rank = (3, 4, 0)
        elif data < (5, 2):
            new_rank = (4, 3, 0)
        elif data < (5, 5):
            new_rank = (4, 4, 0)
        else:
            new_rank = (4, 5, 0)

        star_num = CompRankCalc.rank2StarNum(
            new_rank[1], new_rank[2], rank_id=new_rank[0])

        return star_num

