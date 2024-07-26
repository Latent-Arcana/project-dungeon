using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class DungeonSetup : MonoBehaviour
{
    DangerGeneration Danger_Generator;

    LoreGeneration Lore_Generator;
  
    void Awake(){
        
        Danger_Generator = gameObject.GetComponent<DangerGeneration>();

        Lore_Generator = gameObject.GetComponent<LoreGeneration>();
    }


    
}
