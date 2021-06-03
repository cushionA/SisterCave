public class MaterialData
{
    public MaterialItem[] Contents;

    public MaterialData(params MaterialItem[] contents)
    {//これコンストラクタ付きの変数っすね
        this.Contents = contents;
    }
}