// Description UI.cs : interaction with canvas group
using UnityEngine;
using System.Collections;

public class UI : MonoBehaviour {

	public bool 		FadeIn = false;			// true if the canvas is fade IN				
	public bool 		FadeOut = false;		// true if the canvas is fade out
	private CanvasGroup canvasGroup;			// access canvas component
	public float 		speed = 2;				// choose fade in/out speed
	private float 		t ;	
	private IEnumerator co;
	private IEnumerator co1;

	void Start(){									// --> Start
		canvasGroup = GetComponent<CanvasGroup>();	// Access canvas component
	}

	public void F_Fade_Out(){						// --> Fade Out the canvas group
		co1 = F_FadeIn();
		StopCoroutine(co1);
		co = F_FadeOut();
		StartCoroutine(co);
	}

	public void F_Fade_In(){						// --> Fade In the canvas group
		canvasGroup.gameObject.SetActive(true);
		co = F_FadeOut();
		StopCoroutine(co);
		co1 = F_FadeIn();
		StartCoroutine(co1);
	}

	public void Activate_Obj(){						// --> Activate this gameObject
		gameObject.SetActive(true);	
	}
	public void Deactivate_Obj(){					// --> Deactivate this gameObject
		gameObject.SetActive(false);	
	}

	public bool F_Interactable(){					// --> return if the canvas is interactable or not
		return canvasGroup.interactable;
	}
	public void F_EnableInteractable(){				// --> Make the canvas group interactable
		canvasGroup.interactable = true;
	}
	public void F_DisableInteractable(){			// --> Make the canvas group non interactable
		canvasGroup.interactable = false;
	}

	public void F_Deactivate () {					// --> Deactivate the canvas
		canvasGroup.alpha = 0;						
		canvasGroup.blocksRaycasts = false;			
		gameObject.SetActive(false);				
	}
	public void F_Activate () {						// --> Activate the canvas
		canvasGroup.alpha = 1;
		canvasGroup.blocksRaycasts = true;
		gameObject.SetActive(true);
	}

	IEnumerator F_FadeIn () {						// --> Fade In the canvas group
		t = 0;

		while(Mathf.Round(canvasGroup.alpha*10000) < 1*10000){	// while canvas is opaque
			t += Time.deltaTime/speed;
			canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha,1,t);
			yield return null;
		}
		canvasGroup.alpha = 1;
		canvasGroup.blocksRaycasts = true;				

	}
	IEnumerator F_FadeOut () {						// --> Fade Out the canvas group
		t = 0;
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
		while(Mathf.Round(canvasGroup.alpha*10000) > 0*10000){ // while canvas is transparency
			t += Time.deltaTime/speed;
			canvasGroup.alpha = Mathf.LerpUnclamped(canvasGroup.alpha,0,t*2);
			yield return null;
		}
		canvasGroup.alpha = 0;
		canvasGroup.gameObject.SetActive(false);
	}
}
