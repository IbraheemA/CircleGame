using UnityEngine;
using System.Collections.Generic;
using System.Linq;

static class EntityCatalog {

	static List<EntityConfig> _allEnemies;
	static List<EntityConfig> _allPlayers;
	static EntityCatalog(){
		_allPlayers = new List<EntityConfig>(Resources.LoadAll<EntityConfig>("EntityConfigs/Players"));
		_allEnemies = new List<EntityConfig>(Resources.LoadAll<EntityConfig>("EntityConfigs/Enemies"));
	}

	public static EntityConfig TestGetEnemyConfig(){
		return _allEnemies.First();
	} 

	public static EntityConfig TestGetPlayerConfig(){
		return _allPlayers.First();
	} 
}