using System.Diagnostics;
using StarResonanceDpsAnalysis.Core.Configuration;
using StarResonanceDpsAnalysis.Core.Data;

namespace StarResonanceDpsAnalysis.Core.Services;

/// <summary>
/// Simple logging interface to avoid external dependencies
/// </summary>
public interface ISimpleLogger
{
    void LogInformation(string message, params object[] args);
    void LogWarning(string message, params object[] args);
    void LogError(Exception ex, string message, params object[] args);
    void LogDebug(string message, params object[] args);
}

/// <summary>
/// Console-based logger implementation
/// </summary>
public class ConsoleLogger : ISimpleLogger
{
    public void LogInformation(string message, params object[] args)
    {
        Console.WriteLine($"[INFO] {string.Format(message, args)}");
    }

    public void LogWarning(string message, params object[] args)
    {
        Console.WriteLine($"[WARN] {string.Format(message, args)}");
    }

    public void LogError(Exception ex, string message, params object[] args)
    {
        Console.WriteLine($"[ERROR] {string.Format(message, args)}: {ex.Message}");
    }

    public void LogDebug(string message, params object[] args)
    {
        Console.WriteLine($"[DEBUG] {string.Format(message, args)}");
    }
}

/// <summary>
/// Memory monitoring and automatic optimization service
/// </summary>
public sealed class MemoryMonitorService : IDisposable
{
    private readonly ISimpleLogger _logger;
    private readonly Timer _monitorTimer;
    private readonly Timer _gcTimer;
    private readonly Process _currentProcess;
    private bool _disposed;
    
    private long _lastPrivateMemorySize;
    private long _lastWorkingSet;
    private DateTime _lastMemoryPressureCheck = DateTime.MinValue;

    public MemoryMonitorService(ISimpleLogger? logger = null)
    {
        _logger = logger ?? new ConsoleLogger();
        _currentProcess = Process.GetCurrentProcess();
        
        // Monitor memory every 10 seconds
        _monitorTimer = new Timer(MonitorMemory, null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));
        
        // Periodic GC hints based on configuration
        var gcInterval = TimeSpan.FromSeconds(MemoryConfig.GCHintFrequencySeconds);
        _gcTimer = new Timer(PerformGCHint, null, gcInterval, gcInterval);
        
        _logger.LogInformation("Memory monitoring service started");
    }

    private void MonitorMemory(object? state)
    {
        try
        {
            _currentProcess.Refresh();
            
            var privateMemory = _currentProcess.PrivateMemorySize64;
            var workingSet = _currentProcess.WorkingSet64;
            var managedMemory = GC.GetTotalMemory(false);
            
            var privateMemoryMB = privateMemory / (1024 * 1024);
            var workingSetMB = workingSet / (1024 * 1024);
            var managedMemoryMB = managedMemory / (1024 * 1024);

            if (MemoryConfig.EnableMemoryProfiling)
            {
                _logger.LogDebug("Memory Usage - Private: {PrivateMB}MB, Working Set: {WorkingSetMB}MB, Managed: {ManagedMB}MB",
                    privateMemoryMB, workingSetMB, managedMemoryMB);
            }

            // Check for memory pressure
            CheckMemoryPressure(privateMemoryMB, managedMemoryMB);
            
            // Update tracking variables
            _lastPrivateMemorySize = privateMemory;
            _lastWorkingSet = workingSet;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error monitoring memory usage");
        }
    }

    private void CheckMemoryPressure(long privateMemoryMB, long managedMemoryMB)
    {
        var now = DateTime.UtcNow;
        
        // Only check pressure every 30 seconds to avoid excessive checks
        if (now - _lastMemoryPressureCheck < TimeSpan.FromSeconds(30))
            return;
            
        _lastMemoryPressureCheck = now;

        var totalMemoryMB = privateMemoryMB + managedMemoryMB;
        
        if (totalMemoryMB > MemoryConfig.MaxMemoryUsageMB)
        {
            _logger.LogWarning("High memory usage detected: {TotalMemoryMB}MB (limit: {MaxMemoryMB}MB). Triggering cleanup.",
                totalMemoryMB, MemoryConfig.MaxMemoryUsageMB);
                
            TriggerMemoryCleanup();
        }
        
        // Check for rapid memory growth
        var memoryGrowthMB = (privateMemoryMB - (_lastPrivateMemorySize / (1024 * 1024)));
        if (memoryGrowthMB > 100) // More than 100MB growth
        {
            _logger.LogWarning("Rapid memory growth detected: +{GrowthMB}MB. Consider investigating memory leaks.",
                memoryGrowthMB);
        }
    }

    private void TriggerMemoryCleanup()
    {
        try
        {
            // Trigger cleanup in optimized data storage
            OptimizedDataStorage.ClearOldData();
            
            // Clear weak references
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            
            // Clear string intern pool if aggressive optimization is enabled
            if (MemoryConfig.AggressiveMemoryOptimization)
            {
                GC.Collect();
                GC.WaitForFullGCComplete();
            }
            
            _logger.LogInformation("Memory cleanup completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during memory cleanup");
        }
    }

    private void PerformGCHint(object? state)
    {
        try
        {
            if (MemoryConfig.AggressiveMemoryOptimization)
            {
                // Aggressive GC for maximum memory reclamation
                GC.Collect(2, GCCollectionMode.Optimized);
                GC.WaitForPendingFinalizers();
                GC.Collect(2, GCCollectionMode.Optimized);
            }
            else
            {
                // Gentle GC hint for background cleanup
                GC.Collect(0, GCCollectionMode.Optimized);
            }
            
            if (MemoryConfig.EnableMemoryProfiling)
            {
                var memoryAfterGC = GC.GetTotalMemory(false) / (1024 * 1024);
                _logger.LogDebug("GC hint completed. Managed memory: {ManagedMemoryMB}MB", memoryAfterGC);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing GC hint");
        }
    }

    /// <summary>
    /// Force immediate memory optimization
    /// </summary>
    public void OptimizeMemoryNow()
    {
        _logger.LogInformation("Performing immediate memory optimization");
        TriggerMemoryCleanup();
        PerformGCHint(null);
    }

    /// <summary>
    /// Get current memory usage report
    /// </summary>
    public MemoryUsageReport GetMemoryReport()
    {
        _currentProcess.Refresh();
        
        return new MemoryUsageReport
        {
            PrivateMemoryMB = _currentProcess.PrivateMemorySize64 / (1024 * 1024),
            WorkingSetMB = _currentProcess.WorkingSet64 / (1024 * 1024),
            ManagedMemoryMB = GC.GetTotalMemory(false) / (1024 * 1024),
            Gen0Collections = GC.CollectionCount(0),
            Gen1Collections = GC.CollectionCount(1),
            Gen2Collections = GC.CollectionCount(2),
            Timestamp = DateTime.UtcNow
        };
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _monitorTimer?.Dispose();
            _gcTimer?.Dispose();
            _currentProcess?.Dispose();
            _disposed = true;
            _logger.LogInformation("Memory monitoring service disposed");
        }
    }
}

/// <summary>
/// Memory usage report
/// </summary>
public struct MemoryUsageReport
{
    public long PrivateMemoryMB;
    public long WorkingSetMB;
    public long ManagedMemoryMB;
    public int Gen0Collections;
    public int Gen1Collections;
    public int Gen2Collections;
    public DateTime Timestamp;
    
    public long TotalMemoryMB => PrivateMemoryMB + ManagedMemoryMB;
}