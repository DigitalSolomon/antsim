using UnityEngine;
using System.Collections;

public class TileViewController : MonoBehaviour {
	public int model;
	public AntSimulation.Egg eggModel;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if(eggModel != null && eggModel.ovulationTime != 0)
		{
			if( (Time.time - eggModel.ovulationTime) > 20)
			{
				eggModel.Hatch();
			}
			if( (Time.time - eggModel.ovulationTime) > 10)
			{
				UISprite spr = gameObject.GetComponent<UISprite>();
				spr.spriteName = "Egg3";
				spr.MakePixelPerfect();
			}
			else if( (Time.time - eggModel.ovulationTime) > 5 )
			{
				UISprite spr = gameObject.GetComponent<UISprite>();
				spr.spriteName = "Egg2";
				spr.MakePixelPerfect();
			}

		}
	
	}

	public static TileViewController CreateEgg(AntSimulation.Egg egg, float posX, float posY)
	{
		// This loads a default Tile view prefab which we will then switch to display an Egg
		GameObject view = (GameObject) Instantiate (Resources.Load ("AntSimPrefabs/TileView"));
		TileViewController viewController = view.GetComponent<TileViewController>();
		viewController.eggModel = egg;
		UISprite spr = view.GetComponent<UISprite>();
		spr.spriteName = "Egg";
		spr.MakePixelPerfect();
		
		view.transform.parent = AntSimulation.singleton.tiles.transform;
		//view.transform.localScale = Vector3.one;
		view.transform.localPosition = new Vector3(posX, posY, 0);
		view.transform.localScale = Vector3.one;
		
		return viewController ;
	}

	public static TileViewController Create(int t, float posX, float posY)
	{
		//Make sure we're not trying to instantiate an invalid tile
		if(t == 0)
		{
			Debug.LogWarning("Bad tile type passed to TileViewController");
			return null;
		}

		// This loads a default Tile view prefab
		Object prefab = Resources.Load ("AntSimPrefabs/TileView");
		if(prefab == null)
		{
			Debug.LogError("Please move the AntSimPrefabs directory to the Assets/Resources directory for prefabs to work");
		}
		GameObject view = (GameObject) Instantiate (prefab);
		TileViewController viewController = view.GetComponent<TileViewController>();
		viewController.model = t;
		viewController.eggModel = null;
		UISprite spr = view.GetComponent<UISprite>();

		if(viewController.model == -2)
		{
			spr.spriteName = "SkyTile";
			spr.MakePixelPerfect();
		}
		else if(viewController.model == 1)
		{
			spr.spriteName = "ChunkTile";
			spr.MakePixelPerfect();
		}


		view.transform.parent = AntSimulation.singleton.tiles.transform;
		//view.transform.localScale = Vector3.one;
		view.transform.localPosition = new Vector3(posX, posY, 0);
		view.transform.localScale = Vector3.one;

		UIDragObject dragObj = (UIDragObject)view.GetComponent<UIDragObject>();
		dragObj.target = AntSimulation.singleton.colonyView.transform;

		return viewController ;
	}

	public void OnClick()
	{

	}
}
