using UnityEngine;

public class ShaderManager : MonoBehaviour
{
    public static ShaderManager instance;

    [Header("쉐이더 설정")]
    [SerializeField] private Material screenMaterial; // 도트/흑백 쉐이더가 적용된 머티리얼

    // 쉐이더 그래프의 프로퍼티 이름과 일치해야 합니다.
    private readonly string pixelProperty = "_PixelResolution";
    private readonly string colorProperty = "_ColorSwitch";

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        ResetShaderValue(); // 시작할 때 리셋
    }
    void Update()
    {
        // 테스트용: 1번 키를 누르면 화면이 즉시 깨끗해져야 함
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CleanseScreen();
        }
    }
    // 업그레이드 진행도(0~1)에 따라 쉐이더 강도 조절
    public void UpdateShaderByProgress(float progress)
    {
        if (screenMaterial == null) return;

        // 1. 도트 크기 조절 (진행될수록 해상도 증가: 150 -> 600)
        float currentRes = Mathf.Lerp(150f, 600f, progress);
        screenMaterial.SetFloat(pixelProperty, currentRes);

        // 2. 색상 복구 (진행될수록 흑백 0에서 원본 1로 가되, 최대 70%까지만 복구)
        // 강도가 1.0에서 시작해서 0.3이 된다는 것은, 색상이 0에서 0.7까지 채워진다는 뜻
        float currentColor = Mathf.Lerp(0f, 0.7f, progress);
        screenMaterial.SetFloat(colorProperty, currentColor);
    }

    // 보스 처치 시 완전히 원래대로 돌림 (정화)
    public void CleanseScreen()
    {
        if (screenMaterial == null) return;

        // 해상도 최대, 색상 완전 복구
        screenMaterial.SetFloat(pixelProperty, 2000f);
        screenMaterial.SetFloat(colorProperty, 1f);

    }
    private void OnDisable()
    {
        ResetShaderValue();
    }

    private void ResetShaderValue()
    {
        if (screenMaterial != null)
        {
            screenMaterial.SetFloat("_PixelResolution", 300f);
            screenMaterial.SetFloat("_ColorSwitch", 0f);
        }
    }
}