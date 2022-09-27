using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
public class AlterInputManager : InputManager
{
    //新規定義ボタン

    //  public MMInput.IMButton Button { get; protected set; }

    public MMInput.IMButton sAttackButton { get; protected set; }
    public MMInput.IMButton bAttackButton { get; protected set; }
    public MMInput.IMButton ArtsButton { get; protected set; }

    public MMInput.IMButton CombinationButton { get; protected set; }

    public MMInput.IMButton AvoidButton { get; protected set; }
    public MMInput.IMButton GuardButton { get; protected set; }

    public MMInput.IMButton WeaponChangeButton{ get; protected set; }

    public MMInput.IMButton MenuCallButton { get; protected set; }

    public MMInput.IMButton TipsButton { get; protected set; }

    public MMInput.IMButton SubmitButton { get; protected set; }
    public MMInput.IMButton CancelButton { get; protected set; }

    //追加軸

    protected string SiteHorizontal;
    protected string SiteVertical;

    protected string UIHorizontal;
    protected string UIVertical;

    protected Vector2 _siteMovement = Vector2.zero;


    protected Vector2 _UIMovement = Vector2.zero;


    /// <summary>
    /// ボタンを初期化します。ボタンを増やしたい場合は、必ずここで登録してください。
    /// </summary>
    protected override void InitializeButtons()
    {
        #region
        /*
        ButtonList = new List<MMInput.IMButton>();
        ButtonList.Add(JumpButton = new MMInput.IMButton(PlayerID, "Jump", JumpButtonDown, JumpButtonPressed, JumpButtonUp));
      //  ButtonList.Add(SwimButton = new MMInput.IMButton(PlayerID, "Swim", SwimButtonDown, SwimButtonPressed, SwimButtonUp));
        ButtonList.Add(GlideButton = new MMInput.IMButton(PlayerID, "Glide", GlideButtonDown, GlideButtonPressed, GlideButtonUp));
        ButtonList.Add(InteractButton = new MMInput.IMButton(PlayerID, "Interact", InteractButtonDown, InteractButtonPressed, InteractButtonUp));
      //  ButtonList.Add(JetpackButton = new MMInput.IMButton(PlayerID, "Jetpack", JetpackButtonDown, JetpackButtonPressed, JetpackButtonUp));
        ButtonList.Add(RunButton = new MMInput.IMButton(PlayerID, "Run", RunButtonDown, RunButtonPressed, RunButtonUp));
        ButtonList.Add(GripButton = new MMInput.IMButton(PlayerID, "Grip", GripButtonDown, GripButtonPressed, GripButtonUp));
        ButtonList.Add(DashButton = new MMInput.IMButton(PlayerID, "Dash", DashButtonDown, DashButtonPressed, DashButtonUp));
        ButtonList.Add(RollButton = new MMInput.IMButton(PlayerID, "Roll", RollButtonDown, RollButtonPressed, RollButtonUp));
        ButtonList.Add(FlyButton = new MMInput.IMButton(PlayerID, "Fly", FlyButtonDown, FlyButtonPressed, FlyButtonUp));
        ButtonList.Add(ShootButton = new MMInput.IMButton(PlayerID, "Shoot", ShootButtonDown, ShootButtonPressed, ShootButtonUp));
        ButtonList.Add(SecondaryShootButton = new MMInput.IMButton(PlayerID, "SecondaryShoot", SecondaryShootButtonDown, SecondaryShootButtonPressed, SecondaryShootButtonUp));
        ButtonList.Add(ReloadButton = new MMInput.IMButton(PlayerID, "Reload", ReloadButtonDown, ReloadButtonPressed, ReloadButtonUp));
       // ButtonList.Add(SwitchWeaponButton = new MMInput.IMButton(PlayerID, "SwitchWeapon", SwitchWeaponButtonDown, SwitchWeaponButtonPressed, SwitchWeaponButtonUp));
        ButtonList.Add(PauseButton = new MMInput.IMButton(PlayerID, "Pause", PauseButtonDown, PauseButtonPressed, PauseButtonUp));
        ButtonList.Add(CancelButton = new MMInput.IMButton(PlayerID, "Cancel", CancelButtonDown, CancelButtonPressed, CancelButtonUp));
        ButtonList.Add(SwitchCharacterButton = new MMInput.IMButton(PlayerID, "SwitchCharacter", SwitchCharacterButtonDown, SwitchCharacterButtonPressed, SwitchCharacterButtonUp));
        ButtonList.Add(SubmitButton = new MMInput.IMButton(PlayerID, "Submit", SubmitButtonDown, SubmitButtonPressed, SubmitButtonUp));
        ButtonList.Add(GrabButton = new MMInput.IMButton(PlayerID, "Grab", GrabButtonDown, GrabButtonPressed, GrabButtonUp));
        ButtonList.Add(ThrowButton = new MMInput.IMButton(PlayerID, "Throw", ThrowButtonDown, ThrowButtonPressed, ThrowButtonUp));
        ButtonList.Add(CombinationButton = new MMInput.IMButton(PlayerID, "Combination", CombinationButtonDown, CombinationButtonPressed, CombinationButtonUp));
        ButtonList.Add(sAttackButton = new MMInput.IMButton(PlayerID, "sAttack", sAttackButtonDown, sAttackButtonPressed, sAttackButtonUp));
        ButtonList.Add(bAttackButton = new MMInput.IMButton(PlayerID, "bAttack", bAttackButtonDown, bAttackButtonPressed, bAttackButtonUp));
        ButtonList.Add(ArtsButton = new MMInput.IMButton(PlayerID, "Arts", ArtsButtonDown, ArtsButtonPressed, ArtsButtonUp));
        ButtonList.Add(AvoidButton = new MMInput.IMButton(PlayerID, "Avoid", AvoidButtonDown, AvoidButtonPressed, AvoidButtonUp));
        ButtonList.Add(WeaponChangeButton = new MMInput.IMButton(PlayerID, "WeaponChange", WeaponChangeButtonDown, WeaponChangeButtonPressed, WeaponChangeButtonUp));
        ButtonList.Add(GuardButton = new MMInput.IMButton(PlayerID, "Guard", GuardButtonDown, GuardButtonPressed, GuardButtonUp));

        */
        #endregion
        //新規ボタンのためのテンプレ
        //ButtonList.Add(ThrowButton = new MMInput.IMButton(PlayerID, "Throw", ThrowButtonDown, ThrowButtonPressed, ThrowButtonUp));

        ButtonList = new List<MMInput.IMButton>();
        ButtonList.Add(JumpButton = new MMInput.IMButton(null, "Jump", JumpButtonDown, JumpButtonPressed, JumpButtonUp));
        //  ButtonList.Add(SwimButton = new MMInput.IMButton(null, "Swim", SwimButtonDown, SwimButtonPressed, SwimButtonUp));
    //    ButtonList.Add(GlideButton = new MMInput.IMButton(null, "Glide", GlideButtonDown, GlideButtonPressed, GlideButtonUp));
        ButtonList.Add(InteractButton = new MMInput.IMButton(null, "Interact", InteractButtonDown, InteractButtonPressed, InteractButtonUp));
        //  ButtonList.Add(JetpackButton = new MMInput.IMButton(null, "Jetpack", JetpackButtonDown, JetpackButtonPressed, JetpackButtonUp));
      //  ButtonList.Add(RunButton = new MMInput.IMButton(null, "Run", RunButtonDown, RunButtonPressed, RunButtonUp));
     //   ButtonList.Add(GripButton = new MMInput.IMButton(null, "Grip", GripButtonDown, GripButtonPressed, GripButtonUp));
    //    ButtonList.Add(DashButton = new MMInput.IMButton(null, "Dash", DashButtonDown, DashButtonPressed, DashButtonUp));
    //    ButtonList.Add(RollButton = new MMInput.IMButton(null, "Roll", RollButtonDown, RollButtonPressed, RollButtonUp));
    //    ButtonList.Add(FlyButton = new MMInput.IMButton(null, "Fly", FlyButtonDown, FlyButtonPressed, FlyButtonUp));
      //  ButtonList.Add(ShootButton = new MMInput.IMButton(null, "Shoot", ShootButtonDown, ShootButtonPressed, ShootButtonUp));
    //    ButtonList.Add(SecondaryShootButton = new MMInput.IMButton(null, "SecondaryShoot", SecondaryShootButtonDown, SecondaryShootButtonPressed, SecondaryShootButtonUp));
     //   ButtonList.Add(ReloadButton = new MMInput.IMButton(null, "Reload", ReloadButtonDown, ReloadButtonPressed, ReloadButtonUp));
        // ButtonList.Add(SwitchWeaponButton = new MMInput.IMButton(null, "SwitchWeapon", SwitchWeaponButtonDown, SwitchWeaponButtonPressed, SwitchWeaponButtonUp));
       // ButtonList.Add(PauseButton = new MMInput.IMButton(null, "Pause", PauseButtonDown, PauseButtonPressed, PauseButtonUp));
        ButtonList.Add(CancelButton = new MMInput.IMButton(null, "Cancel", CancelButtonDown, CancelButtonPressed, CancelButtonUp));
        ButtonList.Add(SubmitButton = new MMInput.IMButton(null, "Submit", SubmitButtonDown, SubmitButtonPressed, SubmitButtonUp));
        ButtonList.Add(TipsButton = new MMInput.IMButton(null, "TipsOn", TipsButtonDown, TipsButtonPressed, TipsButtonUp));
        ButtonList.Add(MenuCallButton = new MMInput.IMButton(null, "_Menu", MenuCallButtonDown, MenuCallButtonPressed, MenuCallButtonUp));
        ButtonList.Add(CombinationButton = new MMInput.IMButton(null, "Combination", CombinationButtonDown, CombinationButtonPressed, CombinationButtonUp));
        ButtonList.Add(sAttackButton = new MMInput.IMButton(null, "Fire1", sAttackButtonDown, sAttackButtonPressed, sAttackButtonUp));
        ButtonList.Add(bAttackButton = new MMInput.IMButton(null, "Fire2", bAttackButtonDown, bAttackButtonPressed, bAttackButtonUp));
        ButtonList.Add(ArtsButton = new MMInput.IMButton(null, "Arts", ArtsButtonDown, ArtsButtonPressed, ArtsButtonUp));
        ButtonList.Add(AvoidButton = new MMInput.IMButton(null, "Avoid", AvoidButtonDown, AvoidButtonPressed, AvoidButtonUp));
        ButtonList.Add(WeaponChangeButton = new MMInput.IMButton(null, "WeponHandChange", WeaponChangeButtonDown, WeaponChangeButtonPressed, WeaponChangeButtonUp));
        ButtonList.Add(GuardButton = new MMInput.IMButton(null, "Guard", GuardButtonDown, GuardButtonPressed, GuardButtonUp));

    }



