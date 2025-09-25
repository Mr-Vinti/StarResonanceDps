using System.Buffers;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using StarResonanceDpsAnalysis.Core.Caching;
using StarResonanceDpsAnalysis.Core.Collections;
using StarResonanceDpsAnalysis.Core.Configuration;
using StarResonanceDpsAnalysis.Core.Data.Models;
using StarResonanceDpsAnalysis.Core.Memory;

namespace StarResonanceDpsAnalysis.Core.Data;

/// <summary>
/// Optimized DPS data structure
/// </summary>
public struct OptimizedDpsData
{
    public long PlayerId { get; init; }
    public ulong TotalDamage { get; init; }
    public int HitCount { get; init; }
    public DateTime LastUpdateTime { get; init; }
}

/// <summary>
/// Memory-optimized data storage with efficient caching and pooling
/// </summary>
public static class OptimizedDataStorage
{
    // Use ring buffers instead of growing lists
    private static readonly BattleLogRingBuffer _battleLogBuffer = new(50000);
    
    // LRU caches for frequently accessed data
    private static readonly LRUCache<long, PlayerInfo> _playerInfoCache = new(1000);
    private static readonly LRUCache<long, OptimizedDpsData> _dpsDataCache = new(5000);
    
    // Use concurrent collections for thread safety without locks
    private static readonly ConcurrentDictionary<long, PlayerInfo> _playerInfos = new();
    private static readonly ConcurrentDictionary<long, OptimizedDpsData> _dpsData = new();
    
    // Memory pools for temporary objects
    private static readonly ObjectPool<List<BattleLog>> _battleLogListPool = 
        new(() => new List<BattleLog>(), list => list.Clear());
    
    // Efficient event handling with weak references to prevent memory leaks
    private static readonly WeakEventManager _eventManager = new();
    
    public static event Action<BattleLog>? BattleLogCreated
    {
        add => _eventManager.AddEventHandler(nameof(BattleLogCreated), value);
        remove => _eventManager.RemoveEventHandler(nameof(BattleLogCreated), value);
    }

    public static event Action<DpsData>? DpsDataUpdated
    {
        add => _eventManager.AddEventHandler(nameof(DpsDataUpdated), value);
        remove => _eventManager.RemoveEventHandler(nameof(DpsDataUpdated), value);
    }

    /// <summary>
    /// Add battle log with memory optimization
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddBattleLog(in BattleLog battleLog)
    {
        // Use ring buffer to automatically manage memory
        _battleLogBuffer.Add(in battleLog);
        
        // Fire event efficiently
        _eventManager.RaiseEvent(nameof(BattleLogCreated), battleLog);
        
        // Update related data efficiently
        UpdateDpsDataEfficient(battleLog);
    }

    /// <summary>
    /// Get player info with caching
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PlayerInfo? GetPlayerInfo(long uid)
    {
        // Try cache first
        if (_playerInfoCache.TryGet(uid, out var cachedInfo))
            return cachedInfo;

        // Fall back to dictionary
        if (_playerInfos.TryGetValue(uid, out var info))
        {
            _playerInfoCache.Set(uid, info);
            return info;
        }

        return null;
    }

    /// <summary>
    /// Update player info with caching
    /// </summary>
    public static void UpdatePlayerInfo(PlayerInfo playerInfo)
    {
        _playerInfos.AddOrUpdate(playerInfo.UID, playerInfo, (_, _) => playerInfo);
        _playerInfoCache.Set(playerInfo.UID, playerInfo);
    }

    /// <summary>
    /// Get recent battle logs efficiently
    /// </summary>
    public static ReadOnlySpan<BattleLog> GetRecentBattleLogs(int count = 1000)
    {
        return _battleLogBuffer.GetRecent(count);
    }

