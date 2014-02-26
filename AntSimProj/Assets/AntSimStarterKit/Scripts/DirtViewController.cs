using UnityEngine;
using System.Collections;

public class DirtViewController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnClick()
	{
		Debug.Log ("User tapped on : " + UICamera.currentTouch.pos);
		int[] coords = GetCoordsByTouchPos(UICamera.currentTouch.pos);
		Debug.Log ("Tapped [" + coords[0] + ", " + coords[1] + "]");
	}

	public int[] GetCoordsByTouchPos(Vector3 vec)
	{
		int[] ret = new int[2];
		int row, col;
		float posY = vec.y;
		//Debug.Log ("posY = " + posY);
		float firstRow = 492f;	// Specific to ant queen
		float diff = firstRow - posY;
		float ratio = (diff / AntSimulation.singleton.currentColony.tileSize);
		//Debug.Log ("ratio = " + ratio);
		int tile = (int)ratio;
		if(ratio > (float)(tile + 0.5001f))
		{
			tile++;
		}
		//Debug.Log ("GetRow() = " + tile);
		row = tile;

		float posX = vec.x;
		float firstCol = 3 * -AntSimulation.singleton.currentColony.tileSize;
		diff = posX - firstCol;
		ratio = (diff / AntSimulation.singleton.currentColony.tileSize);
		//Debug.Log ("Ratio: " + ratio);
		tile =  (int)ratio;
		if(ratio > (float)(tile + 0.5001f))
		{
			tile++;
		}
		//Debug.Log ("GetCol() = " + tile);
		col = tile;

		ret[0] = row;
		ret[1] = col;

		return ret;
	}
}
