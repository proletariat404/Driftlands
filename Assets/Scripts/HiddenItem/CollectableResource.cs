using UnityEngine;

public class CollectableResource : Interactable
{
	[Header("资源名称")]
	public string resourceName = "未知资源";

	private bool isCollected = false;

	protected override void OnInteract(GameObject player)
	{
		if (isCollected)
			return;

		// 弹出确认采集窗口
		UIConfirmManager.Instance.ShowConfirm(
			$"是否采集「{resourceName}」？",
			onConfirm: () =>
			{
				// 采集成功的逻辑
				Debug.Log($"✅ 玩家采集了：{resourceName}");

				UIHintManager.Instance?.ShowHint($"获得「{resourceName}」！");

				isCollected = true;

				// 采集后销毁该物体（可改成播放动画）
				Destroy(gameObject);
			},
			onCancel: () =>
			{
				// 玩家取消采集
				Debug.Log($"❌ 玩家取消采集：{resourceName}");
			});
	}
}
