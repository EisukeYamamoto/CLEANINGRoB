using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineManager : MonoBehaviour
{
  [SerializeField] GameObject Engine_Prefab = default;
  // [SerializeField] PlayerMotion player = default;
  [SerializeField] Transform Field = default;
  [SerializeField] int Engine_num_limit = 3;

  private float Field_x;
  private float Field_z;
  // Start is called before the first frame update
  void Start()
  {
    Field_x = Field.localScale.x - 3f;
    Field_z = Field.localScale.z - 3f;
    for(int i = 1; i <= Engine_num_limit; i++){
      GameObject engine = Instantiate(Engine_Prefab);
      engine.transform.position = new Vector3(Random.value * Field_x - Field_x/2f,
                                               0.8f,
                                               Random.value * Field_z - Field_z/2f);
    }
  }

}
