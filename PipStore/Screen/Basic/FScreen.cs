namespace PipStore.Screen.Basic {
	/// <summary>
	///     Credit: Aki
	/// </summary>
    public class FScreen : KScreen
	{
		public const float SCREEN_SORT_KEY = 300f;

#pragma warning disable IDE0051 // Remove unused private members
		new bool ConsumeMouseScroll = true; // do not remove!!!!
#pragma warning restore IDE0051 // Remove unused private members
		private bool shown = false;
		public bool pause = true;



		protected override void OnPrefabInit()
		{

			activateOnSpawn = true;
			gameObject.SetActive(true);
		}
		public virtual void OnClickCancel()
		{
			Reset();
			Deactivate();
		}

		public virtual void Reset()
		{
		}

		public virtual void OnClickApply()
		{
		}

		#region generic kscreen behaviour
		protected override void OnCmpEnable()
		{
			base.OnCmpEnable();
			if (CameraController.Instance != null)
			{
				CameraController.Instance.DisableUserCameraControl = true;
			}
		}

		protected override void OnCmpDisable()
		{
			base.OnCmpDisable();
			if (CameraController.Instance != null)
			{
				CameraController.Instance.DisableUserCameraControl = false;
			}
			Trigger((int)GameHashes.Close, null);
		}

		public override bool IsModal()
		{
			return true;
		}

		public override float GetSortKey()
		{
			return SCREEN_SORT_KEY;
		}

		protected override void OnActivate()
		{
			OnShow(true);
		}

		protected override void OnDeactivate()
		{
			OnShow(false);
		}

		protected override void OnShow(bool show)
		{
			base.OnShow(show);
			if (pause && SpeedControlScreen.Instance != null)
			{
				if (show && !shown)
				{
					SpeedControlScreen.Instance.Pause(false);
				}
				else
				{
					if (!show && shown)
					{
						SpeedControlScreen.Instance.Unpause(false);
					}
				}
				shown = show;
			}
		}

		public override void OnKeyDown(KButtonEvent e)
		{
			if (e.TryConsume(Action.Escape))
			{
				OnClickCancel();
			}
			else
			{
				base.OnKeyDown(e);
			}
		}

		public override void OnKeyUp(KButtonEvent e)
		{
			if (!e.Consumed)
			{
				KScrollRect scroll_rect = GetComponentInChildren<KScrollRect>();
				if (scroll_rect != null)
				{
					scroll_rect.OnKeyUp(e);
				}
			}
			e.Consumed = true;
		}
		#endregion

	}
}