using UnityEngine;
using System.Collections;

public class TerrainGenerator : MonoBehaviour {
	
	public Texture[] textures;
	public GameObject[] trees;

	[Range(0,100)]
	public int minTree;
	[Range(0,100)]
	public int maxTree;

	public Texture2D baseTexture;
	public Texture2D hMap;

	//public Vector2 pos;

	[Range(1,100)]
	public int height;

	private TerrainData tData;
	private Texture2D hMapTmp;

	private TreeInstance[] treeInstances;

	private bool inCreation;
	private bool canAddTexture;

	private PodController pod;
	private PlayerController player;

	private const int offset = 3;
	
	void Start () {

		tData = this.GetComponent<Terrain>().terrainData;
		hMapTmp = null;
		inCreation = false;
		canAddTexture = false;

		pod = GameObject.Find("Pod").GetComponent<PodController>();
		player = GameObject.Find("FPSController").GetComponent<PlayerController>();

		InitTerrain();
	}

	void Update(){
	   if(Input.GetKeyDown(KeyCode.B) && !inCreation && hMapTmp != hMap){
			hMapTmp = hMap;
			inCreation = true;

			//Notify pod
			NotifyPod();
			//Notify player
			NotifyPlayer();

			StartCoroutine(CreateTerrain());
			StartCoroutine(AddTextures());
			StartCoroutine(AddTrees());
		}else if(Input.GetKeyDown(KeyCode.C)){
			/*float[,] h = new float[1,1];
			h[0,0] = 50f;
			tData.SetHeights(Mathf.FloorToInt(pos.x),Mathf.FloorToInt(pos.y),h);*/
		}else if(Input.GetKeyDown(KeyCode.T)){
			RenderTrees();
		}
	}

	void NotifyPod(){
		Vector3 podPosition = pod.transform.position;
		float posY = hMap.GetPixel(Mathf.FloorToInt(podPosition.z) + offset, Mathf.FloorToInt(podPosition.x) + offset).grayscale * tData.size.y;
		StartCoroutine(pod.UpdatePosition(posY));
	}

	void NotifyPlayer(){
		Vector3 playerPosition = player.transform.position;
		float posY = hMap.GetPixel(Mathf.FloorToInt(playerPosition.z) + offset, Mathf.FloorToInt(playerPosition.x) + offset).grayscale * tData.size.y;
		StartCoroutine(player.UpdatePosition(posY));
	}

	void InitTerrain(){

		//Init heights of Terrain
		float[,] heights = new float[hMap.width,hMap.height];
		tData.SetHeights(0,0,heights);

		//Init main texture of Terrain (baseTexture)
		SplatPrototype[] initTexture = new SplatPrototype[1]; 
		initTexture[0] = new SplatPrototype();
		initTexture[0].texture = baseTexture;
		tData.splatPrototypes = initTexture;

		//Init alphamap with the base texture
		float[,,] map = new float[tData.alphamapWidth, tData.alphamapHeight, tData.alphamapLayers];
		for (int y = 0; y < tData.alphamapHeight; y++) {
			for (int x = 0; x < tData.alphamapWidth; x++) {
				map[x,y,0] = 1f;
			}
		}
		tData.SetAlphamaps(0, 0, map);

		treeInstances = new TreeInstance[0];
		tData.treeInstances = treeInstances;
		tData.treePrototypes = new TreePrototype[0];
	}

	void RefreshTerrainCollider(){
		float[,] terrainHeights = tData.GetHeights (0,0,tData.heightmapWidth, tData.heightmapHeight);
		tData.SetHeights (0,0, terrainHeights);
	}

