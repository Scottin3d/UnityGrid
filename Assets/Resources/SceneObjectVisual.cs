using UnityEngine;

public class SceneObjectVisual : MonoBehaviour {
    [SerializeField] private GameObject visual;
    public GameObject Visual => visual;
    public bool IsActive => visual.activeSelf;
    public void SetActive(bool b) {
        visual.SetActive(b);
    }

}
