using Sandbox.Game.Gui;
using Sandbox;
using System;
using VRage;
using VRage.Audio;
using VRage.Input;
using VRage.Game;
using VRage.Utils;
using VRageMath;
using Sandbox.Graphics.GUI;

namespace ChatFilter
{
	public class CFConfig : MyGuiScreenBase
	{
		public CFConfig()
			: base(new Vector2?(new Vector2(0.5f, 0.5f)), new Vector4?(MyGuiConstants.SCREEN_BACKGROUND_COLOR * MySandboxGame.Config.UIBkOpacity), new Vector2?(new Vector2(0.3f, 0.4f)), true, null, 0f, 0f, null)
		{
			base.CanBeHidden = true;
			base.CanHideOthers = true;
			base.CloseButtonEnabled = true;
			this.RecreateControls(true);
		}

		public override string GetFriendlyName()
		{
			return "ChatFilterConfig";
		}

		public override void HandleInput(bool receivedFocusInThisUpdate)
		{
			base.HandleInput(receivedFocusInThisUpdate);
			if (receivedFocusInThisUpdate)
			{
				return;
			}
			if (MyControllerHelper.IsControl(MyControllerHelper.CX_GUI, MyControlsGUI.BUTTON_X, MyControlStateType.NEW_PRESSED, false, false))
			{
				this.OnOk(null);
			}
			if (MyControllerHelper.IsControl(MyControllerHelper.CX_GUI, MyControlsGUI.BUTTON_B, MyControlStateType.NEW_PRESSED, false, false))
			{
				this.OnCancel(null);
			}
		}

		private void OnCancel(MyGuiControlButton button)
		{
			this.CloseScreen(false);
		}

		private void OnOk(MyGuiControlButton button)
		{
			ChatFilter.Settings.HideFaction = this.m_HideFactionCheckbox.IsChecked;
			ChatFilter.Settings.HidePrivate = this.m_HidePrivateCheckbox.IsChecked;
			ChatFilter.Settings.HideServer = this.m_HideServerCheckbox.IsChecked;
			ChatFilter.Settings.HideGlobal = this.m_HideGlobalCheckbox.IsChecked;
			ChatFilter.SaveSettings();
			MyHud.Chat.ShowMessage("ChatFilter", "Settings saved", Color.SlateGray);
			this.ReturnOk();
		}

		public override void RecreateControls(bool constructor)
		{
			base.RecreateControls(constructor);
			Vector2 value = new Vector2(0f, this.m_size.Value.Y / 5f);
			Vector2 value2 = new Vector2(0.018f, 0f);
			Vector2 position = new Vector2(-0.05f, 0f);
			position -= value;
			base.AddCaption("Chat Filter Configuration", new Vector4?(Color.White.ToVector4()), null, 0.8f);
			position += value;
			this.m_HideGlobalCheckbox = new MyGuiControlCheckbox(new Vector2?(position), null, "Hide global chat", ChatFilter.Settings.HideGlobal, MyGuiControlCheckboxStyleEnum.Default, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
			this.m_HideGlobalLabel = new MyGuiControlLabel(new Vector2?(position + value2), null, "Hide global chat", null, 0.8f, "Blue", MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER, false, float.PositiveInfinity, false);
			this.Controls.Add(this.m_HideGlobalCheckbox);
			this.Controls.Add(this.m_HideGlobalLabel);
			position += value;
			this.m_HideFactionCheckbox = new MyGuiControlCheckbox(new Vector2?(position), null, "Hide faction chat", ChatFilter.Settings.HideFaction, MyGuiControlCheckboxStyleEnum.Default, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
			this.m_HideFactionLabel = new MyGuiControlLabel(new Vector2?(position + value2), null, "Hide faction chat", null, 0.8f, "Blue", MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER, false, float.PositiveInfinity, false);
			this.Controls.Add(this.m_HideFactionCheckbox);
			this.Controls.Add(this.m_HideFactionLabel);
			position += value;
			this.m_HidePrivateCheckbox = new MyGuiControlCheckbox(new Vector2?(position), null, "Hide private messages", ChatFilter.Settings.HidePrivate, MyGuiControlCheckboxStyleEnum.Default, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
			this.m_HidePrivateLabel = new MyGuiControlLabel(new Vector2?(position + value2), null, "Hide private messages", null, 0.8f, "Blue", MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER, false, float.PositiveInfinity, false);
			this.Controls.Add(this.m_HidePrivateCheckbox);
			this.Controls.Add(this.m_HidePrivateLabel);
			position += value;
			this.m_HideServerCheckbox = new MyGuiControlCheckbox(new Vector2?(position), null, "Hide messages from server", ChatFilter.Settings.HideServer, MyGuiControlCheckboxStyleEnum.Default, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
			this.m_HideServerLabel = new MyGuiControlLabel(new Vector2?(position + value2), null, "Hide server messages", null, 0.8f, "Blue", MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER, false, float.PositiveInfinity, false);
			this.Controls.Add(this.m_HideServerCheckbox);
			this.Controls.Add(this.m_HideServerLabel);
			position += value;
			this.m_okButton = new MyGuiControlButton(null, MyGuiControlButtonStyleEnum.Default, null, null, MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_CENTER, null, MyTexts.Get(MyCommonTexts.Ok), 0.8f, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, MyGuiControlHighlightType.WHEN_CURSOR_OVER, new Action<MyGuiControlButton>(this.OnOk), GuiSounds.MouseClick, 1f, null, false, false, false, null);
			this.m_cancelButton = new MyGuiControlButton(null, MyGuiControlButtonStyleEnum.Default, null, null, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER, null, MyTexts.Get(MyCommonTexts.Cancel), 0.8f, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, MyGuiControlHighlightType.WHEN_CURSOR_OVER, new Action<MyGuiControlButton>(this.OnCancel), GuiSounds.MouseClick, 1f, null, false, false, false, null);
			this.m_okButton.Position = position - value2;
			this.m_cancelButton.Position = position + value2;
			this.m_okButton.SetToolTip("Save changes and close window");
			this.m_cancelButton.SetToolTip("Discard changes and close window");
			this.Controls.Add(this.m_okButton);
			this.Controls.Add(this.m_cancelButton);
			MyGuiControlLabel control = new MyGuiControlLabel
			{
				Position = this.m_okButton.Position,
				Name = MyGuiScreenBase.GAMEPAD_HELP_LABEL_NAME,
				OriginAlign = MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_BOTTOM
			};
			this.Controls.Add(control);
		}

		private void ReturnOk()
		{
			this.CloseScreen(false);
		}

		public override bool Update(bool hasFocus)
		{
			this.m_okButton.Visible = !MyInput.Static.IsJoystickLastUsed;
			this.m_cancelButton.Visible = !MyInput.Static.IsJoystickLastUsed;
			return base.Update(hasFocus);
		}

		private MyGuiControlButton m_cancelButton;

		private MyGuiControlCheckbox m_HideFactionCheckbox;

		private MyGuiControlLabel m_HideFactionLabel;

		private MyGuiControlCheckbox m_HideGlobalCheckbox;

		private MyGuiControlLabel m_HideGlobalLabel;

		private MyGuiControlCheckbox m_HidePrivateCheckbox;

		private MyGuiControlLabel m_HidePrivateLabel;

		private MyGuiControlCheckbox m_HideServerCheckbox;

		private MyGuiControlLabel m_HideServerLabel;

		private MyGuiControlButton m_okButton;
	}
}
