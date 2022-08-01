using MoreMountains.InventoryEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine;

public class MyInventoryDisplay : InventoryDisplay
{

	/// <summary>
	/// 外側にあるボタン
	/// MenuReset()メソッドで入れ替える
	/// </summary>
	[HideInInspector]
	public Button _outerButton;

    protected override void SetupSlotNavigation()
    {
		Debug.Log("うんち");
		if (!EnableNavigation)
		{
			return;
		}

		for (int i = 0; i < SlotContainer.Count; i++)
		{
			if (SlotContainer[i] == null)
			{
				return;
			}

			//i番目のスロットのコンポーネントを取得
			Navigation navigation = SlotContainer[i].navigation;

			// 上がるときは上がる、下がるときは上がる
			if (i - NumberOfColumns >= 0)
			{
				 //真上のスロット
			     navigation.selectOnUp = SlotContainer[i - NumberOfColumns];

			}
			//一列目はインベントリの上部にあるスロットではないボタンに行く
			else if(i < NumberOfColumns - 1)
			{
                if (_outerButton != null)
                {
					navigation.selectOnUp = _outerButton;
				}
                else
                {
					navigation.selectOnUp = null;
                }
				
			}


			// 下りるときの行き先を決める

			//下にまだ行けるスロットがあるなら。
			//一番下の列じゃないなら
			if (i + NumberOfColumns < SlotContainer.Count)
			{
				navigation.selectOnDown = SlotContainer[i + NumberOfColumns];
			}
			//一番下の列なら設定していたボタンに飛ぶ
			else
			{
				Debug.Log("ちくわ");
				if (_outerButton != null)
				{
					navigation.selectOnUp = _outerButton;
				}
				else
				{
					navigation.selectOnDown = null;
				}
			}



			// 左に行くときの行き先を決める
			if ((i % NumberOfColumns != 0) && (i > 0))
			{
				navigation.selectOnLeft = SlotContainer[i - 1];
			}
			//スロットが左端で、もう左に行けないとき
			else
			{
				//最後の列じゃないなら、右端まで行がみっちり詰まってるので
                if (i + NumberOfColumns < SlotContainer.Count)
                {
					//その列の右端に飛ぶ
					navigation.selectOnLeft = SlotContainer[i + NumberOfColumns - 1];

				}
				//最後の列なら左端から最後のスロットに
                else
                {
					//最後のスロットを
					navigation.selectOnLeft = SlotContainer[SlotContainer.Count - 1];
				}
			}


			// we determine where to go when going right
			if (((i + 1) % NumberOfColumns != 0) && (i < SlotContainer.Count - 1))
			{
				navigation.selectOnRight = SlotContainer[i + 1];
			}
			//右端にあるスロットなら左端に行く
			else
			{
                if (i % NumberOfColumns == 0)
                {
					//仮に右端なら列数だけ引く
					navigation.selectOnRight = SlotContainer[i - (NumberOfColumns - 1)];
                }
				//仮に最後のスロットとかで列数より少ない位置にあるなら
                else
                {
					//その場所から数えて左端に
					navigation.selectOnRight = SlotContainer[i - (i % NumberOfColumns)];
				}
				
			}
			SlotContainer[i].navigation = navigation;
		}
	}


}
