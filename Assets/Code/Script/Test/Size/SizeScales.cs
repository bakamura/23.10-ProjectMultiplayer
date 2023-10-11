using UnityEngine;

[CreateAssetMenu(menuName = "GamePreset/SizeScale")]
public class SizeScales : ScriptableObject {

    [SerializeField] private float[] sizesF = new float[System.Enum.GetValues(typeof(Size.SizeClass)).Length];
    [HideInInspector] public Vector3[] sizes;

    private void OnValidate() {
        sizes = new Vector3[sizesF.Length];
        for(int i = 0; i < sizesF.Length; i++) {
            sizes[i] = Vector3.one * sizesF[i];
        }
    }

}