    /*
     
    新規定義ボタンのためのメソッド
    ボタンの状態を変化させる
    throwを置き換え
    public virtual void ThrowButtonDown() { ThrowButton.State.ChangeState(MMInput.ButtonStates.ButtonDown); }
    public virtual void ThrowButtonPressed() { ThrowButton.State.ChangeState(MMInput.ButtonStates.ButtonPressed); }
    public virtual void ThrowButtonUp() { ThrowButton.State.ChangeState(MMInput.ButtonStates.ButtonUp); }
    */
    public virtual void GuardButtonDown() { GuardButton.State.ChangeState(MMInput.ButtonStates.ButtonDown); }
    public virtual void GuardButtonPressed() { GuardButton.State.ChangeState(MMInput.ButtonStates.ButtonPressed); }
    public virtual void GuardButtonUp() { GuardButton.State.ChangeState(MMInput.ButtonStates.ButtonUp); }
    public virtual void sAttackButtonDown() { sAttackButton.State.ChangeState(MMInput.ButtonStates.ButtonDown); }
    public virtual void sAttackButtonPressed() { sAttackButton.State.ChangeState(MMInput.ButtonStates.ButtonPressed); }
    public virtual void sAttackButtonUp() { sAttackButton.State.ChangeState(MMInput.ButtonStates.ButtonUp); }

