using System.Diagnostics;
namespace Notepad__.Models;
public class Run : Process
{
    public string Key { get; init; }
    public Run(string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("Nome univoco del processo non valido non valida");
        Key = key;
    }
}
