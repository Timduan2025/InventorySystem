using Org.BouncyCastle.Asn1.Cmp;

namespace InventorySystem.Models;

public class OperationResults<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }

    // 成功建構子
    public OperationResults(string message, T data)
    {
        Success = true;
        Message = message;
        Data = data;
    }

    // 失敗建構子
    public OperationResults(string errorMessage)
    {
        Success = false;
        Message = errorMessage;
        Data = default(T); // null
    }

    public static OperationResults<T> SuccessResult(string message, T data)
    {
        return new OperationResults<T>(message, data);
    }

    public static OperationResults<T> ErrorResult(string message)
    {
        return new OperationResults<T>(message);
    }
}