using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIManagement;
using UnityEngine.Rendering.UI;
using System.Linq;
using System.ComponentModel.Design.Serialization;
using UnityEngine.UIElements;

namespace UIManagement
{
    public enum DialogueDisplayState
    {
        Show, Disable
    }

    public class UIProgressPanel : MonoBehaviour, IHomeUI
    {
        
        public GameObject rootCanvasGO;

        public UnityEngine.UI.Slider slider;


        public void UpdateSliderBar(float value)
        {
            slider.value = value;
        }


        private void Start()
        {

            if (UIManager.Instance != null)
            {
                UIManager.Instance.AddUI(this);
            }

            rootCanvasGO = this.gameObject;
            slider = this.GetComponent<UnityEngine.UI.Slider>();
            Show();
        }

        private void OnDestroy()
        {
            if(UIManager.Instance != null)
            UIManager.Instance.RemoveUI(this);
        }


        public void OnOpen(params object[] datas)
        {   
            // Show();
            // 打开的时候， 根据ScriptableObject来读取数据,
            // todo: 数据类型检测？
            Show(datas);
        }

        public void OnClose()
        {
            Hide();
        }

        public void OnUpdate(params object[] datas)
        {
            float value = (float)datas[0];
            UpdateSliderBar(value);
        }

        private void Show(params object[] datas)
        {
            if(!rootCanvasGO.activeSelf)rootCanvasGO.SetActive(true);
        }

        public void Hide()
        {
            StartCoroutine(DelayDisable(4.5f));
            //rootCanvasGO.SetActive(false);
        }

        IEnumerator DelayDisable(float time)
        {
            yield return new WaitForSeconds(time / 2.0f);
            this.gameObject.SetActive(false);
        }


        public AudioClip phoneMusic;
        public AudioClip buttonClickMusic;
        public void PlayPhoneMusic()
        {
            var Go = GameObject.Find("OtherSound");
            if (!Go) return;
            AudioSource tempSource = Go.GetComponent<AudioSource>();
            tempSource.clip = phoneMusic;
            tempSource.loop = false;
            tempSource.Play();
        }

        public void PlayButtonClickMusic()
        {
            var Go = GameObject.Find("OtherSound");
            if (!Go) return;
            AudioSource tempSource = Go.GetComponent<AudioSource>();
            tempSource.clip = buttonClickMusic;
            tempSource.loop = false;
            tempSource.Play();
            
        }
    }
}
