﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StarResonanceDpsAnalysis.Core.Data.Models;

namespace StarResonanceDpsAnalysis.Core.Data
{
    public class BattleLogRecorder
    {
        /// <summary>
        /// 战斗日志列表
        /// </summary>
        private readonly List<BattleLog> _battleLogs = [];

        /// <summary>
        /// 录制器状态
        /// </summary>
        public RunningState State { get; private set; } = RunningState.Standby;
        /// <summary>
        /// 战斗日志列表
        /// </summary>
        public List<BattleLog> BattleLogs
        {
            get 
            {
                lock (this)
                {
                    if (State == RunningState.Standby)
                        return [];
                    if (State == RunningState.Running)
                        throw new InvalidOperationException("Cannot access BattleLogs while recorder is running.");
                    return _battleLogs; 
                }
            }
        }

        /// <summary>
        /// 启动战斗日志录制器
        /// </summary>
        /// <exception cref="InvalidOperationException">录制器不能在已启动或已停止状态再次启动</exception>
        public void Start() 
        {
            lock (this)
            {
                if (State != RunningState.Standby)
                    throw new InvalidOperationException("Recorder is already started or stopped.");

                DataStorage.BattleLogCreated += OnBattleLogCreated; 
            }
        }

        /// <summary>
        /// 新战斗日志创建事件
        /// </summary>
        /// <param name="battleLog">战斗日志</param>
        private void OnBattleLogCreated(BattleLog battleLog)
        {
            lock (this)
            {
                _battleLogs.Add(battleLog); 
            }
        }

        /// <summary>
        /// 停止战斗日志录制器
        /// </summary>
        /// <returns>战斗日志列表</returns>
        /// <exception cref="InvalidOperationException">录制器不能在还未启动时关闭</exception>
        public List<BattleLog> Stop()
        {
            lock (this)
            {
                if (State != RunningState.Running)
                    throw new InvalidOperationException("Recorder is not running.");

                DataStorage.BattleLogCreated -= OnBattleLogCreated;

                State = RunningState.Stopped;

                return BattleLogs; 
            }
        }

        /// <summary>
        /// 启动新的战斗日志录制器
        /// </summary>
        /// <returns>战斗日志录制器</returns>
        public static BattleLogRecorder StartNew()
        {
            var recorder = new BattleLogRecorder();
            recorder.Start();

            return recorder;
        }
    }

    /// <summary>
    /// 录制器运行状态
    /// </summary>
    public enum RunningState : int
    {
        Standby = 0,
        Running = 1,
        Stopped = 2
    }
}
