using UnityEngine;
using System.Collections;

public class TerrainGenerator : MonoBehaviour {
	
	//PUBLIC FIELDS ///////////////////
	
	/*public Texture[] textures;
	public Tree trees;
	public Details details;
	public Wind wind;
	public Water water;

	public Texture2D baseTexture;
	public Texture2D baseNormalMap;*/

	public Seasons season;

	public Texture2D hMap;
	
	[Range(1,100)]
	public int height;
	
	///////////////////////////////////
	
	
	//PRIVATE FIELDS ///////////////////

	private TerrainData tData;
	private Texture2D hMapTmp;

	private TreeInstance[] treeInstances;

	private bool inCreation;
	private bool canAddTexture;

	private PodController pod;
	private PlayerController player;

	private const int offset = 3;
	
	///////////////////////////////////
	
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
		if(Input.GetKey(KeyCode.KeypadEnter) && !inCreation){
			season.NextSeason();
			InitTextures();
			InitTrees();
			InitDetails();
			InitWater();

			canAddTexture = true;
			StartCoroutine(AddTextures());
			AddTrees();
			AddDetails();
			AddWater();
		}
	}
	
	public void Generate(){
		if(!inCreation && hMapTmp != hMap){
			hMapTmp = hMap;
			inCreation = true;

			InitTerrain();

			//Notify player
			NotifyPlayer();

			StartCoroutine(CreateTerrain());
			StartCoroutine(AddTextures());
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

		InitHeights();

		InitTextures();

		InitTrees();

		InitDetails();

		InitWater();
	}

	void InitHeights(){
		//Init heights of Terrain
		float[,] heights = new float[hMap.width,hMap.height];
		tData.SetHeights(0,0,heights);
	}

	void InitTextures(){
		//Init main texture of Terrain (baseTexture)
		SplatPrototype[] initTexture = new SplatPrototype[1]; 
		initTexture[0] = new SplatPrototype();
		initTexture[0].texture = season.seasons[season.CurrentSeason].baseTexture;
		initTexture[0].normalMap = season.seasons[season.CurrentSeason].baseNormalMap;
		initTexture[0].tileSize = new Vector2(2,2);
		
		tData.splatPrototypes = initTexture;
		
		//Init alphamap with the base texture
		float[,,] map = new float[tData.alphamapWidth, tData.alphamapHeight, tData.alphamapLayers];
		for (int y = 0; y < tData.alphamapHeight; y++) {
			for (int x = 0; x < tData.alphamapWidth; x++) {
				map[x,y,0] = 1f;
			}
		}
		tData.SetAlphamaps(0, 0, map);
	}

	void InitTrees(){
		treeInstances = new TreeInstance[0];
		tData.treeInstances = treeInstances;
		tData.treePrototypes = new TreePrototype[0];
	}

	void InitDetails(){
		DetailPrototype[] detailProto = new DetailPrototype[0]; 
		tData.detailPrototypes = detailProto;
	}

	void InitWater(){
		season.seasons[season.CurrentSeason].water.waterGameobject.SetActive(false);
		season.seasons[season.CurrentSeason].water.waterGameobject.transform.position = new Vector3(season.seasons[season.CurrentSeason].water.waterGameobject.transform.position.x, season.seasons[season.CurrentSeason].water.minPosY, season.seasons[season.CurrentSeason].water.waterGameobject.transform.position.z);
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

		GroundManager.Ground(tData, season.seasons[season.CurrentSeason].textures);
		AddTrees();
		AddDetails();
		AddWater();
	}

	IEnumerator AddTextures(){
		if(season.seasons[season.CurrentSeason].textures.Length == 0){
			yield break;
		}

		while(!canAddTexture){
			yield return null;
		}

		// Ajout des textures au Terrain
		SplatPrototype[] terrainTexture = new SplatPrototype[season.seasons[season.CurrentSeason].textures.Length]; 
		
		for(int i = 0 ; i < season.seasons[season.CurrentSeason].textures.Length ; i++){
			terrainTexture[i] = new SplatPrototype(); 
			terrainTexture[i].texture = season.seasons[season.CurrentSeason].textures[i].texture;
			terrainTexture[i].normalMap = season.seasons[season.CurrentSeason].textures[i].normalMap;
			terrainTexture[i].tileSize = new Vector2(2,2);
		}
		tData.splatPrototypes = terrainTexture;

		// For each point on the alphamap...
		for (int y = 0; y < tData.alphamapHeight; y++) {

			//Placement des textures en fonction de la hauteur du Terrain
			float[,,] map = new float[tData.alphamapWidth, 1, tData.alphamapLayers];

			for (int x = 0; x < tData.alphamapWidth; x++) {

				int nbCollisionMap = 0;

				for(int i = 0 ; i < season.seasons[season.CurrentSeason].textures.Length ; i++){
					if(season.seasons[season.CurrentSeason].textures[i].minHeight <= tData.GetHeight(y,x) && season.seasons[season.CurrentSeason].textures[i].maxHeight >= tData.GetHeight(y,x)){
						map[x, 0, i] = 1;
						nbCollisionMap++;
					}else{
						map[x, 0, i] = 0;
					}
				}

				if(nbCollisionMap > 1){
					for(int i = 0 ; i < season.seasons[season.CurrentSeason].textures.Length ; i++){
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

	void AddTrees(){
		//Check if tree is necessary
		float percentage = ((float)GroundManager.NB_GRASSHILL / (float)(tData.alphamapHeight * tData.alphamapWidth));
		Debug.Log("Pourcentage texture herbe : " + percentage);
		Debug.Log("Nb textures herbes : " + GroundManager.NB_GRASSHILL);
		if(percentage < season.seasons[season.CurrentSeason].trees.treeProbability){
			return;
		}
		
		// Ajout des arbres au Terrain
		TreePrototype[] treesProto = new TreePrototype[season.seasons[season.CurrentSeason].trees.treesGamobject.Length]; 
		
		for(int i = 0 ; i < season.seasons[season.CurrentSeason].trees.treesGamobject.Length ; i++){
			treesProto[i] = new TreePrototype(); 
			treesProto[i].prefab = season.seasons[season.CurrentSeason].trees.treesGamobject[i];
		}

		tData.treePrototypes = treesProto;

		int nbTrees = Mathf.FloorToInt((Random.Range(season.seasons[season.CurrentSeason].trees.minTree, season.seasons[season.CurrentSeason].trees.maxTree) / 100f) * GroundManager.NB_GRASSHILL);
		Debug.Log("NBTREES : " + nbTrees);

		treeInstances = new TreeInstance[nbTrees];

		for(int i = 0 ; i < nbTrees ; i++){

			int xPos = 0;
			int zPos = 0;
			int index = 0;
			do{
				xPos = Random.Range(0, hMap.width);
				zPos = Random.Range(0, hMap.height);

				index++;
			}while((tData.GetHeight (zPos,xPos) < season.seasons[season.CurrentSeason].textures[Texture.GRASSHILL].minHeight || tData.GetHeight (zPos,xPos) > season.seasons[season.CurrentSeason].textures[Texture.GRASSHILL].maxHeight) && index < 1000);

			//float yPos = hMap.GetPixel(xPos + offset, zPos + offset).grayscale * tData.size.y - offset;
			float yPos = tData.GetHeight (zPos,xPos);
			Vector3 position = new Vector3(zPos, yPos, xPos); 

			position = new Vector3(position.x / hMap.width, position.y / tData.size.y, position.z / hMap.height);

			treeInstances[i].position = position;
			treeInstances[i].widthScale = 0;
			treeInstances[i].heightScale = 0;
			treeInstances[i].color = Color.white;
			treeInstances[i].lightmapColor = Color.white;
			treeInstances[i].prototypeIndex = Random.Range(0, season.seasons[season.CurrentSeason].trees.treesGamobject.Length - 1);
		}

		tData.treeInstances = treeInstances;

		StartCoroutine(GrowTrees());
	}

	IEnumerator GrowTrees(){
		for(int j = 0 ; j < 100 ; j++){
			for(int i = 0 ; i < treeInstances.Length ; i++){
				treeInstances[i].widthScale += 0.01f;
				treeInstances[i].heightScale += 0.01f;
			}
			
			tData.treeInstances = treeInstances;
			yield return new WaitForSeconds(Time.deltaTime);
		}

		RefreshTerrainCollider();
	}

	void AddDetails(){
		//Check if grass is necessary
		float percentage = ((float)GroundManager.NB_GRASSHILL / (float)(tData.alphamapHeight * tData.alphamapWidth));
		if(percentage < season.seasons[season.CurrentSeason].details.grassProbability){
			return;
		}

		tData.SetDetailResolution(Details.DETAIL_RESOLUTION,Details.DETAIL_PER_PATCH);
		
		// Ajout d'herbe au Terrain
		DetailPrototype[] detailProto = new DetailPrototype[season.seasons[season.CurrentSeason].details.grassTextures.Length + season.seasons[season.CurrentSeason].details.bushsMeshes.Length]; 

		int tmp = 0;

		for(int i = 0 ; i < season.seasons[season.CurrentSeason].details.grassTextures.Length ; i++){
			detailProto[i] = new DetailPrototype(); 
			detailProto[i].renderMode = DetailRenderMode.GrassBillboard;
			detailProto[i].prototypeTexture = season.seasons[season.CurrentSeason].details.grassTextures[i];
			detailProto[i].minWidth = season.seasons[season.CurrentSeason].details.minWidth;
			detailProto[i].maxWidth = season.seasons[season.CurrentSeason].details.maxWidth;
			detailProto[i].minHeight = season.seasons[season.CurrentSeason].details.minHeight;
			detailProto[i].maxHeight = season.seasons[season.CurrentSeason].details.maxHeight;
			detailProto[i].noiseSpread = season.seasons[season.CurrentSeason].details.noiseSpread;
			detailProto[i].dryColor = season.seasons[season.CurrentSeason].details.dryColor;
			detailProto[i].healthyColor = season.seasons[season.CurrentSeason].details.healthyColor;
			detailProto[i].usePrototypeMesh = false;
			detailProto[i].bendFactor = 1f;

			tmp = i;
		}

		tmp++;

		int j = 0;
		for(int i = tmp ; i < tmp + season.seasons[season.CurrentSeason].details.bushsMeshes.Length ; i++){
			detailProto[i] = new DetailPrototype(); 
			detailProto[i].renderMode = DetailRenderMode.Grass;
			detailProto[i].prototype = season.seasons[season.CurrentSeason].details.bushsMeshes[j++];
			detailProto[i].minWidth = season.seasons[season.CurrentSeason].details.minWidth;
			detailProto[i].maxWidth = season.seasons[season.CurrentSeason].details.maxWidth;
			detailProto[i].minHeight = season.seasons[season.CurrentSeason].details.minHeight;
			detailProto[i].maxHeight = season.seasons[season.CurrentSeason].details.maxHeight;
			detailProto[i].noiseSpread = season.seasons[season.CurrentSeason].details.noiseSpread;
			detailProto[i].dryColor = season.seasons[season.CurrentSeason].details.dryColor;
			detailProto[i].healthyColor = season.seasons[season.CurrentSeason].details.healthyColor;
			detailProto[i].usePrototypeMesh = true;
			detailProto[i].bendFactor = 1f;
		}

		tData.detailPrototypes = detailProto;

		for(int layer = 0 ; layer < season.seasons[season.CurrentSeason].details.grassTextures.Length + season.seasons[season.CurrentSeason].details.bushsMeshes.Length ; layer++){

			int nbDetails = Mathf.FloorToInt((Random.Range(season.seasons[season.CurrentSeason].details.minGrass, season.seasons[season.CurrentSeason].details.maxGrass) / 10f) * GroundManager.NB_GRASSHILL);
			Debug.Log("NBGRASS : " + nbDetails);

			int[,] detail = new int[Details.DETAIL_RESOLUTION, Details.DETAIL_RESOLUTION];
			for(int i = 0 ; i < nbDetails ; i++){

				int randomX = 0;
				int randomZ = 0;
				int index = 0;

				do{
					randomX = Random.Range(0, hMap.width * 4);
					randomZ = Random.Range(0, hMap.height * 4);
					
					index++;
				}while((tData.GetHeight (randomZ,randomX) < season.seasons[season.CurrentSeason].textures[Texture.GRASSHILL].minHeight || tData.GetHeight (randomZ,randomX) > season.seasons[season.CurrentSeason].textures[Texture.GRASSHILL].maxHeight) && index < 1000);

				detail[randomX,randomZ] = 1;
			}

			tData.SetDetailLayer(0, 0, layer, detail);
		}

	}

	void AddWater(){

		//Check if water is necessary
		float percentage = ((float)GroundManager.NB_SAND / (float)(tData.alphamapHeight * tData.alphamapWidth));
		Debug.Log("Pourcentage sable : " + percentage);
		if(percentage < season.seasons[season.CurrentSeason].water.waterProbability){
			return;
		}
		
		season.seasons[season.CurrentSeason].water.waterGameobject.SetActive(true);

		StartCoroutine(RaiseWater());
	}

	IEnumerator RaiseWater(){
		float oldPosY = season.seasons[season.CurrentSeason].water.waterGameobject.transform.position.y;
		float newPosY = season.seasons[season.CurrentSeason].textures[Texture.SAND].maxHeight + season.seasons[season.CurrentSeason].water.offsetY;
		
		float posY;
		float time = 100 * Time.deltaTime;
		float elapsedTime = 0f;
		while (elapsedTime < time) {
			posY = Mathf.Lerp(oldPosY, newPosY, elapsedTime / time);
			season.seasons[season.CurrentSeason].water.waterGameobject.transform.position = new Vector3(season.seasons[season.CurrentSeason].water.waterGameobject.transform.position.x, posY, season.seasons[season.CurrentSeason].water.waterGameobject.transform.position.z);
			elapsedTime += Time.deltaTime;
			yield return new WaitForSeconds(Time.deltaTime);
		}
		
		season.seasons[season.CurrentSeason].water.waterGameobject.transform.position = new Vector3(season.seasons[season.CurrentSeason].water.waterGameobject.transform.position.x, newPosY, season.seasons[season.CurrentSeason].water.waterGameobject.transform.position.z);
	}

}
