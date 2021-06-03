public class EnemyData:Item
{
    public EnemyLibrary[] Contents;

    public EnemyData(params EnemyLibrary[] contents)
    {//これコンストラクタ付きの変数っすね
        this.Contents = contents;
    }
}