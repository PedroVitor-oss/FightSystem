using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    
    public Animator ani;
    public float habilidade = 1;
    public void Hit()
    {
        if(Random.Range(1,10)<= habilidade){
            //desviar
            transform.Translate(transform.right * 2);
        }else{
            //receber dano
            ani.SetBool("hit",true);
            Invoke("desligarAniHit",1);
        }
    }
    void desligarAniHit(){
        ani.SetBool("hit",false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
