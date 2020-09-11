using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaperManager : MonoBehaviour
{
    [SerializeField] GameObject Paper_1_Prefab = default;
    [SerializeField] GameObject Paper_2_Prefab = default;
    [SerializeField] GameObject Paper_3_Prefab = default;
    [SerializeField] GameObject Ball_Prefab = default;
    [SerializeField] Transform Field = default;
    [SerializeField] int paper_1_limit = 10;
    [SerializeField] int paper_2_limit = 10;
    [SerializeField] int paper_3_limit = 10;
    [SerializeField] int ball_limit = 3;
    GameObject[] Objects;

    private float Field_x;
    private float Field_z;

    public int paper1_num = 0;
    public int paper2_num = 0;
    public int paper3_num = 0;
    public int ball_num = 0;


    // Start is called before the first frame update
    void Start()
    {
        Field_x = Field.localScale.x - 3f;
        Field_z = Field.localScale.z - 3f;
        for(int i = 1; i <= paper_1_limit; i++){
          GameObject paper_1 = Instantiate(Paper_1_Prefab);
          if (i < paper_1_limit / 2){
            paper_1.transform.position = new Vector3(Random.value * Field_x - Field_x/2f,
                                                     0.5f,
                                                     Random.value * Field_z - Field_z/2f);
          }
          else{
            paper_1.transform.position = new Vector3(Random.value * Field_x - Field_x/2f,
                                                     8f,
                                                     Random.value * Field_z - Field_z/2f);
          }
          paper_1.transform.rotation = Quaternion.Euler(0f, Random.value * 360 - 180, 0f);
        }
        for(int i = 1; i <= paper_2_limit; i++){
          GameObject paper_2 = Instantiate(Paper_2_Prefab);
          if (i < paper_2_limit / 2){
            paper_2.transform.position = new Vector3(Random.value * Field_x - Field_x/2f,
                                                     0.5f,
                                                     Random.value * Field_z - Field_z/2f);
          }
          else{
            paper_2.transform.position = new Vector3(Random.value * Field_x - Field_x/2f,
                                                     8f,
                                                     Random.value * Field_z - Field_z/2f);
          }
          paper_2.transform.rotation = Quaternion.Euler(0f, Random.value * 360 - 180, 0f);
        }
        for(int i = 1; i <= paper_3_limit; i++){
          GameObject paper_3 = Instantiate(Paper_3_Prefab);
          if (i < paper_3_limit / 2){
            paper_3.transform.position = new Vector3(Random.value * Field_x - Field_x/2f,
                                                     0.5f,
                                                     Random.value * Field_z - Field_z/2f);
          }
          else{
            paper_3.transform.position = new Vector3(Random.value * Field_x - Field_x/2f,
                                                     8f,
                                                     Random.value * Field_z - Field_z/2f);
          }
          paper_3.transform.rotation = Quaternion.Euler(0f, Random.value * 360 - 180, 0f);
        }
        for (int i = 1; i <= ball_limit; i++){
          GameObject ball = Instantiate(Ball_Prefab);
          switch(i){
            case 1:
               ball.transform.position = new Vector3(23.4f, 6f, -16f);
               break;
            case 2:
               ball.transform.position = new Vector3(-20f, 0.5f, 19f);
               break;
            case 3:
               ball.transform.position = new Vector3(24f, 2f, 6f);
               break;
            default:
               break;
          }
        }
        paper1_num = Check("Paper_1");
        paper2_num = Check("Paper_2");
        paper3_num = Check("Paper_3");
        ball_num = Check("Ball");
    }

    // Update is called once per frame
    void Update()
    {


    }

    public int Check(string tagname){
        Objects = GameObject.FindGameObjectsWithTag(tagname);
        return Objects.Length;
    }

}