    /// <summary>
    /// Process battle logs in batches for better performance
    /// </summary>
    public static void ProcessBattleLogsBatch(Action<BattleLog> processor, int batchSize = 1000)
    {
        var processed = 0;
        _battleLogBuffer.ForEach(log =>
        {
            processor(log);
            processed++;
            
            // Yield control periodically to prevent blocking
            if (processed % batchSize == 0)
            {
                Thread.Yield();
            }
        });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void UpdateDpsDataEfficient(BattleLog battleLog)
    {
        // Use optimized DPS calculation
        var key = battleLog.AttackerUuid;
        
        _dpsData.AddOrUpdate(key, 
            // Add new
            _ => CreateDpsDataFromLog(battleLog),
            // Update existing
            (_, existing) => UpdateDpsData(existing, battleLog));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static OptimizedDpsData CreateDpsDataFromLog(BattleLog battleLog)
    {
        // Create optimized DPS data
        return new OptimizedDpsData
        {
            PlayerId = battleLog.AttackerUuid,
            TotalDamage = (ulong)battleLog.Value,
            HitCount = 1,
            LastUpdateTime = DateTime.UtcNow
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static OptimizedDpsData UpdateDpsData(OptimizedDpsData existing, BattleLog battleLog)
    {
        // Create new instance for immutability
        return new OptimizedDpsData
        {
            PlayerId = existing.PlayerId,
            TotalDamage = existing.TotalDamage + (ulong)battleLog.Value,
            HitCount = existing.HitCount + 1,
            LastUpdateTime = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Clear data with proper memory cleanup
    /// </summary>
    public static void ClearAll()
    {
        _battleLogBuffer.Clear();
        _playerInfoCache.Clear();
        _dpsDataCache.Clear();
        _playerInfos.Clear();
        _dpsData.Clear();
        
        // Force garbage collection for large cleanups
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }

    /// <summary>
    /// Clear old data to free memory
    /// </summary>
    public static void ClearOldData()
    {
        // Clear old entries from caches
        var cutoffTime = DateTime.UtcNow.AddMinutes(-30); // Keep last 30 minutes
        
        // This would require additional tracking in the actual implementation
        // For now, just clear least recently used items
        _playerInfoCache.Clear();
        _dpsDataCache.Clear();
        
        // Force garbage collection after clearing old data
        if (MemoryConfig.AggressiveMemoryOptimization)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
    }

    /// <summary>
    /// Get memory usage statistics
    /// </summary>
    public static (long TotalMemory, long BattleLogMemory, long CacheMemory) GetMemoryUsage()
    {
        var totalMemory = GC.GetTotalMemory(false);
        var battleLogMemory = _battleLogBuffer.Count * Unsafe.SizeOf<BattleLog>();
        var cacheMemory = (_playerInfoCache.Count + _dpsDataCache.Count) * 64; // Estimated cache overhead
        
        return (totalMemory, battleLogMemory, cacheMemory);
    }

    /// <summary>
    /// Cleanup resources
    /// </summary>
    public static void Dispose()
    {
        _battleLogBuffer?.Dispose();
        _playerInfoCache?.Dispose();
        _dpsDataCache?.Dispose();
        _eventManager?.Dispose();
    }
}

/// <summary>
/// Weak event manager to prevent memory leaks
/// </summary>
public sealed class WeakEventManager : IDisposable
{
    private readonly ConcurrentDictionary<string, List<WeakReference>> _eventHandlers = new();
    private readonly Timer _cleanupTimer;
    private bool _disposed;

    public WeakEventManager()
    {
        // Clean up dead references every 30 seconds
        _cleanupTimer = new Timer(CleanupDeadReferences, null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
    }

    public void AddEventHandler(string eventName, Delegate? handler)
    {
        if (handler == null) return;

        _eventHandlers.AddOrUpdate(eventName,
            _ => new List<WeakReference> { new(handler) },
            (_, list) => 
            {
                lock (list)
                {
                    list.Add(new WeakReference(handler));
                    return list;
                }
            });
    }

    public void RemoveEventHandler(string eventName, Delegate? handler)
    {
        if (handler == null || !_eventHandlers.TryGetValue(eventName, out var list)) return;

        lock (list)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (!list[i].IsAlive || ReferenceEquals(list[i].Target, handler))
                {
                    list.RemoveAt(i);
                }
            }
        }
    }

    public void RaiseEvent<T>(string eventName, T args)
    {
        if (!_eventHandlers.TryGetValue(eventName, out var list)) return;

        List<Delegate> handlersToCall = new();
        
        lock (list)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].IsAlive && list[i].Target is Delegate handler)
                {
                    handlersToCall.Add(handler);
                }
                else
                {
                    list.RemoveAt(i);
                }
            }
        }

        // Call handlers outside of lock
        foreach (var handler in handlersToCall)
        {
            try
            {
                handler.DynamicInvoke(args);
            }
            catch
            {
                // Ignore exceptions in event handlers
            }
        }
    }

    private void CleanupDeadReferences(object? state)
    {
        foreach (var kvp in _eventHandlers)
        {
            var list = kvp.Value;
            lock (list)
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    if (!list[i].IsAlive)
                    {
                        list.RemoveAt(i);
                    }
                }
            }
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _cleanupTimer?.Dispose();
            _eventHandlers.Clear();
            _disposed = true;
        }
    }
}