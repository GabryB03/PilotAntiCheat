public class FileList
{
    public string[] List { get; private set; }

    public FileList(string[] list)
    {
        List = list;
    }
}