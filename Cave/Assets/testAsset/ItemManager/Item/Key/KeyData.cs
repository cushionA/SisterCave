public class KeyData
{
    public KeyItem[] Contents;

    public KeyData(params KeyItem[] contents)
    {//これコンストラクタ付きの変数っすね
        this.Contents = contents;
    }
}