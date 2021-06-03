public class CoreData
{
    public CoreItem[] Contents;

    public CoreData(params CoreItem[] contents)
    {//これコンストラクタ付きの変数っすね
        this.Contents = contents;
    }
}