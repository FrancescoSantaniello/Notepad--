using System.IO;

namespace Notepad__.Models;
public class Document
{
    private string? _percorso;
    private string? _contenuto;

    public string? Percorso
    {
        get => _percorso;
        set
        {
            _percorso = value;
        }
    }
    public string? Contenuto
    {
        get => _contenuto;
        set
        {
            _contenuto = value;
        }
    }
    public string? Nome
    {
        get => _percorso is null ? null : Path.GetFileName(_percorso);
    }
    public bool IsWrited()
    {
        if (_percorso is null)
            return false;

        return _contenuto != File.ReadAllText(_percorso);
    }
    public void Save()
    {
        if (_percorso is not null)
            File.WriteAllText(_percorso, _contenuto);
    }
    public Document(string path)
    {
        Percorso = path;
        Contenuto = File.ReadAllText(path);
    }
    public Document() { }
}
