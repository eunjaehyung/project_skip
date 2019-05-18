using Spine.Unity;
using UnityEngine;

public class SpineSample : MonoBehaviour
{
	[SerializeField]
	private GameObject _targetSpine = null;     // 対象のSpineObject
	[SerializeField]
	private string _animationName = "";         // 再生するアニメーション名

	// ボタンをクリック
	public void OnClick()
	{
		if (_targetSpine == null || _animationName == "")
		{
			return;
		}

		// 指定のアニメーションを再生
		SkeletonAnimation skeletonAnimation = _targetSpine.GetComponent<SkeletonAnimation>();
		if (skeletonAnimation != null)
		{
			Spine.AnimationState animationState = skeletonAnimation.state;
			animationState.SetAnimation(0, _animationName, true);
		}
	}
}
