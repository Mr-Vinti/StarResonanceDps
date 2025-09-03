﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarResonanceDpsAnalysis.Core.Data.Models
{
    public class DpsData
    {
        /// <summary>
        /// 玩家UID
        /// </summary>
        public long UID { get; internal set; }
        /// <summary>
        /// 统计开始的时间戳 (Ticks)
        /// </summary>
        public long? StartLoggedTick { get; internal set; }
        /// <summary>
        /// 最后一次统计的时间戳 (Ticks)
        /// </summary>
        public long LastLoggedTick { get; internal set; }
        /// <summary>
        /// 统计的总伤害
        /// </summary>
        public long TotalAttackDamage { get; internal set; }
        /// <summary>
        /// 统计的总承受伤害
        /// </summary>
        public long TotalTakenDamage { get; internal set; }
        /// <summary>
        /// 统计的总治疗量
        /// </summary>
        public long TotalHeal { get; internal set; }
        /// <summary>
        /// 是否为NPC数据
        /// </summary>
        public bool IsNpcData { get; internal set; } = false;
        /// <summary>
        /// 战斗日志列表
        /// </summary>
        internal List<BattleLog> BattleLogs { get; } = new(16384);
        /// <summary>
        /// 只读战斗日志列表
        /// </summary>
        public IReadOnlyList<BattleLog> ReadOnlyBattleLogs { get => BattleLogs.AsReadOnly(); }
        /// <summary>
        /// 技能统计数据字典
        /// </summary>
        internal Dictionary<long, SkillData> SkillDic { get; } = [];
        /// <summary>
        /// 只读技能统计数据字典
        /// </summary>
        public ReadOnlyDictionary<long, SkillData> ReadOnlySkillDatas { get => SkillDic.AsReadOnly(); }
        /// <summary>
        /// 只读技能统计数据列表
        /// </summary>
        public IReadOnlyList<SkillData> ReadOnlySkillDataList { get => SkillDic.Values.ToList().AsReadOnly(); }
        /// <summary>
        /// 获取或创建技能统计数据
        /// </summary>
        /// <param name="skillId">技能UID</param>
        /// <returns></returns>

        public SkillData GetOrCreateSkillData(long skillId)
        {
            if (!SkillDic.TryGetValue(skillId, out var skillData))
            {
                skillData = new SkillData()
                {
                    SkillId = skillId,
                };
                SkillDic[skillId] = skillData;
            }

            return skillData;
        }
    }
}
