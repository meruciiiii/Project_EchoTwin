using UnityEngine;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour
{
    private RectTransform rectTransform;
    [SerializeField] private Canvas parentCanvas; // 크로스헤어가 속한 캔버스

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        
        // 1. 마우스 커서 숨기기
        Cursor.visible = false;
        
        // 2. 크로스헤어가 클릭을 방해하지 않도록 설정
        if (TryGetComponent(out Image img))
        {
            img.raycastTarget = false;
        }
    }

    private void Update()
    {
        // 캔버스 모드에 따라 처리
        if (parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            // Overlay 모드는 그냥 화면 좌표를 때려박으면 됩니다.
            rectTransform.position = Input.mousePosition;
        }
        else
        {
            // Camera 모드일 때는 화면 좌표를 로컬 좌표로 변환해야 정확합니다.
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvas.transform as RectTransform,
                Input.mousePosition,
                parentCanvas.worldCamera,
                out Vector2 localPoint
            );
            rectTransform.anchoredPosition = localPoint;
        }
    }
    
    // 게임 종료나 일시정지 시 커서를 다시 보여줘야 한다면 사용
    private void OnDisable()
    {
        Cursor.visible = true;
    }
}