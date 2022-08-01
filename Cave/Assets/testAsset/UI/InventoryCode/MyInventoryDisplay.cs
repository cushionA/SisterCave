using MoreMountains.InventoryEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine;

public class MyInventoryDisplay : InventoryDisplay
{

	/// <summary>
	/// �O���ɂ���{�^��
	/// MenuReset()���\�b�h�œ���ւ���
	/// </summary>
	[HideInInspector]
	public Button _outerButton;

    protected override void SetupSlotNavigation()
    {
		Debug.Log("����");
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

			//i�Ԗڂ̃X���b�g�̃R���|�[�l���g���擾
			Navigation navigation = SlotContainer[i].navigation;

			// �オ��Ƃ��͏オ��A������Ƃ��͏オ��
			if (i - NumberOfColumns >= 0)
			{
				 //�^��̃X���b�g
			     navigation.selectOnUp = SlotContainer[i - NumberOfColumns];

			}
			//���ڂ̓C���x���g���̏㕔�ɂ���X���b�g�ł͂Ȃ��{�^���ɍs��
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


			// �����Ƃ��̍s��������߂�

			//���ɂ܂��s����X���b�g������Ȃ�B
			//��ԉ��̗񂶂�Ȃ��Ȃ�
			if (i + NumberOfColumns < SlotContainer.Count)
			{
				navigation.selectOnDown = SlotContainer[i + NumberOfColumns];
			}
			//��ԉ��̗�Ȃ�ݒ肵�Ă����{�^���ɔ��
			else
			{
				Debug.Log("������");
				if (_outerButton != null)
				{
					navigation.selectOnUp = _outerButton;
				}
				else
				{
					navigation.selectOnDown = null;
				}
			}



			// ���ɍs���Ƃ��̍s��������߂�
			if ((i % NumberOfColumns != 0) && (i > 0))
			{
				navigation.selectOnLeft = SlotContainer[i - 1];
			}
			//�X���b�g�����[�ŁA�������ɍs���Ȃ��Ƃ�
			else
			{
				//�Ō�̗񂶂�Ȃ��Ȃ�A�E�[�܂ōs���݂�����l�܂��Ă�̂�
                if (i + NumberOfColumns < SlotContainer.Count)
                {
					//���̗�̉E�[�ɔ��
					navigation.selectOnLeft = SlotContainer[i + NumberOfColumns - 1];

				}
				//�Ō�̗�Ȃ獶�[����Ō�̃X���b�g��
                else
                {
					//�Ō�̃X���b�g��
					navigation.selectOnLeft = SlotContainer[SlotContainer.Count - 1];
				}
			}


			// we determine where to go when going right
			if (((i + 1) % NumberOfColumns != 0) && (i < SlotContainer.Count - 1))
			{
				navigation.selectOnRight = SlotContainer[i + 1];
			}
			//�E�[�ɂ���X���b�g�Ȃ獶�[�ɍs��
			else
			{
                if (i % NumberOfColumns == 0)
                {
					//���ɉE�[�Ȃ�񐔂�������
					navigation.selectOnRight = SlotContainer[i - (NumberOfColumns - 1)];
                }
				//���ɍŌ�̃X���b�g�Ƃ��ŗ񐔂�菭�Ȃ��ʒu�ɂ���Ȃ�
                else
                {
					//���̏ꏊ���琔���č��[��
					navigation.selectOnRight = SlotContainer[i - (i % NumberOfColumns)];
				}
				
			}
			SlotContainer[i].navigation = navigation;
		}
	}


}
