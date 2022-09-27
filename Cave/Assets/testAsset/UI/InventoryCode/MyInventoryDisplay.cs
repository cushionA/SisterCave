using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MoreMountains.InventoryEngine
{
	[SelectionBase]
	/// <summary>
	/// A component that handles the visual representation of an Inventory, allowing the user to interact with it
	/// </summary>
	public class MyInventoryDisplay : InventoryDisplay
	{




		public new MyItemClass ItemClass;

		protected new  MyInventory _targetInventory = null;

		/// <summary>
		/// Grabs the target inventory based on its name
		/// </summary>
		/// <value>The target inventory.</value>
		public new  MyInventory TargetInventory
		{
			get
			{
				if (TargetInventoryName == null)
				{
					return null;
				}
				if (_targetInventory == null)
				{
					MyInventory inventory = _inventoryHub.Find(TargetInventoryName).gameObject.MMGetComponentNoAlloc<MyInventory>();
					if ((inventory != null) && (inventory.PlayerID == PlayerID))
					{
						_targetInventory = inventory;
					}
				}
				return _targetInventory;
			}
		}


		/// <summary>
		/// 外側にあるボタン
		/// MenuReset()メソッドで入れ替える
		/// </summary>
		public Selectable _outerButton;
		/// <summary>
		/// スライダーの高さを操作
		/// 95ずつ高さをいじる
		/// </summary>
		public RectTransform sliderHeight;

		//	[SerializeReference]
		//	Transform _inventoryHub;

		/// <summary>
		/// Creates and sets up the inventory display (usually called via the inspector's dedicated button)
		/// </summary>
		public override void SetupInventoryDisplay()
		{
			if (TargetInventoryName == "")
			{
				Debug.LogError("The " + this.name + " Inventory Display doesn't have a TargetInventoryName set. You need to set one from its inspector, matching an Inventory's name.");
				return;
			}

			if (TargetInventory == null)
			{
				Debug.LogError("The " + this.name + " Inventory Display couldn't find a TargetInventory. You either need to create an inventory with a matching inventory name (" + TargetInventoryName + "), or set that TargetInventoryName to one that exists.");
				return;
			}

			// if we also have a sound player component, we set it up too
			if (this.gameObject.MMGetComponentNoAlloc<InventorySoundPlayer>() != null)
			{
				this.gameObject.MMGetComponentNoAlloc<InventorySoundPlayer>().SetupInventorySoundPlayer();
			}


			

			InitializeSprites();
			AddGridLayoutGroup();
			//SliderSet();

			DrawInventoryTitle();
			ResizeInventoryDisplay();

			DrawInventoryContent();
		}

		/// <summary>
		/// On Awake, initializes the various lists used to keep track of the content of the inventory
		/// </summary>
		protected override void Awake()
		{
			_contentLastUpdate = new List<ItemQuantity>();
			SlotContainer = new List<InventorySlot>();
			_comparison = new List<int>();
			if (!TargetInventory.Persistent)
			{
				RedrawInventoryDisplay();
			}
		}

		/// <summary>
		/// Redraws the inventory display's contents when needed (usually after a change in the target inventory)
		/// </summary>
		protected override void RedrawInventoryDisplay()
		{
			InitializeSprites();
			AddGridLayoutGroup();
			DrawInventoryContent();
			FillLastUpdateContent();
		}

		/// <summary>
		/// Initializes the sprites.
		/// </summary>
		protected override void InitializeSprites()
		{
			// we create a spriteState to specify our various button states
			_spriteState.disabledSprite = DisabledSlotImage;
			_spriteState.selectedSprite = HighlightedSlotImage;
			_spriteState.highlightedSprite = HighlightedSlotImage;
			_spriteState.pressedSprite = PressedSlotImage;
			DrawEmptySlots = TargetInventory.allowEmpty;
		}

		/// <summary>
		/// Adds and sets up the inventory title child object
		/// </summary>
		protected override void DrawInventoryTitle()
		{
			if (!DisplayTitle)
			{
				return;
			}
			if (GetComponentInChildren<InventoryDisplayTitle>() != null)
			{
				if (!Application.isPlaying)
				{
					foreach (InventoryDisplayTitle title in GetComponentsInChildren<InventoryDisplayTitle>())
					{
						DestroyImmediate(title.gameObject);
					}
				}
				else
				{
					foreach (InventoryDisplayTitle title in GetComponentsInChildren<InventoryDisplayTitle>())
					{
						Destroy(title.gameObject);
					}
				}
			}
			GameObject inventoryTitle = new GameObject();
			InventoryTitle = inventoryTitle.AddComponent<InventoryDisplayTitle>();
			inventoryTitle.name = "InventoryTitle";
			inventoryTitle.GetComponent<RectTransform>().SetParent(this.transform);
			inventoryTitle.GetComponent<RectTransform>().sizeDelta = GetComponent<RectTransform>().sizeDelta;
			inventoryTitle.GetComponent<RectTransform>().localPosition = TitleOffset;
			inventoryTitle.GetComponent<RectTransform>().localScale = Vector3.one;
			InventoryTitle.text = Title;
			InventoryTitle.color = TitleColor;
			InventoryTitle.font = TitleFont;
			InventoryTitle.fontSize = TitleFontSize;
			InventoryTitle.alignment = TitleAlignment;
			InventoryTitle.raycastTarget = false;
		}

		/// <summary>
		/// Adds a grid layout group if there ain't one already
		/// </summary>
		protected override void AddGridLayoutGroup()
		{
			if (GetComponentInChildren<InventoryDisplayGrid>() == null)
			{
				GameObject inventoryGrid = new GameObject("InventoryDisplayGrid");
				inventoryGrid.transform.parent = this.transform;
				inventoryGrid.transform.position = transform.position;
				inventoryGrid.transform.localScale = Vector3.one;
				inventoryGrid.AddComponent<InventoryDisplayGrid>();
				InventoryGrid = inventoryGrid.AddComponent<GridLayoutGroup>();
			}
			if (InventoryGrid == null)
			{
				InventoryGrid = GetComponentInChildren<GridLayoutGroup>();
			}
			InventoryGrid.padding.top = PaddingTop;
			InventoryGrid.padding.right = PaddingRight;
			InventoryGrid.padding.bottom = PaddingBottom;
			InventoryGrid.padding.left = PaddingLeft;
			InventoryGrid.cellSize = SlotSize;
			InventoryGrid.spacing = SlotMargin;
		}

		/// <summary>
		/// Resizes the inventory panel, taking into account the number of rows/columns, the padding and margin
		/// </summary>
		protected override void ResizeInventoryDisplay()
		{

			float newWidth = PaddingLeft + SlotSize.x * NumberOfColumns + SlotMargin.x * (NumberOfColumns - 1) + PaddingRight;
			float newHeight = PaddingTop + SlotSize.y * NumberOfRows + SlotMargin.y * (NumberOfRows - 1) + PaddingBottom;

			TargetInventory.ResizeArray(NumberOfRows * NumberOfColumns);

			Vector2 newSize = new Vector2(newWidth, newHeight);
			InventoryRectTransform.sizeDelta = newSize;
			InventoryGrid.GetComponent<RectTransform>().sizeDelta = newSize;
		}

		/// <summary>
		/// Draws the content of the inventory (slots and icons)
		/// </summary>
		protected override void DrawInventoryContent()
		{
			if (SlotContainer != null)
			{
				SlotContainer.Clear();
			}
			else
			{
				SlotContainer = new List<InventorySlot>();
			}
			// we initialize our sprites 
			if (EmptySlotImage == null)
			{
				InitializeSprites();
			}
			// we remove all existing slots
			foreach (InventorySlot slot in transform.GetComponentsInChildren<InventorySlot>())
			{
				if (!Application.isPlaying)
				{
					DestroyImmediate(slot.gameObject);
				}
				else
				{
					Destroy(slot.gameObject);
				}
			}
			// for each slot we create the slot and its content
			for (int i = 0; i < TargetInventory.Content.Length; i++)
			{
				DrawSlot(i);
			}

			if (Application.isPlaying)
			{
				Destroy(_slotPrefab.gameObject);
			}
			else
			{
				DestroyImmediate(_slotPrefab.gameObject);
			}

			if (EnableNavigation)
			{
				SetupSlotNavigation();
			}
		}

		/// <summary>
		/// If the content has changed, we draw our inventory panel again
		/// </summary>
		protected override void ContentHasChanged()
		{
			if (!(Application.isPlaying))
			{
				AddGridLayoutGroup();
				DrawInventoryContent();
#if UNITY_EDITOR
				EditorUtility.SetDirty(gameObject);
#endif
			}
			else
			{
				if (!DrawEmptySlots)
				{
					DrawInventoryContent();
				}
				UpdateInventoryContent();
			}
		}

		/// <summary>
		/// Fills the last content of the update.
		/// </summary>
		protected override void FillLastUpdateContent()
		{
			_contentLastUpdate.Clear();
			_comparison.Clear();
			for (int i = 0; i < TargetInventory.Content.Length; i++)
			{
				if (!InventoryItem.IsNull(TargetInventory.Content[i]))
				{
					_contentLastUpdate.Add(new ItemQuantity(TargetInventory.Content[i].ItemID, TargetInventory.Content[i].Quantity));
				}
				else
				{
					_contentLastUpdate.Add(new ItemQuantity(null, 0));
				}
			}
		}

		/// <summary>
		/// Draws the content of the inventory (slots and icons)
		/// </summary>
		protected override void UpdateInventoryContent()
		{
			if (_contentLastUpdate == null || _contentLastUpdate.Count == 0)
			{
				FillLastUpdateContent();
			}

			// we compare our current content with the one in storage to look for changes
			for (int i = 0; i < TargetInventory.Content.Length; i++)
			{
				if ((TargetInventory.Content[i] == null) && (_contentLastUpdate[i].ItemID != null))
				{
					_comparison.Add(i);
				}
				if ((TargetInventory.Content[i] != null) && (_contentLastUpdate[i].ItemID == null))
				{
					_comparison.Add(i);
				}
				if ((TargetInventory.Content[i] != null) && (_contentLastUpdate[i].ItemID != null))
				{
					if ((TargetInventory.Content[i].ItemID != _contentLastUpdate[i].ItemID) || (TargetInventory.Content[i].Quantity != _contentLastUpdate[i].Quantity))
					{
						_comparison.Add(i);
					}
				}
			}
			if (_comparison.Count > 0)
			{
				foreach (int comparison in _comparison)
				{
					UpdateSlot(comparison);
				}
			}
			FillLastUpdateContent();
		}

		/// <summary>
		/// Updates the slot's content and appearance
		/// </summary>
		/// <param name="i">The index.</param>
		protected override void UpdateSlot(int i)
		{

			if (SlotContainer.Count < i)
			{
				Debug.LogWarning("It looks like your inventory display wasn't properly initialized. If you're not triggering any Load events, you may want to mark your inventory as non persistent in its inspector. Otherwise, you may want to reset and empty saved inventories and try again.");
			}

			if (SlotContainer.Count <= i)
			{
				return;
			}

			if (SlotContainer[i] == null)
			{
				return;
			}
			// we update the slot's bg image
			if (!InventoryItem.IsNull(TargetInventory.Content[i]))
			{
				SlotContainer[i].TargetImage.sprite = FilledSlotImage;
			}
			else
			{
				SlotContainer[i].TargetImage.sprite = EmptySlotImage;
			}
			if (!InventoryItem.IsNull(TargetInventory.Content[i]))
			{
				// we redraw the icon
				SlotContainer[i].DrawIcon(TargetInventory.Content[i], i);
			}
			else
			{
				SlotContainer[i].DrawIcon(null, i);
			}
		}

		/// <summary>
		/// Creates the slot prefab to use in all slot creations
		/// </summary>
		protected override void InitializeSlotPrefab()
		{
			if (SlotPrefab != null)
			{
				_slotPrefab = Instantiate(SlotPrefab);
			}
			else
			{
				GameObject newSlot = new GameObject();
				newSlot.AddComponent<RectTransform>();

				newSlot.AddComponent<Image>();
				newSlot.MMGetComponentNoAlloc<Image>().raycastTarget = true;

				_slotPrefab = newSlot.AddComponent<InventorySlot>();
				_slotPrefab.transition = Selectable.Transition.SpriteSwap;

				Navigation explicitNavigation = new Navigation();
				explicitNavigation.mode = Navigation.Mode.Explicit;
				_slotPrefab.GetComponent<InventorySlot>().navigation = explicitNavigation;

				_slotPrefab.interactable = true;

				newSlot.AddComponent<CanvasGroup>();
				newSlot.MMGetComponentNoAlloc<CanvasGroup>().alpha = 1;
				newSlot.MMGetComponentNoAlloc<CanvasGroup>().interactable = true;
				newSlot.MMGetComponentNoAlloc<CanvasGroup>().blocksRaycasts = true;
				newSlot.MMGetComponentNoAlloc<CanvasGroup>().ignoreParentGroups = false;

				// we add the icon
				GameObject itemIcon = new GameObject("Slot Icon", typeof(RectTransform));
				itemIcon.transform.SetParent(newSlot.transform);
				UnityEngine.UI.Image itemIconImage = itemIcon.AddComponent<Image>();
				_slotPrefab.IconImage = itemIconImage;
				RectTransform itemRectTransform = itemIcon.GetComponent<RectTransform>();
				itemRectTransform.localPosition = Vector3.zero;
				itemRectTransform.localScale = Vector3.one;
				MMGUI.SetSize(itemRectTransform, IconSize);

				// we add the quantity placeholder
				GameObject textObject = new GameObject("Slot Quantity", typeof(RectTransform));
				textObject.transform.SetParent(itemIcon.transform);
				Text textComponent = textObject.AddComponent<Text>();
				_slotPrefab.QuantityText = textComponent;
				textComponent.font = QtyFont;
				textComponent.fontSize = QtyFontSize;
				textComponent.color = QtyColor;
				textComponent.alignment = QtyAlignment;
				RectTransform textObjectRectTransform = textObject.GetComponent<RectTransform>();
				textObjectRectTransform.localPosition = Vector3.zero;
				textObjectRectTransform.localScale = Vector3.one;
				MMGUI.SetSize(textObjectRectTransform, (SlotSize - Vector2.one * QtyPadding));

				_slotPrefab.name = "SlotPrefab";
			}
		}

		/// <summary>
		/// Draws the slot and its content (icon, quantity...).
		/// </summary>
		/// <param name="i">The index.</param>
		protected override void DrawSlot(int i)
		{
			if (!DrawEmptySlots)
			{
				if (InventoryItem.IsNull(TargetInventory.Content[i]))
				{
					return;
				}
			}

			if (_slotPrefab == null)
			{
				InitializeSlotPrefab();
			}

			InventorySlot theSlot = Instantiate(_slotPrefab);

			theSlot.transform.SetParent(InventoryGrid.transform);
			theSlot.TargetRectTransform.localScale = Vector3.one;
			theSlot.transform.position = transform.position;
			theSlot.name = "Slot " + i;

			// we add the background image
			if (!InventoryItem.IsNull(TargetInventory.Content[i]))
			{
				theSlot.TargetImage.sprite = FilledSlotImage;
			}
			else
			{
				theSlot.TargetImage.sprite = EmptySlotImage;
			}
			theSlot.TargetImage.type = SlotImageType;
			theSlot.spriteState = _spriteState;
			theSlot.MovedSprite = MovedSlotImage;
			theSlot.ParentInventoryDisplay = this;
			theSlot.Index = i;

			SlotContainer.Add(theSlot);

			theSlot.gameObject.SetActive(true);

			theSlot.DrawIcon(TargetInventory.Content[i], i);
		}


		protected override void SetupSlotNavigation()
		{

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

				//_windowSelecterとスロットを結びつける
				if (i == 0)
				{
					Navigation startPoint = _outerButton.navigation;
					startPoint.selectOnDown = SlotContainer[0];
					_outerButton.navigation = startPoint;
				}

				// 上がるときは上がる、下がるときは上がる
				if (i - NumberOfColumns >= 0)
				{
					//真上のスロット
					navigation.selectOnUp = SlotContainer[i - NumberOfColumns];

				}
				//一列目はインベントリの上部にあるスロットではないボタンに行く
				else if (i < NumberOfColumns - 1)
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

			//		if (_outerButton != null)
			//		{
			//			navigation.selectOnDown = _outerButton;
			//		}
			//		else
			//		{
						navigation.selectOnDown = null;
			//		}
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

		/// <summary>		
		/// Sets the focus on the first item of the inventory		
		/// </summary>		
		public override void Focus()
		{
			if (!EnableNavigation)
			{
				return;
			}

			if (SlotContainer.Count > 0)
			{
				SlotContainer[0].Select();
			}

			if (EventSystem.current.currentSelectedGameObject == null)
			{
				EventSystem.current.SetSelectedGameObject(transform.GetComponentInChildren<InventorySlot>().gameObject);
			}
		}

		/// <summary>
		/// Returns the currently selected inventory slot
		/// </summary>
		/// <returns>The selected inventory slot.</returns>
		public override InventorySlot CurrentlySelectedInventorySlot()
		{
			return _currentlySelectedSlot;
		}

		/// <summary>
		/// Sets the currently selected slot
		/// </summary>
		/// <param name="slot">Slot.</param>
		public override void SetCurrentlySelectedSlot(InventorySlot slot)
		{
			_currentlySelectedSlot = slot;
		}

		/// <summary>
		/// Goes to the previous (-1) or next (1) inventory, based on the int direction passed in parameter.
		/// </summary>
		/// <param name="direction">Direction.</param>
		public override InventoryDisplay GoToInventory(int direction)
		{
			if (direction == -1)
			{
				if (PreviousInventory == null)
				{
					return null;
				}
				PreviousInventory.Focus();
				return PreviousInventory;
			}
			else
			{
				if (NextInventory == null)
				{
					return null;
				}
				NextInventory.Focus();
				return NextInventory;
			}
		}

		/// <summary>
		/// Sets the return inventory display
		/// </summary>
		/// <param name="inventoryDisplay">Inventory display.</param>
		public override void SetReturnInventory(InventoryDisplay inventoryDisplay)
		{
			ReturnInventory = inventoryDisplay;
		}

		/// <summary>
		/// If possible, returns the focus to the current return inventory focus (after equipping an item, usually)
		/// </summary>
		public override void ReturnInventoryFocus()
		{
			if (ReturnInventory == null)
			{
				return;
			}
			else
			{
				InEquipSelection = false;
				ResetDisabledStates();
				ReturnInventory.Focus();
				ReturnInventory = null;
			}
		}

		/// <summary>
		/// Disables all the slots in the inventory display, except those from a certain class
		/// </summary>
		/// <param name="itemClass">Item class.</param>
		public override void DisableAllBut(ItemClasses itemClass)
		{
			for (int i = 0; i < SlotContainer.Count; i++)
			{
				if (InventoryItem.IsNull(TargetInventory.Content[i]))
				{
					continue;
				}
				if (TargetInventory.Content[i].ItemClass != itemClass)
				{
					SlotContainer[i].DisableSlot();
				}
			}
		}

		/// <summary>
		/// Enables back all slots (usually after having disabled some of them)
		/// </summary>
		public override void ResetDisabledStates()
		{
			for (int i = 0; i < SlotContainer.Count; i++)
			{
				SlotContainer[i].EnableSlot();
			}
		}

		/// <summary>
		/// MMInventoryEventを捕捉して処理する。
		/// </summary>
		/// <param name="inventoryEvent">Inventory event.</param>
		public override void OnMMEvent(MMInventoryEvent inventoryEvent)
		{
			// if this event doesn't concern our inventory display, we do nothing and exit
			if (inventoryEvent.TargetInventoryName != this.TargetInventoryName)
			{
				return;
			}

			if (inventoryEvent.PlayerID != this.PlayerID)
			{
				return;
			}

			switch (inventoryEvent.InventoryEventType)
			{
				case MMInventoryEventType.Select:
					SetCurrentlySelectedSlot(inventoryEvent.Slot);
					break;

				case MMInventoryEventType.Click:
					ReturnInventoryFocus();
					SetCurrentlySelectedSlot(inventoryEvent.Slot);
					break;

				case MMInventoryEventType.Move:
					this.ReturnInventoryFocus();
					UpdateSlot(inventoryEvent.Index);

					break;

				case MMInventoryEventType.ItemUsed:
					this.ReturnInventoryFocus();
					break;

				case MMInventoryEventType.EquipRequest:
					if (this.TargetInventory.InventoryType == MyInventory.InventoryTypes.REquipment || this.TargetInventory.InventoryType == MyInventory.InventoryTypes.LEquipment)
					{
						// if there's no target inventory set we do nothing and exit
						if (TargetChoiceInventory == null)
						{
							Debug.LogWarning("InventoryEngine Warning : " + this + " has no choice inventory associated to it.");
							return;
						}
						// we disable all the slots that don't match the right type
					//	TargetChoiceInventory.DisableAllBut(this.ItemClass);
						// we set the focus on the target inventory
						TargetChoiceInventory.Focus();
						TargetChoiceInventory.InEquipSelection = true;
						// we set the return focus inventory
						TargetChoiceInventory.SetReturnInventory(this);
					}

					break;

				case MMInventoryEventType.ItemEquipped:
					ReturnInventoryFocus();
					break;

				case MMInventoryEventType.Drop:
					this.ReturnInventoryFocus();
					break;

				case MMInventoryEventType.ItemUnEquipped:
					this.ReturnInventoryFocus();
					break;

				case MMInventoryEventType.InventoryOpens:
					Focus();
					InventoryDisplay.CurrentlyBeingMovedItemIndex = -1;
					IsOpen = true;
					EventSystem.current.sendNavigationEvents = true;
					break;

				case MMInventoryEventType.InventoryCloses:
					InventoryDisplay.CurrentlyBeingMovedItemIndex = -1;
					EventSystem.current.sendNavigationEvents = false;
					IsOpen = false;
					SetCurrentlySelectedSlot(inventoryEvent.Slot);
					break;

				case MMInventoryEventType.ContentChanged:
					ContentHasChanged();
					break;

				case MMInventoryEventType.Redraw:
					RedrawInventoryDisplay();
					break;

				case MMInventoryEventType.InventoryLoaded:
					RedrawInventoryDisplay();
					if (GetFocusOnStart)
					{
						Focus();
					}
					break;
			}
		}

		/// <summary>
		/// On Enable, we start listening for MMInventoryEvents
		/// </summary>
		protected override void OnEnable()
		{
			this.MMEventStartListening<MMInventoryEvent>();
		}

		/// <summary>
		/// On Disable, we stop listening for MMInventoryEvents
		/// </summary>
		protected override void OnDisable()
		{
			this.MMEventStopListening<MMInventoryEvent>();
		}





		public void SliderSet()
		{
			if (TargetInventory.Content.Length != 0)
			{
				int Rows = TargetInventory.Content.Length / 3;

				//余りが出るなら1列分足す
				if (TargetInventory.Content.Length % 3 != 0)
				{
					Rows++;
				}

				Vector2 Size = new Vector2(sliderHeight.sizeDelta.x, 95 * Rows);
				sliderHeight.sizeDelta = Size;

			}

		}



	}

}