    public virtual void bAttackButtonDown() { bAttackButton.State.ChangeState(MMInput.ButtonStates.ButtonDown); }
    public virtual void bAttackButtonPressed() { bAttackButton.State.ChangeState(MMInput.ButtonStates.ButtonPressed); }
    public virtual void bAttackButtonUp() { bAttackButton.State.ChangeState(MMInput.ButtonStates.ButtonUp); }

    public virtual void ArtsButtonDown() { ArtsButton.State.ChangeState(MMInput.ButtonStates.ButtonDown); }
    public virtual void ArtsButtonPressed() { ArtsButton.State.ChangeState(MMInput.ButtonStates.ButtonPressed); }
    public virtual void ArtsButtonUp() { ArtsButton.State.ChangeState(MMInput.ButtonStates.ButtonUp); }

    public virtual void CombinationButtonDown() { CombinationButton.State.ChangeState(MMInput.ButtonStates.ButtonDown); }
    public virtual void CombinationButtonPressed() { CombinationButton.State.ChangeState(MMInput.ButtonStates.ButtonPressed); }
    public virtual void CombinationButtonUp() { CombinationButton.State.ChangeState(MMInput.ButtonStates.ButtonUp); }

    public virtual void AvoidButtonDown() { AvoidButton.State.ChangeState(MMInput.ButtonStates.ButtonDown); }
    public virtual void AvoidButtonPressed() { AvoidButton.State.ChangeState(MMInput.ButtonStates.ButtonPressed); }
    public virtual void AvoidButtonUp() { AvoidButton.State.ChangeState(MMInput.ButtonStates.ButtonUp); }