	IEnumerator CreateTerrain(){

		if(hMap == null){
			yield break;
		}

		//Initialisation du Terrain
		tData.heightmapResolution = Mathf.FloorToInt(Mathf.Max(hMap.width, hMap.height));
		tData.alphamapResolution = tData.heightmapResolution - 1;
		tData.size = new Vector3(tData.size.x, height, tData.size.z);


		//Création du Terrain en fonction de la hMap
		for(int y = 0; y < hMap.height; y++){

			float[,] heights = new float[hMap.width,1];

			for(int x = 0; x < hMap.width; x++){
				heights[x,0] = hMap.GetPixel(x,y).grayscale;
			}

			tData.SetHeights(y,0,heights);
			RefreshTerrainCollider();
			canAddTexture = true;

			//Unfreeze the player when the Terrain is created at his position
			if(y > player.transform.position.x + offset){
				player.UnfreezePlayer();
			}

			yield return new WaitForSeconds(Time.fixedDeltaTime);
		}

		inCreation = false;
		canAddTexture = false;
	}

	IEnumerator AddTextures(){
		if(textures.Length == 0){
			yield break;
		}

		while(!canAddTexture){
			yield return null;
		}

		// Ajout des textures au Terrain
		SplatPrototype[] terrainTexture = new SplatPrototype[textures.Length]; 
		
		for(int i = 0 ; i < textures.Length ; i++){
			terrainTexture[i] = new SplatPrototype(); 
			terrainTexture[i].texture = textures[i].texture;
			
		}
		tData.splatPrototypes = terrainTexture;

		// For each point on the alphamap...
		for (int y = 0; y < tData.alphamapHeight; y++) {

			//Placement des textures en fonction de la hauteur du Terrain
			float[,,] map = new float[tData.alphamapWidth, 1, tData.alphamapLayers];

			for (int x = 0; x < tData.alphamapWidth; x++) {

				int nbCollisionMap = 0;

				for(int i = 0 ; i < textures.Length ; i++){
					if(textures[i].minHeight <= tData.GetHeight(y,x) && textures[i].maxHeight >= tData.GetHeight(y,x)){
						map[x, 0, i] = 1;
						nbCollisionMap++;
					}else{
						map[x, 0, i] = 0;
					}
				}

				if(nbCollisionMap > 1){
					for(int i = 0 ; i < textures.Length ; i++){
						if(map[x, 0, i] == 1){
							map[x, 0, i] /= nbCollisionMap;
						}
					}
				}
			}

			//Assignation de la nouvelle alphamap
			tData.SetAlphamaps(y, 0, map);
			yield return new WaitForSeconds(Time.fixedDeltaTime);
		}
	}

	IEnumerator AddTrees(){
		while(inCreation){
			yield return null;
		}
		
		// Ajout des arbres au Terrain
		TreePrototype[] treesProto = new TreePrototype[trees.Length]; 
		
		for(int i = 0 ; i < trees.Length ; i++){
			treesProto[i] = new TreePrototype(); 
			treesProto[i].prefab = trees[i];
		}

		tData.treePrototypes = treesProto;

		int nbTrees = Random.Range(minTree, maxTree);

		treeInstances = new TreeInstance[nbTrees];

		for(int i = 0 ; i < nbTrees ; i++){

			int xPos = 0;
			int zPos = 0;
			int index = 0;
			do{
				xPos = Random.Range(0, hMap.width);
				zPos = Random.Range(0, hMap.height);

				index++;
			}while(tData.GetHeight (zPos,xPos) > textures[0].maxHeight && index < 1000);

			//float yPos = hMap.GetPixel(xPos + offset, zPos + offset).grayscale * tData.size.y - offset;
			float yPos = tData.GetHeight (zPos,xPos) - offset;
			Vector3 position = new Vector3(zPos, yPos, xPos); 

			position = new Vector3(position.x / hMap.width, position.y / tData.size.y, position.z / hMap.height);

			treeInstances[i].position = position;
			treeInstances[i].widthScale = 1;
			treeInstances[i].heightScale = 1;
			treeInstances[i].color = Color.white;
			treeInstances[i].lightmapColor = Color.white;
			treeInstances[i].prototypeIndex = Random.Range(0, trees.Length - 1);
		}

		tData.treeInstances = treeInstances;

		RefreshTerrainCollider();
	}

	void RenderTrees(){
		for(int i = 0 ; i < treeInstances.Length ; i++){
			treeInstances[i].widthScale = 1;
			treeInstances[i].heightScale = 1;
		}

		tData.treeInstances = treeInstances;

		RefreshTerrainCollider();
	}

}
