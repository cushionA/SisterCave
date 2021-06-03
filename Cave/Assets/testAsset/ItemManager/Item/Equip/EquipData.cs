public class EquipData
{
    public Equip[] Contents;

    public EquipData(params Equip[] contents)
    {//これコンストラクタ付きの変数っすね
        this.Contents = contents;
    }
}