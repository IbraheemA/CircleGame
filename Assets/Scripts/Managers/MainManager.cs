using UnityEngine;

public class MainManager : MonoBehaviour {
	
	public static MainManager Instance { get; private set;}
	
	[SerializeField]
	public GameObject playerObjectPrefab;

	public static GameManager GameManager { get; private set; }
	//public static EntityCatalog EntityCatalog { get; private set; }

	void Awake(){
		Instance = this;
		//EntityCatalog = new EntityCatalog();
		GameManager = new GameManager();
	}

	void Update(){
		CameraController.ExecuteCameraPositionChange();
		GameManager.Update();
	}

	void FixedUpdate(){
		GameManager.FixedUpdate();
	}
}