    public virtual void WeaponChangeButtonDown() { WeaponChangeButton.State.ChangeState(MMInput.ButtonStates.ButtonDown); }
    public virtual void WeaponChangeButtonPressed() { WeaponChangeButton.State.ChangeState(MMInput.ButtonStates.ButtonPressed); }
    public virtual void WeaponChangeButtonUp() { WeaponChangeButton.State.ChangeState(MMInput.ButtonStates.ButtonUp); }


    public virtual void MenuCallButtonDown() { MenuCallButton.State.ChangeState(MMInput.ButtonStates.ButtonDown); }
    public virtual void MenuCallButtonPressed() { MenuCallButton.State.ChangeState(MMInput.ButtonStates.ButtonPressed); }
    public virtual void MenuCallButtonUp() { MenuCallButton.State.ChangeState(MMInput.ButtonStates.ButtonUp); }


    public virtual void TipsButtonDown() { TipsButton.State.ChangeState(MMInput.ButtonStates.ButtonDown); }
    public virtual void TipsButtonPressed() { TipsButton.State.ChangeState(MMInput.ButtonStates.ButtonPressed); }
    public virtual void TipsButtonUp() { TipsButton.State.ChangeState(MMInput.ButtonStates.ButtonUp); }

    public virtual void SubmitButtonDown() { SubmitButton.State.ChangeState(MMInput.ButtonStates.ButtonDown); }
    public virtual void SubmitButtonPressed() { SubmitButton.State.ChangeState(MMInput.ButtonStates.ButtonPressed); }
    public virtual void SubmitButtonUp() { SubmitButton.State.ChangeState(MMInput.ButtonStates.ButtonUp); }

    public virtual void CancelButtonDown() { CancelButton.State.ChangeState(MMInput.ButtonStates.ButtonDown); }
    public virtual void CancelButtonPressed() { CancelButton.State.ChangeState(MMInput.ButtonStates.ButtonPressed); }
    public virtual void CancelButtonUp() { CancelButton.State.ChangeState(MMInput.ButtonStates.ButtonUp); }

    //軸の入力改造

    /// <summary>
    /// Initializes the axis strings.
    /// </summary>
    protected override void InitializeAxis()
    {
        _axisHorizontal = PlayerID + "_Horizontal";
        _axisVertical = PlayerID + "_Vertical";

        SiteVertical = PlayerID + "SiteVertical";
        SiteHorizontal = PlayerID + "SiteHorizontal";

        UIVertical = PlayerID + "UIVertical";
        UIHorizontal = PlayerID + "UIHorizontal";
    }

    /// <summary>
    /// Called every frame, if not on mobile, gets primary movement values from input
    /// </summary>
    public virtual void SetSiteMovement()
    {
        if (!IsMobile && InputDetectionActive)
        {
            if (SmoothMovement)
            {
                _siteMovement.x = Input.GetAxis(SiteHorizontal);
                _siteMovement.y = Input.GetAxis(SiteVertical);
            }
            else
            {
                _siteMovement.x = Input.GetAxisRaw(SiteHorizontal);
                _siteMovement.y = Input.GetAxisRaw(SiteVertical);
            }
        }
    }

    /// <summary>
    /// Called every frame, if not on mobile, gets primary movement values from input
    /// </summary>
    public virtual void SetUIMovement()
    {
        if (!IsMobile && InputDetectionActive)
        {
            if (SmoothMovement)
            {
                _UIMovement.x = Input.GetAxis(UIHorizontal);
                _UIMovement.y = Input.GetAxis(UIVertical);
            }
            else
            {
                _UIMovement.x = Input.GetAxisRaw(UIHorizontal);
                _UIMovement.y = Input.GetAxisRaw(UIVertical);
            }
        }
    }

    /// <summary>
    /// At update, we check the various commands and update our values and states accordingly.
    /// </summary>
    protected override void Update()
    {
        if (!IsMobile && InputDetectionActive)
        {
            SetMovement();
          //  SetSecondaryMovement();
       //     SetShootAxis();
            GetInputButtons();

        }
    }






}
