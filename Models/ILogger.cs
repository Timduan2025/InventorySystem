namespace InventorySystem.Models;

public interface ILogger
{
    void Log(string message); // 介面方法，沒有實作
    void LogError(string errorMessage);
}

public class ConsoleLogger : ILogger // 實作 ILogger 介面
{
    public void Log(string message) // 必須實作介面中的所有方法
    {
        Console.WriteLine($"[LOG]: {message}");
    }

    public void LogError(string errorMessage)
    {
        Console.Error.WriteLine($"[ERROR]: {errorMessage}"); // 錯誤輸出到標準錯誤流
    }
}
