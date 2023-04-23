//Description : FadeSprite.cs : Various functions to manipulate sprites alpha
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FadeSprite : MonoBehaviour {

	public List<SpriteRenderer> 		ListObject = new List<SpriteRenderer>();
	public bool							b_Fade = false;
	public float 						alphaTarget = 0;
	public float						FadeSpeed;
	public float 						_alpha =0;


	public void F_FadeIn () {
		alphaTarget = .05F;
		FadeSpeed = .1F;
		b_Fade =true;
	}

	public void F_FadeOut () {
		alphaTarget = 0;
		FadeSpeed = 1;
		b_Fade =true;
	}


	public void F_InitAlphaToPointOne () {
		for(int i = 0;i<ListObject.Count;i++){
			ListObject[i].color =  new Color(ListObject[i].color.r,ListObject[i].color.g,ListObject[i].color.b,.05F);
		}
	}

	public void F_InitAlphaToZero () {
		for(int i = 0;i<ListObject.Count;i++){
			ListObject[i].color =  new Color(ListObject[i].color.r,ListObject[i].color.g,ListObject[i].color.b,0);
		}
	}


	// Update is called once per frame
	void Update(){
		if(b_Fade){
			_alpha = Mathf.MoveTowards(_alpha,alphaTarget,Time.deltaTime*FadeSpeed);
			for(int i = 0;i<ListObject.Count;i++){
				ListObject[i].color =  new Color(ListObject[i].color.r,ListObject[i].color.g,ListObject[i].color.b,_alpha);
			}

			if(_alpha == alphaTarget){
				b_Fade = false;
			}
		}
	}
		
}
