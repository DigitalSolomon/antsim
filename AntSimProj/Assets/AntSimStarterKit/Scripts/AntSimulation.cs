using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AntSimulation : MonoBehaviour {

	public static AntSimulation singleton;

	public Colony blackColony;
	public Ant blackQueen;

	public Colony redColony;
	public Ant redQueen;

	public SimulationState currentState;

	public GameObject colonyView;
	public GameObject buttonsView;
	public GameObject tiles;
	public float stepDuration = 0.75f;
	public UILabel nameLabel;
	public GameObject modalObject;
	public UILabel modalTitleLabel;
	public UILabel modalBodyLabel;
	public GameObject nuptialFlightOption;
	public Ant lastSelectedAnt;
	

	public enum SimulationState
	{
		StartMenu,
		ColonyView,
		SwarmView,
		PatchView
	}

	void Awake()
	{
		Initialize();
	}

	public void Initialize()
	{
		if(singleton == null)
		{
			singleton = this;
		}

		if(!PlayerPrefs.HasKey("hasRun"))
		{
			Debug.Log ("First Run");

			// Defines the core model for the black colony
			blackColony = new Colony(100, 100, 23);

			// Creates the initial Black Queen
			blackQueen = new Ant(1);				//option 1 =  Black Queen 
			Ant other = new Ant(2);
			// Associates the new Queen with the Black Colony
			blackColony.AddAnt(blackQueen);
			blackColony.AddAnt(other);

			// Instantiates visual representation of the Ant
			//AntViewController.Create(blackQueen);
			//AntViewController.Create (other);


			// Pull up the "inside the nest" perspective
			if(!LoadColonyView(blackColony))
			{
				Debug.LogError("Error loading ColonyView");
				return;
			}

			StartCoroutine(Simulate());

		}
	}


	private IEnumerator Simulate()
	{
		Debug.Log ("Ant Population: " + AntSimulation.Ant.count);
		//yield return new WaitForSeconds(1.0f);


		bool stepping = true;
		while (stepping )
		{

			// For each ant
			int i;
			for( i = 0; i < blackColony.ants.Count; i++)
			{
				Ant a = blackColony.ants[i];
				// See where the ant should walk to/what it should do during the duration of the next step
				// ---> This might add to the walk list
				// ---> This might lay an egg
				a.DetermineNextMove();
				UISprite spr = a.viewController.gameObject.GetComponent<UISprite>();
				if(a.walkList.Count > 0)
				{
					Vector3 node = a.walkList[0];
					bool isSpecialCase = false;
					a.walkList.RemoveAt (0);
					a.pastNode2 = a.pastNode1;
					a.pastNode1 = node;

					if(node.x == 1000 && node.y == 1000)
					{
						//Lay egg node
						a.LayEgg();
						isSpecialCase = true;
						continue;
					}
					else if(node.x == 2000 && node.y == 2000)
					{
						a.Dig (Ant.Direction.East);
						isSpecialCase = true;
						continue;
					}
					else if(node.x == 3000 && node.y == 3000)
					{
						a.Dig (Ant.Direction.South);
						isSpecialCase = true;
						continue;
					}
					else if(node.x == 4000 && node.y == 4000)
					{
						a.Dig (Ant.Direction.West);
						isSpecialCase = true;
						continue;
					}
					else if(node.x == 5000 && node.y == 5000)
					{
						//Climb Right
						StartCoroutine(a.ClimbRight());
						isSpecialCase = true;
						continue;

					}
					else if(node.x == 5100 && node.y == 5100)
					{
						//Climb Right 2
						StartCoroutine(a.ClimbRight2());
						isSpecialCase = true;
						continue;
						
					}
					else if(node.x == 5250 && node.y == 5250)
					{
						//Climb Down Left
						StartCoroutine(a.ClimbDownLeft());
						isSpecialCase = true;
						continue;
						
					}
					else if(node.x == 5500 && node.y == 5500)
					{
						//Slide Down left
						StartCoroutine(a.SlideDownLeft());
						isSpecialCase = true;
						continue;
						
					}
					else if(node.x == 5750 && node.y == 5750)
					{
						//Slide Up Right
						StartCoroutine(a.SlideUpRight());
						isSpecialCase = true;
						continue;
						
					}
					else if(node.x == 6000 && node.y == 6000)
					{
						//Slide Up Right
						StartCoroutine(a.CrawlUpLeft());
						isSpecialCase = true;
						continue;
						
					}
					else if(node.x == 9000 && node.y == 9000)
					{
						//Fall
						a.viewController.transform.localPositionTo(AntSimulation.singleton.stepDuration * 0.5f, new Vector3(0,-90,0), true);
						isSpecialCase = true;
						continue;
						
					}
					else if(node.x < 0)
					{
						if(!spr.spriteName.Contains("ToWestFromEast"))
						{
							if(a.IsQueen())
							{
								spr.spriteName = "QueenToWestFromEast";
							}
							else
							{
								spr.spriteName = "WorkerToWestFromEast";
							}
							spr.MakePixelPerfect();
						}
					}
					else if(node.x > 0)
					{
						if(!spr.spriteName.Contains("ToEastFromWest") && !spr.spriteName.Contains ("Flipped") )
						{
							if(a.IsQueen())
							{
								spr.spriteName = "QueenToEastFromWest";
							}
							else
							{
								spr.spriteName = "WorkerToEastFromWest";
							}
							spr.MakePixelPerfect();
						}
						else if(!spr.spriteName.Contains("ToEastFromWest") && spr.spriteName.Contains ("Flipped") )
						{
							if(a.IsQueen())
							{
								spr.spriteName = "QueenToEastFromWest_Flipped";
							}
							else
							{
								spr.spriteName = "WorkerToEastFromWest_Flipped";
							}
							spr.MakePixelPerfect();
						}
					}

					if(node.y > 0)
					{
						if(!spr.spriteName.Contains("ToNorthFromSouth"))
						{
							if(a.IsQueen())
							{
								spr.spriteName = "QueenToNorthFromSouth";
							}
							else
							{
								spr.spriteName = "WorkerToNorthFromSouth";
							}
							spr.MakePixelPerfect();
						}

					}
					else if(node.y < 0)
					{
						if(!spr.spriteName.Contains("ToSouthFromNorth"))
						{
							if(a.IsQueen())
							{
								spr.spriteName = "QueenToSouthFromNorth";
							}
							else
							{
								spr.spriteName = "WorkerToSouthFromNorth";
							}
							spr.MakePixelPerfect();
						}
					}
					else
					{
						a.viewController.gameObject.transform.localRotation = Quaternion.identity;
					}
					if(!isSpecialCase)
					{
						//Debug.Log ("Moving from " + a.viewController.transform.localPosition);
						a.viewController.gameObject.transform.localPositionTo(stepDuration, node, true);
					}
				}



			}

			yield return new WaitForSeconds(stepDuration * 1.05f);
			//Debug.Log ("Moved to " + a.viewController.transform.localPosition);


		}

	}

	public Colony currentColony;

	public bool LoadColonyView(Colony colonyModel)
	{
		// Hide the buttons
		buttonsView.SetActive(false);

		// Position to center
		colonyView.transform.localPosition = Vector3.zero;

		// Build Black Colony View
		string str = "";
		for(int k=0;k < colonyModel.groundLayer.GetLength(0);k++)
		{
			for(int l=0;l < colonyModel.groundLayer.GetLength(1);l++)
			{
				int val = colonyModel.groundLayer[k,l];
				if(val != 0)
				{
					colonyModel.tileLayer[k,l] = TileViewController.Create (val, l * 90, k * -90);
				}

			}
		}

		// Show it
		colonyView.SetActive(true);

		// Set the current state to ColonyView (the perspective of being inside the ant nest)
		currentState = SimulationState.ColonyView;

		// Load the AntViews
		for(int i = 0 ; i < colonyModel.ants.Count; i++)
		{
			Ant a = colonyModel.ants[i];
			AntViewController.Create ( a );
			a.SetRow (4);
			a.SetCol (3);
		}
		currentColony = colonyModel;
		return true;
	}

	public void ShowModal(string title, string body)
	{
		modalTitleLabel.text = title;
		modalBodyLabel.text = body;
		modalObject.SetActive(true);
	}

	public void HideModal()
	{
		modalObject.SetActive(false);
	}

	public void ShowButtons(Ant a)
	{
		GameObject obj = buttonsView;
		obj.transform.localPosition = new Vector3(0, -588, 0);

		if(!a.IsQueen())
		{
			nuptialFlightOption.SetActive(false);
		}
		else
		{
			nuptialFlightOption.SetActive(true);
		}

		obj.SetActive(true);
		nameLabel.text = a.name;
		modalObject.SetActive(false);
		GoEaseType lastType = Go.defaultEaseType;
		Go.defaultEaseType = GoEaseType.BounceOut;
		obj.transform.localPositionTo(1f, new Vector3(0, 588, 0), true);
		Go.defaultEaseType = lastType;

	}

	public void HideButtons()
	{
		GameObject obj = buttonsView;
		GoEaseType lastType = Go.defaultEaseType;
		Go.defaultEaseType = GoEaseType.BounceOut;
		obj.transform.localPositionTo(1f, new Vector3(0, -666, 0), true);
		Go.defaultEaseType = lastType;
	}

	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}


	#region Model Classes
	[System.Serializable]
	public class Colony
	{
		public float tileSize;				// This should be in sq. pixels, and in practice, should be 1 px less than actual sprite size. E. if file is 91 px png, then 90 for tileSize
		public string name;
		public List<Ant> ants;
		public int[,] groundLayer;
		public int[,] insectLayer;
		public int[,] foodLayer;
		public int[,] portalLayer;
		public int[,] eggLayer;
		public TileViewController[,] tileLayer;
		public int waterLevel;

		public Colony(int rows, int cols, int water)
		{
			Debug.Log("Colony::__constructor(rows,cols,water)");
			groundLayer = new int[rows,cols];
			eggLayer = new int[rows,cols];
			insectLayer = new int[rows,cols];
			foodLayer = new int[rows,cols];
			portalLayer = new int[rows,cols];
			waterLevel = water;
			ants = new List<Ant>();

			//Iinitial map
			tileSize = 90f;
			int offset = 3;
			int offset2 = 1;
			tileLayer = new TileViewController[rows,cols];

			//sky tiles
			groundLayer[0,0] = -2;
			groundLayer[0,1] = -2;
			groundLayer[0,2] = -2;
			groundLayer[0,3] = -2;
			groundLayer[0,4] = -2;
			groundLayer[0,5] = -2;
			groundLayer[0,6] = -2;
			groundLayer[0,7] = -2;
			groundLayer[0,8] = -2;
			groundLayer[0 + offset2,0 + offset] = 1;
			groundLayer[0 + offset2,1 + offset] = 1;
			groundLayer[0 + offset2,2 + offset] = 1;
			groundLayer[1 + offset2,1 + offset] = 1;
			groundLayer[2 + offset2,1 + offset] = 1;
			groundLayer[3 + offset2,1 + offset] = 1;
			groundLayer[3 + offset2,0 + offset] = 1;
			/*
			groundLayer[4 + offset2,1 + offset] = 1;
			groundLayer[5 + offset2,1 + offset] = 1;
			groundLayer[5 + offset2,1 + offset] = 2;
			groundLayer[5 + offset2,0 + offset] = 3;
			*/
		}

		public bool AddAnt(AntSimulation.Ant a)
		{
			ants.Add (a);
			a.colony = this;
			return true;
		}
	}

	[System.Serializable]
	public class DNA
	{
		public int strength;
		public int speed;
		public int intelligence;
		public int attraction;
		public Ant.Color color;

		public DNA Copy()
		{
			DNA dna = new DNA();
			dna.strength = strength;
			dna.speed = speed;
			dna.intelligence = intelligence;
			dna.attraction = attraction;
			dna.color = color;

			return dna;
		}

		public DNA Combine(DNA d)
		{
			DNA newDna = new DNA();
			newDna.strength = (strength + d.strength) / 2;
			newDna.speed = (speed + d.speed) / 2;
			newDna.intelligence = (intelligence + d.intelligence) / 2;
			newDna.attraction = (attraction + d.attraction) / 2;

			return newDna;
		}

		public string ToString2()
		{
			return "ST:" + strength + ";SP:" + speed + ";IN:" + intelligence + ";AT:" + attraction;
		}
	}

	[System.Serializable]
	public class Egg
	{
		public DNA dna;
		public float ovulationTime;
		public int row, col;
		public Colony colony;
		public TileViewController viewController;

		public Egg(DNA d, Colony aColony, int r, int c)
		{

			if( d == null)
			{
				Debug.LogError("Null DNA passed to Egg constructor");
			}

			if( r < 0 )
			{
				Debug.LogError ("negative row");
			}

			if (c < 0 )
			{
				Debug.LogError("negative col");
			}
			row = r;
			col = c;

			dna = d;

			// (optional) Apply genetic variance to DNA
			dna.attraction += UnityEngine.Random.Range (-10, 10);
			dna.intelligence += UnityEngine.Random.Range (-10, 10);
			dna.speed += UnityEngine.Random.Range (-10, 10);
			dna.strength += UnityEngine.Random.Range (-10, 10);

			ovulationTime = Time.time;
			colony = aColony;
			colony.eggLayer[row, col] = 1;
			viewController = TileViewController.CreateEgg(this, col * 90, row * -90);
		}


		public void Hatch()
		{
			if(dna == null)
			{
				Debug.LogError ("Null Dna");
				return;
			}
			Ant a = new Ant(dna);
			a.viewController = AntViewController.Create (a);
			colony.AddAnt(a);
			a.SetRow(row);
			a.SetCol(col);


			// Now get rid of the egg
			DestroyMe();
		}

		public void DestroyMe()
		{
			colony.eggLayer[row, col] = 0;
			DestroyObject (viewController.gameObject);
		}
	}

	[System.Serializable]
	public class Ant : Insect
	{
		#region AntEnums
		//Enums
		public enum Gender
		{
			Male,
			Female
		}

		public enum Stage
		{
			Wingless,
			Winged,
			ShedWings
		}

		public enum Color
		{
			Black,
			Red,
			Hybrid
		}

		// Used to represent which direction the Ant is facing
		public enum Direction
		{
			North,
			East,
			South,
			West
		}
		#endregion

		#region AntFields
		public int age;
		public Gender gender;
		public Stage stage;
		public static int count;
		public Colony colony;
		public bool nudgedUp;
		public int eggs;

		public DNA dna;
		public DNA seedDna;
		public Vector3 nextMove;
		public List<Vector3> walkList;
		public Vector3 pastNode1, pastNode2;
		public Direction direction = Direction.East;
		public AntViewController viewController;
		#endregion

		// Constructor
		public Ant(int option = 0)
		{
			Debug.Log("Ant::__constructor()");
			if(option == 0)
			{
				//Random Male
				List<string> maleNames = new List<string>();
				maleNames.Add ("Jerome");
				maleNames.Add ("David");
				maleNames.Add ("Noah");
				maleNames.Add ("Jason");
				maleNames.Add ("Justin");
				maleNames.Add ("Mark");
				maleNames.Add ("Steve");
				maleNames.Add ("George");
				maleNames.Add ("Griswald");
				maleNames.Add ("Furcas");
				maleNames.Add ("Leopold");
				maleNames.Add ("D'Brickashaw");
				maleNames.Add ("D'Squarius");
				maleNames.Add ("Raymond");
				maleNames.Add ("Dimetrius");
				maleNames.Add ("Quentin");
				maleNames.Add ("Julio");
				maleNames.Add ("Jesus");
				maleNames.Add ("Hay Zeus");
				maleNames.Add ("Wayne");
				maleNames.Add ("Marshall");
				maleNames.Add ("Balakay");

				name = maleNames[UnityEngine.Random.Range(0, maleNames.Count)];
				gender = Ant.Gender.Male;
				stage = Ant.Stage.Winged;		//Male & Winged = Drone
				dna = new DNA();
				dna.color = Ant.Color.Black;
				dna.speed = UnityEngine.Random.Range (70,100);
				dna.strength = UnityEngine.Random.Range (70,100);
				dna.intelligence = UnityEngine.Random.Range (70,100);
				dna.attraction = UnityEngine.Random.Range (70,100);
				seedDna = null;

			}
			else if(option == 1)
			{
				Debug.Log("Creating a new Black Queen, impregnated with seed DNA");
				List<string> queenNames = new List<string>();
				queenNames.Add ("Queen Mary");
				queenNames.Add ("Queen Esther");
				queenNames.Add ("Queen Elizabeth");
				queenNames.Add ("Queen Latifah");
				queenNames.Add ("Queen Astarte");
				queenNames.Add ("Queen Victoria");
				queenNames.Add ("Nefertiti");
				queenNames.Add ("Hilary");
				queenNames.Add ("Michelle");



				name = queenNames[UnityEngine.Random.Range(0, queenNames.Count)];
				gender = Ant.Gender.Female;
				stage = Ant.Stage.ShedWings;				// ShedWings && Female --> Queen
				dna = new DNA();
				dna.color = Ant.Color.Black;
				dna.speed = 60;
				dna.strength = 76;
				dna.intelligence = 88;
				dna.attraction = 92;
				seedDna = dna;
				eggs = 10;
			}
			else if(option == 2)
			{
				Debug.Log ("Creating a new Black Worker");
				List<string> workerNames = new List<string>();
				workerNames.Add ("Liza");
				workerNames.Add ("Jenny");
				workerNames.Add ("Salle");
				workerNames.Add ("Lucretia");
				workerNames.Add ("Jasmine");
				workerNames.Add ("Esther");
				workerNames.Add ("Jay Quay Lynn");
				
				
				name = workerNames[UnityEngine.Random.Range(0, workerNames.Count)];
				gender = Ant.Gender.Female;
				stage = Ant.Stage.Wingless;				// Wingless && Female --> Worker
				dna = new DNA();
				dna.color = Ant.Color.Black;
				dna.speed = 70;
				dna.strength = 86;
				dna.intelligence = 58;
				dna.attraction = 42;
				seedDna = dna;
			}
			type = Type.Ant;
			localId = count;
			walkList = new List<Vector3>();
			count++;
			hp = 100;
			Debug.Log(GetProfile());

		}


		public float lastDug;
		public void Dig(Direction dir)
		{
			int row = GetRow ();
			int col = GetCol ();

			if(dir == Direction.East)
			{
				int tileType = colony.groundLayer[row,col];

				TileViewController t = TileViewController.Create (1, (col + 1) * 90, row * -90);
				colony.tileLayer[row, col + 1] = t;
				colony.groundLayer[row, col + 1] = 1;

				if(viewController.sprite.spriteName.Contains("ToNorth"))
				{
					viewController.sprite.spriteName = "WorkerToEastFromWest";
					if(IsQueen())
					{
						viewController.sprite.spriteName = "QueenToEastFromWest";
					}
					viewController.sprite.MakePixelPerfect();
					SetCol(col + 1);
				}
				else if(viewController.sprite.spriteName.Contains("ToSouth"))
				{
					viewController.sprite.spriteName = "WorkerToEastFromWest";
					if(IsQueen())
					{
						viewController.sprite.spriteName = "QueenToEastFromWest";
					}
					viewController.sprite.MakePixelPerfect();
					SetCol(col + 1);
				}
			}
			else if(dir == Direction.West)
			{
				int tileType = colony.groundLayer[row,col];
				
				TileViewController t = TileViewController.Create (1, (col - 1) * 90, row * -90);
				colony.tileLayer[row, col - 1] = t;
				colony.groundLayer[row, col - 1] = 1;
				if(!viewController.sprite.spriteName.Contains ("ToWest"))
				{
					viewController.sprite.spriteName = "WorkerToWestFromEast";
					viewController.sprite.MakePixelPerfect();
					SetCol (col - 1);
				}
			}
			else if(dir == Direction.South)
			{
				int tileType = colony.groundLayer[row,col];
				
				TileViewController t = TileViewController.Create (1, (col) * 90, (row + 1) * -90);
				colony.tileLayer[row + 1, col] = t;
				colony.groundLayer[row + 1, col] = 1;

				if(viewController.sprite.spriteName.Contains ("ToEastFromWest"))
				{
					viewController.sprite.spriteName = "WorkerToSouthFromNorth";
					if(IsQueen())
					{
						viewController.sprite.spriteName = "QueenToSouthFromNorth";
					}
					SetRow (row + 1);
				}
				else if(viewController.sprite.spriteName.Contains ("ToWestFromEast"))
				{
					viewController.sprite.spriteName = "WorkerToSouthFromNorth";

					SetRow (row + 1);
				}
			}
			lastDug = Time.time;
		}

		public IEnumerator ClimbRight()
		{
			int oldRow = GetRow ();
			int oldCol = GetCol ();
			viewController.transform.localPositionTo(AntSimulation.singleton.stepDuration * 0.8f, new Vector3(0,90,0), true);
			yield return new WaitForSeconds(0.9f * AntSimulation.singleton.stepDuration);
			SetRow (oldRow - 1);
			SetCol (oldCol + 1);
			viewController.sprite.spriteName = "WorkerToEastFromWest";
			if(IsQueen())
			{
				viewController.sprite.spriteName = "QueenToEastFromWest";
			}
			viewController.sprite.MakePixelPerfect();
		}

		public IEnumerator ClimbRight2()
		{
			int oldRow = GetRow ();
			int oldCol = GetCol ();
			viewController.transform.localPositionTo(AntSimulation.singleton.stepDuration * 0.8f, new Vector3(-45, 0,0), true);
			yield return new WaitForSeconds(0.9f * AntSimulation.singleton.stepDuration);
			SetRow (oldRow - 1);
			SetCol (oldCol - 1);
			viewController.sprite.spriteName = "WorkerToNorthFromSouth";
			if(IsQueen())
			{
				viewController.sprite.spriteName = "QueenToNorthFromSouth";
			}
			viewController.sprite.MakePixelPerfect();
		}

		public IEnumerator ClimbDownLeft()
		{
			int oldRow = GetRow ();
			int oldCol = GetCol ();
			viewController.transform.localPositionTo(AntSimulation.singleton.stepDuration * 0.8f, new Vector3(-90,0,0), true);
			yield return new WaitForSeconds(0.9f * AntSimulation.singleton.stepDuration);
			SetRow (oldRow + 1);
			SetCol (oldCol - 1);
			viewController.sprite.spriteName = "WorkerToSouthFromNorth";
			if(IsQueen())
			{
				viewController.sprite.spriteName = "QueenToSouthFromNorth";
			}
			viewController.sprite.MakePixelPerfect();
		}

		public IEnumerator SlideDownLeft()
		{
			Vector3 oldPos = viewController.transform.localPosition;

			viewController.transform.localPositionTo(AntSimulation.singleton.stepDuration * 0.8f, new Vector3(0,-45,0), true);
			yield return new WaitForSeconds(0.9f * AntSimulation.singleton.stepDuration);

			viewController.transform.localPosition = oldPos;
			viewController.sprite.spriteName = "WorkerToWestFromEast";
			if(IsQueen())
			{
				viewController.sprite.spriteName = "QueenToWestFromEast";
			}
			viewController.sprite.MakePixelPerfect();
		}

		public IEnumerator SlideUpRight()
		{
			Vector3 oldPos = viewController.transform.localPosition;
			
			viewController.transform.localPositionTo(AntSimulation.singleton.stepDuration * 0.8f, new Vector3(45, 0,0), true);
			yield return new WaitForSeconds(0.9f * AntSimulation.singleton.stepDuration);
			
			viewController.transform.localPosition = oldPos;
			viewController.sprite.spriteName = "WorkerToNorthFromSouth";
			if(IsQueen())
			{
				viewController.sprite.spriteName = "QueenToNorthFromSouth";
			}
			viewController.sprite.MakePixelPerfect();
		}

		public IEnumerator CrawlUpLeft()
		{
			Vector3 oldPos = viewController.transform.localPosition;
			
			viewController.transform.localPositionTo(AntSimulation.singleton.stepDuration * 0.8f, new Vector3(0, 45,0), true);
			yield return new WaitForSeconds(0.9f * AntSimulation.singleton.stepDuration);
			
			viewController.transform.localPosition = oldPos;
			viewController.sprite.spriteName = "WorkerToWestFromEast_Flipped";
			if(IsQueen())
			{
				viewController.sprite.spriteName = "QueenToWestFromEast_Flipped";
			}
			viewController.sprite.MakePixelPerfect();
			Debug.Log ("Crawled up left");
		}


		public bool LayEgg()
		{
//			Debug.Log ("Ant::LayEgg()");

			if(gender != Gender.Female || stage != Stage.ShedWings)
			{
				return false;
			}

			int r = GetRow ();
			int c = GetCol ();

			if(seedDna == null)
			{
				Debug.Log ("No seed DNA");
				return false;
			}

			if(colony.eggLayer == null)
			{
				Debug.LogError("Null egg layer");
				return false;
			}

			if(colony.eggLayer[r, c] != 0)
			{
				//Debug.Log ("Egg already present");
				return false;
			}

			if( (Time.time - lastLaidEgg) < 3)//15 )
			{
				//Debug.Log ("Need to wait more before can lay an egg");
				return false;
			}


			lastLaidEgg = Time.time;


			// Use a *copy* of the DNA to make the egg
			DNA copy = seedDna.Copy();
			new Egg(copy, colony, r, c);

			eggs--;
			return true;
		}

		public Ant(DNA embryo)
		{
			dna = embryo;
			gender = Gender.Female;
			List<string> antNames = new List<string>();
			antNames.Add ("Lisa");
			antNames.Add ("Diana");
			antNames.Add ("Mary");
			antNames.Add ("Stephanie");
			antNames.Add ("Meg");
			antNames.Add ("Synter");
			antNames.Add ("Michelle");
			name = antNames[UnityEngine.Random.Range(0, antNames.Count)];
			type = Type.Ant;
			localId = count;
			walkList = new List<Vector3>();
			count++;

			Debug.Log ("Created Ant '" + name + "'");
		}

		public bool IsQueen()
		{
			return ( (gender == Gender.Female) && stage == Stage.ShedWings );
		}

		/*
		 * Converts from the Vector3 local Y position of the AntView to
		 * the row index of the 2D colony tilemap
		 */
		public int lastRow;
		public int GetRow()
		{
			float posY = viewController.transform.localPosition.y;
			//Debug.Log ("posY = " + posY);
			float firstRow = 492f;	// Specific to ant queen
			float diff = firstRow - posY;
			float ratio = (diff / colony.tileSize);
			//Debug.Log ("ratio = " + ratio);
			int tile = (int)ratio;
			if(ratio > (float)(tile + 0.5001f))
			{
				tile++;
			}
			lastRow = tile;
			//Debug.Log ("GetRow() = " + tile);
			return tile;
		}


		public void SetRow(int r)
		{
			viewController.transform.localPosition = new Vector3(viewController.transform.localPosition.x , 522 + (r * -colony.tileSize), viewController.transform.localPosition.z);
		}


		/*
		 * Converts from the Vector3 local X position of the AntView to
		 * the col index of the 2D colony tilemap
		 */
		public int lastCol;

		public int GetCol()
		{
			float posX = viewController.transform.localPosition.x;
			float firstCol = 3 * -colony.tileSize;
			float diff = posX - firstCol;
			float ratio = (diff / colony.tileSize);
			//Debug.Log ("Ratio: " + ratio);
			int tile =  (int)ratio;
			if(ratio > (float)(tile + 0.5001f))
			{
				tile++;
			}

			lastCol = tile;
			//Debug.Log ("GetCol() = " + tile);
			return tile;
		}

		public void SetCol(int c)
		{
			viewController.transform.localPosition = new Vector3(-270 + (c * colony.tileSize) , viewController.transform.localPosition.y, viewController.transform.localPosition.z);
		}

		public int GetCurrentTile()
		{
			int row = GetRow ();
			int col = GetCol();
			//Debug.Log ("Current [Row,Col] = " + row + ", " + col);

			int tile = colony.groundLayer[row, col];
			return tile;
		}
		private float lastLaidEgg;
		private bool IsMoveConsistent(Vector2 move)
		{
			// First check to see if it's a special-cased move
			if(move.x == 1000 && move.y == 1000)
			{
				if(gender == Ant.Gender.Female && stage == Ant.Stage.ShedWings && eggs > 0)
				{
					return true;
				}

				return false;
			}
			else if(move.x == 2000 && move.y == 2000)
			{
				//It's a Dig Right operation
				if( (Time.time - lastDug) < 3)
				{
					// can't dig back to back
					return false;
				}
				if(stage == Ant.Stage.ShedWings)
				{
					// queens/drones unsuitable for digging
					return false;
				}
				if(viewController.sprite.spriteName.Contains ("ToWest"))
				{
					//cant dig right while facing left
					return false;
				}
				if( (viewController.sprite.spriteName.Contains ("ToEast") || viewController.sprite.spriteName.Contains("ToWest")) && colony.groundLayer[GetRow() + 1, GetCol()] == 1)
				{
					// no digging while freefalling
					return false;
				}

				if ( (viewController.sprite.spriteName.Contains ("ToNorth") || viewController.sprite.spriteName.Contains ("ToSouth")) && colony.groundLayer[GetRow() + 1, GetCol() + 1] == 1)
				{
					// dont make things fall
					return false;
				}

				if( (viewController.sprite.spriteName.Contains ("ToNorth") || viewController.sprite.spriteName.Contains ("ToSouth")) && colony.groundLayer[lastRow - 1, lastCol] == 1 && colony.groundLayer[lastRow + 1, lastCol] == 1)
				{
					///return false;
				}

				if(colony.groundLayer[GetRow (), GetCol () + 1] == 0 )
				{
					Debug.Log ("(" + name + " [" + lastRow + ", " + lastCol + "])Digging east");
					return true;
				}

				return false;

			}
			else if(move.x == 3000 && move.y == 3000)
			{
				//It's a Dig South operation
				if( (Time.time - lastDug) < 3)
				{
					// can't dig back to back
					return false;
				}
				if(stage == Ant.Stage.ShedWings)
				{
					// queens/drones unsuitable for digging
					return false;
				}
				if(viewController.sprite.spriteName.Contains ("ToNorth"))
				{
					//can't dig south while facing north
					return false;
				}
				if(viewController.sprite.spriteName.Contains ("Flipped"))
				{
					//don't dig south while climbing on ceiling
					return false;
				}
				if(colony.groundLayer[GetRow () +1, GetCol () ] == 0)
				{
					Debug.Log ("(" + name + " [" + lastRow + ", " + lastCol + "])Digging south");
					return true;
				}
				
				return false;
				
			}
			else if(move.x == 4000 && move.y == 4000)
			{
				//It's a Dig West operation
				if( (Time.time - lastDug) < 3)
				{
					// can't dig back to back
					return false;
				}
				if(stage == Ant.Stage.ShedWings)
				{
					// queens/drones unsuitable for digging
					return false;
				}

				if(GetCol () == 0)
				{
					// already on edge
					return false;
				}

				if(!viewController.sprite.spriteName.Contains("ToWest"))
				{
					// facing the wrong way
					return false;
				}

				if( (viewController.sprite.spriteName.Contains ("ToEast") || viewController.sprite.spriteName.Contains("ToWest")) && colony.groundLayer[GetRow() + 1, GetCol()] == 1)
				{
					// no digging while freefalling
					return false;
				}

				if(colony.groundLayer[GetRow () , GetCol () - 1 ] == 0)
				{
					Debug.Log ("(" + name + " [" + lastRow + ", " + lastCol + "])Dig west");
					return true;
				}
				
				return false;
				
			}
			else if(move.x == 5000 && move.y == 5000)
			{
				//It's a Climb Right operation

				if(colony.groundLayer[GetRow () -1, GetCol () ] == 1 && colony.groundLayer[lastRow - 1, lastCol + 1] == 1 && viewController.sprite.spriteName.Contains("ToNorth"))
				{
					return true;
				}
				
				return false;
				
			}
			else if(move.x == 5100 && move.y == 5100)
			{
				//It's a Climb Right 2 operation
				if(!viewController.sprite.spriteName.Contains ("Flipped"))
				{
					//this move is for ants crawling on the ceiling
					return false;
				}
				if(colony.groundLayer[lastRow, lastCol - 1] != 1)
				{
					// path to the left not clear
					return false;
				}
				if(colony.groundLayer[lastRow - 1, lastCol - 1] != 1)
				{
					// path to the left and above not clear
					return false;
				}
				if(colony.groundLayer[lastRow -1, lastCol] != 0)
				{
					// tile above must be solid
					return false;
				}


				
				return true;
				
			}
			else if(move.x == 5250 && move.y == 5250)
			{
				//It's a Climb Down Left operation

				if(!viewController.sprite.spriteName.Contains("ToWest"))
				{
					//can only climb down to the left when facing left/west
					return false;
				}

				if(GetCol () == 0)
				{
					// we're on the edge
					return false;
				}
				if(colony.groundLayer[GetRow (), lastCol - 1] != 1)
				{
					//block to the left must be cleared
					return false;
				}
				if(colony.groundLayer[lastRow + 1, lastCol - 1] != 1)
				{
					//block down and to the left must be cleared
					return false;
				}
				
				return true;

			}
			else if(move.x == 5500 && move.y == 5500)
			{
				//It's a Slide Down Left operation
				if(!viewController.sprite.spriteName.Contains ("ToSouth"))
				{
					// Can only perform this move when facing south
					return false;
				}
				if(colony.groundLayer[GetRow () + 1, GetCol () ] == 0)
				{
					return true;
				}
				
				return false;
				
			}
			else if(move.x == 5750 && move.y == 5750)
			{
				//It's a Slide Up Right operation
				if(!viewController.sprite.spriteName.Contains ("ToEast"))
				{
					// Can only perform this move when facing east
					return false;
				}
				if(colony.groundLayer[GetRow () + 1, GetCol () ] != 0)
				{
					return false;
				}
				if(colony.groundLayer[lastRow, lastCol + 1] != 0)
				{
					return false;
				}
				Debug.Log ("(" + name + " [" + lastRow + ", " + lastCol + "])Sliding up right");
				return true;
				
			}
			else if(move.x == 6000 && move.y == 6000)
			{
				//It's a Crawl Up Left operation
				if(!viewController.sprite.spriteName.Contains ("ToNorth"))
				{
					// Can only perform this move when facing east
					return false;
				}
				if(colony.groundLayer[GetRow () - 1, GetCol () ] != 0)
				{
					//there needs to be a ceiling above to crawl on
					return false;
				}
				if(colony.groundLayer[lastRow, lastCol + 1] != 0)
				{
					// there needs to be a wall to the right
					return false;
				}


				Debug.Log ("(" + name + " [" + lastRow + ", " + lastCol + "])Crawl up left");
				return true;
				
			}
			else if(move.x == 9000 && move.y == 9000)
			{
				//It's a fall operation
				
				if(colony.groundLayer[GetRow () + 1, GetCol () ] == 1 && !viewController.sprite.spriteName.Contains ("ToNorth") && !viewController.sprite.spriteName.Contains("ToSouth") && !viewController.sprite.spriteName.Contains("Flipped") )
				{
					return true;
				}

				// if everything alround you is 1's, fall
				if( (colony.groundLayer[lastRow + 1, lastCol] == 1) && 
				   (colony.groundLayer[lastRow , lastCol + 1] == 1) &&
				   (colony.groundLayer[lastRow, lastCol - 1] == 1))
				{
					return true;
				}

				// if stuck vertically
				if( (colony.groundLayer[lastRow + 1, lastCol] == 1) && 
				   (colony.groundLayer[lastRow , lastCol + 1] == 1) &&
				   (viewController.sprite.spriteName.Contains ("ToSouth")))
				{
					return true;
				}



				
				return false;
				
			}
			//Debug.Log ("(" + name + ") Considering move: " + move + ". CurrentTile: " + GetCurrentTile());
			if(move.x > 0)
			{
				if(colony.groundLayer[GetRow (), GetCol () + 1] != 1)
				{
					// tile to the right isn't clear
					return false;
				}
				if(colony.groundLayer[lastRow + 1, GetCol () + 1] == 1)
				{
					// tile to the right below is clear
					return false;
				}
				//Debug.Log ("(" + name + " [" + lastRow + ", " + lastCol + "])Moving to the right");

				return true;
			}
			else if(move.x < 0)
			{
				if(GetCol () == 0)
				{
					//can't move left!
					return false;
				}

				if(colony.groundLayer[GetRow(), GetCol () -1 ] != 1)
				{
					//tile to left isn't clear
					return false;
				}

				if(colony.groundLayer[GetRow() + 1, GetCol () -1 ] != 0)
				{
					//tile to left and under isn't ground
					return false;
				}

				if( (viewController.sprite.spriteName.Contains ("ToSouth") || viewController.sprite.spriteName.Contains ("ToNorth")))
				{
					if(colony.groundLayer[lastRow + 1, lastCol] == 1)
					{
						//nothing underneath
						return false;
					}
				}

				if(colony.groundLayer[lastRow - 1, lastCol -1 ] != 0 && viewController.sprite.spriteName.Contains ("Flipped"))
				{
					//tile to left and above is dug out already
					return false;
				}

				if(colony.groundLayer[lastRow + 1, lastCol -1 ] == 1 && !viewController.sprite.spriteName.Contains("Flipped"))
				{
					//can't walk left across an empty tile
					return false;
				}

				if(colony.groundLayer[lastRow + 1, lastCol] == 1 && !viewController.sprite.spriteName.Contains("Flipped"))
				{
					//can't walk on empty
					return false;
				}
 				//Debug.Log ("(" + name + " [" + lastRow + ", " + lastCol + "])Moving to the left");
				return true;
			}

			if(move.y > 0)
			{
				if( colony.groundLayer[GetRow () - 1, GetCol ()] != 1)
				{
					//above not clear
					return false;
				}
				if( colony.groundLayer[GetRow () - 1, GetCol () + 1] == 1)
				{
					//no right walk to crawl on 
					return false;
				}
				if( colony.groundLayer[GetRow (), GetCol () + 1] == 1 && !viewController.sprite.spriteName.Contains ("Flipped"))
				{
					//no right walk to crawl on 
					return false;
				}

				if( colony.groundLayer[GetRow () - 1, GetCol () ] == 1 && colony.groundLayer[lastRow - 1, lastCol + 1] != 1 && colony.groundLayer[lastRow, lastCol + 1] != 1)
				{
					//Debug.Log ("(" + name + " [" + lastRow + ", " + lastCol + "])Moving up");
					return true;
				}
			}
			else if(move.y < 0)
			{
				if(viewController.sprite.spriteName.Contains("Flipped"))
				{
					return false;
				}
				if(colony.groundLayer[GetRow () + 1, GetCol ()] != 1)
				{
					//no space below
					return false;
				} 
				if(colony.groundLayer[lastRow, lastCol + 1] == 1)
				{
					//can't walk down since no walk on the right
					return false;
				}
				if(colony.groundLayer[lastRow + 1, lastCol + 1] == 1)
				{
					//can't walk down since no walk on the right
					return false;
				}

				//Debug.Log ("(" + name + " [" + lastRow + ", " + lastCol + "])Moving to the south");
				return true;
			}


			return false;
		}

		private bool CheckNorth()
		{
			int tile;

			// , check tile above
			if(GetRow  () == 0)
			{
				// This is the left edge of the tile map and we can't go past here without exceptions
				return false;
			}
			if (GetCol () <= 0)
			{
				return false;
			}
			tile = colony.groundLayer[GetRow () - 1, GetCol () ];
			
			//...if good, go there
			if(tile == 1 )
			{
				//Debug.Log ("Going to 1, above. CurrentPos: " + viewController.transform.localPosition);
				
				Vector3 node = new Vector3(0, 90, 0 );
				if(!node.Equals(pastNode2))
				{
					walkList.Add (node);
					return true;
				}
				
			}

			return false;
		}

		private bool CheckSouth()
		{
			int tile;

			tile = colony.groundLayer[GetRow () + 1, GetCol () ];
			
			//...if good, go there
			if(tile == 1 || tile == 2)
			{
				//Debug.Log ("Going to " + tile + ", below. CurrentPos: " + viewController.transform.localPosition);
				
				Vector3 node = new Vector3(0, -90, 0 );
				if(!node.Equals(pastNode2))
				{
					walkList.Add (node);
					return true;
				}
			}

			return false;
		}

		private int nCount, sCount;
		private bool CheckNorthSouth()
		{
			int tile;

			if( (nCount % 8) == 0)
			{
				//return false;
			}

			if(direction == Direction.South)
			{
				if(!CheckSouth ())
				{
					if(!CheckEast ())
					{
						if(!CheckWest ())
						{
							return false;
						}
						else
						{
							return true;
						}
					}
					else
					{
						return true;
					}
				}
				else
				{
					return true;
				}
			}

			if(!CheckNorth())
			{
				nCount++;
				if( (sCount % 8) == 0)
				{
					//return false;
				}
				if(!CheckSouth())
				{
					sCount++;
					return false;
				}
				return true;
			}

			return true;

		}

		private bool CheckEast()
		{
			int tile;
			
			// Check tile to the right
			tile = colony.groundLayer[GetRow (), GetCol () + 1];
			
			// ...if good, go there
			if(tile == 2)
			{
				Debug.Log ("Going to 2. CurrentPos: " + viewController.transform.localPosition);
				
				Vector3 node = new Vector3(90, 0, 0 );
				walkList.Add (node);
				return true;
			}

			return false;
		}

		private bool CheckWest()
		{
			int tile;

			// Else, check tile to the left
			if(GetCol () == 0)
			{
				// This is the left edge of the tile map and we can't go past here without exceptions
				return CheckEast();
			}
			tile = colony.groundLayer[GetRow (), GetCol () - 1];
			
			//...if good, go there
			if(tile == 3)
			{
				Debug.Log ("Going to 3. CurrentPos: " + viewController.transform.localPosition);
				
				Vector3 node = new Vector3(-90, 0, 0 );
				walkList.Add (node);
				return true;
			}

			return false;
		}

		private bool CheckEastWest()
		{

			if(!CheckEast ())
			{
				if(!CheckWest ())
				{
					return false;
				}
				return true;
			}

			return true; 
		}

		public int moveCount;

		/*
		 * This is where we define and enumerate all the possible moves, and then select one at random
		 *
		 */

		public Vector2 ChooseRandomMove()
		{
			Vector2 move0 = new Vector2(90, 0);
			Vector2 move1 = new Vector2(-90, 0);
			Vector2 move2 = new Vector2(0, 90);
			Vector2 move3 = new Vector2(0, -90);
			Vector2 layEgg = new Vector2(1000, 1000);
			Vector2 digRight = new Vector2(2000, 2000);
			Vector2 digSouth = new Vector2(3000, 3000);
			Vector2 digWest = new Vector2(4000, 4000);
			Vector2 climbRight = new Vector2(5000,5000);
			Vector2 climbRight2 = new Vector2(5100, 5100);
			Vector2 climbDownLeft = new Vector2(5250,5250);
			Vector2 slideDownLeft = new Vector2(5500,5500);
			Vector2 slideUpRight = new Vector2(5750, 5750);
			Vector2 crawlUpLeft = new Vector2(6000,6000);
			Vector2 fall = new Vector2(9000,9000);

			int move = UnityEngine.Random.Range (0, 15);
			
			Vector2 chosenMove = Vector2.zero;
			if(move == 0)
			{
				chosenMove = move0;
			}
			else if(move == 1)
			{
				chosenMove = move1;
			}
			else if(move == 2)
			{
				chosenMove = move2;
			}
			else if(move == 3)
			{
				chosenMove = move3;
			}
			else if(move == 4)
			{
				chosenMove = layEgg;
			}
			else if(move == 5)
			{
				chosenMove = digRight;
			}
			else if(move == 6)
			{
				chosenMove = digSouth;
			}
			else if(move == 7)
			{
				chosenMove = climbRight;
			}
			else if(move == 8)
			{
				chosenMove = fall;
			}
			else if(move == 9)
			{
				chosenMove = digWest;
			}
			else if(move == 10)
			{
				chosenMove = slideDownLeft;
			}
			else if(move == 11)
			{
				chosenMove = climbDownLeft;
			}
			else if(move == 12)
			{
				chosenMove = slideUpRight;
			}
			else if(move == 13)
			{
				chosenMove = crawlUpLeft;
			}
			else if(move == 14)
			{
				chosenMove = climbRight2;
			}

			return chosenMove;
		}
		public void DetermineNextMove()
		{


			int tries = 0;
			while (true)
			{
				Vector2 move = ChooseRandomMove();
				if(IsMoveConsistent(move))
				{
					//Debug.Log ("Found move (" + move + ") after considering " + tries + " others");
					walkList.Add(new Vector3(move.x, move.y, 0));
					break;
				}
				tries++;

				if(tries > 40)
				{
					// Debug.Log ("(" + name + ")Could not determine move");
					break;
				}
			}
			moveCount++;
		}

		public string GetProfile()
		{
			return name + " (" + dna.ToString2() + ")";
		}

		public bool Mate(Ant femaleAnt)
		{
			if(gender == Gender.Female)
			{
				Debug.LogWarning("Ant::Mate() can not be called by Females.");
				return false;
			}
			Debug.Log(GetProfile() + " would like to mate with " + femaleAnt.GetProfile());
			int compatibilityScore = (dna.attraction - femaleAnt.dna.attraction) + (dna.intelligence - femaleAnt.dna.intelligence) + (dna.strength - femaleAnt.dna.strength) + (dna.speed - femaleAnt.dna.speed);
			Debug.Log("Compatibility Score for " + name + " and " + femaleAnt.name + ": " + compatibilityScore);
			if( compatibilityScore < 0 )
			{
				// Not compatible
				return false;
			}
			// Copulation
			if(hp < 50)
			{
				Debug.Log(name + " not strong enough to copulate with " + femaleAnt.name + " despite compatibility.");
				return false;
			}

			if(UnityEngine.Random.Range(0,2) != 1)
			{
				Debug.Log(name + " failed to impregnate " + femaleAnt.name );
				return false;
			}


			Debug.Log(name + " impregnated " + femaleAnt.name);
			femaleAnt.seedDna = dna.Combine(femaleAnt.dna);
			Debug.Log(femaleAnt.name + " is now carrying an embryo with the following DNA: " + femaleAnt.seedDna.ToString2());
			return true;
		}

		public Ant Procreate()
		{
			if(seedDna == null)
			{
				return null;
			}

			return new Ant(seedDna);
		}

		public bool Select()
		{
			return true;
		}

		public IEnumerator NuptialFlight()
		{
			Debug.Log ("Beginning Nuptial Flight");
			AntSimulation.singleton.modalBodyLabel.text = "Beginning Nuptial Flight...";
			yield return new WaitForSeconds(1.5f);
			AntSimulation.singleton.modalBodyLabel.text = "Entering nuptial swarm...";
			yield return new WaitForSeconds(2f);
			bool foundMate = false;
			int attempts = 0;
			while(!foundMate)
			{
				AntSimulation.singleton.modalBodyLabel.text = "Browsing potentials...";
				yield return new WaitForSeconds(1.5f);
				Ant a = new Ant(0);
				AntSimulation.singleton.modalBodyLabel.text = "A male ant approaches!";
				yield return new WaitForSeconds(1.5f);
				AntSimulation.singleton.modalBodyLabel.text += System.Environment.NewLine + "His name is " + a.name;
				AntSimulation.singleton.modalBodyLabel.text += System.Environment.NewLine + "Strength: " + a.dna.strength;
				AntSimulation.singleton.modalBodyLabel.text += System.Environment.NewLine + "Intelligence: " + a.dna.intelligence;
				AntSimulation.singleton.modalBodyLabel.text += System.Environment.NewLine + "Speed: " + a.dna.speed;
				AntSimulation.singleton.modalBodyLabel.text += System.Environment.NewLine + "Attraction: " + a.dna.attraction;
				yield return new WaitForSeconds(1.5f);
				if(!a.Mate (this))
				{
					AntSimulation.singleton.modalBodyLabel.text = "Queen not interested!";
					yield return new WaitForSeconds(1.0f);


				}
				else
				{
					AntSimulation.singleton.modalBodyLabel.text = "It's a match! ";

					yield return new WaitForSeconds(0.5f);
					AntSimulation.singleton.modalBodyLabel.text += System.Environment.NewLine + "After " + attempts + " suitors!";
					AntSimulation.singleton.modalBodyLabel.text += System.Environment.NewLine + "Queen is pregnant." + System.Environment.NewLine + "Flight complete!";
					foundMate = true;
				}
				attempts++;
				yield return new WaitForEndOfFrame();
			}
			eggs = UnityEngine.Random.Range (10, 50);
			AntSimulation.singleton.HideModal();

		}


	}

	[System.Serializable]
	public class Insect
	{
		public enum Type
		{
			Ant,
			Spider,
			Caterpillar,
			Antlion
		}
		public string name;
		public Type type;
		public int localId; 							//id, unique to Type of Insect
		public int globalId;							//id, unique to all Insects
		public int hp;									// health points

	}
	#endregion
}
