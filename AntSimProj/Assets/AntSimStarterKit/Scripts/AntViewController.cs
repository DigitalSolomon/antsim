using UnityEngine;
using System.Collections;

public class AntViewController : MonoBehaviour {
	public AntSimulation.Ant model;
	public static Vector3 startPos = new Vector3(0, 120, 0);
	public static Vector3 startScale = new Vector3(1, 1, 1);
	public UISprite sprite;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static AntViewController Create(AntSimulation.Ant a)
	{
		// This loads a default Ant view prefab

		Object prefab = Resources.Load ("AntSimPrefabs/AntView");
		if(prefab == null)
		{
			Debug.LogError("Please move the AntSimPrefabs directory to the Assets/Resources directory for prefabs to work");
		}
		GameObject view = (GameObject) Instantiate (prefab);
		AntViewController viewController = view.GetComponent<AntViewController>();

		// Associates the core Ant model with this ViewController
		viewController.model = a;
		viewController.sprite = view.GetComponent<UISprite>();
		if(viewController.model.IsQueen())
		{
			// Replace the current sprite with the appropriate name
			viewController.sprite.spriteName = "QueenToWestFromEast";
		}
		viewController.sprite.MakePixelPerfect();
		view.transform.parent = AntSimulation.singleton.colonyView.transform;
		view.transform.localPosition = startPos;
		view.transform.localScale = startScale;

		a.viewController = viewController;
		return viewController ;
	}

	public void OnClick()
	{
		Debug.Log ("Tapped " + model.name);
		AntSimulation.singleton.lastSelectedAnt = model;
		AntSimulation.singleton.ShowButtons(model);


	}
}
