using UnityEngine;
using System.Collections;

public static class GroundManager {

	public static int NB_SAND;
	public static int NB_GRASSHILL;
	public static int NB_GRASSROCKY;
	public static int NB_MUDROCKY;

	static void Init(){
		NB_SAND = NB_GRASSHILL = NB_GRASSROCKY = NB_MUDROCKY = 0;
	}


	public static void Ground(TerrainData tData, Texture[] textures){
		Init();

		for (int y = 0; y < tData.alphamapHeight; y++) {
			for (int x = 0; x < tData.alphamapWidth; x++) {
				if(tData.GetHeight(y,x) < textures[Texture.SAND].maxHeight){
					NB_SAND++;
				}else if(tData.GetHeight(y,x) > textures[Texture.GRASSHILL].minHeight && tData.GetHeight(y,x) < textures[Texture.GRASSHILL].maxHeight){
					NB_GRASSHILL++;
				}else if(tData.GetHeight(y,x) > textures[Texture.GRASSROCKY].minHeight && tData.GetHeight(y,x) < textures[Texture.GRASSROCKY].maxHeight){
					NB_GRASSROCKY++;
				}else if(tData.GetHeight(y,x) > textures[Texture.MUDROCKY].minHeight){
					NB_MUDROCKY++;
				}
			}
		}
	}
}
