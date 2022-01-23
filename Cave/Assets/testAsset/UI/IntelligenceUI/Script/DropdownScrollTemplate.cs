using UnityEngine;
using UnityEngine.UI;

public class DropdownScrollTemplate : MonoBehaviour
{
	private ScrollRect sr;

	Dropdown drop;
	float move;
	float ratio;
	int cValue = 0;
	bool putable;
	public void Awake()
	{
		sr = this.gameObject.GetComponent<ScrollRect>();
	}

	public void Start()
	{
		 drop = GetComponentInParent<Dropdown>();
		if (drop != null)
		{
			var viewport = this.transform.Find("Viewport").GetComponent<RectTransform>();
			var contentArea = this.transform.Find("Viewport/Content").GetComponent<RectTransform>();
			var contentItem = this.transform.Find("Viewport/Content/Item").GetComponent<RectTransform>();

			// Viewport�ɑ΂���Content�̃X�N���[���ʒu�����߂�
			var areaHeight = contentArea.rect.height - viewport.rect.height;
			var cellHeight = contentItem.rect.height;
			ratio = cellHeight / areaHeight;
			
			sr.verticalNormalizedPosition = 1.0f - Mathf.Clamp(drop.value * cellHeight / areaHeight, 0.0f, 1.0f);
			cValue = drop.value;
		}

	}


    private void Update()
    {

		move = GManager.instance.InputR.GetAxisRaw(MainUI.instance.rewiredAction15);

		float d = 0;
		if(move > 0)
        {
			cValue = (cValue + 1) > drop.options.Count ? drop.options.Count : cValue + 1;
			d = 1;
		}
		else if (move < 0)
        {
			cValue = cValue - 1 < 0 ? 0 : (cValue - 1);
			d = -1;
        }
		if(move == 0)
        {
			putable = false;
        }
		d = !putable ? d : 0;
		if (d!= 0 && !putable)
		{

			//sr.verticalNormalizedPosition = 1.0f + Mathf.Clamp(ratio * cValue, 0.0f, 1.0f);
		//	Debug.Log($"�O�̐��l{sr.verticalNormalizedPosition}");
			sr.verticalNormalizedPosition += d * Mathf.Clamp(ratio, 0.0f, 1.0f);
			/*	var viewport = this.transform.Find("Viewport").GetComponent<RectTransform>();
				var contentArea = this.transform.Find("Viewport/Content").GetComponent<RectTransform>();
				var contentItem = this.transform.Find("Viewport/Content/Item").GetComponent<RectTransform>();

				// Viewport�ɑ΂���Content�̃X�N���[���ʒu�����߂�
				var areaHeight = contentArea.rect.height - viewport.rect.height;
				var cellHeight = contentItem.rect.height;
			//cell�̍����������ɐ�߂�̂����߂�
				var scrollRatio = (cellHeight * move) / areaHeight;
				sr.verticalNormalizedPosition = Mathf.Clamp(sr.verticalNormalizedPosition - Mathf.Clamp(scrollRatio, 0.0f, 1.0f), 0.0f, 1.0f);
			//	Mathf.Clamp(sr.verticalNormalizedPosition, 0.0f, 1.0f);*/
			//��ԏオ�P
		//	Debug.Log($"�������l{Mathf.Clamp(ratio , 0.0f, 1.0f)}");
		//	Debug.Log($"�o�����l{sr.verticalNormalizedPosition}");
			putable = true;
		}
		
	}
}