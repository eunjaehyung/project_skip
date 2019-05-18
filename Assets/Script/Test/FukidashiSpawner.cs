using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class FukidashiSpawner : MonoBehaviour
{

    [SerializeField]
    private GameObject _fukidashiPrefab = null;

    [SerializeField]
    private Canvas _canvas = null;

    void Start()
    {
        Vector2[] coordinates = new Vector2[] {
            new Vector2(0.0f, 0.0f),
            new Vector2(100.0f, 50.0f),
            new Vector2(200.0f, 100.0f),
            new Vector2(300.0f, 200.0f),
        };

        foreach (Vector2 coodinate in coordinates) {
            // ※ BoneFollowerGraphicの実装上､Instantiate時にCanvasが親であると指定する必要がある.
            GameObject fukidashiObj = Instantiate(_fukidashiPrefab, _canvas.transform);
            SkeletonGraphic skeletonGraphic = fukidashiObj.GetComponent<SkeletonGraphic>();
            fukidashiObj.GetComponent<RectTransform>().anchoredPosition = coodinate;
        }
    }
}
