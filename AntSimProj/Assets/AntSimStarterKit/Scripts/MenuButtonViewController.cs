using UnityEngine;
using System.Collections;

public class MenuButtonViewController : MonoBehaviour {

	public string label;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnClick()
	{


		if(label.CompareTo("Cancel") == 0)
		{
			Debug.Log ("Cancel!");
			AntSimulation.singleton.HideButtons();
		}
		else if(label.CompareTo("DNA Profile") == 0)
		{
			Debug.Log ("DNA Profile!");
			AntSimulation.Ant a = AntSimulation.singleton.lastSelectedAnt;
			string profile = "";
			profile += System.Environment.NewLine + "Name: " + a.name;
			profile += System.Environment.NewLine + "Strength: " + a.dna.strength;
			profile += System.Environment.NewLine + "Intelligence: " + a.dna.intelligence;
			profile += System.Environment.NewLine + "Speed: " + a.dna.speed;
			profile += System.Environment.NewLine + "Attraction: " + a.dna.attraction;

			if(a.IsQueen())
			{
				profile += System.Environment.NewLine + System.Environment.NewLine + "Seed DNA: ";
				if(a.seedDna != null)
				{
					profile += System.Environment.NewLine + "Strength: " + a.seedDna.strength;
					profile += System.Environment.NewLine + "Intelligence: " + a.seedDna.intelligence;
					profile += System.Environment.NewLine + "Speed: " + a.seedDna.speed;
					profile += System.Environment.NewLine + "Attraction: " + a.seedDna.attraction;
					profile += System.Environment.NewLine + "Eggs Left: " + a.eggs;
				}
				else
				{
					profile += "none";
				}
			}
			AntSimulation.singleton.ShowModal("DNA Profile",profile);


		}
		else if(label.CompareTo("Modal") == 0)
		{
			Debug.Log ("Modal");
			AntSimulation.singleton.HideModal();
		}
		else if(label.CompareTo ("Nuptial Flight") == 0)
		{
			Debug.Log ("Nuptial Flight!");

			AntSimulation.singleton.ShowModal("Nuptial Flight","All deez hoes");
			StartCoroutine(AntSimulation.singleton.lastSelectedAnt.NuptialFlight());
		}
	}
}
