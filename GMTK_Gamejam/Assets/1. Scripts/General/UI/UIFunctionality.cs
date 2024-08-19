using UnityEngine;

namespace General.UI
{
	public class UIFunctionality : MonoBehaviour
	{
		public void QuitApplication()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.ExitPlaymode();
			return;
#endif
			Application.Quit();
		}
	}
}